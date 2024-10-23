using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlayPlayerMovement : MonoBehaviour
{
    public Vector3 movement;
    public Rigidbody myBody;
    public float speed = 5f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool jumpRequest;
    private bool canDoubleJump;

    // Dash variables
    [SerializeField] private float dashSpeed = 20f; // Dash speed (adjustable in Inspector)
    [SerializeField] private float dashDuration = 0.2f; // How long the dash lasts (adjustable in Inspector)
    [SerializeField] private float dashCooldown = 1f; // Time between dashes (adjustable in Inspector)
    private bool canDash = false; // Whether the player has unlocked the dash ability
    private bool isDashing = false; // Whether the player is currently dashing
    private float lastDashTime;
    private TrailRenderer trailRenderer; // Optional for dash effect

    // Last direction variables
    private Vector3 lastDirection;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false; // Disable trail renderer initially
        }
    }

    public void EnableDash()
    {
        canDash = true; // Enable the dash ability
    }

    private void OnMovement(InputValue value)
    {
        movement = value.Get<Vector3>();
        // Track last movement direction
        if (movement != Vector3.zero)
        {
            lastDirection = movement.normalized; // Keep the last direction pressed
        }
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isGrounded)
            {
                jumpRequest = true;
            }
            else if (canDoubleJump)
            {
                jumpRequest = true;
                canDoubleJump = false;
            }
        }
    }

    private void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing && Time.time > lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dashing..."); // Debug statement
        isDashing = true;
        lastDashTime = Time.time;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = true; // Enable trail during dash
        }

        // Use the last direction pressed for the dash
        Vector3 dashDirection = lastDirection; // Get the last movement direction
        dashDirection.y = 0; // Ensure the dash is horizontal
        myBody.velocity = dashDirection * dashSpeed; // Set the dash velocity

        yield return new WaitForSeconds(dashDuration);

        // Stop the dash after the duration
        myBody.velocity = new Vector3(0, myBody.velocity.y, 0); // Keep vertical velocity but reset horizontal velocity

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false; // Disable trail after dash
        }
        isDashing = false;
        Debug.Log("Dash ended."); // Debug statement
    }

   private void FixedUpdate()
{
    // Only apply movement if not dashing
    if (!isDashing)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 globalMovement = (forward * movement.z + right * movement.x).normalized;

        // Calculate new velocity without overwriting platform effects
        myBody.velocity = new Vector3(globalMovement.x * speed, myBody.velocity.y, globalMovement.z * speed);

        if (jumpRequest)
        {
            myBody.velocity = new Vector3(myBody.velocity.x, 0f, myBody.velocity.z); // Reset vertical velocity for consistent jumps
            myBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequest = false;
        }
    }

    isGrounded = CheckGrounded();

    if (isGrounded)
    {
        canDoubleJump = true; // Reset double jump if on the ground
    }
}    
private bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
}