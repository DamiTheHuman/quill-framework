using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// A manager class which handles the actions our player object can perform
/// Based on the classes attached to the "Actions" child object
/// </summary>
public class ActionManager : MonoBehaviour
{
    [Serializable]
    public class ActionDictionary : SerializableDictionary<Type, HedgeAction> { } //Helps force unity to serialize dictionaries so no need to restart on recompile :D

    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField]
    private Sensors sensors;
    [SerializeField, LastFoldoutItem()]
    private InputManager inputManager;
    [HideInInspector]
    public ActionDictionary actionDictionary;
    [SerializeField, Tooltip("List of primary actions available to the current player")]
    public List<HedgePrimaryAction> availablePrimaryActions;
    [SerializeField, Tooltip("List of sub actions available to the current player")]
    public List<HedgeSubAction> availableSubActions;
    [SerializeField, Tooltip("The current active primary action")]
    public HedgePrimaryAction currentPrimaryAction;
    [SerializeField, Tooltip("The previously active primary action")]
    public HedgePrimaryAction previousPrimaryAction;
    [SerializeField, Tooltip("The current active sub actions")]
    public HedgeSubAction currentSubAction;

    private void Reset()
    {
        this.player = this.GetComponent<Player>();
        this.sensors = this.GetComponent<Sensors>();
        this.inputManager = this.GetComponent<InputManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        this.availablePrimaryActions = this.GetComponentsInChildren<HedgePrimaryAction>().ToList();
        this.availableSubActions = this.GetComponentsInChildren<HedgeSubAction>().ToList();
        this.actionDictionary.Clear();
        //Cache our actions to a dictionary so its easier to access
        foreach (HedgePrimaryAction primaryAction in this.availablePrimaryActions)
        {
            this.actionDictionary.Add(primaryAction.GetType(), primaryAction);
        }
        foreach (HedgeSubAction subAction in this.availableSubActions)
        {
            this.actionDictionary.Add(subAction.GetType(), subAction);
        }

        this.actionDictionary.OnBeforeSerialize();
    }
    /// <summary>
    /// Updates the lifecycle for actions and sub actions
    /// </summary>
    public void UpdateActionLifeCycle()
    {
        foreach (HedgePrimaryAction action in this.availablePrimaryActions)
        {
            if (action.performingAction) //While the action is being performed
            {
                action.UpdateSubActionLifeCycle();//Handle Sub actions when applicable 
            }
            if (this.currentSubAction != null)
            {
                this.UpdateActiveSubAction(action, ref this.currentSubAction); //Sub Actions take priority when they are active
            }

            this.UpdatePrimaryAction(action);//A primary actions sub actions are also checked here
        }
    }
    /// <summary>
    /// Performs active sub action operations ands ends them while being performed
    /// NOTE - ** THIS DOES NOT ACTIVATE SUB ACTIONS SUB ACTIONS ARE SET IN THE UPDATEPRIMARYACTIONCYCLES <see cref="UpdateSubActionLifeCycle()"/> function**
    /// <param name="primaryAction">The current primary action being checked</param>
    /// <param name="subAction">The current active sub action </param>
    /// </summary>
    private void UpdateActiveSubAction(HedgePrimaryAction primaryAction, ref HedgeSubAction subAction)
    {
        if (subAction.parentAction == primaryAction)
        {
            if (subAction != null)
            {
                if (subAction.beginAction)//Run the Launch sub action
                {
                    subAction = this.LaunchSubAction(subAction);
                    if (subAction == null)
                    {
                        return;
                    }
                }
                if (subAction.performingAction)  //Perform the active sub action
                {
                    subAction = this.PerformSubActionCycle(subAction);
                    if (subAction == null)
                    {
                        return;
                    }
                }
            }

            subAction.parentAction.WhilePerformingSubAction();//Handle primary actions floating effects where available
            return;
        }
        if (this.currentPrimaryAction != subAction.parentAction && subAction.performingAction)
        {
            this.ExitSubAction(subAction);
            subAction = null;
        }
    }

    /// <summary>
    /// Performs and Updates primary actios as they are being run
    /// <param name="hedgePrimaryAction">The current primary action being checked</param>
    /// </summary>
    private void UpdatePrimaryAction(HedgePrimaryAction hedgePrimaryAction)
    {
        if (this.currentSubAction != null && this.currentSubAction.parentAction == hedgePrimaryAction)
        {
            return;
        }
        if (hedgePrimaryAction == null)
        {
            return;
        }
        if (hedgePrimaryAction.CanPerformAction())  //Check if the actions start conditions are available
        {
            if (this.currentPrimaryAction != hedgePrimaryAction && hedgePrimaryAction.LaunchActionConditon()) //Check if the action is not the current action and its launch conditions are met
            {
                hedgePrimaryAction.beginAction = true;
                hedgePrimaryAction.readyToPerform = false;
            }
            else
            {
                hedgePrimaryAction.readyToPerform = true;
            }
        }
        else
        {
            hedgePrimaryAction.readyToPerform = false;
        }
        if (hedgePrimaryAction.beginAction) //Run the Launch action and begin performing the appropriate action
        {
            this.LaunchPrimaryAction(hedgePrimaryAction);
        }
        if (hedgePrimaryAction.performingAction) //While the action is being performed
        {
            if (hedgePrimaryAction.ExitActionCondition() && this.currentPrimaryAction == hedgePrimaryAction) //Run the exit action if the action is curently being run
            {
                this.ExitPrimaryAction(hedgePrimaryAction);
                return;
            }
            hedgePrimaryAction.OnPerformingAction();
        }
    }
    /// <summary>
    /// Get an action by its type
    /// </summary>
    public HedgeAction GetAction<ObjectType>()
    {
        HedgeAction action = this.actionDictionary.TryGetValue(typeof(ObjectType), out action) ? action : null;
        return action;
    }
    /// <summary>
    /// Forcefully perform an action based on the type of script passed
    /// </summary>
    public void PerformAction<ObjectType>()
    {
        HedgeAction action = this.GetAction<ObjectType>();

        if (action != null)
        {
            if (action is HedgePrimaryAction primaryAction)
            {
                this.LaunchPrimaryAction(primaryAction);
                return;
            }
            else if (action is HedgeSubAction hedgeSubAction)
            {
                HedgeSubAction subAction = (HedgeSubAction)action;
                this.LaunchSubAction(subAction);
                return;
            }
        }
        Debug.Log("Action of type: " + typeof(ObjectType) + " Does not exist in the dictionary");
    }

    /// <summary>
    /// Checks if an action is being performed by its type
    /// </summary>
    public bool CheckActionIsBeingPerformed<ObjectType>()
    {
        HedgeAction action = this.GetAction<ObjectType>();

        if (action != null)
        {
            if (action.performingAction)
            {
                return true;
            }
            return false;
        }
        if (typeof(Transform) is ObjectType)
        {
            Debug.Log("Did you mean to use the 'SupeTransform' type instead of Transform?");
        }
        return false;

    }
    /// <summary>
    /// Launches the beginning phase for the primary action
    /// <param name="hedgePrimaryAction">The primary action to lunch</param>
    /// </summary>
    private HedgePrimaryAction LaunchPrimaryAction(HedgePrimaryAction primaryAction)
    {
        if (this.currentPrimaryAction != null && this.currentPrimaryAction != primaryAction)
        {
            if (this.currentPrimaryAction.performingAction)
            {
                if (this.currentSubAction != null && this.currentSubAction.parentAction == this.currentPrimaryAction && this.currentSubAction.performingAction)
                {
                    this.ExitSubAction(this.currentSubAction);
                }
                this.ExitPrimaryAction(this.currentPrimaryAction);
            }
        }
        primaryAction.beginAction = false;
        primaryAction.readyToPerform = false;
        primaryAction.OnActionStart();
        primaryAction.performingAction = true;
        this.previousPrimaryAction = this.currentPrimaryAction;
        this.currentPrimaryAction = primaryAction;//update the current action
        this.sensors.SetSizeMode(this.currentPrimaryAction.sizeMode);

        if (primaryAction.attackingAction)
        {
            this.player.SetAttackingState(true);
        }

        //Set the players attacking state
        return primaryAction;
    }
    /// <summary>
    /// Exits the primary actions and updates the previousPrimaryAction
    /// <param name="primaryAction">The primary action to exit safely</param>
    /// </summary>
    private void ExitPrimaryAction(HedgePrimaryAction primaryAction)
    {
        this.sensors.SetSizeMode(SizeMode.Regular);//Unset the size to default
        primaryAction.performingAction = false;

        if (primaryAction.attackingAction)
        {
            this.player.SetAttackingState(false);
        }

        //UnSet the players attacking state
        if (primaryAction != this.currentPrimaryAction)
        {
            return;
        }

        if (this.currentSubAction != null && this.currentSubAction.parentAction == this.currentPrimaryAction && this.currentSubAction.performingAction)
        {
            this.ExitSubAction(this.currentSubAction);
        }

        primaryAction.performingAction = false;
        this.currentPrimaryAction = null;
        primaryAction.OnActionEnd();
        primaryAction.Reset();
        this.previousPrimaryAction = primaryAction;
        return;
    }
    /// <summary>
    ///  Launches the beginning phase for the sub action while informing the necessary primary/parent action
    /// <param name="subAction">The sub action to launch</param>
    /// </summary>
    private HedgeSubAction LaunchSubAction(HedgeSubAction subAction)
    {
        if (this.currentPrimaryAction != subAction.parentAction)
        {
            this.LaunchPrimaryAction(subAction.parentAction);
        }
        subAction.OnActionStart();//Launch the sub actionss start function
        subAction.beginAction = false;
        subAction.readyToPerform = false;
        subAction.performingAction = true;
        subAction.parentAction.usedSubAction = true;//Set the flag signifying a sub action has been utilized
        this.currentSubAction = subAction;

        return subAction;
    }
    /// <summary>
    ///  Perform the life cycle update for the sub action
    /// <param name="subAction">The sub action to update</param>
    /// </summary>
    private HedgeSubAction PerformSubActionCycle(HedgeSubAction subAction)
    {
        if (this.actionDictionary.Count == 0)
        {
            this.actionDictionary.OnAfterDeserialize();
        }

        if (subAction.ExitActionCondition() || (this.currentPrimaryAction != null && this.currentPrimaryAction.ExitActionCondition())) //Run the exit action if the action is curently being run
        {
            this.ExitSubAction(subAction);

            return null;
        }

        subAction.OnPerformingAction();

        //if the parent action changes the sub action ends 
        if (this.currentPrimaryAction != subAction.parentAction)
        {
            this.ExitSubAction(subAction);

            return null;
        }

        return subAction;
    }
    /// <summary>
    ///  Exits the curren sub action safely
    /// <param name="subAction">The sub action to exit </param>
    /// </summary>
    private void ExitSubAction(HedgeSubAction subAction)
    {
        subAction.performingAction = false;

        if (this.currentSubAction == subAction)
        {
            this.currentSubAction.Reset();
            this.currentSubAction.OnActionEnd();
            this.currentSubAction = null;

            return;
        }

        subAction.OnActionEnd();
        subAction.Reset();
    }
    /// <summary>
    /// Forcefully end an action based on the object type
    /// </summary>
    public void EndAction<ObjectType>()
    {
        HedgeAction action = this.GetAction<ObjectType>();

        if (action != null)
        {
            if (action is HedgePrimaryAction primaryAction)
            {
                this.ExitPrimaryAction(primaryAction);
            }
            else
            {
                HedgeSubAction subAction = (HedgeSubAction)action;
                this.ExitSubAction(subAction);

                return;
            }

            return;
        }

        Debug.Log("Action of type: " + typeof(ObjectType) + " Does not exist in the dictionary");
    }
    /// <summary>
    /// Clears the current action and sub action
    /// </summary>
    public void ClearActions()
    {
        if (this.currentPrimaryAction != null)
        {
            this.sensors.SetSizeMode(SizeMode.Regular);//Unset the size to default
            if (this.currentPrimaryAction.attackingAction)
            { this.player.SetAttackingState(false); }
            this.currentPrimaryAction.Reset();
            this.currentPrimaryAction = null;
        }

        if (this.currentSubAction != null)
        {
            this.currentSubAction.Reset();
            this.currentSubAction = null;
        }

        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
    }
    /// <summary>
    /// Ends the current active action outside the lifecyle
    /// </summary>
    public void EndCurrentAction(bool forceEnd = false)
    {
        if (forceEnd)
        {
            if (this.currentPrimaryAction != null)
            {
                if (this.currentSubAction != null)
                {
                    this.currentSubAction.OnActionEnd();
                    this.currentSubAction.Reset();
                    this.currentSubAction = null;
                }
                HedgePrimaryAction previousPrimaryAction = this.currentPrimaryAction;
                this.currentPrimaryAction.OnActionEnd();
                //Sometimes actions call other actions when it ends, e.g spindash -> roll so we can make sure no actions are queued by using this
                if (this.currentPrimaryAction != previousPrimaryAction && this.currentPrimaryAction != null)
                {
                    Debug.Log("Woohoo double action deletion was activated - Remvoe this message");
                    this.EndCurrentAction(forceEnd);
                }

                if (this.currentPrimaryAction != null)
                {
                    this.currentPrimaryAction.Reset();
                    this.sensors.SetSizeMode(SizeMode.Regular);//Unset the size to default
                    if (this.currentPrimaryAction.attackingAction)
                    { this.player.SetAttackingState(false); }//Unset the players attacking state
                }
                this.currentPrimaryAction = null;
            }

            return;
        }

        HedgePrimaryAction action = this.currentPrimaryAction;

        if (action != null)
        {
            if (this.currentSubAction != null && this.currentSubAction.parentAction == action)
            {
                this.currentSubAction.OnActionEnd();
                this.currentSubAction.Reset();
                this.currentSubAction = null;
            }
            action.OnActionEnd();
            action.Reset();
            this.currentPrimaryAction = null;
            this.previousPrimaryAction = action;
            this.sensors.SetSizeMode(SizeMode.Regular);//Unset the size to default
            if (action.attackingAction)
            { this.player.SetAttackingState(false); }//Unset the players attacking state
        }
    }

    /// <summary>
    /// Swap the current action data with actions from the new action manager
    /// This filters out all the actions we have and do not have access to and filters allowing for
    /// Switching of actions with losing the current action data and handle abnormalities
    /// <param name="targetActionManager">The action manager to get info from</param>
    /// </summary>
    public void SwapActionData(ActionManager targetActionManager)
    {
        HedgeSubAction placeHolderSubAction = this.currentSubAction;
        bool hadSubAction = this.currentSubAction != null;
        this.RemoveUnExistingActions(targetActionManager);
        this.AddMissingActions(targetActionManager);
        this.Start();

        //Try and restore an appropriate animation
        if (this.currentPrimaryAction != null && placeHolderSubAction == null && hadSubAction)
        {
            this.player.GetAnimatorManager().SwitchActionSubstate(this.currentPrimaryAction.primaryAnimationID);

        }
    }
    /// <summary>
    /// Remove actions that do not exist in the target action
    /// <param name="targetActionManager">The action manager to get info from</param>
    /// </summary>
    private void RemoveUnExistingActions(ActionManager targetActionManager)
    {
        foreach (HedgePrimaryAction action in this.availablePrimaryActions)
        {
            //Check if the current action is within the target actionManager
            HedgePrimaryAction targetActionInCurrentAction = Array.Find(targetActionManager.availablePrimaryActions.ToArray(), primaryAction => primaryAction.GetType() == action.GetType());
            ;
            if (targetActionInCurrentAction != null)
            {
                if (action.subActions.Count > 0)
                {
                    foreach (HedgeSubAction hedgeSubAction in action.subActions)
                    {
                        //Check if the current suba ction exists within the targetAction
                        HedgeSubAction targetSubActionInCurrentSubAction = Array.Find(targetActionInCurrentAction.subActions.ToArray(), subAction => subAction.GetType() == hedgeSubAction.GetType());
                        ;
                        if (targetSubActionInCurrentSubAction == null)
                        {
                            //If we remove a sub action we are currently performing  switch to the parent action to avoid errors
                            if (this.currentSubAction == hedgeSubAction)
                            {
                                this.currentSubAction.OnActionEnd();
                                this.currentSubAction = null;
                            }
                            DestroyImmediate(hedgeSubAction);//If the target sub action cant be found with the current actions delete it
                        }
                    }
                    action.Start();
                }
            }
            else
            {
                //If we are currently performing the action end it forcefully before exiting
                if (this.currentPrimaryAction == action)
                {
                    this.currentPrimaryAction.OnActionEnd();
                    this.currentPrimaryAction = null;
                }

                DestroyImmediate(action);
            }
        }
    }
    /// <summary>
    /// Adds missing actions from the Action Manager to get info from
    /// <param name="targetActionManager">The action manager to get info from</param>
    /// </summary>
    private void AddMissingActions(ActionManager targetActionManager)
    {
        foreach (HedgePrimaryAction action in targetActionManager.availablePrimaryActions)
        {
            HedgePrimaryAction currentActionInTargetActions = Array.Find(this.availablePrimaryActions.ToArray(), a => a.GetType() == action.GetType()); //The action find in the available thing
            if (currentActionInTargetActions != null)
            {
                currentActionInTargetActions.SetPlayer(this.player);

                if (action.subActions.Count > 0 && action != null)
                {
                    foreach (HedgeSubAction hedgeSubAction in action.subActions)
                    {
                        HedgeSubAction currentSubActionInTargetSubActions = Array.Find(currentActionInTargetActions.subActions.ToArray(), sA => sA.GetType() == hedgeSubAction.GetType());

                        if (currentSubActionInTargetSubActions == null)// We currently do not have this move
                        {
                            Component comp = General.CopyComponent(hedgeSubAction, this.availablePrimaryActions[0].transform.Find("Sub Actions").gameObject);
                            HedgeSubAction newSubAction = (HedgeSubAction)comp;
                            newSubAction.parentAction = currentActionInTargetActions;
                            newSubAction.FindParentPlayer();
                            newSubAction.Start();
                        }
                    }

                    currentActionInTargetActions.Start();
                }
            }
            //Copy the missing component unto the active obect
            else
            {
                Component actionCopy = General.CopyComponent(action, this.availablePrimaryActions[0].gameObject);
                HedgePrimaryAction primaryAction = (HedgePrimaryAction)actionCopy;
                primaryAction.SetPlayer(this.player);

                //Reparent sub actions to the parent action
                foreach (HedgeSubAction subAction in primaryAction.subActions)
                {
                    HedgeSubAction subActionInCurrentAction = Array.Find(this.player.GetComponentsInChildren<HedgeSubAction>(), sA => sA.GetType() == subAction.GetType());
                    if (subActionInCurrentAction)
                    {
                        subActionInCurrentAction.parentAction = primaryAction;
                    }
                }
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void CreateAssetWhenReady()
    {
        if (Application.isPlaying)
        {
            if (GMStageManager.Instance().GetPlayer())
            {
                GMStageManager.Instance().GetPlayer().GetActionManager().OnAfterDeserialize();
            }
        }
    }

    public void OnAfterDeserialize()
    {
        this.actionDictionary.OnAfterDeserialize();
        this.Start();
    }
#endif
}
