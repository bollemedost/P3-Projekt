using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private int levelIndex; // The index of the level this collider is linked to

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger
        if (other.CompareTag("Player"))
        {
            // Save the completion of this level (e.g., Level 1)
            PlayerPrefs.SetInt("Level" + levelIndex + "Completed", 1); // Save the completion state
            PlayerPrefs.Save(); // Make sure to save the PlayerPrefs immediately

            // Optionally, log it for debugging
            Debug.Log($"Level {levelIndex} completed. Progress saved to PlayerPrefs.");

            // Load the Level Map scene (or any other scene you want to transition to)
            SceneManager.LoadScene(0); // Replace with the name or index of your level map scene
        }
    }
}


//Partly used ChatGPT to generate the code snippet
