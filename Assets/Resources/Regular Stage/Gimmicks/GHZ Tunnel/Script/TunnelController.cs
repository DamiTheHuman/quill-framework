using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
/// <summary>
/// A gimmick that signifies the player is going through a tunnel by forcing the roll state
/// </summary>
public class TunnelController : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    private Player player;
    [SerializeField]
    private bool allowWhenNotGrounded = false;
    [Tooltip("The direction to begin force rolling in"), SerializeField]
    private ForceTunnelDirection tunnelStartDirection = ForceTunnelDirection.WhenGoingRight;
    public Color debugColor = General.RGBToColour(255, 255, 255, 125);

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Checks if the players solid box comes in contact with the current box collider
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds);

        if (triggerAction)
        {
            triggerAction = this.allowWhenNotGrounded ? true : player.GetGrounded();
        }

        return triggerAction;
    }

    /// <summary>
    /// Child the player to the platform forcing the player to move with the platform
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.player = player;
        Roll roll = player.GetActionManager().GetComponentInChildren<Roll>();

        if (player.GetActionManager().CheckActionIsBeingPerformed<Roll>() == false)
        {
            player.GetActionManager().PerformAction<Roll>();
        }

        int rollDirection = this.tunnelStartDirection == ForceTunnelDirection.WhenGoingRight ? 1 : -1;
        roll.SetForceRollDirection(rollDirection);//Sets and begins the force roll
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.ForceRoll);
    }

    /// <summary>
    /// Watch to end the roll action when the player is not within the bounds of the trigger
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        Bounds solidBoxBounds = player.GetSolidBoxController().GetComponent<BoxCollider2D>().bounds;

        if (this.tunnelStartDirection == ForceTunnelDirection.WhenGoingRight && player.GetHorizontalVelocityDirection() <= 0 && player.transform.position.x < this.transform.position.x)
        {
            player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
            player.GetSensors().groundCollisionInfo.CheckForCollision(this.player.transform.position, ref player.velocity);
        }
        else if (this.tunnelStartDirection == ForceTunnelDirection.WhenGoingLeft && player.GetHorizontalVelocityDirection() >= 0 && player.transform.position.x > this.transform.position.x)
        {
            player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
            player.GetSensors().groundCollisionInfo.CheckForCollision(this.player.transform.position, ref player.velocity);
        }
    }

    private void OnDrawGizmos()
    {

        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }
        GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor);
        Gizmos.color = Color.white;

        if (this.tunnelStartDirection == ForceTunnelDirection.WhenGoingRight)
        {
            Gizmos.DrawLine(this.boxCollider2D.bounds.center - new Vector3(this.boxCollider2D.bounds.size.x / 3, 0), this.boxCollider2D.bounds.center + new Vector3(this.boxCollider2D.bounds.size.x / 3, 0));
            GizmosExtra.Draw2DArrow(this.boxCollider2D.bounds.center + new Vector3(this.boxCollider2D.bounds.size.x / 3, 0), -90, 0, 8);
        }
        else
        {
            Gizmos.DrawLine(this.boxCollider2D.bounds.center + new Vector3(this.boxCollider2D.bounds.size.x / 3, 0), this.boxCollider2D.bounds.center - new Vector3(this.boxCollider2D.bounds.size.x / 3, 0));
            GizmosExtra.Draw2DArrow(this.boxCollider2D.bounds.center - new Vector3(this.boxCollider2D.bounds.size.x / 3, 0), 90, 0, 8);
        }
    }
}
