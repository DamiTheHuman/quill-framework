using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuSaveSlotController : MonoBehaviour, ISubmitHandler, ISelectHandler
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Animator animator;
    [Tooltip("The parent slot controller of the active slot"), SerializeField, LastFoldoutItem()]
    private MainMenuSaveController saveController;
    [SerializeField, Tooltip("The current player data of the player being built")]
    private PlayerData playerData;

    [Tooltip("The slot ID of the current object"), SerializeField, FirstFoldOutItem("Save Slot Settings")]
    private int saveSlotId = 0;
    [Tooltip("Determines whether the save slot has save data ot not"), SerializeField]
    private bool hasSaveData;
    [Tooltip("The current button overlaying the slot"), SerializeField, LastFoldoutItem()]
    private Button saveSlotButton;

    [Tooltip("The object which displays that the current slot is a no save slot"), SerializeField]
    private GameObject noSaveToggle;

    [SerializeField]
    private NewSaveSlotStateData newSaveSlotState;
    [SerializeField]
    private ExistingSaveSlotStateData existingSaveSlotState;
    [SerializeField]
    private ExistingSaveSlotStateData actsClearedSaveSlotState;
    [SerializeField]
    private ExistingSaveSlotStateData curruptedSaveSlotState;
    [SerializeField]
    private bool isCorrupted = false;

    private IEnumerator scrollToPositionCoroutine;

    private void Reset() => this.animator = this.GetComponent<Animator>();

    private void Start()
    {
        if (this.animator == null)
        {
            this.Reset();
        }
    }

    public void OnSubmit(BaseEventData eventData) => this.saveController.OnSaveSlotSubmit(this);

    public void OnSelect(BaseEventData eventData) => this.saveController.OnSaveSlotHighlighted(this);

    private void Update() => this.UpdateSlotDataUI();

    /// <summary>
    /// Updates the save slot data UI that changes based on <see cref="hasSaveData"/>
    /// </summary>
    private void UpdateSlotDataUI()
    {
        //Once we have identified a save file as corrupted it must be deleted before proceeding
        if (this.GetIsCorrupted())
        {
            this.playerData.SetChaosEmeralds(0);
            this.playerData.SetLives(0);
            this.curruptedSaveSlotState.SetDisplayState(this.playerData, true);
            this.newSaveSlotState.SetDisplayState(this.playerData, false);
            this.existingSaveSlotState.SetDisplayState(this.playerData, false);
            this.actsClearedSaveSlotState.SetDisplayState(this.playerData, false);

            return;
        }

        try
        {
            if (this.hasSaveData == false)
            {
                this.newSaveSlotState.SetDisplayState(this.playerData, true);
                this.existingSaveSlotState.SetDisplayState(this.playerData, false);
                this.actsClearedSaveSlotState.SetDisplayState(this.playerData, false);
                this.curruptedSaveSlotState.SetDisplayState(this.playerData, false);
                //Set the playerdata to current stage to the first scene in the stage scene list
                if (GMSceneManager.Instance().GetSceneList().stageScenes.Count > 0)
                {
                    this.playerData.SetCurrentScene(GMSceneManager.Instance().GetSceneList().stageScenes[0]);
                }

                return;
            }


            this.newSaveSlotState.SetDisplayState(this.playerData, false);

            if (this.playerData.GetAllActsCleared())
            {
                this.existingSaveSlotState.SetDisplayState(this.playerData, false);
                this.actsClearedSaveSlotState.SetDisplayState(this.playerData, true);
            }
            else
            {
                this.actsClearedSaveSlotState.SetDisplayState(this.playerData, false);
                this.existingSaveSlotState.SetDisplayState(this.playerData, true);
            }

            this.curruptedSaveSlotState.SetDisplayState(this.playerData, false);
        }
        catch (Exception exception)
        {
            this.SetIsCorrupted(true);
            General.LogErrorMessage(exception, ErrorCode.CorruptedSaveData, "On Save Slot: " + this.GetSaveSlotID());
        }
    }

    /// <summary>
    /// Get a reference to the save slots animator
    /// </summary>
    public Animator GetAnimator() => this.animator;

    /// <summary>
    /// Set the parent save controller of the object
    /// <param name="saveController"> The parent save controller of this slot</param>
    /// </summary>
    public void SetSaveController(MainMenuSaveController saveController) => this.saveController = saveController;

    /// <summary>
    /// Gets the player data for the current save slot
    /// </summary>
    public PlayerData GetPlayerData() => this.playerData;

    /// <summary>
    /// Set the player data for an existing slot
    /// <param name="playerData"> The saved playerdata for an existing slot</param>
    /// </summary>
    public void SetPlayerData(PlayerData playerData) => this.playerData = playerData;

    /// <summary>
    /// Gets the current slot button
    /// </summary>
    public Button GetSlotButton() => this.saveSlotButton;

    /// <summary>
    /// Set the slot id of the current slot item
    /// <param name="saveSlotId"> The slot id value</param>
    /// </summary>
    public void SetSaveSlotID(int saveSlotId) => this.saveSlotId = saveSlotId;

    /// <summary>
    /// Sets the save slot has  save state
    /// <param name="hasSaveData"> The has save value</param>
    /// </summary>
    public void SetHasSaveData(bool hasSaveData) => this.hasSaveData = hasSaveData;

    /// <summary>
    /// Updates the save slot debug mode and toggles the data
    /// </summary>
    public void ToggleDebugMode() => this.playerData.GetPlayerSettings().SetDebugMode(!this.playerData.GetPlayerSettings().GetDebugMode());

    /// <summary>
    /// Gets the slot id of the current object
    /// </summary>
    public int GetSaveSlotID() => this.saveSlotId;

    /// <summary>
    /// Starts a coroutine that moves the slots to the target position
    /// <param name="targetPosition"> The slot position to move towards</param>
    /// </summary>
    public void ScrollToPosition(Vector2 targetPosition)
    {
        if (this.scrollToPositionCoroutine != null)
        {
            this.StopCoroutine(this.scrollToPositionCoroutine);
        }

        this.scrollToPositionCoroutine = this.SmoothScroll(targetPosition);
        this.StartCoroutine(this.scrollToPositionCoroutine);
    }

    /// <summary>
    /// Cycles to the next character in the save slot
    /// </summary>
    public void GetNextCharacter()
    {
        int playableCharacters = Enum.GetNames(typeof(PlayableCharacter)).Length;
        int currentCharacterIndex = (int)this.playerData.GetCharacter();

        if (currentCharacterIndex + 1 >= playableCharacters - 1)
        {
            this.playerData.SetCharacter((PlayableCharacter)0);
        }
        else
        {
            this.playerData.SetCharacter((PlayableCharacter)currentCharacterIndex + 1);
        }
    }

    /// <summary>
    /// Cycles to the previous character in the save slot but skips over super sonic
    /// </summary>
    public void GetPreviousCharacter()
    {
        int playableCharacters = Enum.GetNames(typeof(PlayableCharacter)).Length;
        int currentCharacterIndex = (int)this.playerData.GetCharacter();

        if (currentCharacterIndex == 0)
        {
            // -2 cause super sonic should not be selected from the character select sceen
            this.playerData.SetCharacter((PlayableCharacter)(playableCharacters - 2));
        }
        else
        {
            this.playerData.SetCharacter((PlayableCharacter)currentCharacterIndex - 1);
        }
    }

    /// <summary>
    /// Moves the slots smoothly towards their target position
    /// <param name="targetPosition"> The slot position to move towards</param>
    /// </summary>
    private IEnumerator SmoothScroll(Vector2 targetPosition)
    {
        Vector2 currentPosition = this.transform.localPosition;

        while ((Vector2)this.transform.localPosition != targetPosition)
        {
            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, targetPosition, this.saveController.GetCycleSpeed() * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    /// <summary>
    /// Get the <see cref="isCorrupted"/> value
    /// </summary>
    public bool GetIsCorrupted() => this.isCorrupted;

    /// <summary>
    /// Set the <see cref="isCorrupted"/> value
    /// </summary>
    public void SetIsCorrupted(bool isCorrupted) => this.isCorrupted = isCorrupted;
}

