using UnityEngine;
/// <summary>
/// Ledge balancing an action which determines whether the player is at the tip of an edge
/// </summary>
public class LedgeBalancing : HedgePrimaryAction
{
    [SerializeField, Tooltip("The direction of the ledge relative to the player")]
    private LedgeDirection ledgeDirection = 0;
    [SerializeField, Tooltip("Extra check offset for the player.GetSensors()")]
    private float extraCheckOffset = 4;
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 11f;
        this.primaryAnimationID = ActionSubState.LedgeBalancing;
    }

    /// <summary>
    /// Perform the action when grounded and not moving
    /// </summary>
    public override bool CanPerformAction()
    {

        if (this.player.GetGrounded() && this.player.groundVelocity == 0 && General.CheckIfAngleIsZero(this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees()) && this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHitCount() == 1 && this.player.GetInputManager().GetCurrentInput().y == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// If there is only one hit within the controller begin the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.CanBeginLedgeBalancing())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Sets the ledge direction
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.player.GetAnimatorManager().SetOtherAnimationSubstate(this.player.currentPlayerDirection * this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection() * -1);
        this.ledgeDirection = (LedgeDirection)this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection();
    }

    /// <summary>
    /// Exits the ledge balance conditions based on the following
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded() == false || this.player.groundVelocity != 0 || this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().sensorHitData == SensorHitDirectionEnum.Both || this.player.GetInputManager().GetCurrentInput().y != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Reset the ledge direction and end the action
    /// </summary>
    public override void OnActionEnd()
    {
        this.ledgeDirection = 0;
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
    }

    /// <summary>
    /// Checks if the player is ahead or behind the ledge by checking if the raycast misses
    /// </summary>
    public bool CanBeginLedgeBalancing()
    {
        Vector2 position = this.player.transform.position;
        float castDistance = this.player.GetSensors().characterBuild.playerPixelPivotPoint + this.extraCheckOffset;
        RaycastHit2D ledgeBalanceSensor = Physics2D.Raycast(position, Vector2.down, castDistance, this.player.GetSensors().groundCollisionInfo.GetCollisionMask());
        Debug.DrawLine(position, position + (Vector2.down * castDistance), Color.red);

        return !ledgeBalanceSensor;
    }
}
