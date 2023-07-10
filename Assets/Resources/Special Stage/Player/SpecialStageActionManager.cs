using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// ** FOR SPECIAL STAGES **
/// Duplicate of <see cref="ActionManager"/> but for special stages
/// </summary>
public class SpecialStageActionManager : MonoBehaviour
{
    [SerializeField]
    private SpecialStagePlayer player;
    [SerializeField, Tooltip("List of special sttage actions available to the current player")]
    private List<SpecialStageHedgeAction> availableSpecialStageActions;
    [SerializeField, Tooltip("The current active special stage action")]
    private SpecialStageHedgeAction currentSpecialStageAction;
    [SerializeField, Tooltip("The previously active special stage action")]
    private SpecialStageHedgeAction previousSpecialStageAction;

    private void Reset() => this.player = this.GetComponent<SpecialStagePlayer>();

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        this.availableSpecialStageActions = this.GetComponentsInChildren<SpecialStageHedgeAction>().ToList();
    }
    /// <summary>
    /// Updates the lifecycle for actions
    /// </summary>
    public void UpdateActionLifeCycle()
    {
        foreach (SpecialStageHedgeAction action in this.availableSpecialStageActions)
        {
            if (action.performingAction) //While the action is being performed
            {
                action.UpdateSubActionLifeCycle();//Handle Sub actions when applicable 
            }
            this.UpdateAction(action);
        }
    }

    /// <summary>
    /// Performs and Updates actios as they are being run
    /// <param name="action">The current action being checked</param>
    /// </summary>
    private void UpdateAction(SpecialStageHedgeAction action)
    {
        if (action == null)
        {
            return;
        }
        if (action.CanPerformAction() && GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Running)  //Check if the actions start conditions are available
        {
            if (this.currentSpecialStageAction != action && action.LaunchActionConditon()) //Check if the action is not the current action and its launch conditions are met
            {
                action.beginAction = true;
                action.readyToPerform = false;
            }
            else
            {
                action.readyToPerform = true;
            }
        }
        else
        {
            action.readyToPerform = false;
        }
        if (action.beginAction) //Run the Launch action and begin performing the appropriate action
        {
            this.LaunchAction(action);
        }
        if (action.performingAction) //While the action is being performed
        {
            if (action.ExitActionCondition() && this.currentSpecialStageAction == action) //Run the exit action if the action is curently being run
            {
                this.ExitAction(action);
                return;
            }
            action.OnPerformAction();
        }
    }

    /// <summary>
    /// Forcefully perform an action based on the type of script passed
    /// </summary>
    public void PerformAction<ObjectType>()
    {
        SpecialStageHedgeAction action = this.GetAction<ObjectType>();

        if (action != null)
        {
            if (action is not null)
            {
                SpecialStageHedgeAction primaryAction = action;
                this.LaunchAction(primaryAction);
                return;
            }
        }
    }

    /// <summary>
    /// Get an action by its type
    /// </summary>
    public SpecialStageHedgeAction GetAction<ObjectType>()
    {
        foreach (SpecialStageHedgeAction specialStageHedgeAction in this.availableSpecialStageActions)
        {
            if (specialStageHedgeAction.GetType() == typeof(ObjectType))
            {
                return specialStageHedgeAction;
            }
        }

        return null;
    }

    /// <summary>
    /// Launches the beginning phase for the action
    /// <param name="action">The action to lunch</param>
    /// </summary>
    private SpecialStageHedgeAction LaunchAction(SpecialStageHedgeAction action)
    {
        if (this.currentSpecialStageAction != null && this.currentSpecialStageAction != action)
        {
            if (this.currentSpecialStageAction.performingAction)
            {
                this.ExitAction(this.currentSpecialStageAction);
            }
        }
        action.beginAction = false;
        action.readyToPerform = false;
        action.OnActionStart();
        action.performingAction = true;
        this.previousSpecialStageAction = this.currentSpecialStageAction;
        this.currentSpecialStageAction = action;//update the current action
        if (action.attackingAction)
        {
            this.player.SetAttackingState(true);
        }//Set the players attacking state
        return action;
    }
    /// <summary>
    /// Exits the actions and updates the <see cref="previousSpecialStageAction"/>
    /// <param name="action">The action to exit safely</param>
    /// </summary>
    private void ExitAction(SpecialStageHedgeAction action)
    {
        if (action.attackingAction)
        {
            this.player.SetAttackingState(false);
        }//UnSet the players attacking state
        if (action != this.currentSpecialStageAction)
        {
            return;
        }
        action.performingAction = false;
        this.currentSpecialStageAction = null;
        action.OnActionEnd();
        action.Reset();
        this.previousSpecialStageAction = action;
        return;
    }
}
