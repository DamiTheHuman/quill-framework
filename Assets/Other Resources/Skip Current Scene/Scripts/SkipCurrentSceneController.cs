using UnityEngine;

/// <summary>
/// A helper that allows the user to load the next scene when the enter button is pressed
/// </summary>
public class SkipCurrentSceneController : MonoBehaviour
{
    [SerializeField]
    private InputMaster inputMaster;

    [SerializeField]
    private Transform skipCurrentSceneDebugText;

    private void Awake() => this.inputMaster = new InputMaster();

    private void Start()
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.Transition)
        {
            this.enabled = false;
            this.gameObject.SetActive(false);

            return;
        }

        this.inputMaster = new InputMaster();
        this.inputMaster.Player.StartButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Enable();
    }

    protected virtual void OnEnable()
    {
        if (this.inputMaster == null)
        {
            this.inputMaster = new InputMaster();
        }

        this.inputMaster.Enable();
    }

    protected virtual void OnDisable()
    {
        if (this.inputMaster != null)
        {
            this.inputMaster.Disable();
        }
    }


    /// <summary>
    /// Action that skips the current scene
    /// </summary>
    private void OnInputPressed(UnityEngine.InputSystem.InputActionPhase phase)
    {
        if (phase != UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            return;
        }

        GMSceneManager.Instance().LoadNextScene();
    }

    private void OnDrawGizmos()
    {
        if (this.skipCurrentSceneDebugText != null)
        {
            this.skipCurrentSceneDebugText.gameObject.SetActive(GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.Transition);

            Vector2 debugPosition = HedgehogCamera.Instance().transform.position;
            ;
            debugPosition.y += HedgehogCamera.Instance().GetBounds().size.y / 2 + 32;
            this.skipCurrentSceneDebugText.transform.position = debugPosition;
        }
    }

}
