using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayJumpBoost : MonoBehaviour
{
    [SerializeField] private float boostForce = 10f; // Force of the jump boost

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with the board
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

            // If the player has a Rigidbody, apply the boost force upward
            if (playerRigidbody != null)
            {
                playerRigidbody.AddForce(Vector3.up * boostForce, ForceMode.Impulse);
            }
        }
    }
}