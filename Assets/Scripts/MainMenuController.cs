using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
    
    public void ShowControllerScheme()
    {
        Debug.Log("Showing controller scheme.");

        // Find the parent object where the panel is, assuming it's inside a Canvas
        Transform canvasTransform = GameObject.Find("Canvas").transform;
        Transform controllerSchemePanelTransform = canvasTransform.Find("ControllerSchemePanel");
        Transform newGameTransform = canvasTransform.Find("New Game");
        Transform resumeGameTransform = canvasTransform.Find("Resume Game");
        Transform controllerSchemeTransform = canvasTransform.Find("Controller Scheme");
        Transform titleTransform = canvasTransform.Find("Title");

        if (controllerSchemePanelTransform != null)
        {
            controllerSchemePanelTransform.gameObject.SetActive(true); // Turn on the entire Panel (make it visible)
            newGameTransform.gameObject.SetActive(false); // Turn off the New Game button
            resumeGameTransform.gameObject.SetActive(false); // Turn off the Resume Game button
            controllerSchemeTransform.gameObject.SetActive(false); // Turn off the Controller Scheme button
            titleTransform.gameObject.SetActive(false); // Turn off the Title
        }
        else
        {
            Debug.LogError("ControllerSchemePanel not found in the Canvas.");
        }
    }

    // This method will hide the Controller Scheme by disabling the entire Panel
    public void HideControllerScheme()
    {
        Debug.Log("Hiding controller scheme.");

        // Find the parent object where the panel is, assuming it's inside a Canvas
        Transform canvasTransform = GameObject.Find("Canvas").transform;
        Transform controllerSchemePanelTransform = canvasTransform.Find("ControllerSchemePanel");
        Transform newGameTransform = canvasTransform.Find("New Game");
        Transform resumeGameTransform = canvasTransform.Find("Resume Game");
        Transform controllerSchemeTransform = canvasTransform.Find("Controller Scheme");
        Transform titleTransform = canvasTransform.Find("Title");

        if (controllerSchemePanelTransform != null)
        {
            controllerSchemePanelTransform.gameObject.SetActive(false); // Turn off the entire Panel (make it invisible)
            newGameTransform.gameObject.SetActive(true); // Turn on the New Game button
            resumeGameTransform.gameObject.SetActive(true); // Turn on the Resume Game button
            controllerSchemeTransform.gameObject.SetActive(true); // Turn on the Controller Scheme button
            titleTransform.gameObject.SetActive(true); // Turn on the Title
        }
        else
        {
            Debug.LogError("ControllerSchemePanel not found in the Canvas.");
        }
    }


    
}
