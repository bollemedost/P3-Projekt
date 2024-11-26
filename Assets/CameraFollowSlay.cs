using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // The player's Transform
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10); // Offset from the player
    [SerializeField] private float followSpeed = 10f; // Speed of the camera following the player
    [SerializeField] private float rotationSpeed = 5f; // Speed of the camera's rotation adjustment

    private void LateUpdate()
    {
        // Smoothly move the camera to the target position
        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate the camera to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
