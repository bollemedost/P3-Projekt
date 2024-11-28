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
    [SerializeField] private string ignoreTag = "IgnoreCamera"; // Tag for objects to be ignored by the camera

    private Quaternion targetRotation; // Target rotation for the camera
    private float currentPan = 0f; // Current Y-axis pan value based on input
    private float targetPan = 0f; // Target Y-axis pan value for smoothing
    private float currentPitch = 0f; // Current X-axis (vertical) rotation value
    private float targetPitch = 0f; // Target X-axis (vertical) rotation value for smoothing

    private Vector3 currentVelocity = Vector3.zero; // For SmoothDamp

    private PlayerInput playerInput; // Reference to PlayerInput component
    private InputAction lookAction; // Input action for camera look

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        targetRotation = Quaternion.Euler(0, 0, 0); // Initial target rotation to face the initial direction
        UpdateCameraPosition(); // Set initial position based on offset

        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions["Look"]; // Ensure this matches your Input System action
    }

    private void LateUpdate()
    {
        // Smoothly interpolate towards the target rotation (yaw + pitch)
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        UpdateCameraPosition();
    }

    private void Update()
    {
        // Get input from the Input System
        Vector2 lookDelta = lookAction.ReadValue<Vector2>();

        // Adjust target pan and pitch based on input
        targetPan += lookDelta.x * smoothingFactor;
        targetPitch -= lookDelta.y * smoothingFactor;

        // Clamp both horizontal (yaw) and vertical (pitch) rotations
        targetPan = Mathf.Clamp(targetPan, panClampMin, panClampMax);
        targetPitch = Mathf.Clamp(targetPitch, verticalClampMin, verticalClampMax);

        // Smoothly interpolate the current pan and pitch values towards their respective targets
        currentPan = Mathf.Lerp(currentPan, targetPan, Time.deltaTime * rotationSpeed);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * rotationSpeed);

        // Update target rotation
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
            // Ignore colliders with the specified tag
            if (hit.collider.CompareTag(ignoreTag))
            {
                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPosition, ref currentVelocity, positionSmoothingFactor);
            }
            else
            {
                // Position the camera slightly before the point of impact
                cameraTransform.position = hit.point - (desiredPosition - transform.position).normalized * bufferDistance;
            }
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
