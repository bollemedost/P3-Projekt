using UnityEngine;

public class ButtonActivator : MonoBehaviour
{
    private bool isPlayerOnButton = false;
    public Transform platform; // Reference to the platform
    public float moveSpeed = 2.0f; // Speed of the platform

    private void Update()
    {
        if (isPlayerOnButton)
        {
            // Move the platform up based on the moveSpeed
            platform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure your player has the tag "Player"
        {
            isPlayerOnButton = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnButton = false;
        }
    }
}
