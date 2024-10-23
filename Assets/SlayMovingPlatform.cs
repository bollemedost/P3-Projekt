using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayMovingPlatform : MonoBehaviour
{
    public enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private MovementDirection movementDirection = MovementDirection.Up; // Direction of movement
    [SerializeField] private float moveDistance = 5f; // Distance to move
    [SerializeField] private float moveSpeed = 2f; // Speed of movement

    private Vector3 startingPosition; // Starting position of the platform
    private Vector3 lastPosition; // Last position of the platform for movement calculations

    private void Start()
    {
        startingPosition = transform.position; // Save the starting position
        lastPosition = transform.position; // Initialize last position
    }

    private void Update()
    {
        // Calculate the movement for this frame
        float distanceCovered = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

        // Move the platform based on the selected direction
        switch (movementDirection)
        {
            case MovementDirection.Up:
                transform.position = startingPosition + Vector3.up * distanceCovered; // Move up
                break;
            case MovementDirection.Down:
                transform.position = startingPosition + Vector3.down * distanceCovered; // Move down
                break;
            case MovementDirection.Left:
                transform.position = startingPosition + Vector3.left * distanceCovered; // Move left
                break;
            case MovementDirection.Right:
                transform.position = startingPosition + Vector3.right * distanceCovered; // Move right
                break;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Check if the player is on the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the Rigidbody component of the player
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Calculate the platform's movement since the last frame
                Vector3 platformMovement = transform.position - lastPosition;

                // Apply the platform's velocity to the player
                playerRb.velocity = new Vector3(playerRb.velocity.x + platformMovement.x / Time.deltaTime, playerRb.velocity.y, playerRb.velocity.z + platformMovement.z / Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Optional: You can add any additional logic for when the player first steps on the platform
    }

    private void OnCollisionExit(Collision collision)
    {
        // Optional: You can add any additional logic for when the player leaves the platform
    }

    private void LateUpdate()
    {
        // Update the last position at the end of the frame
        lastPosition = transform.position;
    }
}
