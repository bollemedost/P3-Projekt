using UnityEngine;  // Imports Unity's core functionality

// This class handles the explosion effect of a cube into smaller fragments
// It must be attached to a GameObject in Unity that you want to explode
public class CubeExplosion : MonoBehaviour
{
    // === CONFIGURATION VARIABLES ===
    // These can be adjusted in Unity's Inspector panel
    
    // The prefab (template) for the small cubes that will form the explosion
    // Must be assigned in Unity Inspector before running
    public GameObject cubeFragmentPrefab;
    
    // Determines how many pieces the cube splits into per axis
    // Example: 5 means 5x5x5 = 125 total fragments
    public int fragmentsPerAxis = 5;
    
    // The power of the explosion that pushes fragments outward
    // Higher values make fragments fly faster
    public float explosionForce = 50f;
    
    // How far the explosion effect reaches
    // Affects the area in which fragments will spread
    public float explosionRadius = 5f;
    
    // Minimum time before a fragment disappears
    // Helps manage performance by cleaning up fragments
    public float minDestroyTime = 0.1f;
    
    // Maximum time before a fragment disappears
    // Creates variety in how long fragments last
    public float maxDestroyTime = 2f;
    
    // How close the player needs to be to trigger the explosion
    public float triggerDistance = 2f;
    
    // Adjusts the overall force applied to fragments
    // Lets you fine-tune the explosion effect
    public float fragmentForceMultiplier = 0.2f;

    // === INTERNAL VARIABLES ===
    
    // Prevents the cube from exploding multiple times
    private bool exploded = false;
    
    // Reference to the player's movement script for position tracking
    private PlayerMovementCombined playerMovement;

    // === INITIALIZATION ===
    
    // Called when the script instance is being loaded
    private void Start()
    {
        // Find the player object in the scene using Unity's tag system
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        // If we found the player, get their movement script component
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovementCombined>();
        }
    }

    // === EXPLOSION TRIGGER LOGIC ===
    
    // Public method that can be called to attempt an explosion
    // Checks conditions before actually exploding
    public void TryExplode()
    {
        // If already exploded, stop here
        if (exploded) return;
        
        // Only explode if we have a player reference and they're in range
        if (playerMovement != null && IsPlayerInRange())
        {
            Explode();
        }
        else
        {
            Debug.Log("Player is not in range to trigger the explosion.");
        }
    }

    // Checks if the player is within the trigger distance
    private bool IsPlayerInRange()
    {
        // Get current player position
        Vector3 playerPosition = playerMovement.transform.position;
        
        // Calculate straight-line distance between player and this cube
        float distanceToPlayer = Vector3.Distance(playerPosition, transform.position);
        
        // Log the distance for debugging
        Debug.Log($"Distance to player: {distanceToPlayer}, Trigger Distance: {triggerDistance}");
        
        // Return true if player is close enough
        return distanceToPlayer <= triggerDistance;
    }

    // === MAIN EXPLOSION LOGIC ===
    
    // The core explosion method that creates the fragmentation effect
    public void Explode()
    {
        // Safety check to prevent multiple explosions
        if (exploded) return;
        
        // Mark as exploded immediately to prevent future explosions
        exploded = true;
        
        // Verify we have a fragment prefab to work with
        if (cubeFragmentPrefab == null)
        {
            Debug.LogError("CubeFragmentPrefab is not assigned!");
            return;
        }
        
        // Play explosion sound if audio manager exists
        if (AudioManagerSlay.Instance != null)
        {
            AudioManagerSlay.Instance.PlaySmashSound();
        }
        else
        {
            Debug.LogWarning("AudioManagerSlay instance is missing!");
        }

        // Get the size of the original cube using its renderer bounds
        Vector3 cubeSize = GetComponent<Renderer>().bounds.size;
        
        // Calculate how big each fragment should be
        // (original size divided by number of fragments per axis)
        Vector3 fragmentSize = cubeSize / fragmentsPerAxis;

        // === FRAGMENT CREATION LOOP ===
        // Create a 3D grid of fragments using nested loops
        for (int x = 0; x < fragmentsPerAxis; x++)
        {
            for (int y = 0; y < fragmentsPerAxis; y++)
            {
                for (int z = 0; z < fragmentsPerAxis; z++)
                {
                    // Calculate the position for this fragment
                    // Subtracting fragmentsPerAxis/2 centers the explosion around the original cube
                    Vector3 fragmentPosition = transform.position + new Vector3(
                        fragmentSize.x * (x - fragmentsPerAxis / 2),
                        fragmentSize.y * (y - fragmentsPerAxis / 2),
                        fragmentSize.z * (z - fragmentsPerAxis / 2)
                    );

                    // Create the fragment at the calculated position
                    GameObject fragment = Instantiate(cubeFragmentPrefab, fragmentPosition, Quaternion.identity);
                    
                    // Set the fragment's size
                    fragment.transform.localScale = fragmentSize;
                    
                    // Add physics to the fragment
                    Rigidbody rb = fragment.AddComponent<Rigidbody>();
                    
                    // Calculate direction for the explosion force
                    // This makes fragments fly away from the player's position
                    Vector3 forceDirection = (fragment.transform.position - playerMovement.transform.position).normalized;
                    
                    // Add randomness to make the explosion look more natural
                    float randomMultiplier = Random.Range(0.8f, 1.2f);
                    
                    // Apply the explosion force to the fragment
                    // ForceMode.Impulse applies the force instantly
                    rb.AddForce(forceDirection * explosionForce * fragmentForceMultiplier * randomMultiplier, ForceMode.Impulse);
                    
                    // Schedule the fragment for destruction after a random time
                    float randomDestroyTime = Random.Range(minDestroyTime, maxDestroyTime);
                    Destroy(fragment, randomDestroyTime);
                }
            }
        }

        // Destroy the original cube since it has been replaced by fragments
        Destroy(gameObject);
    }
}

// Made with the combined power of the human mind and computing power from ChatGPT
