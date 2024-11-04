using UnityEngine;
using System.Collections;

public class Sleigh : MonoBehaviour
{
    public Vector3 slideDirection = new Vector3(1, 0, 0); // Slide direction (1, 0, 0) slides along the X-axis
    public float slideDistance = 3f; // Distance to slide
    public float slideSpeed = 2f; // Speed of the slide (units per second)
    public float shakeDuration = 0.2f; // Duration of the shake effect
    public float shakeIntensity = 0.1f; // Intensity of the shake effect

    private bool isSliding = false;
    private bool hasSlid = false; // Track if the slide has already occurred
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform playerTransform;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + slideDirection.normalized * slideDistance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Trigger slide only if the player lands on it, it hasn't slid yet, and it's not already sliding
        if (collision.gameObject.CompareTag("Player") && !isSliding && !hasSlid)
        {
            playerTransform = collision.transform;
            playerTransform.SetParent(transform); // Make the player a child of the object to move with it
            isSliding = true;
            hasSlid = true; // Mark the slide as completed
            StartCoroutine(SlideAndShake());
        }
    }

    private IEnumerator SlideAndShake()
    {
        float distanceToTravel = Vector3.Distance(startPosition, targetPosition);
        float totalDuration = distanceToTravel / slideSpeed; // Calculate time based on speed
        float elapsedTime = 0f;

        // Slide towards the target position
        while (elapsedTime < totalDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / totalDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it reaches the target position
        transform.position = targetPosition;

        // Shake effect
        float shakeTime = 0f;
        while (shakeTime < shakeDuration)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            transform.position = targetPosition + shakeOffset;
            shakeTime += Time.deltaTime;
            yield return null;
        }

        // Reset to final position after shake
        transform.position = targetPosition;
        playerTransform.SetParent(null); // Detach the player from the object
        isSliding = false; // Allow other interactions, if needed
    }

    // Reset method to set the sleigh back to its original position
    public void ResetPosition()
    {
        transform.position = startPosition; // Reset to the original starting position
        isSliding = false; // Reset sliding state
        hasSlid = false; // Allow it to slide again if needed
    }
}
