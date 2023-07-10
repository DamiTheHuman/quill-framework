using UnityEngine;
/// <summary>
/// Base platform controller class
/// </summary>
public class PlatformController : SolidContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    protected BoxCollider2D boxCollider2D;
    [SerializeField, LastFoldoutItem()]
    protected SpriteRenderer spriteRenderer;

    [SerializeField, Tooltip("The playe on the platform"), FirstFoldOutItem("Gimmick Info")]
    protected Player player;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
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
    /// Sync child objects to the platforms position
    /// </summary>
    protected void SyncChildPositions()
    {
        if (this.transform.childCount > 0) //Syncs up child objects to the platforms movement
        {
            if ((this.spriteRenderer != null && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer))
                || HedgehogCamera.Instance().AreBoundsWithinCameraView(this.boxCollider2D.bounds))
            {
                Physics2D.SyncTransforms();
            }
        }
    }

    /// <summary>
    /// Checks if the players is untop of the moving platform
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if (player.GetGrounded())
        {
            //When untop of the platform
            if (this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y <= 0)
            {
                triggerAction = true;
            }
        }

        return triggerAction;
    }

    /// <summary>
    /// Child the player to the platform forcing the player to move with the platform
    /// <param name="player">The player object untop of the platform </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        if (player.transform.parent != this.transform)
        {
            player.transform.parent = this.gameObject.transform;
            this.player = player;
        }
    }

    /// <summary>
    /// Watch to unchild the player if they are not grounded or their ground sensors are not targeted at this
    /// <param name="player">The player object untop of the platform </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        if (player.GetGrounded() == false || player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform != this.transform)
        {
            if (player.transform.parent == this.transform)
            {
                player.transform.parent = null;
                this.player = null;
            }
        }
    }
}
