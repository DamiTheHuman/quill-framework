using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// Manages the interactivity of buttons on the menu
/// </summary>
[RequireComponent(typeof(MainMenuInputManager))]
public class MainMenuController : MonoBehaviour
{
    [Tooltip("The confirmation menu manager"), SerializeField]
    private MenuConfirmationController confirmationOption;
    [Tooltip("The background controller for the main menu"), SerializeField]
    private MainMenuBackgroundController backgroundController;
    [Tooltip("The current save menu controller"), SerializeField]
    private MainMenuSaveController saveController;

    [FirstFoldOutItem("Menu Info"), Tooltip("The first menu the user sees"), SerializeField]
    private GameObject initialMenu;
    [Tooltip("The currently active menu"), SerializeField]
    private GameObject activeMenu;
    [Tooltip("The menu to be loaded when a menu switch is requested"), SerializeField]
    public GameObject targetMenu;
    [Tooltip("The current node data"), SerializeField]
    public MainMenuNodeData currentNodeData;
    [Tooltip("The current event system of the main menu"), SerializeField]
    private EventSystem mainMenuEventSystem;
    [LastFoldoutItem(), Tooltip("A list of menu items showing the currently traversed menu items as they go down a tree of gameobjects"), SerializeField]
    public List<GameObject> menuHistory;

    [FirstFoldOutItem("Transition Info"), Tooltip("The animator for the menu transition"), SerializeField]
    private Animator menuTransitionAnimator;
    [LastFoldoutItem(), Tooltip("The time taken between each button action in seconds"), SerializeField]
    private float transitionTime = 1;

    [FirstFoldOutItem("Volume Slider"), LastFoldoutItem(), Tooltip("The current value of the slider"), SerializeField]
    private Slider volumeSlider;

    [FirstFoldOutItem("Button Audio"), Tooltip("The audio played when a button is highlighted but not pressed"), SerializeField]
    private AudioClip buttonChangeClip;
    [LastFoldoutItem(), Tooltip("The audio played when a highlighted button is pressed"), SerializeField]
    private AudioClip buttonAcceptClip;

    [FirstFoldOutItem("User Action UI"), Tooltip("The Back Button UI Element"), SerializeField]
    private GameObject backButtonUI;
    [Tooltip("The confirm button UI Element"), SerializeField]
    private GameObject confirmButtonUI;
    [Tooltip("The Delete button UI Element"), SerializeField]
    private GameObject deleteButtonUI;
    [LastFoldoutItem(), Tooltip("The game options button UI Element"), SerializeField]
    private GameObject gameOptionsButtonUI;

    ///<summary> This defines what type of method you're going to call. after confirming</summary>
    private delegate void MethodToCall();

    private void Start()
    {
        this.initialMenu.SetActive(true);
        this.activeMenu = this.initialMenu;
        this.volumeSlider.onValueChanged.AddListener(delegate
        { this.OnVolumeChange(this.volumeSlider.value); });
        this.menuHistory.Add(this.activeMenu);
        this.UpdateNodeGuideElements(this.activeMenu);

        if (this.volumeSlider != null)
        {
            this.volumeSlider.value = AudioListener.volume;
        }
    }

    /// <summary>
    /// Gets the initial menu
    /// </summary>
    public GameObject GetInitialMenu() => this.initialMenu;

    /// <summary>
    /// Gets the current activ emenu
    /// </summary>
    public GameObject GetActiveMenu() => this.activeMenu;

    /// <summary>
    /// Loads the game when the play button is hit
    /// <param name="playerData">The current player data </param>
    /// </summary>
    public void OnLoadGameWithSaveData(PlayerData playerData) => GMSceneManager.Instance().SwitchScene(playerData.GetCurrentScene().GetSceneId());

    /// <summary>
    /// When the user button action selects a menu change action
    /// <param name="value">The menu (gameobject) to be enabled</param>
    /// </summary
    public void OnChangeMenu(GameObject value)
    {
        this.targetMenu = value;//set the target menu to be changed too after transition
        this.menuHistory.Add(this.targetMenu);
        this.StartCoroutine(this.DelayButtonAction(this.transitionTime, this.SwitchToTargetMenu));
    }

    /// <summary>
    /// Closes the game when the quit option is selected
    /// </summary>
    public void OnQuitSelected()
    {
        Debug.Log("Quitting");
        this.StartCoroutine(this.DelayButtonAction(this.transitionTime, Application.Quit));
    }

    /// <summary>
    /// Updates the master volume based on the value of the slider
    /// </summary>
    private void OnVolumeChange(float volume) => AudioListener.volume = volume;

    /// <summary>
    /// Plays the set audio when a button on the menu is selected
    /// </summary>
    public void OnButtonChange() => GMAudioManager.Instance().PlayOneShot(this.buttonChangeClip);

    /// <summary>
    /// Plays the set audio when a button is pressed
    /// </summary>
    public void OnButtonSelect() => GMAudioManager.Instance().PlayOneShot(this.buttonAcceptClip);

    /// <summary>
    /// Switches the current menu to the target menu
    /// </summary
    private void SwitchToTargetMenu()
    {
        this.activeMenu.SetActive(false);
        this.targetMenu.SetActive(true);
        this.activeMenu = this.targetMenu;
    }

    /// <summary>
    /// Update the node guide elements based on the current active node
    /// </summary
    private void UpdateNodeGuideElements(GameObject activeMenu)
    {
        this.currentNodeData = activeMenu.GetComponent<MainMenuNodeData>();

        if (this.currentNodeData != null)
        {
            this.backButtonUI.SetActive(this.currentNodeData.backButton);
            this.confirmButtonUI.SetActive(this.currentNodeData.confirmButton);
            this.deleteButtonUI.SetActive(this.currentNodeData.deleteButton);
            this.gameOptionsButtonUI.SetActive(this.currentNodeData.debugButton);
        }
    }

    /// <summary>
    /// Gets the back button UI
    /// </summary
    public GameObject GetBackButtonUI() => this.backButtonUI;

    /// <summary>
    /// Gets the confirm button UI
    /// </summary
    public GameObject GetConfirmButtonUI() => this.confirmButtonUI;

    /// <summary>
    /// Gets the delete button UI
    /// </summary
    public GameObject GetDeleteButtonUI() => this.deleteButtonUI;

    /// <summary>
    /// Gets game options button UI
    /// </summary
    public GameObject GetGameOptionsButtonUI() => this.gameOptionsButtonUI;

    /// <summary>
    /// When the back button is pressed load the previous menu game object
    /// </summary
    public void OnBackButtonInput()
    {
        if (this.menuHistory.Count <= 1 || this.confirmationOption.GetConfrimationMenuIsActive())
        {
            return;
        }

        this.OnButtonSelect();
        this.menuHistory.Remove(this.activeMenu);
        this.targetMenu = this.menuHistory[this.menuHistory.Count - 1];
        this.StartCoroutine(this.DelayButtonAction(this.transitionTime, this.SwitchToTargetMenu));
    }

    /// <summary>
    /// When the delete button is pressed delete the current slot
    /// </summary
    public void OnDeleteButtonInput()
    {
        if (this.currentNodeData.deleteButton && this.deleteButtonUI.activeSelf)
        {
            this.saveController.SetSlotButtonsInteractable(false);
            this.confirmationOption.SetConfirmation(this.mainMenuEventSystem, this.saveController.DeleteCurrentSlot, null, this.OnDeleteSaveConfrimationCloseMenu);
        }
    }

    /// <summary>
    /// When the game options button is pressed set debug mode on the currentslot
    /// </summary
    public void OnGameOptionsButtonPressed()
    {
        if (this.gameOptionsButtonUI.activeSelf)
        {
            this.saveController.ToggleCurrentSaveSlot();
        }
    }

    /// <summary>
    /// When user hits the up button check to update the character for a save slot
    /// <param name="direction">the direction of the user input </param>
    /// </summary
    public void OnMenuDirectionalInput(Vector2 direction)
    {
        if (this.gameOptionsButtonUI.activeSelf)
        {
            this.saveController.CycleCharacter((int)direction.y);
        }
    }

    /// <summary>
    /// When the confirmation menu is closed for the delete menu operation
    /// </summary
    private void OnDeleteSaveConfrimationCloseMenu() => this.saveController.OnDeleteSaveConfrimationCloseMenu();

    /// <summary>
    /// Set the event sytstem target to the specified gameobject
    /// <param name="gameObject">The gameobject to be set as the target </param>
    /// </summary>
    public void SetEventSystemTarget(GameObject gameObject) => this.mainMenuEventSystem.SetSelectedGameObject(gameObject);

    /// <summary>
    /// Waits a set amount of time before performing the specified action
    /// <param name="time">The time waited before the action is launched </param>
    /// <param name="methodToCall">The method to be called when the time has run oout</param>
    /// </summary>
    private IEnumerator DelayButtonAction(float time, MethodToCall methodToCall)
    {

        this.menuTransitionAnimator.SetTrigger("transition");

        yield return new WaitForSeconds(time / 2);

        if (this.targetMenu != null)
        {
            this.UpdateNodeGuideElements(this.targetMenu);
            this.backgroundController.OnMenuUpdate(this.targetMenu);
        }

        methodToCall();

        yield return new WaitForSeconds(time / 2);

        this.SetEventSystemTarget(this.currentNodeData.firstSelectedButton);
    }
}
