using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GMStageHUDManager : MonoBehaviour
{
    [SerializeField, Tooltip("The regular stage HUD object")]
    private GameObject regularStageHUD = null;
    [SerializeField, Tooltip("The regular stage HUD object")]
    private GameObject specialStageHUD = null;
    [SerializeField, Tooltip("The title card displayed at zone start")]
    private GameObject titleCardUI = null;
    [SerializeField, Tooltip("The act clear UI")]
    private GameObject actClearUI = null;
    [SerializeField, Tooltip("The animation that plays after the act Clear UI")]
    private GameObject postActClearUI = null;
    [SerializeField, Tooltip("The UI for the life icon")]
    private SpriteRenderer lifeIconUI = null;
    [SerializeField, Tooltip("The UI for the superform")]
    private GameObject superFormUI = null;
    [SerializeField, Tooltip("The superform child elements")]
    private Image[] superFormUIChildElements = null;

    [SerializeField, Tooltip("The debug controller for the applcication")]
    private HUDDebugController debugController = null;

    /// <summary>
    /// The single instance of the regular HUD manager
    /// </summary>
    private static GMStageHUDManager instance;

    /// <summary>
    /// Get a reference to the static instance of the regular HUD manager
    /// </summary>
    public static GMStageHUDManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMStageHUDManager>();
        }

        return instance;
    }

    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        this.UpdateHUDVisibility();
        this.SetActClearUIActive(false);
        this.superFormUIChildElements = this.superFormUI.GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        this.ManageDebugHUDVisibility();
        this.ManageSuperFormHUDVisibility();
    }

    /// <summary>
    /// Enables the hud type based on the scene
    /// </summary>
    public void UpdateHUDVisibility()
    {
        if (this.specialStageHUD == null || this.regularStageHUD == null)
        {
            return;
        }

        if (GMSceneManager.Instance() == null)
        {
            return;
        }

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.SpecialStage)
        {
            if (this.regularStageHUD.activeSelf)
            {
                Debug.Log("Current scene is a special stage. Deactivating Regular Stage HUD");
            }

            this.specialStageHUD.SetActive(true);
            this.regularStageHUD.SetActive(false);
        }
        else if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage)
        {
            if (this.specialStageHUD.activeSelf)
            {
                Debug.Log("Current scene is a special stage. Deactivating Regular Stage HUD");
            }

            this.regularStageHUD.SetActive(true);
            this.specialStageHUD.SetActive(false);
        }
        else
        {
            if (this.regularStageHUD.activeSelf || this.specialStageHUD.activeSelf)
            {
                Debug.Log("Current scene is a not a stage. Deactivating Special Stage and Regular Stage HUD");
            }

            this.specialStageHUD.SetActive(false);
            this.regularStageHUD.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the visibility of the title card
    /// <param name="value">the new activate state for the  title card object</param>
    /// </summary>
    public void SetTitleCardActive(bool value) => this.titleCardUI.gameObject.SetActive(value);
    /// <summary>
    /// Sets the value of the act clear UI
    /// <param name="value">the new activate state for the act clear object</param>
    /// </summary>
    public void SetActClearUIActive(bool value) => this.actClearUI.SetActive(value);

    /// <summary>
    /// Sets the value of post act clear fadeout 
    /// <param name="value">the new activate state for the death scene</param>
    /// </summary>
    public void SetPostActClearFadeout(bool value) => this.postActClearUI.SetActive(value);

    /// <summary>
    /// Sets the material for the life icon
    /// <param name="material">the new material for the life icon</param>
    /// </summary>
    public void SetLifeIconMaterial(Material material)
    {
        this.lifeIconUI.material = material;
        this.lifeIconUI.sharedMaterial = material;
    }

    /// <summary>
    /// Get the act clear UI for the HUD
    /// </summary>
    public GameObject GetActClearUI() => this.actClearUI;

    /// <summary>
    /// Actions that take place after the player has left the view of the camera post death
    /// </summary>
    public void LaunchEndDeathSequence() => this.StartCoroutine(this.DecrementLifeCoroutine(1f));

    /// <summary>
    /// A coroutine that waits to decrement the players life after the set amount of time has passed
    /// <param name="waitTime">The amount of time to wait before reducing the life count</param>
    /// </summary>
    private IEnumerator DecrementLifeCoroutine(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);

        GMRegularStageScoreManager.Instance().DecrementLifeCount();
        GMSaveSystem.Instance().GetCurrentPlayerData().SerializeRegularStageData(GMStageManager.Instance().GetPlayer());
        GMSaveSystem.Instance().SaveAndOverwriteData();
        GMRegularStageScoreManager.Instance().UpdateUIElements();

        if (GMSaveSystem.Instance().GetCurrentPlayerData().GetLives() >= 0)
        {
            GMSceneManager.Instance().ReloadCurrentScene();
        }
        else
        {
            Debug.Log("GAME OVER!");
            GMSaveSystem.Instance().GetCurrentPlayerData().ResetLifeCountToDefault();
            GMSaveSystem.Instance().SaveAndOverwriteData();
            GMSceneManager.Instance().LoadTitleScreenScene();
        }
    }

    /// <summary>
    /// Handles the visibilty of the DEBUG HUD
    /// </summary>
    private void ManageDebugHUDVisibility()
    {
        bool debugMode = GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage
            && GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetDebugMode()
            && GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetShowDebugUI();

        this.debugController.gameObject.SetActive(debugMode);
    }

    /// <summary>
    /// Handles the visibilty of  super form icon
    /// </summary>
    private void ManageSuperFormHUDVisibility()
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.RegularStage)
        {
            return;
        }

        Player player = GMStageManager.Instance().GetPlayer();

        if (player != null)
        {
            SuperTransform superTransform = player.GetActionManager().GetAction<SuperTransform>() as SuperTransform;

            if (superTransform == null)
            {
                this.superFormUI.SetActive(false);

                return;
            }
            if (player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm)
            {
                this.superFormUI.SetActive(false);
                return;
            }
            else if (GMStageManager.Instance().GetBoss() != null)
            {
                if (GMStageManager.Instance().GetBoss().IsDefeated())
                {
                    this.superFormUI.SetActive(false);

                    return;
                }
            }
            else if (GMStageManager.Instance().GetStageState() == RegularStageState.ActClear)
            {
                this.superFormUI.SetActive(false);
                return;
            }
            if (GMRegularStageScoreManager.Instance().GetRingCount() < 50 || GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds() <7)
            {
                this.superFormUI.SetActive(false);
                return;
            }

            this.superFormUI.SetActive(true);
            this.SetSuperFormChildElementTransparency(superTransform.CanPerformAction());
        }
    }

    /// <summary>
    /// Sets the transparency of the child elements belonging to the super gameObject
    /// <param name="makeOpaque">Checks whether to make the UI element see through or not</param>
    /// </summary>
    private void SetSuperFormChildElementTransparency(bool makeOpaque)
    {
        if (this.superFormUIChildElements.Length > 1)
        {
            //If the colors are already faded or opaque skip
            if ((this.superFormUIChildElements[0].color.a == 1 && makeOpaque)
                || (this.superFormUIChildElements[0].color.a == 0.5
                && makeOpaque == false))
            {
                return;
            }

            float transparency = makeOpaque ? 1f : 0.5f;

            foreach (Image superChild in this.superFormUIChildElements)
            {
                superChild.color = new Color(superChild.color.r, superChild.color.g, superChild.color.b, transparency);
            }
        }
    }

    private void OnValidate() => this.UpdateHUDVisibility();
}
