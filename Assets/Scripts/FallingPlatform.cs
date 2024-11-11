using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f; // Time before the platform falls
    public float resetDelay = 3f; // Time before the platform resets
    private Rigidbody rb;
    private bool isTriggered = false;
    
    // Reference to the particle system
    public ParticleSystem fallEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Keep it kinematic at the start
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player has jumped on the platform
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            Invoke("Fall", fallDelay); // Start the fall after the delay
        }
    }

    void Fall()
    {
        rb.isKinematic = false; // Make the platform fall
        fallEffect.transform.position = transform.position; // Position the particle effect
        fallEffect.Play(); // Play the particle effect
        Invoke("ResetPlatform", resetDelay); // Reset the platform after the delay
    }

    void ResetPlatform()
    {
        rb.isKinematic = true; // Reset kinematic state
        transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z); // Adjust to reset position
        isTriggered = false; // Reset trigger state
    }
}
