using UnityEngine;
/// <summary>
/// The super transform action, place this on chracters that can transform
/// </summary>
public class SuperTransform : HedgeSubAction
{
    [SerializeField, Tooltip("Check if the transform audio has been played")]
    private bool audioPlayed = false;
    public override void Start()
    {
        this.player.GetAnimatorManager().GetAnimator().updateMode = UnityEngine.AnimatorUpdateMode.Normal;
        base.Start();
    }

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 1.99f;
        this.parentAction = this.GetComponentInParent<Jump>();
    }

    /// <summary>
    /// Can only perform the dropdash while jumping and only if the Jump sub action has not been used up
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>() && this.parentAction.usedSubAction == false
            && GMGrantPowerUpManager.Instance().GetSuperFormAvailable())
        {
            //The boss is defeated so no retransforming
            if (GMStageManager.Instance().GetBoss() != null)
            {
                if (GMStageManager.Instance().GetBoss().IsDefeated())
                {
                    return false;
                }
            }
            if (GMStageManager.Instance().GetStageState() == RegularStageState.ActClear)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs the jump button again before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetSpecialButton().GetButtonPressed())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Begins the transformation process when the button is input
    /// </summary>
    public override void OnActionStart()
    {
        this.player.SetGrounded(false);
        this.player.GetAnimatorManager().SwitchSubstate(SubState.Aerial);
        this.player.GetAnimatorManager().SwitchActionSubstate(this.parentAction.primaryAnimationID);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(this.subAnimationID);//Play the transformation animation
        this.player.velocity.y = 0;
        this.player.SetBothHorizontalVelocities(0);
        this.player.GetHealthManager().SetHealthStatus(HealthStatus.Invulnerable);//Make the player intangible to attacks but not invincible :D
        this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
    }

    public override void OnPerformingAction()
    {
        this.player.velocity.y = 0;
        this.player.GetAnimatorManager().SwitchSubstate(SubState.Aerial);
        this.player.SetBothHorizontalVelocities(0);
        this.player.SetGrounded(false);

        if (this.player.GetAnimatorManager().AnimationIsPlaying("Transform End") && this.audioPlayed == false)
        {
            this.audioPlayed = true;
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().superForm);
        }
    }

    /// <summary>
    /// Exits transformation when the animation sequence is over
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetAnimatorManager().AnimationIsPlaying("Transform End") && this.player.GetAnimatorManager().GetCurrentAnimationNormalizedTime() > 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Switches out the current character post transformation
    /// </summary>
    public override void OnActionEnd()
    {
        this.audioPlayed = false;
        this.player.GetHedgePowerUpManager().GoSuperForm();
        this.player.SetAttackingState(false);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetAnimatorManager().SwitchSubstate(0);
    }
}
