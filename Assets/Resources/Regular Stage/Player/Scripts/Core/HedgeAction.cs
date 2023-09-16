using UnityEngine;
/// <summary>
/// Parent class where all the actions will inherit from
/// </summary>
public class HedgeAction : MonoBehaviour
{
    [FirstFoldOutItem("Dependencies")]
    public Player player;
    [Tooltip("Check if the action is ready to perform")]
    public bool readyToPerform;
    [Tooltip("Flag to begin the action")]
    public bool beginAction;
    [Tooltip("Whether the action is being performed")]
    public bool performingAction;
    [Tooltip("The id of the action")]
    public float actionID = 0;
    [Tooltip("The animation ID ")]
    public ActionSubState primaryAnimationID = 0;

    /// <summary>
    /// Try and find the parent of the hedge actions
    /// Used explicitly during character swapping
    /// </summary>
    public void FindParentPlayer() => this.player = this.GetComponentInParent<Player>();

    /// <summary>
    /// Resets the actions to its default states
    /// </summary>
    public virtual void Reset()
    {
        if (this.player == null || this.player.gameObject.activeSelf == false)
        {
            this.FindParentPlayer();
        }

        this.readyToPerform = false;
        this.performingAction = false;
        this.beginAction = false;
        this.primaryAnimationID = (ActionSubState)this.actionID;
    }

    public virtual void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Set the <see cref="player"/>
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayer(Player player) => this.player = player;

    /// <summary>
    /// Performs an initial check on whether the action can be performed or not
    /// </summary>
    public virtual bool CanPerformAction() => false;

    /// <summary>
    /// The condition in which the current action can be exited
    /// </summary>
    public virtual bool ExitActionCondition() => false;

    /// <summary>
    /// The condition on which the action should begin
    /// </summary>
    public virtual bool LaunchActionConditon() => false;

    /// <summary>
    /// The actions performed as soon as the action is launched
    /// </summary>
    public virtual void OnActionStart() { }
    /// <summary>
    /// The actions performed while the action is being performed like a button being held
    /// </summary>
    public virtual void OnPerformingAction() { }
    /// <summary>
    /// The actions performed before the action is ended
    /// </summary>
    public virtual void OnActionEnd() { }
}
