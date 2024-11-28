using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TriggerSceneChanger : MonoBehaviour
{
    public int levelIndex; // The index of the level this collider is linked to
    private int requiredLevelIndex; // The index of the prerequisite level
    public GameObject popUpPanel; // UI Panel for messages
    public Text popUpText; // Text for messages
    public Sprite completedSprite; // Sprite to indicate level completion
    public Image levelUIImage; // Image to update for completed levels

    private bool playerInTrigger = false;
    private PlayerInputs inputActions;

    public Image popupImage; // Optional image in the pop-up

    private void Awake()
    {
        inputActions = new PlayerInputs();
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
        // Required level is the previous index
        requiredLevelIndex = levelIndex - 1;

        // Update sprite if the level is completed
        if (PlayerPrefs.GetInt("Level" + levelIndex + "Completed", 0) == 2)
        {
            UpdateLevelSprite();
        }

        // Hide pop-up initially
        if (popUpPanel != null)
            popUpPanel.SetActive(false);
    }

    private void UpdateLevelSprite()
    {
        if (completedSprite != null && levelUIImage != null)
        {
            levelUIImage.sprite = completedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("2D Player"))
        {
            playerInTrigger = true;

            // Check if the required level is completed
            bool hasCompletedRequiredLevel = PlayerPrefs.GetInt("Level" + requiredLevelIndex + "Completed", 0) == 2;

            if (hasCompletedRequiredLevel || levelIndex == 2) // Level 1 is always accessible
            {
                ShowPopUp("Press [Enter] to start this level.");

                if (popupImage != null)
                    popupImage.gameObject.SetActive(true);
            }
            else
            {
                ShowPopUp("This level is locked.");

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

            if (popUpPanel != null)
                popUpPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInTrigger && inputActions.InGame.EnterLevel.triggered)
        {
            bool hasCompletedRequiredLevel = PlayerPrefs.GetInt("Level" + requiredLevelIndex + "Completed", 0) == 2;

            if (hasCompletedRequiredLevel || levelIndex == 2)
            {
                // Save player's position
                Vector3 playerPosition = GameObject.FindGameObjectWithTag("2D Player").transform.position;
                PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
                PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
                PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
                PlayerPrefs.Save();

                // Mark level as completed
                PlayerPrefs.SetInt("Level" + levelIndex + "Completed", 1);
                PlayerPrefs.Save();

                UpdateLevelSprite();

                Debug.Log($"Loading level {levelIndex}.");
                SceneManager.LoadScene(levelIndex);
            }
            else
            {
                ShowPopUp("Complete the required level first.");
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
