using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The Save system manager for the current game
/// </summary>
public class GMSaveSystem : MonoBehaviour
{
    public readonly string _KEY = "Quill_Framework";
    private readonly string _SAVE_EXTENSION = ".json";

    [Tooltip("The current player data on the currentSaveSlot"), SerializeField]
    private PlayerData currentPlayerData;

    public static GMSaveSystem instance;

    /// <summary>
    /// Everytime a new scene is loaded this is called, This will server as the Start function
    /// </summary>
    private void Awake()
    {
        this.transform.parent = null;
        DontDestroyOnLoad(this);

        if (Instance() != null && Instance() != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);

            return;
        }
    }

    /// <summary>
    /// Everytime a new scene is loaded this is called, This will serve as the Start function
    /// </summary>
    private void OnSceneLoaded() { }

    /// <summary>
    /// The actions performed on every scene reload
    /// </summary>
    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode) => this.OnSceneLoaded();

    /// <summary>
    /// Get a reference to the static instance of the menu manager
    /// </summary>
    public static GMSaveSystem Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMSaveSystem>();
            SceneManager.sceneLoaded += instance.OnLoadCallback;
        }

        return instance;
    }

    /// <summary>
    /// Retrieves the number of possible save slots
    /// </summary>
    public int GetSaveSlotCount() => Enum.GetNames(typeof(SaveSlot)).Length;

    /// <summary>
    /// Retreives the path of the current player data save path
    /// </summary>
    public string GetCurrentPlayerDataSavePath() => Application.persistentDataPath + "/Data/" + this._KEY + "_" + this.currentPlayerData.GetSaveSlot() + this._SAVE_EXTENSION;

    /// <summary>
    /// Get the current player data of the active player while the game has started
    /// </summary>
    public PlayerData GetCurrentPlayerData() => this.currentPlayerData;

    /// <summary>
    /// Sets the current player data to the value passed
    /// </summary>
    public void SetCurrentPlayerData(PlayerData currentPlayerData) => this.currentPlayerData = currentPlayerData;

    /// <summary>
    /// Creates a new save with default player data
    /// <param name="customPlayerData">Player data that is set from the save slot outside of defaults</param>
    /// </summary>
    public void CreateNewSave(PlayerData customPlayerData = null)
    {
        if (customPlayerData.GetSaveSlot() == SaveSlot.NoSave)
        {
            Debug.Log("Can't create save as the player data is in No Save Mode!");

            return;
        }

        string playerDataAsJSON = JsonUtility.ToJson(customPlayerData ?? new PlayerData(SaveSlot.NoSave));
        string path = Application.persistentDataPath + "/Data/" + this._KEY + "_" + customPlayerData.GetSaveSlot() + this._SAVE_EXTENSION;

        if (this.FileExists(path) == false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        File.WriteAllText(path, JSONHelper.EncodeBase64JSONData(playerDataAsJSON));
    }

    /// <summary>
    /// Set the target save slot
    /// <param name="saveSlot">the ID of the target save slot</param>
    /// </summary>
    public void SetSaveSlot(SaveSlot saveSlot) => this.currentPlayerData.SetSaveSlot(saveSlot);

    /// <summary>
    /// Delete a specified save slot
    /// <param name="saveSlot">the ID of the to be deleted save slot</param>
    /// </summary>
    public void DeleteSaveSlot(SaveSlot saveSlot)
    {
        string path = Application.persistentDataPath + "/Data/" + this._KEY + "_" + saveSlot + this._SAVE_EXTENSION;

        if (this.SaveExists(saveSlot))
        {
            if (this.currentPlayerData.GetSaveSlot() == saveSlot)
            {
                this.currentPlayerData = new PlayerData(saveSlot);
            }

            File.Delete(path);
        }
    }

    /// <summary>
    /// Saves the player data of the currently set save slot    
    /// </summary>
    public void SaveAndOverwriteData()
    {
        if (this.currentPlayerData.GetSaveSlot() == SaveSlot.NoSave)
        {
            Debug.Log("Aborting Save: Current Save is No Save Slot!");

            return;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        string path = this.GetCurrentPlayerDataSavePath();

        //Save doesnt exist so make a new one, ideally this should never happen
        if (this.FileExists(path) == false)
        {
            this.CreateNewSave(this.currentPlayerData);

            return;
        }

        string playerDataAsJSON = JsonUtility.ToJson(this.currentPlayerData);
        File.WriteAllText(path, JSONHelper.EncodeBase64JSONData(playerDataAsJSON));
    }

    /// <summary>
    /// Loads the player data of an existing save slot
    /// <param name="saveSlot">the save slot to load</param>
    /// </summary>
    public PlayerData LoadPlayerData(SaveSlot saveSlot)
    {
        if (saveSlot == SaveSlot.NoSave)
        {
            Debug.Log("No Save Slot! Using playerdata in memory");

            return this.currentPlayerData;
        }

        string path = Application.persistentDataPath + "/Data/" + this._KEY + "_" + saveSlot + this._SAVE_EXTENSION;

        if (this.SaveExists(saveSlot))
        {

            string playerDataJSON = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(JSONHelper.DecodeBase64JSONData(playerDataJSON));

            return playerData;
        }

        return null;
    }

    /// <summary>
    /// Checks if a save exists
    /// <param name="saveSlot">the ID of the save slot to check for</param>
    /// </summary>
    public bool SaveExists(SaveSlot saveSlot)
    {
        string path = Application.persistentDataPath + "/Data/" + this._KEY + "_" + saveSlot + this._SAVE_EXTENSION;

        return File.Exists(path);
    }

    /// <summary>
    /// Checks if the file at the specified path exists
    /// <param name="path">The path of the file</param>
    /// </summary>
    public bool FileExists(string path) => File.Exists(path);
}
