using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace

public class NewSlayCamera : MonoBehaviour
{
    private Transform cameraTransform;       // Reference to the camera transform
    private int rotationState = 0;           // Track the rotation state of the player (0, 90, -90)

    [SerializeField] private Vector3 cameraOffset = new Vector3(0, -10, -10); // Offset of the camera relative to the player
    [SerializeField] private float rotationSpeed = 5f; // Speed of the camera rotation smoothing
    [SerializeField] private float panSpeed = 2f;      // Speed of the panning effect
    [SerializeField] private float panLimit = 5f;      // Maximum panning distance from the target

    private Quaternion targetRotation; // Target rotation for the camera
    private Vector3 panOffset = Vector3.zero; // Panning offset

    private Vector2 panInput; // Input for panning

    void Start()
    {
        cameraTransform = Camera.main.transform;
        targetRotation = Quaternion.Euler(0, 0, 0); // Initial target rotation to face the initial direction
        UpdateCameraPosition(); // Set initial position based on offset
    }

    void LateUpdate()
    {
        // Smoothly interpolate towards the target rotation
        cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Update the pan offset based on input
        panOffset.x = Mathf.Clamp(panOffset.x + panInput.x * panSpeed * Time.deltaTime, -panLimit, panLimit);
        panOffset.z = Mathf.Clamp(panOffset.z + panInput.y * panSpeed * Time.deltaTime, -panLimit, panLimit);

        UpdateCameraPosition();
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool landedOnTop = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                landedOnTop = true;
                break;
            }
        }

        // Only respond to rotation platforms if landed on top
        if (landedOnTop)
        {
            HandleRotationPlatform(collision);
        }
    }

    private void HandleRotationPlatform(Collision collision)
    {
        if (collision.gameObject.CompareTag("RotatePlatformLeft") && rotationState != -90)
        {
            SetTargetRotation(-90);
            rotationState = -90;
        }
        else if (collision.gameObject.CompareTag("RotatePlatformNormal") && rotationState != 0)
        {
            SetTargetRotation(0);
            rotationState = 0;
        }
        else if (collision.gameObject.CompareTag("RotatePlatformRight") && rotationState != 90)
        {
            SetTargetRotation(90);
            rotationState = 90;
        }
    }

    private void SetTargetRotation(float yRotation)
    {
        // Set the target rotation based on the specified y-rotation angle around the player
        targetRotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void UpdateCameraPosition()
    {
        // Calculate the camera's position based on the offset, pan offset, and target rotation
        Vector3 desiredPosition = transform.position + targetRotation * (cameraOffset + panOffset);
        cameraTransform.position = desiredPosition;
        cameraTransform.LookAt(transform.position);
    }

    // Input System callback for camera panning
    public void OnCameraPan(InputAction.CallbackContext context)
    {
        panInput = context.ReadValue<Vector2>();
    }
}
