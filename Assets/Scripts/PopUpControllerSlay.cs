using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpControllerSlay : MonoBehaviour
{
    public GameObject popUpWindow; // Reference to the pop-up panel

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TogglePopUp();
        }
    }

    public void TogglePopUp()
    {
        if (popUpWindow != null)
        {
            popUpWindow.SetActive(!popUpWindow.activeSelf);
        }
    }
}
