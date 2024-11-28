using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpControllerSlay : MonoBehaviour
{
    public GameObject popupWindow; // Assign your popup panel here in the Inspector
    private bool isPopupActive = false;
    private PlayerInputs playerInputs;

    private void Awake()
    {
        // Initialize PlayerInputs
        playerInputs = new PlayerInputs();

        // Bind the TogglePopup action
        playerInputs.InGame.Dictionary.performed += OnTogglePopup;
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    private void OnTogglePopup(InputAction.CallbackContext context)
    {
        // Toggle the popup visibility
        isPopupActive = !isPopupActive;
        popupWindow.SetActive(isPopupActive);
    }
}
