using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Controls the special stages
/// </summary>
public class GMSpecialStageManager : MonoBehaviour
{
    [SerializeField, Tooltip("The state of the special stage")]
    private SpecialStageState specialStageState = SpecialStageState.NotAvailable;
    [Tooltip("The main player object"), SerializeField]
    private SpecialStagePlayer player;
    [Tooltip("The slider that moves the player while on the special stage"), SerializeField]
    private SpecialStageSliderController specialStageSliderController;
    [Tooltip("The special stage message object of the scene"), SerializeField]
    private HUDSpecialStageMessageController specialStageMessageController;
    [Tooltip("The audio when a player goes in or out of a special stage"), SerializeField]
    private AudioClip specialStageWarpClip;
    [Tooltip("The audio when a player exits a special stage"), SerializeField]
    private AudioClip specialStageExitClip;

    [SerializeField]
    private HalfPipePalleteScriptableObject halfPipePalette;

    /// <summary>
    /// The single instance of the stage manager
    /// </summary>
    private static GMSpecialStageManager instance;

    private void Reset()
    {
        this.player = null;
        this.specialStageState = SpecialStageState.NotAvailable;
        this.specialStageSliderController = null;
    }

    private void Awake()
    {
        this.transform.parent = null;
        DontDestroyOnLoad(this);

        if (Instance() != null && Instance() != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);//Destroy duplicate stage managers (Worthless Consumer Models)

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
        this.FindSpecialStageSlider();

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.SpecialStage)
        {
            this.SetSpecialStageState(SpecialStageState.Idle);

            if (GMSceneManager.Instance().GetCurrentStartPoint() != null)
            {
                this.GetSpecialStageSlider().transform.position = GMSceneManager.Instance().GetCurrentStartPoint().transform.position;//Move the slider to the start postion
            }

            HedgehogCamera.Instance().SetCameraTarget(this.player.gameObject);
            GMAudioManager.Instance().PlayBGM(BGMToPlay.MainBGM);
            this.halfPipePalette.UpdatePaletteMaterial(Instance().GetNextEmeraldNumber());
        }
        else
        {
            this.SetSpecialStageState(SpecialStageState.NotAvailable);
        }
    }

    /// <summary>
    /// The actions performed on every scene reload
    /// </summary>
    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        this.Reset();
        this.OnSceneLoaded();
    }

    /// <summary>
    /// Get a reference to the static instance of the special stage manager
    /// </summary>
    public static GMSpecialStageManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMSpecialStageManager>();
            SceneManager.sceneLoaded += instance.OnLoadCallback;
        }

        return instance;
    }

    /// <summary>
    /// Loads a special stage based on the current emeralds the player has
    /// </summary>
    public void LoadSpecialStage()
    {
        GMAudioManager.Instance().PlayOneShot(this.specialStageWarpClip);
        int specialStageIndex = this.GetNextEmeraldNumber();
        this.SetSpecialStageState(SpecialStageState.Idle);
        GMSceneManager.Instance().SwitchScene(GMSceneManager.Instance().GetSceneList().specialStageScenes[specialStageIndex].GetSceneId());
    }

    /// <summary>
    /// Go back to the regular stage scene
    /// </summary>
    public void GoBackToRegularStage()
    {
        GMAudioManager.Instance().PlayOneShot(this.specialStageWarpClip);
        GMSceneManager.Instance().SwitchScene(GMSaveSystem.Instance().GetCurrentPlayerData().GetCurrentScene().GetSceneId());
    }

    /// <summary>
    /// Load the act clear scene
    /// </summary>
    private void LoadActClearScene()
    {
        GMAudioManager.Instance().MuteBGM(BGMToPlay.MainBGM);
        GMAudioManager.Instance().PlayOneShot(this.specialStageExitClip);
        GMHistoryManager.Instance().SaveSpecialStageHistory();
        GMSceneManager.Instance().SwitchScene(GMSceneManager.Instance().GetSceneList().specialStageActClearScene.GetSceneId());
    }

    /// <summary>
    /// Gets the next emerald number to load
    /// </summary>
    public int GetNextEmeraldNumber() => Mathf.Clamp(GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds(), 0, GMSceneManager.Instance().GetSceneList().specialStageScenes.Count - 1);

    /// <summary>
    /// Actions performed after the clears the special stage, saving and reloading etc;
    /// </summary>
    public void SpecialStageActClear()
    {
        GMSaveSystem.Instance().GetCurrentPlayerData().IncrementChaosEmeraldCount();//Grant the player an emerald
        GMSaveSystem.Instance().GetCurrentPlayerData().SerializeSpecialStageData(this.GetPlayer());
        GMSaveSystem.Instance().SaveAndOverwriteData();
        this.specialStageState = SpecialStageState.Clear;
        this.Invoke(nameof(LoadActClearScene), 1f);
    }

    /// <summary>
    /// Actions that take place when a special stage is failed
    /// <param name=failureReason"> Sets the reason the player failed the special stage with a default of hit orb</param>
    /// </summary>
    public void SpecialStageFailed(SpecialStageFailureReason failureReason = SpecialStageFailureReason.HitOrb)
    {
        this.specialStageState = SpecialStageState.Failed;
        this.GetSpecialStageSlider().SetTravelSpeed(0);

        if (failureReason == SpecialStageFailureReason.TimeOver)
        {
            this.specialStageMessageController.DisplayMessage(SpecialStageMessage.TimeOver);
            this.Invoke(nameof(LoadActClearScene), 2f);

            return;
        }

        this.Invoke(nameof(LoadActClearScene), 1f);
    }

    /// <summary>
    /// Finds the active player within the scene
    /// </summary
    public void FindPlayer()
    {
        SpecialStagePlayer[] playerObjects = FindObjectsOfType<SpecialStagePlayer>();

        for (int x = 0; x < playerObjects.Length; x++)
        {
            if (playerObjects[x].GetSleep() == false)
            {
                this.SetPlayer(playerObjects[x]);

                return;
            }
        }
    }

    /// <summary>
    /// Sets the current active player
    /// <param name="player">the new player value</param>
    /// </summary
    public void SetPlayer(SpecialStagePlayer player) => this.player = player;
    /// <summary>
    /// Get the current active player
    /// </summary
    public SpecialStagePlayer GetPlayer()
    {
        if (this.player == null)
        {
            this.FindPlayer();
        }

        return this.player;
    }

    /// <summary>
    /// Find the special stage slider in the scene
    /// </summary>
    public void FindSpecialStageSlider() => this.SetSpecialStageSlider(FindObjectOfType<SpecialStageSliderController>());
    /// <summary>
    /// Get the current special stage slider
    /// </summary>
    public SpecialStageSliderController GetSpecialStageSlider()
    {
        if (this.specialStageSliderController == null)
        {
            this.FindSpecialStageSlider();
        }

        return this.specialStageSliderController;
    }

    /// <summary>
    /// Sets the current active special stage slider
    /// <param name="specialStageSlider">the new special stage slider</param>
    /// </summary
    public void SetSpecialStageSlider(SpecialStageSliderController specialStageSlider) => this.specialStageSliderController = specialStageSlider;
    /// <summary>
    /// Get the current special stage message controller
    /// </summary>
    public HUDSpecialStageMessageController GetSpecialStageMessageController() => this.specialStageMessageController;

    /// <summary>
    /// Sets the current special stage message controller
    /// <param name="specialStageMessageController">the new special stage message controller</param>
    /// </summary
    public void SetSpecialStageMessageController(HUDSpecialStageMessageController specialStageMessageController) => this.specialStageMessageController = specialStageMessageController;
    /// <summary>
    /// Get the current state of the special stage
    /// </summary>
    public SpecialStageState GetSpecialStageState() => this.specialStageState;
    /// <summary>
    /// Sets the current state of the special stage
    /// </summary
    public void SetSpecialStageState(SpecialStageState specialStageState) => this.specialStageState = specialStageState;

    private void OnValidate()
    {
        if (this.halfPipePalette == null || GMSceneManager.Instance() == null)
        {
            return;
        }

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.SpecialStage)
        {
            this.halfPipePalette.UpdatePaletteMaterial(Instance().GetNextEmeraldNumber());
        }
    }
}
