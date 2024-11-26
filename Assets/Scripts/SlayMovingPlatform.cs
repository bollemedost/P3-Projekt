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
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // Calculate new platform position
        Vector3 newPosition = startPosition + moveDirection * Mathf.Sin(Time.time * speed);
        Vector3 platformVelocity = (newPosition - lastPosition) / Time.fixedDeltaTime;
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
                // Apply the platform's movement to the player's position
                Vector3 platformMovement = transform.position - lastPosition;
                playerRb.MovePosition(playerRb.position + platformMovement);
            }
        }
    }
}
