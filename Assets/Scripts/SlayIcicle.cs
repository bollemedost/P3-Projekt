using UnityEngine;

public class SlayIcicle : MonoBehaviour
{
    public GameObject breakingEffectPrefab; // Assign a particle system or shatter prefab

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the icicle hits the ground
        if (collision.gameObject.CompareTag("groundLayerAioli"))
        {
            // Spawn breaking effect
            if (breakingEffectPrefab != null)
            {
                Instantiate(breakingEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the icicle
            Destroy(gameObject);
        }
    }
}
