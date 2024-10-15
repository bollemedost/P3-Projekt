using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayCameraMovement : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    private float playerHeight = 3.0f; // Height offset from the player
    [SerializeField] private float distanceFromPlayer = -5.0f; // Distance behind the player
    [SerializeField] private float mouseSensitivity = 2.0f; // Sensitivity of mouse movement

    private float rotationY = 0.0f; // Vertical rotation
    private float rotationX = 0.0f; // Horizontal rotation

    void Start()
    {
        // Lock the cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update rotation based on mouse movement
        rotationY -= mouseY; // Rotate up and down
        rotationY = Mathf.Clamp(rotationY, -30f, 30f); // Clamp vertical rotation to prevent flipping

        rotationX += mouseX; // Rotate left and right

        // Apply rotation to the camera
        transform.localEulerAngles = new Vector3(rotationY, rotationX, 0); // Apply both vertical and horizontal rotation

        // Calculate the desired position for the camera
        Vector3 targetPosition = player.position + transform.rotation * new Vector3(0, playerHeight, distanceFromPlayer);

        // Move the camera to the target position
        transform.position = targetPosition;

        // Optional: make the camera look at the player
        transform.LookAt(player);
    }
}
