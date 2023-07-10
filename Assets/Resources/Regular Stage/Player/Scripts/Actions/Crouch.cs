/// <summary>
/// The crouch action for the character which makes the character crouch while 'STANDING' still
/// </summary>
public class Crouch : HedgePrimaryAction
{
    public override void Start() => base.Start();
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 3;
        this.sizeMode = SizeMode.Shrunk;
        base.Reset();
    }

    /// <summary>
    /// Perform the crouch action when grounded and not moving
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded() && this.player.groundVelocity == 0 && this.player.GetActionManager().CheckActionIsBeingPerformed<Roll>() == false && this.player.GetActionManager().CheckActionIsBeingPerformed<Spindash>() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs down before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetCurrentInput().y == -1)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// While crouching restrict the players x axis movement
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);
        HedgehogCamera.Instance().GetCameraLookHandler().SetLookDown(true);
    }

    /// <summary>
    /// Exits the crouch action conditions based on the criteria set
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetInputManager().GetCurrentInput().y != -1 || this.player.GetGrounded() == false || this.player.groundVelocity != 0 || this.player.GetActionManager().CheckActionIsBeingPerformed<Spindash>())
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
        HedgehogCamera.Instance().GetCameraLookHandler().SetLookDown(false);
    }
}

