using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
/// <summary>
/// Handles functionality directly relating to the pause menu of the game
/// </summary>
[RequireComponent(typeof(MenuConfirmationController))]
public class GMPauseMenuManager : MonoBehaviour
{
    [SerializeField]
    private bool paused = false;
    private RegularStageState previousStageState = RegularStageState.NotAvailable;
    [Tooltip("The confrimation menu for the in game pause menu"), SerializeField]
    private MenuConfirmationController confrimationMenu;
    [Tooltip("The pause menu UI of the stage"), SerializeField]
    private GameObject pauseMenuUI = null;
    [Tooltip("The pause menu node of the stage"), SerializeField]
    private GameObject pauseMenuUINode = null;
    [Tooltip("The event system for the pause menu"), SerializeField]
    public EventSystem pauseMenuEventSystem;

    [Tooltip("The active volume effects game objects"), SerializeField]
    private Transform gameVolume = null;
    [Tooltip("The current color adjustements set on the volume object")]
    public ColorAdjustments colorAdjustments;

    public static GMPauseMenuManager instance;

    // Start is called before the first frame update
    private void Start()
    {
        this.pauseMenuUINode.SetActive(false);
        Volume volume = this.gameVolume.GetComponent<Volume>();
        this.pauseMenuUI.SetActive(false);
        this.paused = false;

        if (volume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            this.colorAdjustments = colorAdjustments;
        }
    }

    /// <summary>
    /// Get a reference to the static instance of the pause menu manager
    /// </summary>
    public static GMPauseMenuManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMPauseMenuManager>();
        }

        return instance;
    }

    /// <summary>
    /// Checks whether the game is currently paaused
    /// </summary>
    public bool GameIsPaused() => Time.timeScale == 0 || this.paused;
    /// <summary>
    /// Whenever the users input manager hits the pause button
    /// <param name="phase">the phase of the pause button press for </param>
    /// </summary
    public void OnPauseButtonPress(InputActionPhase phase)
    {
        if (phase != InputActionPhase.Performed)
        {
            return;
        }

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() is not (SceneType.RegularStage or SceneType.SpecialStage))
        {
            return;
        }

        if (this.GameIsPaused() == false && Time.timeSinceLevelLoad > 1f)
        {
            this.OnPauseGame();
        }
    }

    /// <summary>
    /// Actions performed when the game is paused
    /// </summary>
    public void OnPauseGame()
    {
        this.pauseMenuUI.SetActive(true);
        this.paused = true;
        this.EnablePauseMenuUINode();
        this.colorAdjustments.saturation.value = -100;//Makes the screen black & white
        GMAudioManager.Instance().PauseAudio();
        Time.timeScale = 0f;//Freeze the game
        this.previousStageState = GMStageManager.Instance().GetStageState();
    }

    /// <summary>
    /// Actions performed when the game is to be resumed
    /// </summary>
    public void OnResumeGame()
    {
        this.paused = false;
        this.colorAdjustments.saturation.value = 0f;
        this.pauseMenuUINode.SetActive(false);
        GMAudioManager.Instance().ResumeAudio();
        this.pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Prepares the continue functionality based on the users button selection
    /// </summary>
    public void OnContinuePressed() => this.OnResumeGame();
    /// <summary>
    /// Prepares the restart functionality based on the users button selection
    /// </summary>
    public void OnResetartPressed()
    {
        this.DisablePauseMenuUINode();
        this.confrimationMenu.SetConfirmation(this.pauseMenuEventSystem, GMStageManager.Instance().RestartStage, this.EnablePauseMenuUINode);
    }

    /// <summary>
    /// Prepares to send the user back to the main menu
    /// </summary>
    public void OnExitPressed()
    {
        this.DisablePauseMenuUINode();
        this.confrimationMenu.SetConfirmation(this.pauseMenuEventSystem, GMStageManager.Instance().ExitGame, this.EnablePauseMenuUINode);
    }

    /// <summary>
    /// Disable the pause menu UI Node
    /// </summary>
    private void DisablePauseMenuUINode() => this.pauseMenuUINode.SetActive(false);
    /// <summary>
    /// Enable the pause menu UI Node
    /// </summary>
    private void EnablePauseMenuUINode()
    {
        this.pauseMenuUINode.SetActive(true);
        this.pauseMenuEventSystem.SetSelectedGameObject(this.pauseMenuEventSystem.firstSelectedGameObject);
        Animator buttonAnimator = this.pauseMenuEventSystem.firstSelectedGameObject.GetComponent<Animator>();

        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("Normal");
            buttonAnimator.SetTrigger("Selected");
        }
    }

}
