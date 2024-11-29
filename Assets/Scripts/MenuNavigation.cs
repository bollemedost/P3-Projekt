using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public InputActionAsset inputActions; // Assign your Input Action Asset here in the Inspector
    public Button[] menuButtons; // Assign the buttons from your menu here in the Inspector
    private int currentIndex = 0; // Tracks the currently selected button
    private InputAction navigateAction;
    private InputAction submitAction;

    private void OnEnable()
    {
        // Locate actions from the InputActionAsset
        navigateAction = inputActions.FindActionMap("Buttons").FindAction("Navigate");
        submitAction = inputActions.FindActionMap("Buttons").FindAction("Submit");

        navigateAction.Enable();
        submitAction.Enable();

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;

        HighlightButton(currentIndex); // Highlight the first button on enable
    }

    private void OnDisable()
    {
        // Clean up the action callbacks
        navigateAction.performed -= OnNavigate;
        submitAction.performed -= OnSubmit;

        navigateAction.Disable();
        submitAction.Disable();
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
