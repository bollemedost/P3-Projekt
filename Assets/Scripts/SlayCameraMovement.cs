using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlayCameraMovement : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    private float playerHeight = 3.0f; // Height offset from the player
    [SerializeField] private float distanceFromPlayer = -5.0f; // Distance behind the player
    [SerializeField] private float sensitivity = 2.0f; // Sensitivity for input

    private float rotationY = 0.0f; // Vertical rotation
    private float rotationX = 0.0f; // Horizontal rotation

    private Vector2 lookInput; // Stores input from the controller or mouse

    private bool useController = false; // Tracks whether the player is using a controller

    void Start()
    {
        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Called by Unity Input System when the "Look" action is performed
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        useController = true; // Assume controller if input comes from here
    }

    void Update()
    {
        // Detect mouse movement (fallback to mouse if controller isn't active)
        if (!useController)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            lookInput = new Vector2(mouseX, mouseY);
        }
        
        // Handle rotation logic
        float inputX = lookInput.x * sensitivity;
        float inputY = lookInput.y * sensitivity;

        // Update rotation based on input
        rotationY -= inputY; // Rotate up and down
        rotationY = Mathf.Clamp(rotationY, -30f, 30f); // Clamp vertical rotation to prevent flipping

        rotationX += inputX; // Rotate left and right

        // Apply rotation to the camera
        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0); // Apply both vertical and horizontal rotation

        // Calculate the desired position for the camera
        Vector3 targetPosition = player.position + transform.rotation * new Vector3(0, playerHeight, distanceFromPlayer);

        // Move the camera to the target position
        transform.position = targetPosition;

        // Optional: make the camera look at the player
        transform.LookAt(player);

        // Switch back to mouse input if there's movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            useController = false; // Revert to mouse when movement is detected
        }
    }
}
