using System.Collections;
using UnityEngine;

public class IcicleSpawner : MonoBehaviour
{
    public GameObject iciclePrefab; // Assign your icicle prefab in the inspector
    public Transform[] spawnPoints; // Assign spawn points in the inspector
    public float minTimeBetweenSpawns = 2f; // Minimum time before the next icicle spawns
    public float maxTimeBetweenSpawns = 5f; // Maximum time before the next icicle spawns

    private void Start()
    {
        StartCoroutine(SpawnIcicles());
    }

    private IEnumerator SpawnIcicles()
    {
        while (true) // Infinite loop to continuously spawn icicles
        {
            // Choose a random spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // Instantiate the icicle at the chosen spawn point
            Instantiate(iciclePrefab, spawnPoint.position, spawnPoint.rotation);

            // Wait for a random time between the specified minimum and maximum
            float waitTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
