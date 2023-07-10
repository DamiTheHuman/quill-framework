using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Get the current act clear controller on the scene
/// </summary>
public class ActClearBase : MonoBehaviour
{
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected InputMaster inputMaster;

    [SerializeField, FirstFoldOutItem("UI Objects")]
    protected Text timerBonusUI = null;
    [SerializeField]
    protected Text ringBonusUI = null;
    [SerializeField]
    protected Text coolBonusUI = null;
    [SerializeField]
    protected Text machBonusUI = null;
    [SerializeField, LastFoldoutItem()]
    protected Text endTotalCountUI = null;

    [Tooltip("The value assigned to the timer bonus")]
    [SerializeField, FirstFoldOutItem("Values")]
    protected int timerBonusValue;
    [Tooltip("The value assigned to the ring bonus")]
    [SerializeField]
    protected int ringBonusValue;
    [Tooltip("The value assigned to the cool bonus")]
    [SerializeField]
    protected int coolBonusValue;
    [SerializeField, Tooltip("The value assigned to the mach bonus")]
    protected int machBonusValue;
    [Tooltip("The value assigned to the total count value")]
    [SerializeField, LastFoldoutItem()]
    protected int totalCountValue;

    [SerializeField]
    protected AudioSource scoreAddAudioSource = null;
    [SerializeField]
    protected AudioClip scoreAddSound = null;
    [SerializeField]
    protected AudioClip scoreTotalSound = null;
    [Tooltip("The amount of time in steps till end animation is played Post Calculation")]
    public float timeTillEnd = 360;
    [Tooltip("The amount subtracted from the regular variables but added to the totalCountValue")]
    public int calculationSpeed = 100;
    [SerializeField]
    [Tooltip("The current state of the act clear HUD")]
    protected ActClearHUDState actClearHUDMode = ActClearHUDState.None;

    protected virtual void Awake()
    {
        this.inputMaster = new InputMaster();
        GameObject scoreAudioParent = new GameObject("Act Clear Audio");
        scoreAudioParent.transform.parent = this.transform;
        this.scoreAddAudioSource = new GameObject("Score Add Audio Source").AddComponent<AudioSource>();
        this.scoreAddAudioSource.transform.parent = scoreAudioParent.transform;
        this.scoreAddAudioSource.clip = this.scoreAddSound;
        this.scoreAddAudioSource.loop = true;
        this.scoreAddAudioSource.playOnAwake = false;
        this.inputMaster.Player.AButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Player.BButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Player.XButton.performed += ctx => this.OnInputPressed(ctx.phase);
    }


    protected virtual void OnEnable()
    {
        this.inputMaster.Enable();
        this.actClearHUDMode = ActClearHUDState.Available;
    }

    protected virtual void OnDisable() => this.inputMaster.Disable();

    /// <summary>
    /// When the player makes use of any of the mapped inputs
    /// </summary>
    protected virtual void OnInputPressed(InputActionPhase phase) { }
}
