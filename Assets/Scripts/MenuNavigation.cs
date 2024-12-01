using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public InputActionAsset inputActions; // Assign your Input Action Asset here in the Inspector
    public Button[] menuButtons; // Assign the buttons from your menu here in the Inspector
    public Button[] closeButtons; // Assign your Close buttons here in the Inspector (this will be a separate array)
    private int currentIndex = 0; // Tracks the currently selected button
    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction closeAction; // New action for the "Close" button

    private void OnEnable()
    {
        // Locate actions from the InputActionAsset
        navigateAction = inputActions.FindActionMap("Buttons").FindAction("Navigate");
        submitAction = inputActions.FindActionMap("Buttons").FindAction("Submit");
        closeAction = inputActions.FindActionMap("Buttons").FindAction("Close"); // Assign Close action

        navigateAction.Enable();
        submitAction.Enable();
        closeAction.Enable(); // Enable the Close action

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;
        closeAction.performed += OnClose; // Add listener for Close action

        HighlightButton(currentIndex); // Highlight the first button on enable
    }

    private void OnDisable()
    {
        // Clean up the action callbacks
        navigateAction.performed -= OnNavigate;
        submitAction.performed -= OnSubmit;
        closeAction.performed -= OnClose; // Remove listener for Close action

        navigateAction.Disable();
        submitAction.Disable();
        closeAction.Disable(); // Disable the Close action
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        // Navigate up or down
        if (input.y > 0) // Up
        {
            currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;
            HighlightButton(currentIndex);
        }
        else if (input.y < 0) // Down
        {
            currentIndex = (currentIndex + 1) % menuButtons.Length;
            HighlightButton(currentIndex);
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        // Trigger the button's OnClick event
        menuButtons[currentIndex].onClick.Invoke();
    }

    private void OnClose(InputAction.CallbackContext context)
    {
        // Check if we have assigned close buttons and trigger the first one
        if (closeButtons.Length > 0)
        {
            closeButtons[0].onClick.Invoke(); // You can modify this to select a specific close button if needed
        }
    }

    private void HighlightButton(int index)
    {
        // Loop through buttons and visually highlight the currently selected one
        for (int i = 0; i < menuButtons.Length; i++)
        {
            var colors = menuButtons[i].colors;
            colors.normalColor = i == index ? Color.red : Color.white; // Highlight selected button
            menuButtons[i].colors = colors;
        }
    }
}
