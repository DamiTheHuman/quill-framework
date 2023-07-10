using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Manages the save controller menu and the respective saved items
/// </summary>
public class MainMenuSaveController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private MainMenuController mainMenuController;
    [Tooltip("The event system of the save system"), SerializeField]
    private MainMenuNodeData nodeData;
    [Tooltip("Manages the main prefab of the save item to be cloned"), SerializeField]
    private GameObject saveSlotPrefab;
    [Tooltip("Manages the main prefab for the no save slot"), SerializeField, LastFoldoutItem()]
    private GameObject noSaveSlotPrefab;

    [Tooltip("The id of the current slot"), SerializeField]
    private int currentSaveSlotId = 0;
    [Tooltip("The save items active on the scene"), SerializeField]
    private List<MainMenuSaveSlotController> activeSaveSlots;

    [Tooltip("The space between each menu item"), SerializeField, FirstFoldOutItem("Spawned Slot Positioning")]
    private float menuItemPadding = 112;
    [Tooltip("The speed in which menu items will be cycled through"), SerializeField]
    private float cycleSpeed = 300;
    [Tooltip("The position of the slot vertically"), SerializeField, LastFoldoutItem()]
    private float verticalPosition = 44;

    private void Awake()
    {
        this.CreateNoSaveSlot(this.activeSaveSlots.Count);
        this.SpawnSlots();
        this.nodeData.firstSelectedButton = this.activeSaveSlots[0].gameObject;
    }

    /// <summary>
    /// Spawns a set amount of slots available to the player
    /// </summary>
    public void SpawnSlots()
    {
        for (int x = 0; x < GMSaveSystem.Instance().GetSaveSlotCount(); x++)
        {
            SaveSlot currentSaveSlot = (SaveSlot)x;

            if (currentSaveSlot == SaveSlot.NoSave)
            {
                continue;
            }

            MainMenuSaveSlotController newSaveSlot = Instantiate(this.saveSlotPrefab).GetComponent<MainMenuSaveSlotController>();

            newSaveSlot.transform.SetParent(this.transform);//parent to the this object
            Vector2 saveSlotLocalPosition = new Vector2(0, 0);
            saveSlotLocalPosition += new Vector2(x * this.menuItemPadding, -4);//apply padding to position
            newSaveSlot.transform.localPosition = saveSlotLocalPosition;
            newSaveSlot.name = this.saveSlotPrefab.name + " - " + x;
            newSaveSlot.SetSaveController(this);
            newSaveSlot.SetSaveSlotID(x);
            this.activeSaveSlots.Add(newSaveSlot);

            if (GMSaveSystem.Instance().SaveExists(currentSaveSlot))
            {
                newSaveSlot.SetHasSaveData(true);

                try
                {
                    newSaveSlot.SetPlayerData(GMSaveSystem.Instance().LoadPlayerData(currentSaveSlot));
                }
                catch (Exception exception)
                {
                    General.LogErrorMessage(exception, ErrorCode.CorruptedSaveData, "On Save Slot: " + newSaveSlot.GetSaveSlotID());
                    newSaveSlot.SetIsCorrupted(true);
                }

                continue;
            }

            newSaveSlot.SetHasSaveData(false);
            newSaveSlot.SetPlayerData(new PlayerData(currentSaveSlot));
        }
    }

    /// <summary>
    /// Creates a save slot that does not save user data
    /// <param name="slotsCreated"> The number of slots created prior to this one</param>
    /// </summary>
    public void CreateNoSaveSlot(int slotsCreated)
    {
        MainMenuSaveSlotController noSaveSlot = Instantiate(this.noSaveSlotPrefab).GetComponent<MainMenuSaveSlotController>();

        noSaveSlot.transform.SetParent(this.transform);//parent to the this object
        Vector2 noSaveSlotLocalPosition = new Vector2(0, 0);
        noSaveSlotLocalPosition += new Vector2(slotsCreated * this.menuItemPadding, -4);//apply padding to position
        noSaveSlot.transform.localPosition = noSaveSlotLocalPosition;
        noSaveSlot.name = "No Save Slot";
        noSaveSlot.SetSaveController(this);
        noSaveSlot.SetSaveSlotID((int)SaveSlot.NoSave);
        noSaveSlot.SetHasSaveData(false);
        noSaveSlot.SetPlayerData(new PlayerData(SaveSlot.NoSave));

        this.activeSaveSlots.Add(noSaveSlot);
    }

    private void OnDisable()
    {
        for (int x = 0; x < this.activeSaveSlots.Count; x++)
        {
            this.activeSaveSlots[x].transform.localPosition = new Vector2(x * this.menuItemPadding, this.verticalPosition);
        }
    }

    /// <summary>
    /// Deletes the current slot
    /// </summary>
    public void DeleteCurrentSlot()
    {
        GMSaveSystem.Instance().DeleteSaveSlot(this.activeSaveSlots[this.currentSaveSlotId].GetPlayerData().GetSaveSlot());
        this.activeSaveSlots[this.currentSaveSlotId].SetHasSaveData(false);
        this.activeSaveSlots[this.currentSaveSlotId].SetIsCorrupted(false);
        this.activeSaveSlots[this.currentSaveSlotId].SetPlayerData(new PlayerData(this.activeSaveSlots[this.currentSaveSlotId].GetPlayerData().GetSaveSlot()));//Reload the empty slot id
        this.SetSlotButtonsInteractable(false);
    }

    /// <summary>
    /// Gets the current save slot and toggles its debug mode
    /// </summary>
    public void ToggleCurrentSaveSlot() => this.activeSaveSlots[this.currentSaveSlotId].ToggleDebugMode();

    /// <summary>
    /// Switches the character based on the direction passed
    /// <param name="direction">The vertical direction of the players input which signifies what character to cycle too</param>
    /// </summary>
    public void CycleCharacter(int direction)
    {
        if (direction == 1)
        {
            this.activeSaveSlots[this.currentSaveSlotId].GetNextCharacter();
            this.activeSaveSlots[this.currentSaveSlotId].GetAnimator().SetTrigger("Normal");
            this.activeSaveSlots[this.currentSaveSlotId].GetAnimator().SetTrigger("Selected");
        }
        else if (direction == -1)
        {
            this.activeSaveSlots[this.currentSaveSlotId].GetPreviousCharacter();
            this.activeSaveSlots[this.currentSaveSlotId].GetAnimator().SetTrigger("Normal");
            this.activeSaveSlots[this.currentSaveSlotId].GetAnimator().SetTrigger("Selected");
        }
    }

    /// <summary>
    /// Sets the interaction for all the slots
    /// <param name="value"> The new interactable value for the slots</param>
    /// </summary>
    public void SetSlotButtonsInteractable(bool value)
    {
        foreach (MainMenuSaveSlotController saveSlot in this.activeSaveSlots)
        {
            saveSlot.GetSlotButton().interactable = value;
        }
    }

    /// <summary>
    /// When the slots are reactivated after interaction with the confrimation menu
    /// </summary>
    public void OnDeleteSaveConfrimationCloseMenu()
    {
        this.SetSlotButtonsInteractable(true);
        this.mainMenuController.SetEventSystemTarget(this.activeSaveSlots[this.currentSaveSlotId].gameObject);
        this.activeSaveSlots[this.currentSaveSlotId].GetAnimator().SetTrigger("Normal");
        this.activeSaveSlots[this.currentSaveSlotId].GetAnimator().SetTrigger("Selected");
    }

    /// <summary>
    /// Gets the current cycle speed of all the slots
    /// </summary>
    public float GetCycleSpeed() => this.cycleSpeed;

    /// <summary>
    /// Performs the on slot submit action
    /// <param name="saveSlot"> The save slot to load</param>
    /// </summary>
    public void OnSaveSlotSubmit(MainMenuSaveSlotController saveSlot)
    {
        if (saveSlot.GetIsCorrupted())
        {
            return;
        }

        if (saveSlot.GetPlayerData().GetSaveSlot() != SaveSlot.NoSave && GMSaveSystem.Instance().SaveExists(saveSlot.GetPlayerData().GetSaveSlot()) == false)
        {
            GMSaveSystem.Instance().CreateNewSave(saveSlot.GetPlayerData());
        }

        //If we are on the last stage cycle back to the first regular stage if the last stages has been completed
        if (saveSlot.GetPlayerData().GetAllActsCleared() && saveSlot.GetPlayerData().HasClearedLastAct())
        {
            saveSlot.GetPlayerData().SetCurrentScene(GMSceneManager.Instance().GetSceneList().stageScenes.First(x => x.GetSceneType() == SceneType.RegularStage));
            saveSlot.GetPlayerData().GetClearedActs().Clear();
        }

        GMSaveSystem.Instance().SetSaveSlot(saveSlot.GetPlayerData().GetSaveSlot());
        GMSaveSystem.Instance().LoadPlayerData(saveSlot.GetPlayerData().GetSaveSlot());
        GMSaveSystem.Instance().SetCurrentPlayerData(saveSlot.GetPlayerData());
        GMStageManager.Instance().SetOnSaveSlotGameLoad(true);
        GMSaveSystem.Instance().SaveAndOverwriteData();
        GMHistoryManager.Instance().ClearHistory();
        this.mainMenuController.OnLoadGameWithSaveData(GMSaveSystem.Instance().GetCurrentPlayerData());
        this.mainMenuController.OnButtonSelect();
    }

    /// <summary>
    /// Updates the positioning of all the slots when a slot is selected
    /// <param name="saveSlot"> The save slot to highlighted</param>
    /// </summary>
    public void OnSaveSlotHighlighted(MainMenuSaveSlotController saveSlot)
    {
        for (int x = 0; x < this.activeSaveSlots.Count; x++)
        {
            this.activeSaveSlots[x].ScrollToPosition(new Vector2(112 * (this.activeSaveSlots[x].GetSaveSlotID() - saveSlot.GetSaveSlotID()), this.activeSaveSlots[x].transform.localPosition.y));
        }

        this.mainMenuController.OnButtonChange();
        this.currentSaveSlotId = saveSlot.GetSaveSlotID();
        this.mainMenuController.GetDeleteButtonUI().SetActive(GMSaveSystem.Instance().SaveExists(saveSlot.GetPlayerData().GetSaveSlot()) || saveSlot.GetIsCorrupted());
        this.mainMenuController.GetConfirmButtonUI().SetActive(saveSlot.GetIsCorrupted() == false);
        this.mainMenuController.GetGameOptionsButtonUI().SetActive(!GMSaveSystem.Instance().SaveExists(saveSlot.GetPlayerData().GetSaveSlot()));
    }
}
