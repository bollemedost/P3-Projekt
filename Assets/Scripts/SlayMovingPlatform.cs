using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayMovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(1, 0, 0); // Direction of platform movement
    public float speed = 2f; // Speed of the platform
    private Vector3 startPosition;
    private Vector3 lastPosition;

    private void Start()
    {
        startPosition = transform.position;
        lastPosition = startPosition;
    }

    private void FixedUpdate()
    {
        // Calculate the new position of the platform
        Vector3 newPosition = startPosition + moveDirection * Mathf.Sin(Time.time * speed);
        transform.position = newPosition;

        lastPosition = newPosition;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Move the player based on the platform's movement
                Vector3 platformMovement = transform.position - lastPosition;
                playerRb.MovePosition(playerRb.position + platformMovement);
            }
        }
    }
}
