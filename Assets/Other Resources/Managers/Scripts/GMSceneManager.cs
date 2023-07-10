using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/// <summary>
/// Controls scene transitions and alike
/// </summary>
public class GMSceneManager : MonoBehaviour
{
    [SerializeField, Tooltip("A list of scenes that help keep track of what scene to load next")]
    private SceneListScriptableObject sceneList;
    [SerializeField, Tooltip("The current scenes data"), Help("This Value is set by the scene list")]
    private SceneData currentSceneData;
    [Tooltip("The gameobject signifying the start point of the stage"), SerializeField, IsDisabled]
    private StartPointController currentStartPoint;
    [Tooltip("The active blit controller for all our scenes"), SerializeField]
    private FXBlitController blitController;
    [SerializeField]
    private SceneTransitionType currentSceneTransition = SceneTransitionType.Fade;
    [Help("If the scene you are looking to add is not displaying remember to add it to the 'Scenes in Build' in  File/Build Settings...")]
    [Tooltip("Determines whethere a scene is being loaded"), SerializeField]
    private bool isLoadingNextScene;

    /// <summary>
    /// The single instance of the scene manager
    /// </summary>
    private static GMSceneManager instance;
    public delegate void OnDismountScene();
    public static event OnDismountScene OnDismountSceneEvent;
    private GizmosEnabledHelper gizmosEnabledHelper;

    public void Awake()
    {
        SceneData sceneData = this.sceneList.GetSceneData(SceneManager.GetActiveScene().buildIndex);

        if (sceneData != null)
        {
            this.currentSceneData = new SceneData(sceneData);
        }

        this.FindStartPoint();

        if (this.currentStartPoint != null)
        {
            GMCutsceneManager.Instance().SetActStartCutscene(this.currentStartPoint.GetActStartCutscene());
        }

        if (this.currentSceneData.GetSceneType() == SceneType.RegularStage)
        {
            Player player = GMCharacterManager.Instance().CheckToInstantiatePlayer();
        }
        else if (this.currentSceneData.GetSceneType() == SceneType.SpecialStage)
        {
            GMCharacterManager.Instance().CheckToInstantiateSpecialStagePlayer();
        }

#if UNITY_EDITOR
        this.gizmosEnabledHelper = this.gameObject.AddComponent<GizmosEnabledHelper>();
#endif
        Time.timeScale = 1;
    }

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;

        if (this.currentSceneTransition == SceneTransitionType.Fade)
        {
            this.blitController.BeginFadeIn();
        }
    }
#if UNITY_EDITOR
    // Update is called once per frame
    private void Update()
    {
        Keyboard keyboard = InputSystem.GetDevice<Keyboard>();

        if (keyboard.tKey.wasPressedThisFrame)
        {
            this.LoadNextScene();
        }
    }
#endif
    /// <summary>
    /// Get a reference to the static instance of the scene manager
    /// </summary>
    public static GMSceneManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMSceneManager>();
        }

        return instance;
    }

    /// <summary>
    /// Gets the value of the current scene
    /// </summary>
    public SceneData GetCurrentSceneData() => this.currentSceneData;

    /// <summary>
    /// Reloads/Restarts the current active scene
    /// </summary>
    public void ReloadCurrentScene() => this.SwitchScene(this.currentSceneData.GetSceneId());

    /// <summary>
    /// Loads the next scene
    /// </summary>
    public void LoadNextScene() => this.StartCoroutine(this.TransitionLevelLoad(this.currentSceneData.GetNextSceneId()));

    /// <summary>
    /// Switches from the current scene to the specified scene
    /// </summary>
    public void SwitchScene(int sceneToLoad)
    {
        if (this.isLoadingNextScene)
        {
            return;
        }

        sceneToLoad = Mathf.Max(0, sceneToLoad);//ensure the target scene exists if not go back to the disclaimer scene
        this.StartCoroutine(this.TransitionLevelLoad(sceneToLoad));
    }

    /// <summary>
    /// Transitions to the next smooth but adds a delay on the certain scenes
    /// </summary>
    private IEnumerator TransitionLevelLoad(int sceneToLoad)
    {
        this.isLoadingNextScene = true;
        Time.timeScale = 0;
        HedgehogCamera.Instance().GetCameraRenderFreezeHandler().UpdateFreezeCameraImage();
        HedgehogCamera.Instance().GetCameraRenderFreezeHandler().SetFreezeRenderImageVisibility(true);

        yield return new WaitForEndOfFrame();

        this.InvokeRepool();

        if (this.currentSceneTransition == SceneTransitionType.Fade)
        {
            this.blitController.BeginFadeOut();

            yield return new WaitUntil(() => this.blitController.GetTransitionState() == BlitTransitionState.None);
        }

        SceneManager.LoadScene(sceneToLoad);
        this.isLoadingNextScene = false;

        yield return null;
    }

    /// <summary>
    /// Find the start point for the player if available
    /// </summary>
    public void FindStartPoint()
    {
        if (this.currentStartPoint == null)
        {
            StartPointController[] characterSpecificStartPointControllers = FindObjectsOfType<StartPointController>().Where(x => x.startPointType == StartPointType.CharacterSpecific && x.gameObject.activeSelf).ToArray();

            if (characterSpecificStartPointControllers != null)
            {
                foreach (StartPointController characterSpecificStartPoint in characterSpecificStartPointControllers)
                {
                    if (characterSpecificStartPoint.GetCharactersToSpawnAtUniquePoint().Contains(GMSaveSystem.Instance().GetCurrentPlayerData().GetCharacter()))
                    {
                        this.currentStartPoint = characterSpecificStartPoint;
                        GMCutsceneManager.Instance().SetActStartCutscene(this.currentStartPoint.GetActStartCutscene());
                        Debug.Log("Using a Unique start point for character: " + GMSaveSystem.Instance().GetCurrentPlayerData().GetCharacter());

                        return;
                    }
                }
            }

            this.currentStartPoint = FindObjectsOfType<StartPointController>().Where(x => x.startPointType == StartPointType.Default).First();
        }

        if (this.currentStartPoint == null)
        {
            Debug.Log("NO START POINT FOUND");
        }
    }

    /// <summary>
    /// Get the start point object of the act
    /// </summary>
    public StartPointController GetCurrentStartPoint()
    {
        if (this.currentStartPoint == null)
        {
            this.FindStartPoint();
        }

        return this.currentStartPoint;
    }

    /// <summary>
    /// Get the value that determines if the next scene is being loaded
    /// </summary>
    public bool IsLoadingNextScene() => this.isLoadingNextScene;

    /// <summary>
    /// Repool everything in the scene
    /// </summary>
    public void InvokeRepool() => OnDismountSceneEvent?.Invoke();


    /// <summary>
    /// Go Back to the title screen
    /// </summary>
    public void LoadTitleScreenScene() => this.SwitchScene(this.sceneList.introScenes[0].GetSceneId());

    /// <summary>
    /// Get the scene list
    /// </summary>
    public SceneListScriptableObject GetSceneList() => this.sceneList;

    /// <summary>
    /// Checks whether the gizmos in a scene are active
    /// </summary>
    public bool GetGizmosEnabled()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            return true;
        }
        else if (this.gizmosEnabledHelper != null)
        {
            return this.gizmosEnabledHelper.GetGizmosEnabled();
        }
#endif
        return false;
    }

    /// <summary>
    /// Get access to the g
    /// </summary>
    public FXBlitController GetBlitController() => this.blitController;

    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            SceneData previousSceneData = this.currentSceneData;
            SceneData sceneData = this.sceneList.GetSceneData(SceneManager.GetActiveScene().buildIndex);

            if (sceneData != null)
            {
                this.currentSceneData = new SceneData(sceneData);

                if (this.currentSceneData != previousSceneData)
                {
                    General.SetDirty(this.gameObject);
                }
            }
        }

        if (GMStageHUDManager.Instance() != null)
        {
            GMStageHUDManager.Instance().UpdateHUDVisibility();
        }

        foreach (UIActDependantController UIActDependantController in FindObjectsOfType<UIActDependantController>())
        {
            UIActDependantController.UpdateActImage();
        }
    }
}
