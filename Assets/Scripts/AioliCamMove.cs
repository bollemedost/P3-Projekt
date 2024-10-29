using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AioliCamMove : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player
    [SerializeField] private float sensitivityX = 10f; // Sensitivity of X-axis rotation
    [SerializeField] private float sensitivityY = 10f; // Sensitivity of Y-axis rotation
    [SerializeField] private float distanceFromPlayer = 5f; // Distance of the camera from the player
    [SerializeField] private float minYAngle = -20f; // Minimum Y-axis rotation angle
    [SerializeField] private float maxYAngle = 60f; // Maximum Y-axis rotation angle

    // Boundary Constraints
    [SerializeField] private float minX = -15f; // Minimum X position for camera
    [SerializeField] private float maxX = 26f;  // Maximum X position for camera
    [SerializeField] private float minY = -5f;   // Minimum Y position for camera
    [SerializeField] private float maxY = 48f;  // Maximum Y position for camera
    [SerializeField] private float minZ = 5f; // Minimum Z position for camera
    [SerializeField] private float maxZ = 18f;  // Maximum Z position for camera

    private float rotationX = 0f; // Current X-axis rotation
    private float rotationY = 0f; // Current Y-axis rotation

    private void Start()
    {
        // Initialize camera rotation based on current position
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        // Lock the cursor for better camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Mouse input for camera rotation
        float mouseX = Mouse.current.delta.x.ReadValue() * sensitivityX * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * sensitivityY * Time.deltaTime;

        // Adjust the rotation values based on mouse input
        rotationX += mouseX;
        rotationY -= mouseY;
        rotationY = Mathf.Clamp(rotationY, minYAngle, maxYAngle); // Clamp Y rotation to prevent flipping

        // Calculate the new camera position and apply boundary constraints
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 positionOffset = rotation * new Vector3(0, 0, -distanceFromPlayer);
        Vector3 targetPosition = player.position + positionOffset;

        // Apply constraints to keep camera within the defined boundaries
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);

        // Set the camera position and rotation
        transform.position = targetPosition;
        transform.LookAt(player.position);
    }
}
