/// <summary>
/// The look up action for the character which makes the character face upwards while 'STANDING' still
/// </summary>
public class LookUp : HedgePrimaryAction
{
    public override void Start() => base.Start();
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 2;
        base.Reset();
    }

    /// <summary>
    /// Perform the look up action when grounded and not moving
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded() && this.player.groundVelocity == 0 && this.player.GetActionManager().CheckActionIsBeingPerformed<SuperPeelOut>() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs up before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetCurrentInput().y == 1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// While looking up restrict the players x axis movement
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);
        HedgehogCamera.Instance().GetCameraLookHandler().SetLookUp(true);
    }

    /// <summary>
    /// Exits the look up conditions based on the criteria set
    /// </summary>
    public override bool ExitActionCondition()
    {

        if (this.player.GetInputManager().GetCurrentInput().y != 1 || this.player.GetGrounded() == false || this.player.groundVelocity != 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Restore the users input after the action is completed
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        HedgehogCamera.Instance().GetCameraLookHandler().SetLookUp(false);
    }
}
