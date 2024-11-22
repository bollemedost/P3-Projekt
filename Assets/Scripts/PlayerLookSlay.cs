using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookSlay : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float lookSpeedX = 2f;  // Sensitivity for horizontal look
    [SerializeField] private float lookSpeedY = 2f;  // Sensitivity for vertical look

    private Transform playerBody;  // Reference to the player's body (for rotation)
    private float xRotation = 0f; // Store the current X axis (up/down) rotation
    private PlayerInput playerInput; // Reference to the PlayerInput component
    private InputAction lookAction; // The input action for looking

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); // Get the PlayerInput component
        lookAction = playerInput.actions["Look"]; // Assuming you have an action called "Look"
    }

    void Start()
    {
        playerBody = transform; // Set the player's body (the object this script is attached to)
    }

    void Update()
    {
        // Get the look input (mouse delta or controller right stick)
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        // Handle the horizontal (X-axis) look (rotation of the body)
        float mouseX = lookInput.x * lookSpeedX;
        playerBody.Rotate(Vector3.up * mouseX);

        // Handle the vertical (Y-axis) look (clamping and rotating the camera up/down)
        xRotation -= lookInput.y * lookSpeedY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent the camera from rotating too far up/down
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
