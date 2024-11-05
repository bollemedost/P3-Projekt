using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementCombined : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private float lastDashTime = 0f;

    private bool isDoubleJumpActive = false;
    private bool isDashActive = false;

    private Vector3 movement;
    private Vector3 lastMovementDirection; // Store last movement direction
    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpRequest;
    private bool canDoubleJump;
    private TrailRenderer trailRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }

    private void OnMovement(InputValue value)
    {
        movement = value.Get<Vector3>();

        // Update last movement direction if there is input
        if (movement != Vector3.zero)
        {
            lastMovementDirection = movement.normalized;
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
            else if (isDoubleJumpActive && canDoubleJump)
            {
                jumpRequest = true;
                canDoubleJump = false;
            }
        }
    }

    private void OnDash(InputValue value)
    {
        if (value.isPressed && isDashActive && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void Update()
    {
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            ActivateDoubleJump();
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            ActivateDash();
        }
    }

    private void ActivateDoubleJump()
    {
        isDoubleJumpActive = true;
        isDashActive = false;
        Debug.Log("Double Jump Activated");
    }

    private void ActivateDash()
    {
        isDashActive = true;
        isDoubleJumpActive = false;
        Debug.Log("Dash Activated");
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            Camera camera = Camera.main;
            Vector3 forward = camera.transform.forward;
            Vector3 right = camera.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 globalMovement = (forward * movement.z + right * movement.x).normalized;

            rb.velocity = new Vector3(globalMovement.x * speed, rb.velocity.y, globalMovement.z * speed);

            if (jumpRequest)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpRequest = false;
            }

            isGrounded = CheckGrounded();

            if (isGrounded)
            {
                canDoubleJump = true;
            }
        }
    }

    public IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        trailRenderer.emitting = true;

        Vector3 dashDirection;
        
        // Use the last movement direction as the dash direction
        if (lastMovementDirection != Vector3.zero)
        {
            dashDirection = (transform.right * lastMovementDirection.x + transform.forward * lastMovementDirection.z).normalized;
        }
        else
        {
            dashDirection = transform.forward; // Default to forward if no last input
        }

        rb.AddForce(dashDirection * dashSpeed, ForceMode.Impulse);

        yield return new WaitForSeconds(dashDuration);

        trailRenderer.emitting = false;

        isDashing = false;
    }

    private bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
}
