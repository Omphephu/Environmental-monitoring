using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text statusText;

    private async void Awake() {
        if (UnityServices.State == ServicesInitializationState.Uninitialized) {
            Debug.Log("Services Initializing");
            await UnityServices.InitializeAsync();
        }
    }

    public async void StartSignUpUser() { 
        string username = usernameInput != null ? usernameInput.text : string.Empty;
        string password = passwordInput != null ? passwordInput.text : string.Empty;

        //user validation
        if (string.IsNullOrWhiteSpace(username)) {
            Debug.LogWarning("Username cannot be empty.");
            return;
        }
        if (string.IsNullOrWhiteSpace(password)) {
            Debug.LogWarning("Password cannot be empty.");
            return;
        }

        //Disable signup button while processing login
        if (registerButton != null) registerButton.interactable = false;

        try {
            await SignUpWithUsernamePasswordAsync(username, password);
            usernameInput.text = string.Empty;
            passwordInput.text = string.Empty;
        }
        finally { 
            //enable register button
            if (registerButton != null) registerButton.interactable = true;
        }
    }

    public async void StartSignInUser() {
        string username = usernameInput != null ? usernameInput.text : string.Empty;
        string password = passwordInput != null ? passwordInput.text : string.Empty;

        //user validation
        if (string.IsNullOrWhiteSpace(username)) {
            SetStatus("Username cannot be empty.", isError: true);
            return;
        }
        if (string.IsNullOrWhiteSpace(password)) {
            SetStatus("Password cannot be empty.", isError: true);
            return;
        }

        //Disable UI while processing
        SetInteractable(false);
        SetStatus("Signing in...", isError: false);

        try {
            //call async method
            await SignInWithUsernamePasswordAsync(username, password);

            //store user data for use in dashboard scene
            //SessionManager.Instance.SetUser(username);

            //SetStatus("Sign-in successful", isError: false);

            // Load dashboard scene
            //await LoadDashboardSceneAsync("DashBoardScene");

            usernameInput.text = string.Empty;
            passwordInput.text = string.Empty;
        }
        catch (System.Exception ex) {
            // This catch only handles unexpected exceptions from the wrapper, 
            // your method already logs AuthenticationException/RequestFailedException.
            Debug.LogException(ex);
            SetStatus("Sign-in failed. Check console for details.", isError: true);
        }

        finally {
            SetInteractable(true);
        }
    }

    private async Task LoadDashboardSceneAsync(string sceneName) {
        //Display loading message
        SetStatus("Loading Dashboard...", false);

        // start asynchronous load
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = true;

        while (!op.isDone) {
            await Task.Yield();
        }
    }

    private async Task SignUpWithUsernamePasswordAsync(string username, string password) {
        try {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("Signup is successful");
            // store user data
            SessionManager.Instance.SetUser(username);
            // Load dashboard scene
            await LoadDashboardSceneAsync("Dashboard_Scene");
        }
        catch (AuthenticationException ex){
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex) {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password) {
        try {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("User " + username + " SignIn is successful.");
            // store user data
            SessionManager.Instance.SetUser(username);
            // Load dashboard scene
            await LoadDashboardSceneAsync("Dashboard_Scene");
        }
        catch (AuthenticationException ex) {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex) {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    private void SetInteractable(bool enabled) {
        if (loginButton) loginButton.interactable = enabled;
        if (usernameInput) usernameInput.interactable = enabled;
        if (passwordInput) passwordInput.interactable = enabled;
    }

    private void SetStatus(string msg, bool isError) {
        if (!statusText) return;
        statusText.text = msg;
        statusText.color = isError ? Color.red : Color.white;
    }
}
