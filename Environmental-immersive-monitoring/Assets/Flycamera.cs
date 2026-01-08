using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 10.0f;           // Base movement speed
    public float sprintMultiplier = 2.0f;     // Speed when holding Shift
    public float lookSensitivity = 2.0f;      // Mouse look sensitivity
    public bool focusOnEnable = true;         // Lock cursor when scene starts

    private Vector3 velocity; // Current movement velocity (for smooth stop)

    // Property to lock/unlock cursor
    private static bool Focused
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
        }
    }

    void OnEnable()
    {
        if (focusOnEnable)
            Focused = true;
    }

    void OnDisable()
    {
        Focused = false;
    }

    void Update()
    {
        // Lock cursor on left-click if not already focused
        if (!Focused && Input.GetMouseButtonDown(0))
        {
            Focused = true;
        }

        // Only process input when cursor is locked
        if (Focused)
        {
            UpdateInput();
        }

        // Apply smooth damping (slows down when no keys pressed)
        velocity = Vector3.Lerp(velocity, Vector3.zero, 5.0f * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    void UpdateInput()
    {
        // Keyboard movement (WASD, Space, Ctrl)
        Vector3 moveInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveInput += Vector3.back;
        if (Input.GetKey(KeyCode.A)) moveInput += Vector3.left;
        if (Input.GetKey(KeyCode.D)) moveInput += Vector3.right;
        if (Input.GetKey(KeyCode.Space)) moveInput += Vector3.up;
        if (Input.GetKey(KeyCode.LeftControl)) moveInput += Vector3.down;

        // Apply movement relative to camera’s current rotation
        Vector3 direction = transform.TransformVector(moveInput.normalized);
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier;

        velocity += direction * currentSpeed;

        // Mouse look (rotate camera)
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * lookSensitivity; // Invert Y

        Quaternion rotation = transform.rotation;
        Quaternion horiz = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion vert = Quaternion.AngleAxis(mouseY, Vector3.right);
        transform.rotation = horiz * rotation * vert;

        // Unlock cursor with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Focused = false;
        }
    }
}
