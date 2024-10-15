using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlayPlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar; // Reference to the UI Slider
    public GameObject gameOverScreen; // Reference to the Game Over UI
    public Button restartButton; // Reference to the Restart Button

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        Debug.Log("Health Bar Set Up, Current Health: " + currentHealth);

        // Hide the Game Over screen at the start
        gameOverScreen.SetActive(false);
        restartButton.onClick.AddListener(RestartGame); // Add listener to restart button
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Player took damage: " + damage);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        Debug.Log("Player healed: " + amount);
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;
    }

    void Die()
    {
        // Logic for player death
        Debug.Log("Player Died!");
        gameOverScreen.SetActive(true); // Show Game Over screen
        Destroy(gameObject); // Destroy the player GameObject
    }

    void RestartGame()
    {
        // Logic to restart the game, such as loading the scene again
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
