using UnityEngine;

public class Spindash : HedgePrimaryAction
{
    [Tooltip("The current rev of the spin dash")]
    public float currentSpinRev = 0;
    [SerializeField]
    [Tooltip("The minimum rev that can be attained from spindashing")]
    public float minSpindashRev = 8;
    [Tooltip("How much is added to the currentSpinRev ever Jump input")]
    [SerializeField]
    private float spinRevCharge = 2;
    [Tooltip("How much is depleted from the currentSpinRev every physics step")]
    [SerializeField]
    private float spinDepletionValue = 32f;
    private AudioSource spinDashAudioSource;
    public override void Start()
    {
        base.Start();
        this.SetUpSpindashRevAudio();
    }

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.attackingAction = true;
        this.sizeMode = SizeMode.Shrunk;
        this.actionID = 5;
        base.Reset();
    }

    /// <summary>
    /// Creates an audio component specifically for the spindash rev
    /// </summary>
    public void SetUpSpindashRevAudio()
    {
        GameObject audioObject = new GameObject();
        audioObject.transform.parent = this.transform;
        audioObject.transform.localPosition = Vector3.zero;
        audioObject.name = "Spindash Audio Object";
        this.spinDashAudioSource = audioObject.AddComponent<AudioSource>();
        this.spinDashAudioSource.playOnAwake = false;
        this.spinDashAudioSource.clip = this.player.GetPlayerActionAudio().spindashCharge;
    }

    /// <summary>
    /// Can Perform the spindash action when grounded and crouching
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded() && this.player.GetActionManager().CheckActionIsBeingPerformed<Crouch>() && this.player.GetActionManager().CheckActionIsBeingPerformed<Victory>() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs the jump button before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Add velocity to the players spin
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.SpindashDust, true);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.currentSpinRev = 0;
    }

    /// <summary>
    /// Contionously watch to add velocity to the players spin until its done
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed())
        {
            this.currentSpinRev += this.spinRevCharge;
            this.spinDashAudioSource.Play();//Play the spindash rev
        }
        else
        {
            this.currentSpinRev -= this.currentSpinRev / this.spinDepletionValue;
        }
    }

    /// <summary>
    /// Exits the Spindash when the player stops holding down
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetInputManager().GetCurrentInput().y != -1 || this.player.GetGrounded() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Launches the roll action on end
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.SpindashDust, false);
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);

        if (this.player.GetGrounded())
        {
            this.player.groundVelocity = ((this.currentSpinRev / 2.0f) + this.minSpindashRev) * this.player.currentPlayerDirection;
            this.player.velocity = this.player.CalculateSlopeMovement(this.player.groundVelocity);
            HedgehogCamera.Instance().GetCameraLookHandler().BeginDashLag();
        }
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().spindashRelease);
        this.player.GetActionManager().PerformAction<Roll>();
    }
}
