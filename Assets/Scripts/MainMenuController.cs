using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // This method is linked to the "New Game" button
    public void NewGame()
    {
        // Delete all saved data in PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Make sure the deletion is saved

        Debug.Log("Game progress has been reset.");

        // Load the Level Map scene
        SceneManager.LoadScene(1); // 1 is the build index of the first scene
    }

    // Method to resume the game without resetting progress
    public void ResumeGame()
    {
        Debug.Log("Resuming game from the last saved state.");

        // Load the Level Map or the last unlocked level
        SceneManager.LoadScene(1); // Replace with your level map or hub scene
    }
}
