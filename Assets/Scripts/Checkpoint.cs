using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player reached the checkpoint
        if (other.CompareTag("Player"))
        {
            // Get the player's health script and update the checkpoint
            SlayPlayerHealth playerHealth = other.GetComponent<SlayPlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint reached!");
            }
        }
    }
}
