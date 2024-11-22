using UnityEngine;
using UnityEngine.InputSystem;

public class CameraPanHandler : MonoBehaviour
{
    [SerializeField] private Transform target;       // Reference to the target the camera follows
    [SerializeField] private float panSensitivity = 2f; // Sensitivity for panning the camera
    [SerializeField] private float panLimit = 45f;      // Maximum panning angle

    private float currentYaw = 0f; // Tracks the current yaw angle of the camera

    private void LateUpdate()
    {
        // Apply the rotation around the target
        if (target != null)
        {
            Quaternion rotation = Quaternion.Euler(0, currentYaw, 0);
            transform.position = target.position - (rotation * Vector3.forward * 10) + Vector3.up * 5; // Adjust these values for the desired position
            transform.LookAt(target);
        }
    }

    public void OnLook(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        // Adjust yaw based on horizontal input
        currentYaw += input.x * panSensitivity;

        // Clamp the yaw angle to the specified limits
        currentYaw = Mathf.Clamp(currentYaw, -panLimit, panLimit);
    }
}
