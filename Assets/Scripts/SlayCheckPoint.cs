using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayCheckPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SlayPlayerHealth playerHealth = other.GetComponent<SlayPlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.SetCheckpoint(transform.position);
                Debug.Log("Checkpoint reached at: " + transform.position);

                // Disable this checkpoint to prevent retriggers
                gameObject.SetActive(false);
            }
        }
    }
}
