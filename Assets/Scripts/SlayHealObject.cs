using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayHealObject : MonoBehaviour
{
    public int healAmount = 25;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);

        if (other.CompareTag("Player")) // Check if the other collider has the "Player" tag
        {
            SlayPlayerHealth playerHealth = other.GetComponent<SlayPlayerHealth>();
            if (playerHealth != null)
            {
                // Check if the player is not at full health
                if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    playerHealth.Heal(healAmount); // Heal the player
                    Debug.Log("Healed player for: " + healAmount);
                    Destroy(gameObject); // Destroy the heal object after use
                }
                else
                {
                    Debug.Log("Player is at full health, cannot heal.");
                }
            }
            else
            {
                Debug.Log("PlayerHealth component not found on: " + other.gameObject.name);
            }
        }
    }
}
