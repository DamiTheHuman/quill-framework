using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Data pertaining to the player that differs between saves
/// </summary>
[System.Serializable]
public class PlayerData
{
    public PlayerData(SaveSlot saveSlot) => this.saveSlot = saveSlot;

    [SerializeField, Tooltip("The current save slot")]
    private SaveSlot saveSlot = SaveSlot.NoSave;
    [SerializeField, Tooltip("The current score of the active player")]
    private int score = 0;
    [SerializeField, Tooltip("The amount of lives the active player has")]
    private int lives = 3;
    [SerializeField, Tooltip("The current character selected by the player")]
    private PlayableCharacter character = PlayableCharacter.Sonic;
    [SerializeField, Tooltip("The current stage of the player")]
    private PlayerSceneData currentScene = new PlayerSceneData();
    [Range(0, 7)]
    [SerializeField, Tooltip("The number of chaos emeralds the player has")]
    private int chaosEmeralds = 3;
    [SerializeField, Tooltip("The current shield the player has on the save slot")]
    private ShieldType activeShieldType = ShieldType.None;
    [SerializeField, Tooltip("The positions of special stage data that been used accrossed multiple stages")]
    private List<SpecialStageData> usedSpecialStageData = new List<SpecialStageData>();
    [SerializeField, Tooltip("The scene id start stage cutscenes that have been watched already"), SceneList]
    private List<int> watchedActStartCutscenes = new List<int>();
    [SerializeField, Tooltip("The scene id of acts the player has completed"), SceneList]
    private List<int> clearedActs = new List<int>();
    [SerializeField, Tooltip("Determines whether all acts are cleared")]
    private bool allActsCleared = false;
    [SerializeField, Tooltip("Setting specifically for the player")]
    private PlayerSettingsData playerSettings = new PlayerSettingsData();

    /// <summary>
    /// Get the save slot
    /// </summary>
    public SaveSlot GetSaveSlot() => this.saveSlot;

    /// <summary>
    /// Set the save slot
    /// </summary>
    public void SetSaveSlot(SaveSlot saveSlot) => this.saveSlot = saveSlot;

    /// <summary>
    /// Get the player score
    /// </summary>
    public int GetScore() => this.score;

    /// <summary>
    /// Set the player score
    /// </summary>
    public int SetScore(int score) => this.score = score;

    /// <summary>
    /// Get the player life count
    /// </summary>
    public int GetLives() => this.lives;

    /// <summary>
    /// Set the player life count
    /// </summary>
    public void SetLives(int lives) => this.lives = lives;

    /// <summary>
    /// Get the current character
    /// </summary>
    public PlayableCharacter GetCharacter() => this.character;

    /// <summary>
    /// Set the current character
    /// </summary>
    public void SetCharacter(PlayableCharacter character) => this.character = character;

    /// <summary>
    /// Get the current scene/act
    /// </summary>
    public PlayerSceneData GetCurrentScene() => this.currentScene;

    /// <summary>
    /// Get the chaos emerald count
    /// </summary>
    public int GetChaosEmeralds() => this.chaosEmeralds;

    /// <summary>
    /// Set the chaos emerald count
    /// </summary>
    public void SetChaosEmeralds(int chaosEmeralds) => this.chaosEmeralds = chaosEmeralds;

    /// <summary>
    /// Get the shield the player has
    /// </summary>
    public ShieldType GetCurrentShield() => this.activeShieldType;

    /// <summary>
    /// Set the shield the player has
    /// </summary>
    public ShieldType SetCurrentShield(ShieldType shield) => this.activeShieldType = shield;

    /// <summary>
    /// Get the special stages the player has touched
    /// </summary>
    public List<SpecialStageData> GetUsedSpecialStageData() => this.usedSpecialStageData;

    /// <summary>
    /// Get the act start cutscenes the player has watched
    /// </summary>
    public List<int> GetWatchedActStartCutscenes() => this.watchedActStartCutscenes;

    /// <summary>
    /// Get the  acts cleared
    /// </summary>
    public List<int> GetClearedActs() => this.clearedActs;

    /// <summary>
    /// Get the  all acts cleared
    /// </summary>
    public bool GetAllActsCleared() => this.allActsCleared;

    /// <summary>
    /// Set the player life count
    /// </summary>
    public void ResetLifeCountToDefault() => this.lives = new PlayerData(SaveSlot.NoSave).lives;

    /// <summary>
    /// Adds a special stage position to the save data
    /// <param name="position">The position of the special stage object i.e Big Ring</param>
    /// </summary>
    public void AddSpecialStageDataPosition(Vector2 position)
    {
        if (this.CheckIfSpecialDataObjectExists(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId(), position))
        {
            return;
        }

        this.usedSpecialStageData.Add(new SpecialStageData(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId(), position));
    }

    /// <summary>
    /// Gives the current save slot a set amount of emeralds
    /// </summary>
    public void IncrementChaosEmeraldCount(int count = 1)
    {
        Debug.Log(this.saveSlot + " Has been granted " + count + " Chaos Emerald(s)!");
        this.chaosEmeralds += count;
        this.chaosEmeralds = Mathf.Clamp(this.chaosEmeralds, 0, 7);
    }

    /// <summary>
    /// Checks if a special stage object already exists on the current scene
    /// <param name="sceneId">The scene id of the special stage ring</param>
    /// <param name="position">The position of the special stage object </param>
    /// </summary>
    public bool CheckIfSpecialDataObjectExists(int sceneId, Vector2 position) => this.usedSpecialStageData.Where(x => x.GetSceneId() == sceneId && x.GetPosition() == position).ToArray().Count() != 0;

    /// <summary>
    /// Add the scene id of the stage containing the start stage cutscene
    /// <param name="sceneId">The scene id of the cutscene</param>
    /// </summary>
    public void AddStartStageCutsceneId(int sceneId)
    {
        if (this.PlayerHasWatchedCutscene(sceneId))
        {
            return;
        }

        this.watchedActStartCutscenes.Add(sceneId);
    }

    /// <summary>
    /// Checks if the current save has already seen the cutscene
    /// <param name="sceneId">The scene id of the cutscene</param>
    /// </summary>
    public bool PlayerHasWatchedCutscene(int sceneId) => this.watchedActStartCutscenes.Where(x => x == sceneId).ToArray().Count() != 0;

    /// <summary>
    /// Add a scene to the current player data act cleared list
    /// <param name="sceneId">The scene id of the cutscene</param>
    /// </summary>
    public void AddSceneToActsClearedList(int sceneId)
    {
        if (this.clearedActs.Contains(sceneId))
        {
            return;
        }

        this.clearedActs.Add(sceneId);

        if (this.HasClearedLastAct())
        {
            this.allActsCleared = true;
        }
    }

    /// <summary>
    /// Convert the <see cref="SceneData"/> into <see cref="PlayerSceneData"/> because the thumbnail field cannot be serialized
    /// <param name="sceneData">The scene data to convert </param>
    /// </summary>
    public void SetCurrentScene(SceneData sceneData) => this.currentScene = new PlayerSceneData
    (
        sceneData.GetSceneType(),
        sceneData.GetActNumber(),
        sceneData.GetSceneId(),
        sceneData.GetNextSceneId(),
        sceneData.name
    );

    /// <summary>
    /// Serializes the player data based ont he info from the regular stage
    /// <param name="player">Information of the player object</param>
    /// </summary>
    public void SerializeRegularStageData(Player player)
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.RegularStage)
        {
            Debug.LogError("Player Data not updated current scene is not: " + SceneType.RegularStage);

            return;
        }

        this.character = GMCharacterManager.Instance().currentCharacter;
        this.SerializeRegularStageScoreData(player);
        this.SerializeCurrentSceneData();
    }

    /// <summary>
    /// Serializes the player data based ont he info from the special stage
    /// <param name="player">Information of the player object</param>
    /// </summary>
    public void SerializeSpecialStageData(SpecialStagePlayer player)
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.SpecialStage)
        {
            Debug.LogError("Player Data not updated current scene is not: " + SceneType.SpecialStage);

            return;
        }

        this.character = GMCharacterManager.Instance().currentCharacter;
    }

    /// <summary>
    /// Serializes the player data scores
    /// <param name="player">Information of the player object</param>
    /// </summary>
    private void SerializeRegularStageScoreData(Player player)
    {
        if (player == null)
        {
            return;
        }

        this.lives = GMRegularStageScoreManager.Instance().GetLifeCount();
        this.score = GMRegularStageScoreManager.Instance().GetScoreCount();
        this.activeShieldType = player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType();
    }

    /// <summary>
    /// Serializes the current scenes data
    /// </summary>
    private void SerializeCurrentSceneData()
    {
        //If the last act is cleared do not use the current scene data
        if (GMStageManager.Instance().GetStageState() == RegularStageState.ActClear && this.currentScene.GetSceneId() != GMSceneManager.Instance().GetSceneList().stageScenes.Last().GetSceneId())
        {
            this.SetCurrentScene(GMSceneManager.Instance().GetSceneList().GetSceneData(this.currentScene.GetNextSceneId()));

            return;
        }

        this.SetCurrentScene(GMSceneManager.Instance().GetSceneList().GetSceneData(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId()));
    }

    /// <summary>
    /// Whether the player has cleared the last act
    /// </summary
    public bool HasClearedLastAct() => this.clearedActs.Any(current => current == GMSceneManager.Instance().GetSceneList().stageScenes.Last().GetSceneId());

    /// <summary>
    /// Get the player settings
    /// </summary
    public PlayerSettingsData GetPlayerSettings() => this.playerSettings;
}
