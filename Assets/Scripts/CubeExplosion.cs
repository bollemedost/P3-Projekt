using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeExplosion : MonoBehaviour
{
    public GameObject cubeFragmentPrefab; // Reference to your small cube prefab
    public int fragmentsPerAxis = 5; // Number of small cubes per axis (e.g., 5x5x5 = 125 fragments)
    public float explosionForce = 50f; // Force applied to the small cubes
    public float explosionRadius = 5f; // Radius of the explosion
    public float minDestroyTime = 0.1f; // Minimum time before fragment is destroyed
    public float maxDestroyTime = 2f; // Maximum time before fragment is destroyed

    private bool exploded = false; // To prevent multiple explosions

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

                    // Apply a random explosion force to each fragment
                    Rigidbody rb = fragment.AddComponent<Rigidbody>();
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

                    // Randomize the destruction time for each fragment between minDestroyTime and maxDestroyTime
                    float randomDestroyTime = Random.Range(minDestroyTime, maxDestroyTime);
                    Destroy(fragment, randomDestroyTime);
                }
            }
        }

        // Destroy the original large cube
        Destroy(gameObject);
    }

    // This method listens for input to trigger the explosion
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click triggers the explosion
        {
            Explode();
        }
    }
}
