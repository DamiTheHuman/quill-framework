using System.Collections;
/// <summary>
/// Turns the player with a skid flip animation
/// </summary>
public class SkidFlip : HedgeSubAction
{
    private Skidding skidding;
    private IEnumerator skidFlipCoroutine = null;
    public float minSkidTurnSpeed = 30f;
    public override void Start()
    {
        base.Start();
        this.skidding = this.GetComponentInParent<Skidding>();
    }
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 4.1f;
        this.parentAction = this.GetComponentInParent<Skidding>();
        this.skidding = this.GetComponentInParent<Skidding>();
        this.skidding.skidFlip = this;
        base.Reset();
    }

    /// <summary>
    /// The parameter s that must be smet before the skid flip can be considered
    /// </summary>
    public override bool CanPerformAction()
    {

        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Skidding>() && this.skidding.usedSubAction == false)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// The parameters that must be required before the move is launched
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetCurrentInput().x != 0)
        {
            if ((this.skidding.skidStartDirection > 0 && this.player.groundVelocity - (this.player.currentDeceleration * 2) < 0) || (this.skidding.skidStartDirection < 0 && this.player.groundVelocity + (this.player.currentDeceleration * 2) > 0) || this.player.groundVelocity == 0f)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Set up the skid flip and locks the players controls
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(this.subAnimationID);
        this.player.currentPlayerDirection = this.skidding.skidStartDirection * -1;//Flip the player
        this.player.GetSpriteController().SetSpriteDirection(this.player.currentPlayerDirection);//Update the sprite orientation
    }

    /// <summary>
    /// Exit the skid flip when the specified criteria is met
    /// </summary>
    public override bool ExitActionCondition()
    {
        if ((this.player.GetAnimatorManager().AnimationIsPlaying("Skid Turn") && this.player.GetAnimatorManager().GetCurrentAnimationNormalizedTime() > 1) || this.player.GetActionManager().CheckActionIsBeingPerformed<Roll>() || this.player.GetActionManager().CheckActionIsBeingPerformed<Skidding>() == false)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Ends the skid flip and sets the player to the necessary state where possibe
    /// </summary>
    public override void OnActionEnd()
    {
        if (this.skidFlipCoroutine != null)
        {
            this.StopCoroutine(this.skidFlipCoroutine);
        }

        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);

        if (this.player.GetGrounded())
        {
            SubState substate = this.player.groundVelocity == 0 ? SubState.Idle : SubState.Moving;//Switch to the running or idle substate based on the players ground velocity
            this.player.GetAnimatorManager().SwitchSubstate(substate);
            this.player.GetAnimatorManager().SwitchActionSubstate(0);
        }
    }
}
