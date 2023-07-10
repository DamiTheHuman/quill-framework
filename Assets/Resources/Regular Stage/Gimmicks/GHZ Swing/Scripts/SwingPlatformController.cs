using UnityEngine;

///<summary>
/// The end swing platform controller
/// Author - LARK SS orignal by DW & Damizean [Sonic Worlds]  
/// Changes - Converted to UNITY and made it more approachable
///</summary>
public class SwingPlatformController : SolidContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SwingController swingController;

    [Tooltip("The current link ID of the swing which affects its distance and is based on its distance from the swing controller")]
    public int linkID = 0;
    [Tooltip("The distance of the current swing link from the parent")]
    public float distance = 0;
    [SerializeField]
    protected Player player;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.swingController = this.GetComponentInParent<SwingController>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.distance = (this.swingController.swingLength + 1) * this.swingController.linkItemHeight;
    }

    private void FixedUpdate()
    {
        this.SwingMovement();

        if (this.transform.childCount > 0 && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer)) //Syncs up child objects to the platforms movement
        {
            Physics2D.SyncTransforms();
        }
    }

    /// <summary>
    /// Move the swing platform based on the distance to the top point
    /// </summary>
    private void SwingMovement()
    {
        Vector2 pos = new Vector2
        {
            x = this.swingController.transform.position.x + (this.distance * Mathf.Cos(this.swingController.swingAngle * Mathf.Deg2Rad)),
            y = this.swingController.transform.position.y + (this.distance * Mathf.Sin(this.swingController.swingAngle * Mathf.Deg2Rad))
        };

        this.transform.position = pos;
    }

    /// <summary>
    /// Checks if the players collider is within the activitable range of the bouncy side 
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if (player.GetGrounded() && this.swingController.swingType == SwingType.Platform)
        {
            //Perform action if the player is untop of the platform
            if (this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y <= 0)
            {
                triggerAction = true;
            }
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

        if (this.swingController.swingType == SwingType.Platform)
        {
            if (player.transform.parent != this.transform)
            {
                player.transform.parent = this.gameObject.transform;
            }
        }
    }

    /// <summary>
    /// Watch to unchild the player if they are not grounded or their ground sensors are not targeted at this
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);

        if (this.swingController.swingType == SwingType.Platform)
        {
            if (player.GetGrounded() == false || player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform != this.transform)
            {
                if (player.transform.parent == this.transform)
                {
                    player.transform.parent = null;
                }
            }
        }
    }

}
