using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AioliPlayerMovement : MonoBehaviour
{
    private Vector3 movement; // Movement input (X, Z axis)
    private Rigidbody myBody; // Rigidbody component to move the player
    [SerializeField] private float speed = 5f;  // Movement speed

    [SerializeField] private float jumpForce = 5f; // Force applied for jumping
    [SerializeField] private LayerMask groundLayer; // Layer used to detect the ground

    private bool isGrounded; // Track if player is on the ground
    private bool jumpRequest; // Whether the player has pressed the jump/space button

    private void Awake()
    {
        myBody = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    private void OnMovement(InputValue value)
    {
        // Get the movement input from the Input System as a Vector3
        movement = value.Get<Vector3>();
    }

    private void OnJump(InputValue value)
    {
        // Trigger jump request when space is pressed and player is grounded
        if (value.isPressed && isGrounded)
        {
            jumpRequest = true;
        }
    }

    private void FixedUpdate()
    {
        // Preserve the Y-axis velocity (gravity) when moving on X and Z axes
        myBody.velocity = new Vector3(movement.x * speed, myBody.velocity.y, movement.z * speed);

        // Handle jump logic
        if (jumpRequest)
        {
            myBody.velocity = new Vector3(myBody.velocity.x, 0f, myBody.velocity.z); // Reset Y velocity
            myBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply the jump force
            jumpRequest = false; // Reset jump request after jump
        }

        // Check if player is grounded
        isGrounded = CheckGrounded();
    }

    private bool CheckGrounded()
    {
        // Raycast downwards to check if player is on the ground (adjust raycast distance for player height)
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
}

