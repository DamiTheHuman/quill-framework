using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
///Handles the input of the main menu system
///Like the back button or delete button input
/// </summary>
[RequireComponent(typeof(MainMenuController))]
public class MainMenuInputManager : MonoBehaviour
{
    [SerializeField, Tooltip("The current main menu controller")]
    private MainMenuController mainMenuController;
    private MenuInputMaster mainMenuMaster;

    private void Awake()
    {
        if (this.mainMenuController == null)
        {
            this.mainMenuController = this.GetComponent<MainMenuController>();
        }

        this.mainMenuMaster = new MenuInputMaster();
        this.mainMenuMaster.UI.Cancel.started += ctx => this.BackButtonPressed(ctx.phase);//When the back button is pressed
        this.mainMenuMaster.UI.Delete.started += ctx => this.DeleteButtonPressed(ctx.phase);//When the delete button is pressed
        this.mainMenuMaster.UI.GameOptions.started += ctx => this.GameOptionsButtonPressed(ctx.phase);//When the gameoptions button is pressed
        this.mainMenuMaster.UI.Navigate.performed += ctx => this.NavigationButtonPressed(ctx.phase, ctx.ReadValue<Vector2>());
        this.mainMenuMaster.Enable();
    }

    private void OnEnable()
    {
        if (this.mainMenuMaster == null && Time.timeSinceLevelLoad != 0)
        {
            this.Awake();
        }

        this.mainMenuMaster.Enable();
    }

    private void OnDisable() => this.mainMenuMaster.Disable();

    /// <summary>
    /// The moment the back button is interacted with
    /// <param name="phase">The phase of the input</param>
    /// </summary>
    private void BackButtonPressed(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            this.mainMenuController.OnBackButtonInput();
        }
    }

    /// <summary>
    /// The moment the delete button is interacted with
    /// <param name="phase">The phase of the input</param>
    /// </summary>
    private void DeleteButtonPressed(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            this.mainMenuController.OnDeleteButtonInput();
        }
    }

    /// <summary>
    /// The moment the game options button is interacted with
    /// <param name="phase">The phase of the input</param>
    /// </summary>
    private void GameOptionsButtonPressed(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            this.mainMenuController.OnGameOptionsButtonPressed();
        }
    }

    /// <summary>
    /// The moment the directional input is pressed 
    /// <param name="phase">The phase of the input</param>
    /// <param name="direction">The direction the user is inputting in</param>
    /// </summary>
    private void NavigationButtonPressed(InputActionPhase phase, Vector2 direction)
    {
        if (phase == InputActionPhase.Performed)
        {
            this.mainMenuController.OnMenuDirectionalInput(direction);
        }
    }
}
