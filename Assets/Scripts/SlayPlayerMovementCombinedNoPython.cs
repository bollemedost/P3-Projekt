using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // For UI Text

public class SlayPlayerMovementCombinedNoPython : MonoBehaviour
{
    // Movement
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rotationSpeed = 10f; // Speed for smooth rotation
    private Vector3 movement;
    private Vector3 lastMovementDirection;
    private Rigidbody rb;
    private bool isGrounded;
    private const float GroundCheckDistance = 1.1f;

    //Dash
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private float lastDashTime = 0f;
    private bool isDashActive = false;
    private TrailRenderer trailRenderer;

    //Double Jump
    private bool isDoubleJumpActive = false;
    private bool jumpRequest;
    private bool canDoubleJump;

    //Smash
    [SerializeField] private CubeExplosion cubeExplosion;
    private bool isSmashActive = false; // Track if smash power-up is active

    //Level
    private int currentLevel = 1;
    private const int Level2Unlock = 2;
    private const int Level3Unlock = 3;
    
    //Animation
    private PlayerAnimationController animationHandler;

    // UI Elements for Power-Up Texts
    [SerializeField] private Text doubleJumpText;
    [SerializeField] private Text dashText;
    [SerializeField] private Text smashText;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        animationHandler = GetComponent<PlayerAnimationController>();

        SetLevelBasedOnScene();

        // Make sure the UI texts are hidden at the start
        doubleJumpText.gameObject.SetActive(false);
        dashText.gameObject.SetActive(false);
        smashText.gameObject.SetActive(false);
    }

    private void SetLevelBasedOnScene()
    {
        string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        if (activeSceneName == "scene brat slay brat slay purr")
        {
            currentLevel = Level2Unlock;
        }
        else if (activeSceneName == "Level 3 - Emil" || activeSceneName == "Level 4 - Aioli")
        {
            currentLevel = Level3Unlock;
        }
        else
        {
            currentLevel = 1;
        }

        UnityEngine.Debug.Log($"Active Scene: {activeSceneName}, Current Level: {currentLevel}");
    }

    private void OnMovement(InputValue value)
    {
        movement = value.Get<Vector3>();

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

                // Trigger double jump animation
                animationHandler.TriggerDoubleJump();
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
        var playerInput = GetComponent<PlayerInput>(); // Ensure PlayerInput component is referenced

        // Retrieve the input actions from the PlayerInput
        var inputActions = playerInput.actions;

        // Trigger power-up animation when the respective actions are performed
        if (currentLevel >= 1 && inputActions["DoubleJump"].WasPressedThisFrame())
        {
            ActivateDoubleJump();
            animationHandler.TriggerPowerUpAnimation();  // Trigger power-up animation
        }

        if (currentLevel >= Level2Unlock && inputActions["DashKey"].WasPressedThisFrame())
        {
            ActivateDash();
            animationHandler.TriggerPowerUpAnimation();  // Trigger power-up animation
        }

        if (currentLevel >= Level3Unlock && inputActions["SmashKey"].WasPressedThisFrame())
        {
            ActivateSmash();
        }

        if (isSmashActive && inputActions["Smash"].WasPressedThisFrame())
        {
            PerformSmash();
        }

        // Update animations
        animationHandler.UpdateAnimationStates(
            movement,
            isGrounded,
            jumpRequest && isGrounded,
            jumpRequest && !isGrounded && canDoubleJump,
            isDashing,
            isSmashActive && inputActions["Smash"].WasPressedThisFrame()
        );

        RotatePlayer(); // Handle player rotation
    }

    private void ActivateDoubleJump()
    {
        isDoubleJumpActive = true;
        isDashActive = false;
        UnityEngine.Debug.Log("Double Jump Activated");

        // Show Double Jump text
        ShowPowerUpText(doubleJumpText);
    }

    private void ActivateDash()
    {
        isDashActive = true;
        isDoubleJumpActive = false;
        UnityEngine.Debug.Log("Dash Activated");

        // Show Dash text
        ShowPowerUpText(dashText);
    }

    private void ActivateSmash()
    {
        isSmashActive = true;
        isDoubleJumpActive = false;
        isDashActive = false;
        UnityEngine.Debug.Log("Smash Activated");

        // Show Smash text
        ShowPowerUpText(smashText);
    }

    private void PerformSmash()
    {
        UnityEngine.Debug.Log("Performing Smash!");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f); // Adjust radius as needed

        foreach (var hitCollider in hitColliders)
        {
            CubeExplosion cubeExplosion = hitCollider.GetComponent<CubeExplosion>();
            if (cubeExplosion != null)
            {
                cubeExplosion.TryExplode();
            }
        }
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

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        // Trigger dash animation once
        animationHandler.TriggerDashAnimation();

        trailRenderer.emitting = true;

        // Use the player's facing direction for the dash
        Vector3 dashDirection = transform.forward;

        // Apply the dash force
        rb.velocity = Vector3.zero; // Reset velocity to ensure consistent dash speed
        rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        trailRenderer.emitting = false;
        isDashing = false;
    }

    private bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, GroundCheckDistance, groundLayer);
    }

    private void RotatePlayer()
    {
        if (movement != Vector3.zero)
        {
            // Calculate the desired rotation based on movement direction
            Vector3 movementDirection = Camera.main.transform.TransformDirection(movement);
            movementDirection.y = 0; // Ensure no vertical rotation
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // Method to show power-up text
    private void ShowPowerUpText(Text powerUpText)
    {
        powerUpText.gameObject.SetActive(true);
        StartCoroutine(HidePowerUpTextAfterDelay(powerUpText));
    }

    private IEnumerator HidePowerUpTextAfterDelay(Text powerUpText)
    {
        yield return new WaitForSeconds(1f);
        powerUpText.gameObject.SetActive(false);
    }
}