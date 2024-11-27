using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovementCombined : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 movement;
    private Vector3 lastMovementDirection; // Store last movement direction
    private Rigidbody rb;
    private bool isGrounded;
    private PlayerInput playerInput;

    // Constants for grounded raycast
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
    private bool isSmashActive = false;
    [SerializeField] private CubeExplosion cubeExplosion; // Reference to the CubeExplosion script

   
    //Level
    private int currentLevel = 1; // Default level
    
    // Constants for level restrictions
    private const int Level2Unlock = 2; // Level where Smash unlocks (Level 3 - Emil)
    private const int Level3Unlock = 3; // Level where all actions unlock (scene brat slay brat slay purr and Level 4 - Aioli)

    //Animation
    private PlayerAnimationController animationHandler;
    [SerializeField] private float rotationSpeed = 10f; // Speed for smooth rotation


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        playerInput = GetComponent<PlayerInput>();
        animationHandler = GetComponent<PlayerAnimationController>();

        SetLevelBasedOnScene(); // Automatically set level based on active scene
    }

    private void SetLevelBasedOnScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;

        if (activeSceneName == "Level 3 - Emil")
        {
            currentLevel = Level2Unlock; // Set level 2
        }
        else if (activeSceneName == "scene brat slay brat slay purr" || activeSceneName == "Level 4 - Aioli")
        {
            currentLevel = Level3Unlock; // Set level 3 for Level 3 and Level 4
        }
        else
        {
            currentLevel = 1; // Default to level 1
        }

        UnityEngine.Debug.Log($"Active Scene: {activeSceneName}, Current Level: {currentLevel}");
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

    private void OnSmash(InputValue value)
    {
        if (value.isPressed && isSmashActive)
        {
            // Find nearby cubes and trigger explosion
            foreach (CubeExplosion cube in FindObjectsOfType<CubeExplosion>())
            {
                cube.TryExplode();
            }
        }
    }

    private void Update()
    {
        // Check level restrictions
        if (currentLevel == 1)
        {
            CheckInputAction("DoubleJump");
        }

        if (currentLevel == Level3Unlock)
        {
            CheckInputAction("DoubleJump");
            CheckInputAction("DashKey");
        }

        if (currentLevel >= Level2Unlock)
        {
            CheckInputAction("SmashKey");
            CheckInputAction("DoubleJump");
        }

        // Update animations
        animationHandler.UpdateAnimationStates(
            movement,
            isGrounded,
            jumpRequest && isGrounded,
            jumpRequest && !isGrounded && canDoubleJump,
            isDashing,
            isSmashActive && playerInput.actions["Smash"].WasPressedThisFrame()
        );

        RotatePlayer(); // Handle player rotation
    }

    private void CheckInputAction(string actionName)
    {
        // Check if the input action is triggered (pressed) based on the player's input device (keyboard, controller, etc.)
        if (playerInput.actions[actionName].triggered)
        {
            UnityEngine.Debug.Log($"{actionName} action triggered");
            StartCoroutine(HandleHandSign());
        }
    }
    
    private IEnumerator HandleHandSign()
    {
        UnityEngine.Debug.Log("HandleHandSign called");
        string detectedAction = LaunchPythonAndGetAction();

        UnityEngine.Debug.Log($"Action Detected: {detectedAction}");
        switch (detectedAction)
        {
            case "DoubleJump":
                ActivateDoubleJump();
                animationHandler.TriggerPowerUpAnimation();  // Trigger power-up animation
                break;
            case "Dash":
                ActivateDash();
                animationHandler.TriggerPowerUpAnimation();  // Trigger power-up animation
                break;
            case "Smash":
                ActivateSmash();
                animationHandler.TriggerPowerUpAnimation();  // Trigger power-up animation
                break;
            default:
                UnityEngine.Debug.Log($"No matching action detected. Received: {detectedAction}");
                break;
        }

        yield return null;
    }


    private string LaunchPythonAndGetAction()
    {
        string pythonPath = "/opt/anaconda3/bin/python3"; // Ensure Python is in your system's PATH
        string projectPath = Application.dataPath; // Path to the Assets folder
        string scriptPath = Path.Combine(projectPath, "PythonScript", "Hand_detection.py"); // Dynamically locate the Python script

        UnityEngine.Debug.Log($"Executing Python script at: {scriptPath}");

        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\"", // Ensure paths with spaces are handled
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        string detectedAction = "";
        try
        {
            using (Process process = Process.Start(psi))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    detectedAction = reader.ReadToEnd().Trim();
                }
            }

            UnityEngine.Debug.Log($"Python Script Output: {detectedAction}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Error running Python script: {e.Message}");
        }

        return detectedAction;
    }

    private void ActivateDoubleJump()
    {
        isDoubleJumpActive = true;
        isDashActive = false;
        isSmashActive = false;
        UnityEngine.Debug.Log("Double Jump Activated");

        // Trigger the power-up animation when Double Jump is activated
        animationHandler.TriggerPowerUpAnimation();
    }

    private void ActivateDash()
    {
        isDashActive = true;
        isDoubleJumpActive = false;
        isSmashActive = false;
        UnityEngine.Debug.Log("Dash Activated");

        // Trigger the power-up animation when Dash is activated
        animationHandler.TriggerPowerUpAnimation();
    }

    private void ActivateSmash()
    {
        isSmashActive = true;
        isDoubleJumpActive = false;
        isDashActive = false;
        UnityEngine.Debug.Log("Smash Activated");

        // Trigger the power-up animation when Smash is activated
        animationHandler.TriggerPowerUpAnimation();
    }

    public bool IsSmashActive()
    {
        return isSmashActive;
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
}
