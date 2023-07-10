using UnityEngine;
/// <summary>
/// The famous spike hazard which knocks the player backwards on collision with its hazardous side
/// </summary>
public class SpikeController : SolidContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [Tooltip("The hazardous side of the spike"), SerializeField]
    private TriggerEffectSide hazardousSide = TriggerEffectSide.Top;
    [Tooltip("The Color of the spike"), SerializeField]
    private Color debugColor = Color.red;
    [Tooltip("The line width of the debug side"), SerializeField]
    private float activeColorWidth = 4;
    [SerializeField]
    private bool autoUpdate = true;

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
    /// Checks if the players collider is within the activitable range of the hazardous side 
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        if (player.GetHealthManager().GetHealthStatus() != HealthStatus.Vulnerable)
        {
            return false;
        }

        switch (this.hazardousSide)
        {
            case TriggerEffectSide.Top:
                return this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y <= 0;
            case TriggerEffectSide.Right:
                return this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheRight(solidBoxColliderBounds) && player.velocity.x <= 0;
            case TriggerEffectSide.Bottom:
                return this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheBottom(solidBoxColliderBounds) && player.velocity.y >= 0;
            case TriggerEffectSide.Left:
                return this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheLeft(solidBoxColliderBounds) && player.velocity.x >= 0;
            case TriggerEffectSide.Always:
                return true;
            default:
                break;
        }

        return false;
    }

    /// <summary>
    /// Knock back the player on hit
    /// <param name="player">The player object  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.GetHealthManager().VerifyHit(this.transform.position.x);
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider2D = this.GetComponent<BoxCollider2D>();
        GizmosExtra.DrawBoxCollider2D(this.transform, boxCollider2D, Color.white);
        Gizmos.color = this.debugColor;
        float angleInDegrees = Mathf.RoundToInt(this.transform.eulerAngles.z);

        //When the spike is placed for the first time auto assume the harzardous side based on its rotation
        if (this.autoUpdate && Application.isPlaying == false)
        {
            TriggerEffectSide newHazardousSide = TriggerEffectSide.Always;
            newHazardousSide = (TriggerEffectSide)(angleInDegrees / 90);
            if (this.hazardousSide != newHazardousSide)
            {
                this.hazardousSide = newHazardousSide;
                General.SetDirty(this);
            }
        }

        switch (this.hazardousSide)
        {
            case TriggerEffectSide.Top:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, boxCollider2D.bounds.max, new Vector2(boxCollider2D.bounds.min.x, boxCollider2D.bounds.max.y), Gizmos.color);

                break;
            case TriggerEffectSide.Right:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, boxCollider2D.bounds.max, new Vector2(boxCollider2D.bounds.max.x, boxCollider2D.bounds.min.y), Gizmos.color);

                break;
            case TriggerEffectSide.Bottom:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, boxCollider2D.bounds.min, new Vector2(boxCollider2D.bounds.max.x, boxCollider2D.bounds.min.y), Gizmos.color);

                break;
            case TriggerEffectSide.Left:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, boxCollider2D.bounds.min, new Vector2(boxCollider2D.bounds.min.x, boxCollider2D.bounds.max.y), Gizmos.color);

                break;
            case TriggerEffectSide.Always:
                GizmosExtra.DrawWireRect(boxCollider2D.bounds, Color.red);

                break;
            default:
                break;
        }
    }
}
