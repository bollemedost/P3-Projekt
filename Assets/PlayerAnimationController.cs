using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private bool hasJumped = false;  // To track if the jump animation has been triggered

    // Animation parameters
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int Smash = Animator.StringToHash("Smash");

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the player!");
        }
    }

    public void UpdateAnimationStates(
        Vector3 movement,
        bool isGrounded,
        bool isJumping,
        bool isDoubleJumping,
        bool isDashing,
        bool isSmashing)
    {
        // Set running state
        bool isRunning = movement.magnitude > 0.1f;
        animator.SetBool(IsRunning, isRunning);

        // Set grounded state
        animator.SetBool(IsGrounded, isGrounded);

        // Trigger animations
        if (isJumping && isGrounded && !hasJumped) // Trigger jump if grounded and hasn't jumped before
        {
            animator.SetTrigger(Jump);
            hasJumped = true;  // Mark that jump animation was triggered
            AudioManagerSlay.Instance.PlayJumpSound();  // Play jump sound using AudioManager
        }

        if (isDoubleJumping) // Trigger double jump
        {
            animator.SetTrigger(DoubleJump);
        }


        /*if (isDashing) // Trigger dash
        {
            animator.SetTrigger(Dash);
        }*/

        if (isSmashing) // Trigger smash
        {
            animator.SetTrigger(Smash);
           // AudioManagerSlay.Instance.PlaySmashSound();  // Play smash sound
        }

        // Reset the jump flag when the player lands
        if (isGrounded && hasJumped)
        {
            hasJumped = false;  // Reset jump flag when grounded
        }
    }

    public void TriggerDoubleJump()
    {
        animator.SetTrigger(DoubleJump);
        AudioManagerSlay.Instance.PlayDoubleJumpSound();  // Play double jump sound
    }

    public void TriggerPowerUpAnimation()
    {
        animator.SetTrigger("PowerUpActivated");  // Trigger the power-up activation animation
       AudioManagerSlay.Instance.PlayPowerUpSound();  // Play power-up sound
    }

public void SetDashing(bool isDashing)
{
    animator.SetBool(IsDashing, isDashing);
}
}
