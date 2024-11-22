using UnityEngine;
using System.Collections;

public class SlayFallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f; // Time before the platform falls
    public float wobbleDuration = 0.5f; // Duration of the wobble effect
    public float wobbleIntensity = 0.1f; // Intensity of the wobble
    private Rigidbody rb;
    private bool isTriggered = false;

    private Vector3 originalPosition; // Store the platform's initial position

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Keep it kinematic at the start
        originalPosition = transform.position; // Save the starting position
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player has jumped on the platform
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(WobbleEffect());
            Invoke("Fall", fallDelay); // Start the fall after the delay
        }
    }

    IEnumerator WobbleEffect()
    {
        float elapsedTime = 0f;
        Vector3 currentPosition = transform.position;

        while (elapsedTime < wobbleDuration)
        {
            // Create a wobble effect by adding small random offsets to the platform's position
            float offsetX = Random.Range(-wobbleIntensity, wobbleIntensity);
            float offsetZ = Random.Range(-wobbleIntensity, wobbleIntensity);
            transform.position = currentPosition + new Vector3(offsetX, 0, offsetZ);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset the platform's position after wobble
        transform.position = currentPosition;
    }

    void Fall()
    {
        rb.isKinematic = false; // Make the platform fall
    }

    public void ResetPlatform()
    {
        rb.isKinematic = true; // Reset kinematic state
        transform.position = originalPosition; // Reset to the original position
        isTriggered = false; // Reset trigger state
    }
}
