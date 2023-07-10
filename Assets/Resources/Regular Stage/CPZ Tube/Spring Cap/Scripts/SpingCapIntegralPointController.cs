using UnityEngine;
/// <summary>
/// The intergal point for the spring cap which controls the opening and closing of the cap
/// Functionality wise it is the same in as the Door Integral Points method with minimal changes
/// </summary>
public class SpingCapIntegralPointController : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    private SpringCapController springCapController = null;
    [Tooltip("The current activation side of the spring cap"), SerializeField]
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
    /// Checks if the players collider is within the activatable ranges of the spring cap
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
    /// As long as the players position is before spring caps positions they are within activatable boundaries
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        this.springCapController.SetCapOpenState(this.activationSide == TriggerWatchBoundsType.ActivivatableSide);
    }
    /// <summary>
    /// Checks to close the spring cap as soon as contect with the player has ended
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);

        if (Vector2.Distance(player.transform.position, this.transform.position) >= this.boxCollider2D.size.x / 2 || this.activationSide == TriggerWatchBoundsType.NonActivitableSide)
        {
            this.springCapController.SetCapOpenState(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }

        GizmosExtra.DrawBoxCollider2D(this.transform, this.boxCollider2D, this.debugColor);

        if (this.activationSide == TriggerWatchBoundsType.ActivivatableSide)
        {
            Gizmos.color = Color.white;
            Vector2 spawnPosition = (Vector2)this.boxCollider2D.bounds.center + new Vector2(0, this.boxCollider2D.bounds.size.y / 16);
            Gizmos.DrawLine(spawnPosition, spawnPosition - General.TransformEulerToVector2(16, this.transform, 0));
            GizmosExtra.Draw2DArrow(spawnPosition, this.transform.eulerAngles.z, 0, 8);
        }
    }
}
