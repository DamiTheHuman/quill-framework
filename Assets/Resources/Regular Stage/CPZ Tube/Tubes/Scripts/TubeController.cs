using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// This class implements functionality similar to that of the tubes in CPZ by making use of waypoints
/// </summary>
public class TubeController : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [Tooltip("The type of interaction required when the tube begins performing"), FirstFoldOutItem("Tube Info")]
    public TriggerEffectSide entryType = TriggerEffectSide.Top;
    [SerializeField, Tooltip("The spring cap attached to the end of the tube")]
    private SpringCapController springCapController = null;
    public Transform pathPoints;
    [Tooltip("The active player on the tube")]
    private Player activePlayerOnTube;
    [Tooltip("The velocity of the player on the end tube")]
    public Vector2 endVelocity = new Vector2(0, 16);
    [Tooltip("The current index the player is moving towards")]
    public int currentPointTarget = 0;
    [Tooltip("The speed the player moves on while on the tube")]
    public float speed = 8;
    [Tooltip("The path points on the tube"), LastFoldoutItem(), IsDisabled]
    public List<Transform> tubePathPoints = new List<Transform>();

    [FirstFoldOutItem("Audio")]
    [SerializeField, Tooltip("The sound played while the player enters the tube")]
    private AudioClip spinSound = null;
    [SerializeField, Tooltip("The sound played when the player exits the tube with a cap on the end"), LastFoldoutItem()]
    private AudioClip exitSound = null;

    [FirstFoldOutItem("Debug")]
    public Color debugColor = General.RGBToColour(255, 255, 255, 125);
    public Color debugLineColor = Color.cyan;
    [Tooltip("The line width of the debug side"), LastFoldoutItem()]
    public float activeColorWidth = 4;
    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        this.tubePathPoints = this.pathPoints.GetComponentsInChildren<Transform>().ToList();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate()
    {
        if (this.activePlayerOnTube != null)
        {
            if (this.activePlayerOnTube.GetPlayerState() == PlayerState.Sleep)
            {
                this.activePlayerOnTube = null;

                return;
            }

            this.PointToPointTubeMovement(this.activePlayerOnTube.transform.position);
        }
    }

    /// <summary>
    /// Moves the player continously from one point to another
    /// <param name="position">The current platform position</param>
    /// </summary>
    private void PointToPointTubeMovement(Vector2 position)
    {
        if (position - (Vector2)this.tubePathPoints[this.currentPointTarget].position == Vector2.zero)
        {
            if (this.currentPointTarget == this.tubePathPoints.Count - 1)
            {
                this.TubeExitAction();

                return;
            }
            else
            {
                //Move to the next target
                this.currentPointTarget++;
            }
        }

        position = Vector2.MoveTowards(position, this.tubePathPoints[this.currentPointTarget].position, this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
        this.activePlayerOnTube.velocity = position - (Vector2)this.activePlayerOnTube.transform.position;
        this.activePlayerOnTube.transform.position = position;
    }

    /// <summary>
    /// Applies velocity to the player at the end of the tube
    /// </summary>
    private void TubeExitAction()
    {
        this.activePlayerOnTube.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        this.activePlayerOnTube.SetMovementRestriction(MovementRestriction.None);//Restore Movement
        this.activePlayerOnTube.velocity = this.endVelocity;
        this.activePlayerOnTube.GetAnimatorManager().SwitchGimmickSubstate(0);

        if (Mathf.Abs(this.endVelocity.y) > 0)
        {
            this.activePlayerOnTube.SetGrounded(false);
        }

        this.activePlayerOnTube = null;

        if (this.springCapController != null)
        {
            GMAudioManager.Instance().PlayOneShot(this.exitSound);
        }
    }

    /// <summary>
    /// On Contact immedietly performed action and prepare for tube movement
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction;
        triggerAction = this.CheckEntrySide(player, solidBoxColliderBounds, this.entryType) && player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InTube;

        return triggerAction;
    }

    /// <summary>
    /// Checks the players range within the entry side of the tube
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// <param name="sideToCheck">The entry side to check against  </param>
    /// </summary>
    public bool CheckEntrySide(Player player, Bounds solidBoxColliderBounds, TriggerEffectSide sideToCheck)
    {
        if (player.GetSolidBoxController().InteractingWithContactEventOfType<SpringController>(this.gameObject))
        {
            return false;
        }

        switch (sideToCheck)
        {
            case TriggerEffectSide.Top:
                return this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && player.velocity.y <= 0;
            case TriggerEffectSide.Left:
                return this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && player.velocity.x <= 0;
            case TriggerEffectSide.Right:
                return this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && player.velocity.y >= 0;
            case TriggerEffectSide.Bottom:
                return this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && player.velocity.x >= 0;
            case TriggerEffectSide.Always:
                return true;
            default:
                break;
        }

        return false;
    }

    /// <summary>
    /// Begins the movement of the player on the tube 
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.SetGrounded(false);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.InTube);
        player.SetMovementRestriction(MovementRestriction.Both);//The tube will now handle movement
        player.velocity = Vector2.zero;
        player.groundVelocity = this.speed;
        player.GetActionManager().EndCurrentAction();

        if (this.endVelocity.x > 0 && this.endVelocity.y == 0)
        {
            player.GetSensors().MoveAndCollide(Vector2.zero);
            player.SetGrounded(true);
        }

        player.GetActionManager().PerformAction<Roll>();
        this.currentPointTarget = 0;
        this.activePlayerOnTube = player;
        GMAudioManager.Instance().PlayOneShot(this.spinSound);
    }

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }

        GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor);
        Gizmos.DrawLine(this.boxCollider2D.bounds.center + new Vector3(this.boxCollider2D.bounds.size.x / 3, 0), this.boxCollider2D.bounds.center - new Vector3(this.boxCollider2D.bounds.size.x / 3, 0));

        Vector3 position = this.transform.position;
        Transform[] tubeTargetPoints = this.pathPoints.GetComponentsInChildren<Transform>();
        Gizmos.color = this.debugLineColor;

        for (int x = 0; x < tubeTargetPoints.Length; x++)
        {
            if (x > 1)
            {
                Gizmos.DrawLine(tubeTargetPoints[x - 1].position, tubeTargetPoints[x].position);
                float differenceAngle = General.AngleBetweenVector2(tubeTargetPoints[x - 1].position, tubeTargetPoints[x].position);
                float differenceLength = Vector2.Distance(tubeTargetPoints[x - 1].position, tubeTargetPoints[x].position);
                GizmosExtra.Draw2DArrow(tubeTargetPoints[x - 1].position, (differenceAngle * Mathf.Rad2Deg) - 90, (differenceLength / 2) + 8);
            }

            GizmosExtra.Draw2DCircle(tubeTargetPoints[x].position, 3, Color.red);
        }

        Gizmos.color = this.debugColor;

        switch (this.entryType)
        {
            case TriggerEffectSide.Top:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.max, new Vector2(this.boxCollider2D.bounds.min.x, this.boxCollider2D.bounds.max.y), Gizmos.color);

                break;
            case TriggerEffectSide.Right:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.max, new Vector2(this.boxCollider2D.bounds.max.x, this.boxCollider2D.bounds.min.y), Gizmos.color);

                break;
            case TriggerEffectSide.Bottom:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.min, new Vector2(this.boxCollider2D.bounds.max.x, this.boxCollider2D.bounds.min.y), Gizmos.color);

                break;
            case TriggerEffectSide.Left:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.min, new Vector2(this.boxCollider2D.bounds.min.x, this.boxCollider2D.bounds.max.y), Gizmos.color);

                break;
            case TriggerEffectSide.Always:
                GizmosExtra.DrawWireRect(this.boxCollider2D.bounds, Gizmos.color);

                break;
            default:
                break;
        }

        Gizmos.color = Color.white;
    }
}
