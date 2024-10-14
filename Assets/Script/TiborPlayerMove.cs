using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiborPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of movement
    public float jumpForce = 5f; // Force applied when jumping
    private int jumpCount = 0; // Track the number of jumps
    private int maxJumps = 2; // Maximum number of jumps allowed

    private Rigidbody rb;
    private int rotationState = 0; // Track the rotation state of the player
    private Transform cameraTransform; // Reference to the camera transform
    private Vector3 platformVelocity; // Track the platform's velocity when on a platform
    private bool isOnPlatform = false;
    private PlatformMovement currentPlatform; // Reference to the current platform's movement script

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform; // Get the main camera's transform
    }

    void Update()
    {
        // Movement input
        float moveHorizontal = Input.GetAxis("Horizontal"); // For A and D or Left/Right Arrow keys
        float moveVertical = Input.GetAxis("Vertical"); // For W and S or Up/Down Arrow keys

        // Calculate movement direction relative to the camera's orientation
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Flatten the camera's forward and right vectors to ignore vertical rotation
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Create the movement direction based on camera orientation
        Vector3 movement = (cameraForward * moveVertical + cameraRight * moveHorizontal).normalized;

        // Continuously update the platform velocity if the player is on a platform
        if (isOnPlatform && currentPlatform != null)
        {
            platformVelocity = currentPlatform.GetPlatformVelocity();
        }
        else
        {
            platformVelocity = Vector3.zero; // Reset platform velocity when not on a platform
        }

        // Apply the movement to the player and add the platform's velocity if on a platform
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed) + platformVelocity;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset the Y velocity before applying the jump force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpCount++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player lands on the top of the platform to reset jump count
        bool landedOnTop = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                landedOnTop = true;
                break; // Exit the loop since we found a top contact point
            }
        }

        // Only reset the jump count if the player lands on top of the platform
        if (landedOnTop && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("RotatePlatformLeft") ||
            collision.gameObject.CompareTag("RotatePlatformRight") || collision.gameObject.CompareTag("RotatePlatformNormal")))
        {
            jumpCount = 0; // Reset the jump count to allow double jump again
        }

        // Handle platform movement
        if (landedOnTop && (collision.gameObject.CompareTag("RotatePlatformLeft") || collision.gameObject.CompareTag("RotatePlatformRight") ||
            collision.gameObject.CompareTag("RotatePlatformNormal")))
        {
            isOnPlatform = true; // Mark that the player is on a platform
            currentPlatform = collision.gameObject.GetComponent<PlatformMovement>();
        }

        // Handle rotation logic separately for RotatePlatformLeft, RotatePlatformRight, and RotatePlatformNormal
        if (collision.gameObject.CompareTag("RotatePlatformLeft") && rotationState != -90)
        {
            RotatePlayerCounterClockwise();
            rotationState = -90; // Update rotation state
        }
        else if (collision.gameObject.CompareTag("RotatePlatformNormal") && rotationState != 0)
        {
            RotatePlayerToNormal();
            rotationState = 0; // Update rotation state to the original position
        }
        else if (collision.gameObject.CompareTag("RotatePlatformRight") && rotationState != 90)
        {
            RotatePlayerClockwise();
            rotationState = 90; // Update rotation state
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Stop tracking the platform's velocity when the player leaves the platform
        if (collision.gameObject.CompareTag("RotatePlatformLeft") || collision.gameObject.CompareTag("RotatePlatformRight") ||
            collision.gameObject.CompareTag("RotatePlatformNormal"))
        {
            isOnPlatform = false; // Mark that the player is no longer on the platform
            currentPlatform = null; // Remove reference to the platform
            platformVelocity = Vector3.zero; // Reset the platform velocity
        }
    }

    private void RotatePlayerCounterClockwise()
    {
        // Rotate the player 90 degrees counterclockwise around the Y-axis
        transform.Rotate(0, -90, 0, Space.World);
    }

    private void RotatePlayerClockwise()
    {
        // Rotate the player 90 degrees clockwise around the Y-axis
        transform.Rotate(0, 90, 0, Space.World);
    }

    private void RotatePlayerToNormal()
    {
        // Reset the player rotation to the original state (0 degrees)
        if (rotationState == -90)
        {
            transform.Rotate(0, 90, 0, Space.World); // If the state is -90, rotate 90 degrees clockwise
        }
        else if (rotationState == 90)
        {
            transform.Rotate(0, -90, 0, Space.World); // If the state is 90, rotate 90 degrees counterclockwise
        }
    }
}
