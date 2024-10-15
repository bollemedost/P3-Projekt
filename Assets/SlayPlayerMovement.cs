using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlayPlayerMovement : MonoBehaviour
{
    public Vector3 movement; // Movement input (X, Z axis)
    public Rigidbody myBody; // Rigidbody component to move the player
    [SerializeField] private float speed = 5f;  // Movement speed

    [SerializeField] private float jumpForce = 5f; // Force applied for jumping
    [SerializeField] private LayerMask groundLayer; // Layer used to detect the ground

    private bool isGrounded; // Track if player is on the ground
    private bool jumpRequest; // Whether the player has pressed the jump/space button
    private bool canDoubleJump; // Track if player can perform a double jump

    // Dash variables
    [SerializeField] private float dashSpeed = 15f; // Speed during the dash
    [SerializeField] private float dashDuration = 0.3f; // Duration of the dash
    [SerializeField] private float dashCooldown = 1f; // Cooldown time between dashes
    private bool isDashing = false; // Track if the player is currently dashing
    private float lastDashTime = 0f; // Time when the last dash occurred

    // Trail Renderer
    [SerializeField] private TrailRenderer trailRenderer; // Reference to the Trail Renderer

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>(); // Get the Rigidbody component
        // Optionally, get the Trail Renderer if it's not set in the Inspector
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
    }

    private void OnMovement(InputValue value)
    {
        // Get the movement input from the Input System as a Vector3
        movement = value.Get<Vector3>();
    }

    private void OnJump(InputValue value)
    {
        // Trigger jump or double jump request
        if (value.isPressed)
        {
            if (isGrounded) // First jump
            {
                jumpRequest = true;
            }
            else if (canDoubleJump) // Double jump
            {
                jumpRequest = true;
                canDoubleJump = false; // Disable double jump after use
            }
        }
    }

    private void OnDash(InputValue value)
    {
        // Trigger dash request if input is pressed and cooldown is finished
        if (value.isPressed && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing) // Only move if not dashing
        {
            // Calculate movement direction relative to the camera
            Camera camera = Camera.main; // Assuming there's only one main camera
            Vector3 forward = camera.transform.forward; // Get camera forward direction
            Vector3 right = camera.transform.right; // Get camera right direction

            // Set the y component to zero for flat movement
            forward.y = 0; 
            right.y = 0; 

            // Normalize the directions
            forward.Normalize();
            right.Normalize();

            // Calculate global movement direction
            Vector3 globalMovement = (forward * movement.z + right * movement.x).normalized;

            // Preserve the Y-axis velocity (gravity) when moving on X and Z axes
            myBody.velocity = new Vector3(globalMovement.x * speed, myBody.velocity.y, globalMovement.z * speed);

            // Handle jump logic
            if (jumpRequest)
            {
                myBody.velocity = new Vector3(myBody.velocity.x, 0f, myBody.velocity.z); // Reset Y velocity
                myBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply the jump force
                jumpRequest = false; // Reset jump request after jump
            }

            // Check if player is grounded
            isGrounded = CheckGrounded();

            // Allow double jump if the player is airborne but not grounded
            if (isGrounded)
            {
                canDoubleJump = true; // Reset double jump when grounded
            }
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true; // Set dashing state to true
        lastDashTime = Time.time; // Record the time of the dash

        // Enable the trail renderer
        trailRenderer.emitting = true;

        // Apply dash force in the direction the player moved last
        Camera camera = Camera.main; // Assuming there's only one main camera
        Vector3 dashDirection = (camera.transform.forward * movement.z + camera.transform.right * movement.x).normalized;
        dashDirection.y = 0; // Prevent vertical movement during dash

        myBody.AddForce(dashDirection * dashSpeed, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration); // Wait for the duration of the dash

        // Disable the trail renderer after the dash
        trailRenderer.emitting = false;

        isDashing = false; // Reset dashing state after duration
    }

    private bool CheckGrounded()
    {
        // Raycast downwards to check if player is on the ground (adjust raycast distance for player height)
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
}
