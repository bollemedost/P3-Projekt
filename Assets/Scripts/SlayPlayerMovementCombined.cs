using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlayPlayerMovementCombined : MonoBehaviour
{
    // Movement Settings
    [SerializeField] private float speed = 5f; // Movement speed
    [SerializeField] private float jumpForce = 5f; // Force applied for jumping
    [SerializeField] private LayerMask groundLayer; // Layer used to detect the ground

    // Dash variables
    [SerializeField] private float dashSpeed = 15f; // Speed during the dash
    [SerializeField] private float dashDuration = 0.3f; // Duration of the dash
    [SerializeField] private float dashCooldown = 1f; // Cooldown time between dashes
    private bool isDashing = false;
    private float lastDashTime = 0f;

    // Ability States
    private bool isDoubleJumpActive = false;
    private bool isDashActive = false;

    private Vector3 movement;
    private Rigidbody rb;
    private bool isGrounded;
    private bool jumpRequest;
    private bool canDoubleJump;
    private TrailRenderer trailRenderer;

    // Camera and Rotation Settings
    [SerializeField] private Transform cameraPivot; // The pivot point for rotating the camera
    [SerializeField] private float rotationSpeed = 5f; // Speed of camera rotation
    [SerializeField] private float sensitivity = 0.5f; // Camera sensitivity to control rotation responsiveness
    [SerializeField] private float smoothTime = 0.1f; // Time for smooth damp effect
    private float cameraYaw; // Yaw rotation for left/right
    private float cameraPitch; // Pitch rotation for up/down
    private float targetYaw; // Target yaw for smoother transition
    private float targetPitch; // Target pitch for smoother transition
    private Vector3 smoothVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }
    void Start()
    {
        // Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnMovement(InputValue value)
    {
        movement = value.Get<Vector3>();
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

    private void OnLook(InputValue value)
    {
        Vector2 lookInput = value.Get<Vector2>();
        targetYaw += lookInput.x * sensitivity; // Apply sensitivity
        targetPitch -= lookInput.y * sensitivity;
        targetPitch = Mathf.Clamp(targetPitch, -30f, 60f); // Limit pitch to avoid flipping
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

        // Smoothly transition to target rotation
        cameraYaw = Mathf.Lerp(cameraYaw, targetYaw, rotationSpeed * Time.deltaTime);
        cameraPitch = Mathf.Lerp(cameraPitch, targetPitch, rotationSpeed * Time.deltaTime);

        // Rotate the camera pivot based on the smoothed yaw and pitch
        cameraPivot.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
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

            // Rotate the player to face the movement direction
            if (globalMovement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(globalMovement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            }
        }
    }

    public IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        trailRenderer.emitting = true;

        Vector3 dashDirection = (transform.right * movement.x + transform.forward * movement.z).normalized;

        if (dashDirection.magnitude == 0)
        {
            dashDirection = transform.forward;
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
