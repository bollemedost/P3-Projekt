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

    private int currentLevel = 1; // Default level

    // Constants for level restrictions
    private const int Level2Unlock = 2; // Level where Dash unlocks
    private const int Level3Unlock = 3; // Level where Smash unlocks

    // Constants for grounded raycast
    private const float GroundCheckDistance = 1.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;

        SetLevelBasedOnScene(); // Automatically set level based on active scene
    }

    private void SetLevelBasedOnScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;

        if (activeSceneName == "scene brat slay brat slay purr")
        {
            currentLevel = Level2Unlock; // Set level 2
        }
        else if (activeSceneName == "Level 3 - Emil" || activeSceneName == "Level 4 - Aioli")
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
        // Check level restrictions
        if (currentLevel >= 1 && Keyboard.current.jKey.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log("J Key Pressed");
            StartCoroutine(HandleHandSign("DoubleJump"));
        }

        if (currentLevel >= Level2Unlock && Keyboard.current.kKey.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log("K Key Pressed");
            StartCoroutine(HandleHandSign("Dash"));
        }

        if (currentLevel >= Level3Unlock && Keyboard.current.lKey.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log("L Key Pressed");
            StartCoroutine(HandleHandSign("Smash"));
        }
    }

    private IEnumerator HandleHandSign(string action)
    {
        UnityEngine.Debug.Log($"HandleHandSign called for action: {action}");
        string detectedAction = LaunchPythonAndGetAction();

        if (detectedAction == action)
        {
            UnityEngine.Debug.Log($"Action Detected: {detectedAction}");
            if (action == "DoubleJump")
            {
                ActivateDoubleJump();
            }
            else if (action == "Dash")
            {
                ActivateDash();
            }
            else if (action == "Smash")
            {
                UnityEngine.Debug.Log("Smash Activated");
                // Add Smash functionality here if required
            }
        }
        else
        {
            UnityEngine.Debug.Log($"No matching action detected. Received: {detectedAction}");
        }

        yield return null;
    }

    private string LaunchPythonAndGetAction()
    {
        string pythonPath = "python"; // Ensure Python is in your system's PATH
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
        UnityEngine.Debug.Log("Double Jump Activated");
    }

    private void ActivateDash()
    {
        isDashActive = true;
        isDoubleJumpActive = false;
        UnityEngine.Debug.Log("Dash Activated");
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
}
