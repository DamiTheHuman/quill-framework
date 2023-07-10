using UnityEngine;
/// <summary>
/// This platfom begins to fall towards the ground on contact with it
/// </summary>
public class FallingPlatformController : SolidContactGimmick
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [Tooltip("A flag to determine whether the platform is falling"), SerializeField]
    private bool falling = false;
    [Tooltip("The speed in which the platfomr whill fall"), SerializeField]
    private float fallVelocity = 1f;
    [Tooltip("The current velocity of the falling platform"), SerializeField]
    private Vector2 velocity;

    public override void Reset()
    {
        base.Reset();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.spriteRenderer == null)
        {
            this.Reset();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (this.falling)
        {
            this.velocity.y = -this.fallVelocity;
            this.Move(this.velocity);
            if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.spriteRenderer.bounds.max))
            {
                if (this.transform.childCount > 0)
                {
                    this.transform.DetachChildren();
                }
                this.gameObject.SetActive(false);//Disable the gameobject
            }
            if (this.transform.childCount > 0 && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer)) //Syncs up child objects to the platforms movement
            {
                Physics2D.SyncTransforms();
            }
        }
    }

    private void Move(Vector2 velocity) => this.transform.position += (Vector3)(velocity * GMStageManager.Instance().GetPhysicsMultiplier()) * Time.deltaTime;
    /// <summary>
    /// Checks if the players is untop of the falling platform
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = player.GetGrounded() && player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform == this.transform;

        return triggerAction;
    }

    /// <summary>
    /// Child the player to the platform forcing the player to move with the platform
    /// <param name="player">The player object untop of the platform </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.falling = true;
        player.transform.parent = this.transform;
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
            }
        }
    }
}
