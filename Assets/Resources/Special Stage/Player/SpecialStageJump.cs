using UnityEngine;
/// ** FOR SPECIAL STAGES **
public class SpecialStageJump : SpecialStageHedgeAction
{
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 1;
        this.attackingAction = true;
        base.Reset();
    }

    /// <summary>
    /// Perform the jump action when grounded
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded())
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
    /// Add velocity to the players jump
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetInputManager().GetJumpButton().SetButtonPressed(false);//Exhaust the jumpdown
        this.player.GetAnimatorManager().SwitchActionSubstate(ActionSubState.Jump);
        this.player.SetGrounded(false);
        float angleInRadians = GMSpecialStageManager.Instance().GetSpecialStageSlider().GetAngle() * Mathf.Deg2Rad;
        this.player.SetGrounded(false);
        this.player.velocity = this.CalculateJumpVelocity(angleInRadians);//Get the jump velocity in relation the current ground angle
        this.player.transform.parent = GMSpecialStageManager.Instance().GetSpecialStageSlider().GetPlayerParent().parent.transform;
        GMSpecialStageManager.Instance().GetSpecialStageSlider().SetAngle(Mathf.Asin(this.player.transform.position.x / GMSpecialStageManager.Instance().GetSpecialStageSlider().GetRange()) * Mathf.Rad2Deg);
        GMSpecialStageManager.Instance().GetSpecialStageSlider().CalculatePlayerPosition();
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().jump);
        this.player.transform.eulerAngles = new Vector3(90, 180, 0);
        GMSpecialStageManager.Instance().GetPlayer().GetSpriteController().transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Exits the jump conditions based on the criteria set as long as the player is not transforming
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// End the jump animation
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.SetGrounded(true);
    }

    /// <summary>
    /// Calculate the velocity of the jump based on the angle of the player
    /// <param name="angleInRadians"> The angle of the current terrain in radians</param>
    /// </summary>
    private Vector2 CalculateJumpVelocity(float angleInRadians)
    {
        Vector2 newVelocity = this.player.velocity;
        ;
        newVelocity.x -= this.player.GetPlayerPhysicsInfo().specialStageJumpVelocity * Mathf.Sin(angleInRadians);
        newVelocity.y += -this.player.GetPlayerPhysicsInfo().specialStageJumpVelocity * Mathf.Cos(angleInRadians);

        return newVelocity;
    }
}
