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

    private void Start()
    {
        startingPosition = transform.position; // Save the starting position
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

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player stepped on the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // Make the player a child of the platform
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the player stepped off the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // Remove the player from the platform's hierarchy
        }
    }
}
