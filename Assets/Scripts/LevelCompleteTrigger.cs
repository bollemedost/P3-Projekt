using UnityEngine;
using UnityEngine.SceneManagement; // For scene management functions

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Level Completed"; // The name of the completion scene to load

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has entered the trigger
        if (other.CompareTag("Player"))
        {
            // Load the "Game Done" scene
            SceneManager.LoadScene(nextSceneName);
        }
    }
}


//Partly used ChatGPT to generate the code snippet
