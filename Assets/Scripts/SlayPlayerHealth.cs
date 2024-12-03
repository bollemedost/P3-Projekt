using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlayPlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health
    public int currentHealth;  // Current health
    public Image[] heartImages;   // Array of heart image UI elements
    public GameObject gameOverScreen; // Game over screen

    private Vector3 lastCheckpointPosition; // Player's last checkpoint position

    // Screen shake and flash settings
    public float shakeDuration = 0.2f; // Duration of screen shake
    public float shakeMagnitude = 0.3f; // Intensity of screen shake
    public Renderer playerRenderer; // Renderer for the player
    private Color originalColor; // Original color of the player

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        UpdateHeartSprites(); // Update heart sprites at the start

        // Hide Game Over screen at the start
        gameOverScreen.SetActive(false);

        // Store the original color for flashing effect
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }

        // Set the checkpoint to the player's initial position at the start
        lastCheckpointPosition = transform.position;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds

        UpdateHeartSprites(); // Update the heart sprites

        // Trigger screen shake and flash effects
        StartCoroutine(Shake(shakeDuration, shakeMagnitude));
        StartCoroutine(FlashRed());

        // Play damage sound
        AudioManagerSlay.Instance.PlayDamageSound(); // Call the damage sound method

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds
        UpdateHeartSprites(); // Update the heart sprites
    }

    void Die()
    {
        gameOverScreen.SetActive(true); // Show Game Over screen
        StartCoroutine(ReloadSceneAfterDelay(0.8f)); // Reload scene after delay
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnAtCheckpoint();
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        lastCheckpointPosition = checkpointPosition; // Update checkpoint
        Debug.Log("Checkpoint updated to: " + checkpointPosition);
    }

    void RespawnAtCheckpoint()
    {
        // Respawn player at the last checkpoint
        transform.position = lastCheckpointPosition;
        currentHealth = maxHealth; // Reset health
        UpdateHeartSprites(); // Update the heart sprites
        gameOverScreen.SetActive(false); // Hide Game Over screen

        // Reset all falling platforms
        SlayFallingPlatform[] platforms = FindObjectsOfType<SlayFallingPlatform>();
        foreach (SlayFallingPlatform platform in platforms)
        {
            platform.ResetPlatform();
        }

        // Reset sleigh position if it exists
        GameObject sleigh = GameObject.FindGameObjectWithTag("Sleigh");
        if (sleigh != null)
        {
            Sleigh sleighScript = sleigh.GetComponent<Sleigh>();
            if (sleighScript != null)
            {
                sleighScript.ResetPosition();
            }
        }
    }

    // Update Heart Sprites based on current health
    private void UpdateHeartSprites()
    {
        int fullHearts = currentHealth / (maxHealth / heartImages.Length); // Determine how many hearts are full
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < fullHearts)
            {
                heartImages[i].enabled = true; // Show the heart (full)
            }
            else
            {
                heartImages[i].enabled = false; // Hide the heart (empty)
            }
        }
    }

    // Screen Shake Coroutine
    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = originalPosition; // Reset to original position
    }

    // Flash Red Coroutine
    private IEnumerator FlashRed()
    {
        if (playerRenderer != null)
        {
            // Flash to red
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f); // Keep red for a moment

            // Restore original color
            playerRenderer.material.color = originalColor;
        }
    }
}
