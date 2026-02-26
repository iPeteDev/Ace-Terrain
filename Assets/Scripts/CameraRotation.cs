using UnityEngine;

/// <summary>
/// CameraRotation.cs
/// Attach to the Camera (child of Player).
/// </summary>
public class CameraRotation : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 200f;
    public float verticalClamp = 80f;

    [Header("References")]
    public Transform playerBody;

    private float _xRotation = 0f;
    private bool _isApplicationFocused = false;

    void Start()
    {
        // Auto-find playerBody if not assigned (looks for parent transform)
        if (playerBody == null)
        {
            if (transform.parent != null)
            {
                playerBody = transform.parent;
                Debug.Log("[CameraRotation] playerBody auto-assigned to parent: " + playerBody.name);
            }
            else
            {
                Debug.LogWarning("[CameraRotation] playerBody is not assigned and no parent found. Horizontal look will not work.");
            }
        }

        LockCursor();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        _isApplicationFocused = hasFocus;

        // Re-lock cursor when the game window regains focus
        if (hasFocus && Cursor.lockState == CursorLockMode.Locked)
            LockCursor();
    }

    void Update()
    {
        // Only process input when the application has focus
        if (!_isApplicationFocused) return;

        HandleLook();
        HandleCursorToggle();
    }

    void HandleLook()
    {
        // Skip mouse look if cursor is unlocked (e.g. after pressing Escape)
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -verticalClamp, verticalClamp);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleCursorToggle()
    {
        // Unlock cursor with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
            UnlockCursor();

        // Re-lock cursor on left click when unlocked
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
            LockCursor();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}