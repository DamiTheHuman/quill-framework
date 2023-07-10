using UnityEngine;
/// <summary>
/// The actions that take place when the player has cleared an act
/// </summary>
public class Victory : HedgePrimaryAction
{

    [Tooltip("A flag to determine when the velocity has been added"), SerializeField]
    private bool addedVelocity;

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 16;
        this.primaryAnimationID = ActionSubState.Victory;
        this.addedVelocity = false;
        base.Reset();
    }

    /// <summary>
    /// This action is launched from its perform action function
    /// </summary>
    public override bool CanPerformAction() => false;

    /// <summary>
    /// This action is launched from its perform action function
    /// </summary>
    public override bool LaunchActionConditon() => false;

    /// <summary>
    /// Sets the variables to its defaults
    /// </summary>
    public override void OnActionStart()
    {
        this.player.velocity = Vector2.zero;
        this.player.groundVelocity = 0;
        this.player.SetBeginVictoryActionOnGroundContact(false);
        this.addedVelocity = false;
        this.player.GetInputManager().SetInputRestriction(InputRestriction.All);

        if (this.player.GetPlayerPhysicsInfo().character != PlayableCharacter.Sonic)
        {
            this.player.velocity = Vector2.zero;
            this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
            this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        }
    }

    /// <summary>
    /// Apply Velocity to the player on contact with the action and switch the substate on reground
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.player.GetGrounded())
        {
            if (this.player.GetPlayerPhysicsInfo().character is PlayableCharacter.Sonic or PlayableCharacter.Knuckles)
            {
                if (this.addedVelocity == false)
                {
                    Vector2 victoryVelocity = this.player.GetPlayerPhysicsInfo().victoryVelocity;
                    this.player.velocity = new Vector2(victoryVelocity.x * this.player.currentPlayerDirection, victoryVelocity.y);
                    this.player.SetGrounded(false);
                    this.addedVelocity = true;
                    this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
                    this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
                }
                else
                {
                    this.player.GetAnimatorManager().SetOtherAnimationSubstate(1);
                    this.player.SetBothHorizontalVelocities(0);
                }
            }
        }
    }

    /// <summary>
    /// If the stage is uncleared then end the action
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (GMStageManager.Instance().GetStageState() != RegularStageState.ActClear)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// End the action and restore the users input
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
        this.player.GetAnimatorManager().SetOtherAnimationSubstate(0);
    }
}
