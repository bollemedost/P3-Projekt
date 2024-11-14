using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChanger : MonoBehaviour
{
    public int levelIndex; // The index of the level this collider is linked to
    public int requiredLevelIndex; // The index of the required level for this level to be accessible

    private bool playerInTrigger = false;

    private void Start()
    {
        requiredLevelIndex = levelIndex - 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("2D Player"))
        {
            playerInTrigger = true;

            // Check if player has completed the required level
            bool hasCompletedRequiredLevel = PlayerPrefs.GetInt("Level" + requiredLevelIndex + "Completed", 0) == 1;

            if (hasCompletedRequiredLevel || levelIndex == 1) // Level 1 is always accessible
            {
                Debug.Log($"Player entered level {levelIndex} trigger. Press 'X' to enter level.");
            }
            else
            {
                Debug.Log($"Level {levelIndex} is locked. Complete level {requiredLevelIndex} first.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("2D Player"))
        {
            playerInTrigger = false;
        }
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.X))
        {
            bool hasCompletedRequiredLevel = PlayerPrefs.GetInt("Level" + requiredLevelIndex + "Completed", 0) == 1;

            if (hasCompletedRequiredLevel || levelIndex == 1) // Level 1 is always accessible
            {
                // Save player's position ONLY when entering a level from the map
                Vector3 playerPosition = GameObject.FindGameObjectWithTag("2D Player").transform.position;
                PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
                PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
                PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
                PlayerPrefs.Save();

                // Load the level
                Debug.Log("Loading level " + levelIndex);
                SceneManager.LoadScene(levelIndex);
            }
            else
            {
                Debug.Log($"You must complete level {requiredLevelIndex} before accessing this level.");
            }
        }
    }
}
