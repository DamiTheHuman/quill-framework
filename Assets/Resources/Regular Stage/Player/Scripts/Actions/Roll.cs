using UnityEngine;
/// <summary>
/// The roll action of the player object allowing the player to curl into a ball and attack enemies based on their velocity
/// </summary>
public class Roll : HedgePrimaryAction
{
    [SerializeField, Tooltip("The minimum speed needed to begin rolling"), FirstFoldOutItem("Roll Info")]
    private float minRollSpeed = 0.53333333333f;
    [Tooltip("A flag to signify that uncurling will occure when the player reattains contact with the ground")]
    [SerializeField]
    private bool unCurlOnLanding = false;
    [FirstFoldOutItem("Force Roll Info")]
    [Tooltip("The direction the force roll began in"), SerializeField]
    private int forceRollDirection = 1;
    [Tooltip("The amount of velocity to add when the ground velocity is at 0 during a force roll"), SerializeField, LastFoldoutItem()]
    private float forceRollPushSpeed = 2f;

    public override void Start() => base.Start();
    public override void Reset()
    {
        base.Reset();
        this.actionID = 6;
        this.sizeMode = SizeMode.Shrunk;
        this.attackingAction = true;
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.unCurlOnLanding = false;
    }

    /// <summary>
    /// Perform the roll action when grounded and moving and not holding in any direction
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetGrounded() && Mathf.Abs(this.player.groundVelocity) > this.minRollSpeed && this.player.GetInputManager().GetCurrentInput().x == 0
            && this.player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.Sliding
            && this.player.GetActionManager().CheckActionIsBeingPerformed<Glide>() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs downwards to begin rolling
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
    /// Put the player in a roll state
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);//If the player input is restricted
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.unCurlOnLanding = false;
        //Force Rolling
        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.ForceRoll)
        {
            if (this.player.groundVelocity == 0)
            {
                this.player.groundVelocity = this.forceRollPushSpeed * this.forceRollDirection;
            }

            this.player.currentPlayerDirection = this.forceRollDirection;
        }

        if (this.player.GetActionManager().currentPrimaryAction == null && this.player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.OnFlipper && this.player.GetGrounded())
        {
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().roll);
        }
    }

    /// <summary>
    /// If the player leaves the ground uncurl when they reattain contact with the ground
    /// </summary>
    public override void OnPerformingAction()
    {
        // Prepare to uncurl when the player touches the ground again as long as we aren't forcing the roll
        if (this.player.GetGrounded() == false && this.player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.ForceRoll)
        {
            this.unCurlOnLanding = true;
        }

        if (this.unCurlOnLanding && this.player.GetGrounded())
        {
            this.unCurlOnLanding = false;
        }

        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.ForceRoll)
        {
            if (this.player.groundVelocity == 0)
            {
                this.player.groundVelocity = this.forceRollPushSpeed * this.player.currentPlayerDirection;
            }
        }
    }

    /// <summary>
    /// Exits the roll conditions based on the criteria set
    /// </summary>
    public override bool ExitActionCondition()
    {
        //As long as the force roll is inactive end the roll
        if (this.player.GetGimmickManager().GetActiveGimmickMode() is not GimmickMode.ForceRoll and not GimmickMode.OnFlipper and not GimmickMode.InTube)
        {
            //As long as the player is not moving too slow while ont the ground
            if (Mathf.Abs(this.player.groundVelocity) <= this.player.GetPlayerPhysicsInfo().basicMinRollThreshold && this.player.GetGrounded())
            {
                return true;
            }
            //As long as the player is not planning on rolling again then end the roll
            else if (this.unCurlOnLanding && this.player.GetGrounded() && this.player.GetInputManager().GetCurrentInput().y != -1)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Resets the uncurl flag and ends the roll action animation
    /// </summary>
    public override void OnActionEnd() => this.player.GetAnimatorManager().SwitchActionSubstate(0);

    /// <summary>
    /// Sets the force roll to be active alongside the appropriate force roll direction
    /// <param name="forceRollDirection"> The direction to begin force rolling in </param>
    /// </summary>
    public void SetForceRollDirection(int forceRollDirection) => this.forceRollDirection = forceRollDirection;
}
