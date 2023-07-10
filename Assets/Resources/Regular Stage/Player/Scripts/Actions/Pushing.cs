using UnityEngine;

/// <summary>
/// The ability of the player to push against a solid object
/// </summary>
public class Pushing : HedgePrimaryAction
{
    public int blockDirection = 0;
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 10f;
    }

    /// <summary>
    /// The pushing action can be performed as long as the player is grounded
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded() && General.RoundToDecimalPlaces(Mathf.Abs(this.player.groundVelocity)) <= this.player.currentAcceleration)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs in the directions of the wall player.GetSensors() collision direction
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().ForceGetCurrentInput().x == this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection() && this.player.GetInputManager().ForceGetCurrentInput().x != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the push action direction to the position of the collision
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.blockDirection = this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection();
    }

    /// <summary>
    /// Exits the push conditions based on the criteria set
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetInputManager().ForceGetCurrentInput().x != this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection() || this.player.GetInputManager().ForceGetCurrentInput().x == 0 || this.player.GetGrounded() == false || General.RoundToDecimalPlaces(Mathf.Abs(this.player.groundVelocity)) > this.player.currentAcceleration)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Reset the push direction and end the action
    /// </summary>
    public override void OnActionEnd()
    {
        this.blockDirection = 0;
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
    }
}
