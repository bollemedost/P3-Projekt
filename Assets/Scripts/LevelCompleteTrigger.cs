using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private int levelIndex; // The index of the level this collider is linked to

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Save the completion state
            PlayerPrefs.SetInt("Level" + levelIndex + "Completed", 2); // Use 2 to indicate completed
            PlayerPrefs.Save();

            Debug.Log($"Level {levelIndex} completed. Progress saved to PlayerPrefs.");

            // Load the Map scene (index 1)
            SceneManager.LoadScene(1);
        }
    }
}



//Partly used ChatGPT to generate the code snippet
