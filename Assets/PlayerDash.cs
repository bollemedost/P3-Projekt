using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class PlayerDash : MonoBehaviour
{
    public float dashSpeed = 20f;         // Speed of the dash
    public float dashDuration = 0.2f;      // Duration of the dash
    public float dashCooldown = 1f;        // Cooldown between dashes
    private bool canDash = true;           // Whether the player can dash
    private Rigidbody rb;                  // Reference to the Rigidbody component
    private float lastDashTime;            // Time of the last dash

    void Start()
    {
        rb = GetComponent<Rigidbody>();     // Get the Rigidbody component
    }

    void Update()
    {
        // Check if the dash key (E) is pressed and the player can dash
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());          // Start the dash coroutine
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;                    // Disable dashing
        lastDashTime = Time.time;           // Record the current time

        // Get the forward direction of the player
        Vector3 dashDirection = transform.forward;

        // Set the velocity for the dash
        rb.velocity = dashDirection * dashSpeed;

        // Wait for the dash duration
        yield return new WaitForSeconds(dashDuration);

        // Stop the dash by resetting the velocity
        rb.velocity = Vector3.zero;

        // Enable dashing after the cooldown period
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;                     // Reset dashing ability
    }
}*/