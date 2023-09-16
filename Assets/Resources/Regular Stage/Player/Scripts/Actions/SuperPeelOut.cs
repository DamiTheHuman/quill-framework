using System.Collections;
using UnityEngine;
/// <summary>
/// Grants player the ability to the user Super Peel Out from Sonic CD
/// </summary>
public class SuperPeelOut : HedgePrimaryAction
{
    [Tooltip("How many steps to charge the peel out"), SerializeField]
    private float superPeelOutChargeTime = 30;
    [SerializeField]
    private bool isSuperPeelOutCharged = false;
    [Tooltip("The speed of the player after charging"), SerializeField]
    private float superPeelOutReleaseValue = 12;
    private IEnumerator superPeelOutCoroutine;
    private AudioSource superPeelOutAudioSource;

    public override void Start()
    {
        base.Start();
        this.SetUpSuperPeelOutAudio();
    }

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = (int)ActionSubState.SuperPeelOut;
        this.isSuperPeelOutCharged = false;
        base.Reset();
    }

    /// <summary>
    /// Creates an audio component specifically for the ><see cref="superPeelOutAudioSource"/>
    /// </summary>
    public void SetUpSuperPeelOutAudio()
    {
        GameObject audioObject = new GameObject();
        audioObject.transform.parent = this.transform;
        audioObject.transform.localPosition = Vector3.zero;
        audioObject.name = "Super Peel Out Audio Object";
        this.superPeelOutAudioSource = audioObject.AddComponent<AudioSource>();
        this.superPeelOutAudioSource.playOnAwake = false;
        this.superPeelOutAudioSource.clip = this.player.GetPlayerActionAudio().spindashCharge;
    }

    /// <summary>
    /// Can Perform the <see cref="SuperPeelOut"/> action when grounded and crouching
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded()
            && this.player.GetActionManager().CheckActionIsBeingPerformed<LookUp>()
            && this.player.GetActionManager().CheckActionIsBeingPerformed<Victory>() == false)
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
    /// Reset players peel out velocity to a 0 val
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.SpindashDust, true);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.isSuperPeelOutCharged = false;
        this.superPeelOutAudioSource.Play();
        this.superPeelOutCoroutine = this.ChargeSuperPeelOut();
        this.StartCoroutine(this.superPeelOutCoroutine);
    }

    /// <summary>
    /// Exits the Super Peel out when the player stops holding up or leaves the ground
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetInputManager().GetCurrentInput().y != 1 || this.player.GetGrounded() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Launches the player at the end if <see cref="isSuperPeelOutCharged"/> is true otherwise nothing happens
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.SpindashDust, false);
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.superPeelOutAudioSource.Stop();

        if(this.superPeelOutCoroutine != null)
        {
            this.StopCoroutine(this.superPeelOutCoroutine);
            this.superPeelOutCoroutine = null;
        }

        if (this.player.GetGrounded() && this.isSuperPeelOutCharged)
        {
            this.player.groundVelocity = this.superPeelOutReleaseValue * this.player.currentPlayerDirection;
            this.player.velocity = this.player.CalculateSlopeMovement(this.player.groundVelocity);
            HedgehogCamera.Instance().GetCameraLookHandler().BeginDashLag();
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().spindashRelease);
        }

        this.isSuperPeelOutCharged = false;
    }


    /// <summary>
    /// Waits until <see cref="superPeelOutChargeTime"/> steps before marking the peel out as charged
    /// </summary>
    private IEnumerator ChargeSuperPeelOut()
    {
        this.isSuperPeelOutCharged = false;
        yield return new WaitForSeconds(General.StepsToSeconds(this.superPeelOutChargeTime));
        this.isSuperPeelOutCharged = true;
        yield return null;
    }

}
