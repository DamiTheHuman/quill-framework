using UnityEngine;

public class Jump : HedgePrimaryAction
{
    [Tooltip("Flag to check if the player was rolling before jumping"), SerializeField]
    private bool rollJump;
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 1;
        this.sizeMode = SizeMode.Shrunk;
        this.attackingAction = true;
        base.Reset();
    }

    /// <summary>
    /// Perform the jump action when grounded
    /// </summary>
    public override bool CanPerformAction()
    {
        bool playerIsAttemptingSuperPeelOut = this.player.GetActionManager().CheckActionIsBeingPerformed<LookUp>() && this.player.GetActionManager().GetAction<SuperPeelOut>();
        if (this.player.GetGrounded()
            && this.player.GetActionManager().CheckActionIsBeingPerformed<Crouch>() == false
            && playerIsAttemptingSuperPeelOut == false
            && this.player.GetActionManager().CheckActionIsBeingPerformed<Spindash>() == false
            && this.player.GetActionManager().CheckActionIsBeingPerformed<SuperPeelOut>() == false
            && this.player.GetSensors().GetLowCeiling() == false
            && this.player.GetActionManager().CheckActionIsBeingPerformed<Victory>() == false
            && this.player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.Sliding)
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
        this.usedSubAction = false;//reset the used sub action flag
        this.player.GetInputManager().GetJumpButton().SetButtonUp(false);
        this.player.GetInputManager().GetJumpButton().SetButtonPressed(false);//Exhaust the jump button pressed so actions like dropdash or shield abilities can be notified on the next input
        //Roll jump so no directional input is allowed
        if (this.player.GetActionManager().currentPrimaryAction is Roll)
        {
            this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);
            this.rollJump = true;
        }

        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        float angleInRadians = this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians();
        this.player.SetGrounded(false);
        this.player.velocity = this.CalculateJumpVelocity(angleInRadians);//Get the jump velocity in relation the current ground angle
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().jump);
    }

    /// <summary>
    /// After adding velocity to the jump move check for variable jump
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.ShouldSetToMinJumpThreshold())
        {
            this.player.velocity.y = this.player.currentJumpReleaseThreshold;
        }
    }

    /// <summary>
    /// Exits the jump conditions based on the criteria set as long as the player is not transforming or gliding
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded())
        {
            if (!(this.player.GetActionManager().CheckActionIsBeingPerformed<SuperTransform>() || this.player.GetActionManager().CheckActionIsBeingPerformed<Glide>()))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// End the jump animation
    /// </summary>
    public override void OnActionEnd()
    {
        if (this.player.GetActionManager().currentSubAction is not DropDash)
        {
            this.player.GetAnimatorManager().SwitchActionSubstate(0);
        }
        //If a roll jump is performed unrestic them
        if (this.rollJump)
        {
            this.rollJump = false;
            this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        }
    }

    /// <summary>
    /// Calculate the velocity of the jump based on the angle of the player
    /// <param name="angleInRadians"> The angle of the current terrain in radians</param>
    /// </summary>
    private Vector2 CalculateJumpVelocity(float angleInRadians)
    {
        Vector2 jumpVelocity = this.player.velocity;
        jumpVelocity.x -= this.player.currentJumpVelocity * Mathf.Sin(angleInRadians);
        jumpVelocity.y += this.player.currentJumpVelocity * Mathf.Cos(angleInRadians);

        return jumpVelocity;
    }

    /// <summary>
    /// Checks whether to set the player to the minimum Jump Threshold
    /// </summary>
    public bool ShouldSetToMinJumpThreshold()
    {
        float jumpRelThreshold = this.player.currentJumpReleaseThreshold;
        if (this.player.velocity.y > jumpRelThreshold && this.player.GetInputManager().GetJumpButton().GetButtonUp())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the value for the roll jump
    /// </summary>
    public bool GetRollJump() => this.rollJump;
    /// <summary>
    /// Sets the value for roll jump
    /// <param name="rollJump"> The new value for rll jump</param>
    /// </summary>
    public void SetRollJump(bool rollJump) => this.rollJump = rollJump;
}
