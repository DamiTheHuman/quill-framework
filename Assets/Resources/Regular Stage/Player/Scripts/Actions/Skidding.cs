using UnityEngine;
/// <summary>
/// Starts the player skidding action which adds an additional deceleration when moving opposite the direction of velocity
/// </summary>
public class Skidding : HedgePrimaryAction
{
    public SkidFlip skidFlip;
    [Tooltip("The minimum speed that can be attained before skidding is considered")]
    public float minSkidSpeed = 4f;
    [Tooltip("The extra deceleration added to the players velocity when skidding")]
    public float skidVelocityDepletion = 0.01666667f;
    [Tooltip("The direction the player was facing when skidding begain")]
    public int skidStartDirection = 0;

    public override void Reset()
    {
        this.actionID = 4;
        base.Reset();
    }

    /// <summary>
    /// Can only examine skidding when grounded with the appropriate speed and input is present
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded() && Mathf.Abs(this.player.groundVelocity) >= this.minSkidSpeed && this.player.currentGroundMode == GroundMode.Floor && this.player.GetActionManager().CheckActionIsBeingPerformed<Roll>() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// If the player inputs away from their opposite ground velocity
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetCurrentInput().x != Mathf.Sign(this.player.groundVelocity) && this.player.GetInputManager().GetCurrentInput().x != 0 && this.player.currentPlayerDirection == Mathf.Sign(this.player.groundVelocity))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Put the player in a skidding state
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.skidStartDirection = this.player.currentPlayerDirection;
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().skidding);
    }

    /// <summary>
    /// Adjust the players velocity by the skid rate
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.player.groundVelocity < 0 && this.player.GetInputManager().GetCurrentInput().x == 1)
        {
            this.player.groundVelocity = Mathf.Min(this.player.groundVelocity + this.skidVelocityDepletion, 0);
        }
        else if (this.player.groundVelocity > 0 && this.player.GetInputManager().GetCurrentInput().x == -1)
        {
            this.player.groundVelocity = Mathf.Max(this.player.groundVelocity - this.skidVelocityDepletion, 0);
        }
    }

    /// <summary>
    /// Exits the skid action with the appopriate conditions
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetInputManager().GetCurrentInput().x != -Mathf.Sign(this.player.groundVelocity) && this.player.GetAnimatorManager().CheckIfCurrentAnimationIsAtEnd())
        {
            return true;
        }
        if (this.player.GetGrounded() == false || (this.player.groundVelocity == 0) || this.skidStartDirection != this.player.currentPlayerDirection || this.player.GetActionManager().CheckActionIsBeingPerformed<Roll>())
        {
            if (this.player.GetActionManager().currentSubAction is not SkidFlip)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// The commands performed at the end of the drop dash action
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.usedSubAction = false;
    }
}
