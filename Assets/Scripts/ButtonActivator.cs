using UnityEngine;

public class ButtonActivator : MonoBehaviour
{
    private bool isPlayerOnButton = false;
    public Transform platform; // Reference to the platform
    public float moveSpeed = 2.0f; // Speed of the platform
    public Transform button; // Reference to the button object
    public float buttonPressDepth = 0.2f; // Depth the button presses down

    private Vector3 initialButtonPosition; // To store the button's original position

    private void Start()
    {
        // Store the initial position of the button
        if (button != null)
        {
            initialButtonPosition = button.position;
        }
    }

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

            // Press the button down
            if (button != null)
            {
                button.position = initialButtonPosition + Vector3.down * buttonPressDepth;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnButton = false;

            // Reset the button to its original position
            if (button != null)
            {
                button.position = initialButtonPosition;
            }
        }
    }
}
