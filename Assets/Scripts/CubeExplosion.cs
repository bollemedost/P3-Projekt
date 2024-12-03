using UnityEngine;

public class CubeExplosion : MonoBehaviour
{
    public GameObject cubeFragmentPrefab; // Reference to small cube prefab
    public int fragmentsPerAxis = 5; // Number of small cubes per axis
    public float explosionForce = 50f; // Force applied to fragments
    public float explosionRadius = 5f; // Radius of the explosion
    public float minDestroyTime = 0.1f; // Min time before fragment destruction
    public float maxDestroyTime = 2f; // Max time before fragment destruction
    public float triggerDistance = 2f; // Distance to trigger the explosion
    public float fragmentForceMultiplier = 0.2f; // Multiplier for fragment force

    private bool exploded = false; // To prevent multiple explosions
    private PlayerMovementCombined playerMovement; // Reference to player script

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovementCombined>();
        }
    }

    public void TryExplode()
    {
        if (exploded) return;

        if (playerMovement != null && IsPlayerInRange())
        {
            Explode();
        }
        else
        {
            Debug.Log("Player is not in range to trigger the explosion.");
        }
    }

    private bool IsPlayerInRange()
    {
        // Calculate distance between the player and the cube
        Vector3 playerPosition = playerMovement.transform.position;
        float distanceToPlayer = Vector3.Distance(playerPosition, transform.position);
        Debug.Log($"Distance to player: {distanceToPlayer}, Trigger Distance: {triggerDistance}");
        return distanceToPlayer <= triggerDistance;
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (cubeFragmentPrefab == null)
        {
            Debug.LogError("CubeFragmentPrefab is not assigned!");
            return;
        }
        
        // Play the smash sound
        if (AudioManagerSlay.Instance != null)
        {
            AudioManagerSlay.Instance.PlaySmashSound();
        }
        else
        {
            Debug.LogWarning("AudioManagerSlay instance is missing!");
        }

        Vector3 cubeSize = GetComponent<Renderer>().bounds.size;
        Vector3 fragmentSize = cubeSize / fragmentsPerAxis;

        for (int x = 0; x < fragmentsPerAxis; x++)
        {
            for (int y = 0; y < fragmentsPerAxis; y++)
            {
                for (int z = 0; z < fragmentsPerAxis; z++)
                {
                    Vector3 fragmentPosition = transform.position + new Vector3(
                        fragmentSize.x * (x - fragmentsPerAxis / 2),
                        fragmentSize.y * (y - fragmentsPerAxis / 2),
                        fragmentSize.z * (z - fragmentsPerAxis / 2)
                    );

                    GameObject fragment = Instantiate(cubeFragmentPrefab, fragmentPosition, Quaternion.identity);
                    fragment.transform.localScale = fragmentSize;

                    Rigidbody rb = fragment.AddComponent<Rigidbody>();
                    Vector3 forceDirection = (fragment.transform.position - playerMovement.transform.position).normalized;
                    float randomMultiplier = Random.Range(0.8f, 1.2f);
                    rb.AddForce(forceDirection * explosionForce * fragmentForceMultiplier * randomMultiplier, ForceMode.Impulse);

                    float randomDestroyTime = Random.Range(minDestroyTime, maxDestroyTime);
                    Destroy(fragment, randomDestroyTime);
                }
            }
        }

        Destroy(gameObject);
    }
}
