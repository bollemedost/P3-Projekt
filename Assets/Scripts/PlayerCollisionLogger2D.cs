using UnityEngine;

public class PlayerCollisionLogger2D : MonoBehaviour
{
    // Called when the player collides with another 2D collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("2D Collision detected with: " + collision.gameObject.name);
    }

    // Called when the player enters a 2D trigger collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("2D Trigger entered with: " + other.gameObject.name);
    }
}
