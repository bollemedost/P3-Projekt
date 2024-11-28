using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Required for the new Input System

public class TriggerSceneChanger : MonoBehaviour
{
    public int levelIndex; // The index of the level this collider is linked to
    public int requiredLevelIndex; // The index of the required level for this level to be accessible
    public GameObject popUpPanel; // Reference to the UI Panel
    public Text popUpText; // Reference to the Text inside the Panel
    public Sprite completedSprite; // Sprite to display when level is completed
    public Image levelUIImage; // Optional: If using UI Image for the map

    private bool playerInTrigger = false;
    private PlayerInputs inputActions; // Input System actions

    public Image popupImage; // Reference to the popup-specific image

    private void Awake()
    {
        inputActions = new PlayerInputs(); // Initialize input actions
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        requiredLevelIndex = levelIndex - 1;

        // Check if the level is already completed and update the sprite
        if (PlayerPrefs.GetInt("Level" + levelIndex + "Completed", 0) == 1)
        {
            UpdateLevelSprite();
        }

        // Ensure the pop-up is hidden initially
        if (popUpPanel != null)
            popUpPanel.SetActive(false);
    }

    private void UpdateLevelSprite()
    {
        if (completedSprite != null)
        {
            if (levelUIImage != null)
            {
                levelUIImage.sprite = completedSprite; // Change the sprite for UI Image
            }
        }
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
                ShowPopUp($"Press     to enter.");
                
                // Show the popup-specific image
                if (popupImage != null)
                    popupImage.gameObject.SetActive(true);
            }
            else
            {
                ShowPopUp($"This level is locked.");
                
                // Hide the popup-specific image
                if (popupImage != null)
                    popupImage.gameObject.SetActive(false);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("2D Player"))
        {
            playerInTrigger = false;

            // Hide pop-up when the player leaves
            if (popUpPanel != null)
                popUpPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInTrigger && inputActions.InGame.EnterLevel.triggered) // Using the input action
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

                // Mark this level as completed for later
                PlayerPrefs.SetInt("Level" + levelIndex + "Completed", 1);
                PlayerPrefs.Save();

                // Update the sprite to the completed version
                UpdateLevelSprite();

                // Load the level
                Debug.Log("Loading level " + levelIndex);
                SceneManager.LoadScene(levelIndex);
            }
            else
            {
                ShowPopUp($"Complete another level first.");
            }
        }
    }

    private void ShowPopUp(string message)
    {
        if (popUpPanel != null && popUpText != null)
        {
            popUpPanel.SetActive(true);
            popUpText.text = message;
        }
    }
}
