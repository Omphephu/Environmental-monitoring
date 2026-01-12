using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    public string CurrentUsername { get; private set; } = string.Empty;
    public bool IsAuthenticated { get; private set; } = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetUser(string username) { 
        CurrentUsername = username;
        IsAuthenticated = true;
    }

    public void ClearUser() { 
        CurrentUsername = string.Empty;
        IsAuthenticated = false;   
    }
}
