using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotationHandler : MonoBehaviour
{
    private Transform cameraTransform; // Reference to the camera transform

    [SerializeField] private Vector3 cameraOffset = new Vector3(0, -10, -10); // Offset of the camera relative to the player
    [SerializeField] private float rotationSpeed = 5f; // Speed of the camera rotation smoothing
    [SerializeField] private float panClampMin = -45f; // Minimum Y-axis rotation (side-to-side)
    [SerializeField] private float panClampMax = 45f; // Maximum Y-axis rotation (side-to-side)
    [SerializeField] private float verticalClampMin = -30f; // Minimum X-axis rotation (up)
    [SerializeField] private float verticalClampMax = 60f; // Maximum X-axis rotation (down)
    [SerializeField] private float smoothingFactor = 0.1f; // Smoothing factor for input
    [SerializeField] private float positionSmoothingFactor = 0.1f; // Smoothing for camera movement

    private Quaternion targetRotation; // Target rotation for the camera
    private float currentPan = 0f; // Current Y-axis pan value based on input
    private float targetPan = 0f; // Target Y-axis pan value for smoothing
    private float currentPitch = 0f; // Current X-axis (vertical) rotation value
    private float targetPitch = 0f; // Target X-axis (vertical) rotation value for smoothing

    private Vector3 currentVelocity = Vector3.zero; // For SmoothDamp

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        targetRotation = Quaternion.Euler(0, 0, 0); // Initial target rotation to face the initial direction
        UpdateCameraPosition(); // Set initial position based on offset
    }

    private void LateUpdate()
    {
        // Smoothly interpolate towards the target rotation (yaw + pitch)
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        UpdateCameraPosition();
    }

    // This method will be called from the input system
    public void OnLook(InputValue value)
    {
        Vector2 lookDelta = value.Get<Vector2>();

        // Adjust the target pan value based on the input delta for side-to-side movement
        targetPan += lookDelta.x * smoothingFactor; // Apply smoothing to the mouse input (yaw)

        // Adjust the target pitch value based on the input delta for up and down movement
        targetPitch -= lookDelta.y * smoothingFactor; // Apply smoothing to the mouse input (pitch)

        // Clamp both horizontal (yaw) and vertical (pitch) rotations
        targetPan = Mathf.Clamp(targetPan, panClampMin, panClampMax);
        targetPitch = Mathf.Clamp(targetPitch, verticalClampMin, verticalClampMax);
    }

    private void Update()
    {
        // Smoothly interpolate the current pan and pitch values towards their respective targets
        currentPan = Mathf.Lerp(currentPan, targetPan, Time.deltaTime * rotationSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * rotationSpeed);

        // Update target rotation, including yaw (y) and pitch (x)
        SetTargetRotation(currentPan, currentPitch);
    }

    private void SetTargetRotation(float yRotation, float xRotation)
    {
        // Set the target rotation based on the specified y-rotation (yaw) and x-rotation (pitch)
        targetRotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    private void UpdateCameraPosition()
    {
        // Calculate the desired camera position based on the offset and rotation
        Vector3 desiredPosition = transform.position + targetRotation * cameraOffset;

        // Define a buffer distance to stop the camera before the collider
        float bufferDistance = 0.5f;

        // Use raycasting to check for obstacles between the player and the desired camera position
        RaycastHit hit;
        if (Physics.Linecast(transform.position, desiredPosition, out hit))
        {
            // If there is an obstacle, position the camera slightly before the point of impact
            cameraTransform.position = hit.point - (desiredPosition - transform.position).normalized * bufferDistance;
        }
        else
        {
            // Smoothly move the camera to the new position using SmoothDamp
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPosition, ref currentVelocity, positionSmoothingFactor);
        }

        // Ensure the camera is always looking at the player
        cameraTransform.LookAt(transform.position);
    }
}
