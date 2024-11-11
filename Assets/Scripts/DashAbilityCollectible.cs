using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbilityCollectible : MonoBehaviour
{
    // Reference to the SlayPlayerMovement script
    private SlayPlayerMovement playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger
        if (other.CompareTag("Player"))
        {
            // Get the SlayPlayerMovement component from the player
            playerMovement = other.GetComponent<SlayPlayerMovement>();

            if (playerMovement != null)
            {
                Debug.Log("Collectable collected!"); // Debug statement
                playerMovement.EnableDash(); // Call the method to enable dash
                Destroy(gameObject); // Remove the collectible
            }
        }
    }
}
