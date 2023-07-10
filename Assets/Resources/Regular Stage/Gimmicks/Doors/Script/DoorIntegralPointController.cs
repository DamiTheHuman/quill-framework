using UnityEngine;
/// <summary>
/// The integral point of the door hich uses a box collider 2D to identify the boundaries of the boor and to update the <see cref="doorController"/>  states
/// </summary> 
public class DoorIntegralPointController : TriggerContactGimmick
{
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private DoorController doorController = null;

    [Tooltip("The current activation side of the door"), SerializeField]
    private TriggerWatchBoundsType activationSide = TriggerWatchBoundsType.ActivivatableSide;

    [SerializeField, Tooltip("Color used for debugging ")]
    public Color debugColor = General.RGBToColour(0, 255, 0, 125);

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
    /// Checks if the players collider is within the activatable ranges of the door
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = this.TargetBoundsAreWithinVerticalBounds(solidBoxColliderBounds, this.boxCollider2D.bounds);

        return triggerAction;
    }

    /// <summary>
    /// As long as the players position is before doors positions they are within activatable boundaries
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (this.activationSide == TriggerWatchBoundsType.ActivivatableSide)
        {
            this.doorController.SetDoorState(DoorState.Opening);
        }
    }

    /// <summary>
    /// As long as the players position is before doors positions they are within activatable boundaries
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);

        if (this.activationSide == TriggerWatchBoundsType.ActivivatableSide && this.doorController.GetDoorState() != DoorState.Opening)
        {
            this.doorController.SetDoorState(DoorState.Opening);
        }
    }

    /// <summary>
    /// Checks to close the door as soon as contect with the player has ended
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);

        if (Vector2.Distance(player.transform.position, this.transform.position) >= this.boxCollider2D.size.x / 2 || this.activationSide == TriggerWatchBoundsType.NonActivitableSide)
        {
            this.doorController.SetDoorState(DoorState.Closing);
        }
    }

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }
        if (this.doorController.enabled == false)
        {
            Color disabledColor = Color.gray;
            disabledColor.a = this.debugColor.a;
            GizmosExtra.DrawRect(this.transform, this.boxCollider2D, disabledColor, true);
        }
        else
        {
            GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor, true);
        }
        if (this.activationSide == TriggerWatchBoundsType.ActivivatableSide)
        {
            Gizmos.color = Color.white;
            Vector2 spawnPosition = (Vector2)this.boxCollider2D.bounds.center + new Vector2(0, this.boxCollider2D.bounds.size.y / 16);
            if (this.doorController.GetFlipOpenDirection())
            {
                Gizmos.DrawLine(spawnPosition, spawnPosition + General.TransformEulerToVector2(16, this.transform, 0));
                GizmosExtra.Draw2DArrow(spawnPosition, -this.transform.eulerAngles.z, 0, 8);
            }
            else
            {
                Gizmos.DrawLine(spawnPosition, spawnPosition - General.TransformEulerToVector2(16, this.transform, 0));
                GizmosExtra.Draw2DArrow(spawnPosition, this.transform.eulerAngles.z, 0, 8);
            }
        }
    }
}
