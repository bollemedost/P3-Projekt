using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private int levelIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("Level" + levelIndex + "Completed", 2); // Mark level as completed
            PlayerPrefs.Save();
            Debug.Log($"Level {levelIndex} marked as completed in PlayerPrefs.");

            SceneManager.LoadScene(1); // Return to map scene
        }
    }
}

