using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of platform movement
    public float moveDistance = 3f; // Distance the platform moves left and right

    private Vector3 startPosition;
    private Vector3 lastPosition; // Track the platform's last position
    private Vector3 platformVelocity; // Track the platform's velocity
    private int direction = 1; // 1 for right, -1 for left

    void Start()
    {
        startPosition = transform.position; // Store the starting position of the platform
        lastPosition = startPosition; // Initialize lastPosition to the starting position
    }

    void Update()
    {
        // Calculate the new position of the platform
        float movement = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + new Vector3(movement * direction, 0, 0);

        // Calculate the platform's velocity
        platformVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position; // Update lastPosition to the current position
    }

    public Vector3 GetPlatformVelocity()
    {
        return platformVelocity;
    }
}
