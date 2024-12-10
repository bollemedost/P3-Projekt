using System.Collections;
using UnityEngine;

public class SlayPlatformWobble : MonoBehaviour
{
    public float wobbleStrength = 0.1f;  // The strength of the wobble (higher is more intense)
    public float wobbleDuration = 0.5f;  // How long the wobble lasts
    public float wobbleSpeed = 5f;  // Speed at which the platform wobbles
    private Vector3 originalPosition;
    private bool isWobbling = false;
    private Transform player;  // Reference to the player’s transform
    private bool playerOnPlatform = false;  // Flag to track if player is on the platform

    void Start()
    {
        originalPosition = transform.position;  // Store the initial position of the platform
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.transform;
            playerOnPlatform = true;

            if (!isWobbling)
            {
                isWobbling = true;
                StartCoroutine(Wobble());
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
        }
    }

    private IEnumerator Wobble()
    {
        float elapsedTime = 0f;

        // While the wobble duration is not over, move the platform and the player
        while (elapsedTime < wobbleDuration)
        {
            // Create subtle back and forth motion (side-to-side) and up and down motion
            float xOffset = Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength;  // Horizontal wobble (back and forth)
            float yOffset = Mathf.Cos(Time.time * wobbleSpeed) * wobbleStrength;  // Vertical wobble (up and down)

            // Apply the wobble to the platform's position
            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);

            // If the player is on the platform, move them along with it
            if (playerOnPlatform)
            {
                player.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the platform's position after wobble is finished
        transform.position = originalPosition;

        // Optionally, reset player’s position to original position if needed
        if (playerOnPlatform)
        {
            player.position = new Vector3(player.position.x, player.position.y, player.position.z);
        }

        isWobbling = false;
    }
}