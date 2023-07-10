using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Highest action in the heirachy which can contain sub actions that can be branche too
/// </summary>
public class HedgePrimaryAction : HedgeAction
{
    public ActionAvailability actionAvailability = ActionAvailability.GroundOnly;
    [Tooltip("Whether the action when performed is an attacking action")]
    public bool attackingAction;
    [Tooltip("The mode defining the player size while this action is active")]
    public SizeMode sizeMode;
    [Tooltip("The sub actions that belong to current action")]
    public List<HedgeSubAction> subActions;
    [Tooltip("A flag to signify that the parent action has utilized a sub action")]
    public bool usedSubAction = false;
    public override void Start()
    {
        base.Start();
        this.subActions.Clear();//Clear the action list first

        //Check if the current action contains sub actions and if so add it to the list
        foreach (HedgeSubAction hedgeSubAction in this.player.GetComponentsInChildren<HedgeSubAction>())
        {
            if (hedgeSubAction.parentAction == this && this.subActions.Contains(hedgeSubAction) == false)
            {
                this.subActions.Add(hedgeSubAction);
            }

        }
        if (this.subActions.Count > 0)
        {
            this.subActions = this.subActions.OrderBy(i => i.actionID).ToList();
        }
    }

    /// <summary>
    /// Updates the lifecycle based on the sub actions being performed
    /// </summary>
    public virtual void UpdateSubActionLifeCycle()
    {
        foreach (HedgeSubAction subAction in this.subActions)
        {
            if (subAction.CanPerformAction())//Check if the actions start conditions are available
            {
                if (subAction.LaunchActionConditon())//Check if the action is not the current action and its launch conditions are met
                {
                    subAction.beginAction = true;
                    subAction.readyToPerform = false;
                    this.player.GetActionManager().currentSubAction = subAction;
                    this.usedSubAction = true;//Set the flag signifying a sub action has been utilized
                }
                else
                {
                    subAction.readyToPerform = true;
                }
            }
            else
            {
                subAction.readyToPerform = false;
            }
        }
    }

    /// <summary>
    /// Reset the current <see cref="subActions"/> without ending the <see cref="HedgePrimaryAction"/> state
    /// </summary>
    public virtual void ResetCurrentSubAction()
    {
        if (this.player.GetActionManager().currentSubAction.parentAction != this)
        {
            return;
        }

        this.usedSubAction = false;
        this.player.GetActionManager().currentSubAction.OnActionEnd();
        this.player.GetActionManager().currentSubAction.performingAction = false;
        this.player.GetActionManager().currentSubAction = null;
    }

    /// <summary>
    /// Actions the primary action can perform while performing sub actions this will take over the OnPerformAction
    /// </summary>
    public virtual void WhilePerformingSubAction() { }
}
