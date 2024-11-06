using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SlayPlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;
    public GameObject gameOverScreen;
    //public Button restartButton;

    private Vector3 lastCheckpointPosition;

    // Screen shake and flash settings
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;
    public Renderer playerRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Hide the Game Over screen at the start
        gameOverScreen.SetActive(false);
        //restartButton.onClick.AddListener(RestartGame);

        // Set initial checkpoint to the player's starting position
        lastCheckpointPosition = transform.position;

        // Store the original color for flashing effect
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        // Trigger screen shake and flash effects
        StartCoroutine(Shake(shakeDuration, shakeMagnitude));
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;
    }

    void Die()
    {
        gameOverScreen.SetActive(true); // Show Game Over screen
        StartCoroutine(ReloadSceneAfterDelay(0.3f)); // Reload scene after a x-second delay
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    void RestartGame()
    {
        gameOverScreen.SetActive(false);
        RespawnAtCheckpoint();

        // Reset all sleigh objects
        foreach (Sleigh sleigh in FindObjectsOfType<Sleigh>())
        {
            sleigh.ResetPosition();
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        lastCheckpointPosition = checkpointPosition;
    }

    void RespawnAtCheckpoint()
    {
        transform.position = lastCheckpointPosition;
        currentHealth = maxHealth;
        healthBar.value = currentHealth;
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
            yield return null;
        }

        transform.position = originalPosition;
    }

    
    // Flash Red Coroutine
    private IEnumerator FlashRed()
    {
        if (playerRenderer != null)
        {
            // Store the current color directly before flashing
            originalColor = playerRenderer.material.color;

            // Flash to red
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f); // Keep red for a moment

            // Flash to the stored original color
            playerRenderer.material.color = originalColor; // Restore original color
        }
    }
}
