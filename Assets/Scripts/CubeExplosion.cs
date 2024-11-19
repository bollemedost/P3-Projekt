using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeExplosion : MonoBehaviour
{
    public GameObject cubeFragmentPrefab; // Reference to your small cube prefab
    public int fragmentsPerAxis = 5; // Number of small cubes per axis (e.g., 5x5x5 = 125 fragments)
    public float explosionForce = 50f; // Overall force applied to the fragments
    public float explosionRadius = 5f; // Radius of the explosion
    public float minDestroyTime = 0.1f; // Minimum time before fragment is destroyed
    public float maxDestroyTime = 2f; // Maximum time before fragment is destroyed
    public float triggerDistance = 2f; // Distance to trigger the explosion
    public float fragmentForceMultiplier = 0.2f; // Multiplier to control fragment force intensity

    private bool exploded = false; // To prevent multiple explosions
    private PlayerMovementCombined playerMovement; // Reference to the PlayerMovementCombined script

    private void Start()
    {
        // Find the player object with tag "Player" and get the PlayerMovementCombined component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovementCombined>();
        }
    }

    private void Update()
    {
        // Check if the player is close enough, the left mouse button is pressed, and Smash is activated
        if (playerMovement != null && IsPlayerInRange() && Input.GetKeyDown(KeyCode.E) && playerMovement.IsSmashActive())
        {
            Explode();
        }
    }

    private bool IsPlayerInRange()
    {
        // Calculate the distance between the player and the cube
        Vector3 playerPosition = playerMovement.transform.position;
        float distanceToPlayer = Vector3.Distance(playerPosition, transform.position);

        return distanceToPlayer <= triggerDistance;
    }

    public void Explode()
    {
        if (exploded) return; // Prevent multiple explosions
        exploded = true;

        Vector3 cubeSize = GetComponent<Renderer>().bounds.size; // Get size of the original cube
        Vector3 fragmentSize = cubeSize / fragmentsPerAxis; // Calculate size of each fragment

        for (int x = 0; x < fragmentsPerAxis; x++)
        {
            for (int y = 0; y < fragmentsPerAxis; y++)
            {
                for (int z = 0; z < fragmentsPerAxis; z++)
                {
                    // Calculate position for each fragment
                    Vector3 fragmentPosition = transform.position + new Vector3(
                        fragmentSize.x * (x - fragmentsPerAxis / 2),
                        fragmentSize.y * (y - fragmentsPerAxis / 2),
                        fragmentSize.z * (z - fragmentsPerAxis / 2)
                    );

                    // Instantiate the small cube prefab at the calculated position
                    GameObject fragment = Instantiate(cubeFragmentPrefab, fragmentPosition, Quaternion.identity);

                    // Adjust the scale of the fragment
                    fragment.transform.localScale = fragmentSize;

                    // Apply a subtle force away from the player
                    Rigidbody rb = fragment.AddComponent<Rigidbody>();
                    Vector3 forceDirection = (fragment.transform.position - playerMovement.transform.position).normalized;
                    float randomMultiplier = Random.Range(0.8f, 1.2f);
                    rb.AddForce(forceDirection * explosionForce * fragmentForceMultiplier * randomMultiplier, ForceMode.Impulse);

                    // Randomize the destruction time for each fragment between minDestroyTime and maxDestroyTime
                    float randomDestroyTime = Random.Range(minDestroyTime, maxDestroyTime);
                    Destroy(fragment, randomDestroyTime);
                }
            }
        }

        // Destroy the original large cube
        Destroy(gameObject);
    }
}
