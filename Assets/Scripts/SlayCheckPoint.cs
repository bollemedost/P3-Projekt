using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayCheckPoint : MonoBehaviour
{
    [SerializeField]
    private Vector3 spawnOffset = new Vector3(0, 1, 0); // Default offset (customizable in Inspector)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SlayPlayerHealth playerHealth = other.GetComponent<SlayPlayerHealth>();
            if (playerHealth != null)
            {
                // Set the checkpoint position + offset
                playerHealth.SetCheckpoint(transform.position + spawnOffset);
                Debug.Log("Checkpoint reached at: " + (transform.position + spawnOffset));
            }
        }
    }
}
