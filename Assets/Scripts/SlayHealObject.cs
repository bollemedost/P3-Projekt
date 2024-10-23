using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayHealObject : MonoBehaviour
{
    public int healAmount = 20;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        if (other.CompareTag("Player")) // Check if the other collider has the "Player" tag
        {
            SlayPlayerHealth playerHealth = other.GetComponent<SlayPlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
                Debug.Log("Healed player for: " + healAmount);
                Destroy(gameObject); // Optionally destroy the heal object after use
            }
            else
            {
                Debug.Log("PlayerHealth component not found on: " + other.gameObject.name);
            }
        }
    }
}
