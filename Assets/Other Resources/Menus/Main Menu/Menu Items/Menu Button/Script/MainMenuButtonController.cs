using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Handles the generic Main Menu actions
/// </summary>
public class MainMenuButtonController : MonoBehaviour, ISelectHandler
{
    [Tooltip("The main menu controller which handles all the buttons"), SerializeField]
    private MainMenuController mainMenuController;

    private void Start()
    {
        if (this.mainMenuController == null)
        {
            this.mainMenuController = FindObjectOfType<MainMenuController>().GetComponent<MainMenuController>();
        }
    }

    /// <summary>
    /// Plays an audio clip when the button is selected but not pressed
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        if (eventData.currentInputModule == null)
        {
            return;
        }   //If the selection was not from the user i.e the default selected button no sound should play
        this.mainMenuController.OnButtonChange();
    }
}
