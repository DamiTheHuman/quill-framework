using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GMStageManager : MonoBehaviour
{
    [SerializeField, Tooltip("The state of the current stage")]
    private RegularStageState stageState = RegularStageState.NotAvailable;
    [SerializeField]
    private PlayerDeathReason deathReason = PlayerDeathReason.NotAvailable;
    [Tooltip("The physics value that other physics movements are applied too match"), SerializeField]
    private float physicsMultiplier = 60f;
    [SerializeField, Tooltip("The max size of singular block piece which directly affects highspeed movement")]
    private float maxBlockSize = 16f;
    [Tooltip("The main player object"), SerializeField]
    private Player player;
    [LayerList]
    [Tooltip("The general layer for generic hazards like spike balls"), FirstFoldOutItem("Key Layers"), SerializeField]
    private int hazardLayer = 21;
    [Tooltip("A layers for all one way objects"), LastFoldoutItem(), SerializeField]
    private LayerMask oneWayLayers;

    [SerializeField, Tooltip("The Current boss of the act")]
    private Boss boss;
    [Tooltip("The act end controller for the stage"), SerializeField]
    private ActClearGimmick actClearGimmick;
    [Tooltip("The target frame rate of the game which affects physics as well"), SerializeField, FirstFoldOutItem("Debug")]
    private int targetFrameRate = 60;
    [Tooltip("A flag to determine whether the game was just loaded from the menu"), SerializeField, LastFoldoutItem()]
    private bool onSaveSlotGameLoad;

    /// <summary>
    /// The single instance of the stage manager
    /// </summary>
    private static GMStageManager instance;

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
    /// Everytime a new scene is loaded this is called, This will server as the Start function
    /// </summary>
    private void OnSceneLoaded()
    {
        instance = this;
        this.FindPlayer();
        GMSceneManager.Instance().FindStartPoint();

        if (this.player != null)
        {
            HedgehogCamera.Instance().SetCameraTarget(this.player.gameObject);
            GMHistoryManager.Instance().CheckAddStartPointAsSpawnPoint();
        }

        this.actClearGimmick = null;

        //Update the scene on the save scene based on the stage type
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage)
        {
            GMSaveSystem.Instance().GetCurrentPlayerData().SetCurrentScene(GMSceneManager.Instance().GetSceneList().GetSceneData(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId()));
            this.player.transform.position = this.GetPlayerSpawnPoint();
            HedgehogCamera.Instance().SetCameraPosition(this.player.transform.position);

            if (GMCutsceneManager.Instance().HasStartActTimeLineTriggers() && GMSaveSystem.Instance().GetCurrentPlayerData().PlayerHasWatchedCutscene(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId()) == false)
            {
                GMCutsceneManager.Instance().PlayActStartTimeLineTriggers(this.OnStageStart);
                GMStageHUDManager.Instance().SetTitleCardActive(false);
            }
            else
            {
                this.OnStageStart();
            }
        }
        else
        {
            this.SetStageState(RegularStageState.NotAvailable);
        }
    }

    /// <summary>
    /// The actions performed on every scene reload
    /// </summary>
    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode) => this.OnSceneLoaded();

    /// <summary>
    /// Get a reference to the static instance of the stage manager
    /// </summary>
    public static GMStageManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMStageManager>();
            if (Application.isPlaying)
            {
                SceneManager.sceneLoaded += instance.OnLoadCallback;
            }
        }

        return instance;
    }

    /// <summary>
    /// Actions that take place when a stage starts
    /// </summary
    public void OnStageStart()
    {
        this.SetStageState(RegularStageState.Idle);
        GMStageHUDManager.Instance().SetTitleCardActive(true);
        GMAudioManager.Instance().PlayBGM(BGMToPlay.MainBGM);

        if (GMCutsceneManager.Instance().HasStartActTimeLineTriggers())
        {
            GMSaveSystem.Instance().GetCurrentPlayerData().AddStartStageCutsceneId(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId());
            GMSaveSystem.Instance().SaveAndOverwriteData();
        }
    }

    /// <summary>
    /// Gets the physics multiplier for the game
    /// </summary
    public float GetPhysicsMultiplier() => this.physicsMultiplier;
    /// <summary>
    /// Gets the maximum block size for the game
    /// </summary
    public float GetMaxBlockSize() => this.maxBlockSize;
    /// <summary>
    /// Makes the value frame rate independent 
    /// <param name="currentValue">the value to be converted</param>
    /// </summary>
    public float ConvertToDeltaValue(float currentValue)
    {
        float delta = Time.timeScale != 0 ? Time.deltaTime : Time.unscaledDeltaTime;

        return currentValue * this.targetFrameRate * delta;
    }

    /// <summary>
    /// Sets the value of onSaveSlotGameLoad to inform that a game has been loaded from the main menu
    /// <param name="onSaveSlotGameLoad">The new value of onGameLoad</param>
    /// </summary>
    public void SetOnSaveSlotGameLoad(bool onSaveSlotGameLoad) => this.onSaveSlotGameLoad = onSaveSlotGameLoad;
    /// <summary>
    /// Gets the value of onSaveSlotGameLoad
    /// </summary>
    public bool GetOnSaveSlotGameLoad() => this.onSaveSlotGameLoad;
    /// <summary>
    /// Actions that take place when the player is initialized and the game is loaded for the first time post main menu
    /// </summary>
    public void OnPlayerInitialize()
    {
        //Tells us if the player is coming in from the character manager if that is case the correct character based of the save is already correct
        if (GMCharacterManager.Instance().GetCharacterInstantiated() == false)
        {
            GMCharacterManager.Instance().SwapCharacter(GMSaveSystem.Instance().GetCurrentPlayerData().GetCharacter());
        }

        ShieldType shieldToGrantOnStart = this.GetOnSaveSlotGameLoad() ? GMSaveSystem.Instance().GetCurrentPlayerData().GetCurrentShield() : GMHistoryManager.Instance().GetActiveShieldType();

        switch (GMSaveSystem.Instance().GetCurrentPlayerData().GetCurrentShield())
        {
            case ShieldType.BubbleShield:
                GMGrantPowerUpManager.Instance().GrantPowerUp(this.player, PowerUp.BubbleShield);
                break;
            case ShieldType.ElectricShield:
                GMGrantPowerUpManager.Instance().GrantPowerUp(this.player, PowerUp.ElectricShield);
                break;
            case ShieldType.FlameShield:
                GMGrantPowerUpManager.Instance().GrantPowerUp(this.player, PowerUp.FlameShield);
                break;
            case ShieldType.RegularShield:
                GMGrantPowerUpManager.Instance().GrantPowerUp(this.player, PowerUp.RegularShield);
                break;
            case ShieldType.None:
                break;
            default:
                break;
        }

        this.SetOnSaveSlotGameLoad(false);
    }

    /// <summary>
    /// Finds the active player within the scene
    /// </summary
    public void FindPlayer()
    {
        Player[] playerObjects = FindObjectsOfType<Player>();

        for (int x = 0; x < playerObjects.Length; x++)
        {
            if (playerObjects[x].GetPlayerState() == PlayerState.Awake)
            {
                this.SetPlayer(playerObjects[x]);

                return;
            }
        }
    }

    /// <summary>
    /// Get the current active player
    /// </summary>
    public Player GetPlayer()
    {
        if (this.player == null)
        {
            this.FindPlayer();
        }

        return this.player;
    }

    /// <summary>
    /// Sets the current active player
    /// <param name="player">the new player value</param>
    /// </summary>
    public void SetPlayer(Player player) => this.player = player;

    /// <summary>
    /// Pausees the game
    /// This function freezes animations and objects that 
    /// Move via the fixed update cycle it doesnt affect objects that move in the update cycle
    /// </summary>
    public void FreezePhysicsCycle() => Time.timeScale = 0;

    /// <summary>
    /// Gets the spawn point of the player
    /// </summary>
    public Vector2 GetPlayerSpawnPoint()
    {
        if (GMCutsceneManager.Instance().HasStartActTimeLineTriggers() && GMSaveSystem.Instance().GetCurrentPlayerData().PlayerHasWatchedCutscene(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId()) == false)
        {
            return GMCutsceneManager.Instance().GetActStartCutscene().GetTimelineTriggers().First().transform.position;
        }

        if (GMHistoryManager.Instance().GetRegularStageScoreHistory().HasSpawnPoints())
        {
            return GMHistoryManager.Instance().GetRegularStageScoreHistory().HasSpawnPointsOfType(SpawnPointType.SpecialStage) ? GMHistoryManager.Instance().GetRegularStageScoreHistory().GetRecentSpawnPosition(SpawnPointType.SpecialStage) : GMHistoryManager.Instance().GetRegularStageScoreHistory().GetRecentSpawnPosition(SpawnPointType.CheckPoint);
        }

        return GMSceneManager.Instance().GetCurrentStartPoint().transform.position;
    }

    /// <summary>
    /// Resets to the earliest position of the scene
    /// </summary>
    public void RestartStage()
    {
        GMSaveSystem.Instance().GetCurrentPlayerData().SetCurrentShield(ShieldType.None);
        GMSaveSystem.Instance().GetCurrentPlayerData().SetCharacter(GMCharacterManager.Instance().currentCharacter);
        GMSaveSystem.Instance().SaveAndOverwriteData();
        this.SetStageState(RegularStageState.Restarting);

        //We only want to clear history when in regular stages and not in special stages if the functionality is availble in them
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage)
        {
            GMHistoryManager.Instance().ClearHistory();
        }

        GMSceneManager.Instance().ReloadCurrentScene();
    }

    /// <summary>
    /// After the Act has been cleared perform actions like saving etc
    /// </summary>
    public void OnActClear()
    {
        GMSaveSystem.Instance().GetCurrentPlayerData().SetCharacter(GMCharacterManager.Instance().currentCharacter);
        GMSaveSystem.Instance().GetCurrentPlayerData().SerializeRegularStageData(this.GetPlayer());
        GMSaveSystem.Instance().GetCurrentPlayerData().AddSceneToActsClearedList(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId());
        GMSaveSystem.Instance().SaveAndOverwriteData();
        GMHistoryManager.Instance().ClearHistoryInformation();
        GMRegularStageScoreManager.Instance().SetTimerCount(0);
        GMRegularStageScoreManager.Instance().SetRingCount(0);
    }

    /// <summary>
    /// Returns the player back to the main menu
    /// </summary
    public void LoadMainMenu()
    {
        GMHistoryManager.Instance().ClearHistoryInformation();
        GMSceneManager.Instance().SwitchScene(0);
    }

    /// <summary>
    /// Exits the game without savinig
    /// </summary
    public void ExitGame()
    {
        GMHistoryManager.Instance().ClearHistoryInformation();
        this.SetStageState(RegularStageState.ExitingStage);
        Time.timeScale = 0;
        this.LoadMainMenu();
    }

    /// <summary>
    /// Set the act end
    /// <param name="actClearGimmick">Set the act end gimmick the player has activated</param>
    /// </summary
    public void SetActClearGimmick(ActClearGimmick actClearGimmick) => this.actClearGimmick = actClearGimmick;

    /// <summary>
    /// Get the act end
    /// </summary
    public ActClearGimmick GetActClearGimmick() => this.actClearGimmick;

    /// <summary>
    /// Sets the active boss of the act
    /// <param name="bossMode">The boss mode to be set to </param>
    /// </summary>
    public void SetBoss(Boss boss) => this.boss = boss;
    /// <summary>
    /// Gets the current boss for the act
    /// </summary>
    public Boss GetBoss() => this.boss;

    /// <summary>
    /// Get the current state of the stage
    /// </summary>
    public RegularStageState GetStageState() => this.stageState;

    /// <summary>
    /// Sets the current state of the stage
    /// </summary
    public void SetStageState(RegularStageState stageState) => this.stageState = stageState;

    /// <summary>
    /// Get the current reason the player died
    /// </summary>
    public PlayerDeathReason GetDeathReason() => this.deathReason;

    /// <summary>
    /// Sets the current death reason
    /// </summary
    public void SetDeathReason(PlayerDeathReason deathReason) => this.deathReason = deathReason;

    /// <summary>
    /// Returns the hazard layer of the stage
    /// </summary
    public bool IsInHazardLayer(GameObject gameObject) => gameObject.layer == this.hazardLayer;

    /// <summary>
    /// Checks if the sensor has collided with something on the one way layer
    /// <param name="sensor"> The sensor to check</param>
    /// </summary>
    public bool SensorHitOneWayLayer(RaycastHit2D sensor) => General.ContainsLayer(this.oneWayLayers, sensor.transform.gameObject.layer);

    /// <summary>
    /// Begins a coroutine that delays an action for the specified number of fixed update cycles
    /// <param name="action"> The action to perform at the end of the frame delay period</param>
    /// <param name="fixedUpdates"> The number of fixed update cycles to wait for with a minimum of 1</param>
    /// </summary
    public IEnumerator DelegateTillNextFixedUpdateCycles(Action action, int fixedUpdates = 1)
    {
        if (fixedUpdates < 1)
        {
            fixedUpdates = 1;
        }

        IEnumerator delegateTillNextFixedUpdateCyclesCoroutine = this.DelegateTillNextFixedUpdatesCyclesCoroutine(action, fixedUpdates);
        this.StartCoroutine(delegateTillNextFixedUpdateCyclesCoroutine);

        return delegateTillNextFixedUpdateCyclesCoroutine;
    }

    /// <summary>
    /// A coroutine that calls an action at the start of the next fixed update
    /// </summary
    private IEnumerator DelegateTillNextFixedUpdatesCyclesCoroutine(Action action, int fixedUpdates)
    {
        for (int x = 0; x < fixedUpdates; x++)
        {
            yield return new WaitForFixedUpdate();
        }

        action();

        yield return null;
    }

    /// <summary>
    /// Begins a coroutine that delays an action till the end of a number of frames
    /// <param name="action"> The action to perform at the end of the frame delay period</param>
    /// <param name="frames"> The number of frames to wait for with a minimum of 1</param>
    /// </summary
    public IEnumerator DelegateTillEndOfFrames(Action action, int frames = 1)
    {
        if (frames < 1)
        {
            frames = 1;
        }

        IEnumerator delegateTillEndOfFramesCoroutine = this.DelegateTillEndOfFramesCoroutine(action, frames);
        this.StartCoroutine(delegateTillEndOfFramesCoroutine);

        return delegateTillEndOfFramesCoroutine;
    }
    /// <summary>
    /// A coroutine that calls an action at the end of the frame
    /// </summary
    private IEnumerator DelegateTillEndOfFramesCoroutine(Action action, int frames = 1)
    {
        for (int x = 0; x < frames; x++)
        {
            yield return new WaitForEndOfFrame();
        }

        action();

        yield return null;
    }
}

