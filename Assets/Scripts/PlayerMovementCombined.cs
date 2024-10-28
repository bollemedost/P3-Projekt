using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementCombined : MonoBehaviour
{
    // Movement Settings
    [SerializeField] private float speed = 5f; // Movement speed
    [SerializeField] private float jumpForce = 5f; // Force applied for jumping
    [SerializeField] private LayerMask groundLayer; // Layer used to detect the ground

    // Dash variables
    [SerializeField] private float dashSpeed = 15f; // Speed during the dash
    [SerializeField] private float dashDuration = 0.3f; // Duration of the dash
    [SerializeField] private float dashCooldown = 1f; // Cooldown time between dashes
    private bool isDashing = false; // Track if the player is currently dashing
    private float lastDashTime = 0f; // Time when the last dash occurred

    // Ability States
    private bool isDoubleJumpActive = false; // Is double jump active
    private bool isDashActive = false; // Is dash active

    private Vector3 movement; // Movement input
    private Rigidbody rb; // Rigidbody component
    private bool isGrounded; // Track if player is on the ground
    private bool jumpRequest; // Whether the player has pressed the jump button
    private bool canDoubleJump; // Track if player can perform a double jump
    private TrailRenderer trailRenderer; // Reference to the Trail Renderer

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        trailRenderer = GetComponent<TrailRenderer>(); // Get the Trail Renderer
    }

    private void OnMovement(InputValue value)
    {
        // Get the movement input from the Input System as a Vector3
        movement = value.Get<Vector3>();
    }

    private void OnJump(InputValue value)
    {
        // Handle jump or double jump only if double jump ability is active
        if (value.isPressed)
        {
            if (isGrounded) // First jump
            {
                jumpRequest = true;
            }
            else if (isDoubleJumpActive && canDoubleJump) // Double jump only if active
            {
                jumpRequest = true;
                canDoubleJump = false; // Disable double jump after use
            }
        }
    }

    private void OnDash(InputValue value)
    {
        // Handle dash only if dash ability is active
        if (value.isPressed && isDashActive && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void Update()
    {
        // Switch ability to double jump (J key)
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            ActivateDoubleJump();
        }

        // Switch ability to dash (K key)
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            ActivateDash();
        }
    }

    private void ActivateDoubleJump()
    {
        isDoubleJumpActive = true;
        isDashActive = false; // Disable dash when double jump is active
        Debug.Log("Double Jump Activated");
    }

    private void ActivateDash()
    {
        isDashActive = true;
        isDoubleJumpActive = false; // Disable double jump when dash is active
        Debug.Log("Dash Activated");
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
            rb.velocity = new Vector3(globalMovement.x * speed, rb.velocity.y, globalMovement.z * speed);

            // Handle jump logic
            if (jumpRequest)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset Y velocity
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply the jump force
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

    public IEnumerator Dash()
    {
        isDashing = true; // Set dashing state to true
        lastDashTime = Time.time; // Record the time of the dash

        // Enable the trail renderer
        trailRenderer.emitting = true;

        // Calculate dash direction based on player facing direction and input
        Vector3 dashDirection = (transform.right * movement.x + transform.forward * movement.z).normalized;

        if (dashDirection.magnitude == 0)
        {
            // Default to forward dash if no input is given
            dashDirection = transform.forward;
        }

        // Apply dash force in the calculated dash direction
        rb.AddForce(dashDirection * dashSpeed, ForceMode.Impulse);

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
