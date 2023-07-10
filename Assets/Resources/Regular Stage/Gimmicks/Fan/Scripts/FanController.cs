using UnityEngine;
/// <summary>
/// The Fan gimmick
/// Author  - Nihil And Lake Fepard -in Sonic Core
/// Changes - Converted to unity with some minor tweaks for customization
/// </summary>
public class FanController : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [Header("The speed that delta is incremented with"), SerializeField]
    private float hoverSpeed = 2;
    [Tooltip("The current delta of the platform movement affecting its position within its lifetime"), SerializeField]
    private float delta = 0;
    [Tooltip("The radius of length of the fans movement boundaries"), SerializeField]
    private float range = 8;
    [Tooltip("The y velocity of the player while on the fan"), SerializeField]
    private float windSpeed = 1;

    public Color debugColor = General.RGBToColour(255, 255, 255, 125);

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        this.startPosition = this.transform.position;

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate()
    {
        this.MoveFan();
        this.delta = General.ClampDegreeAngles(this.delta);

        if (this.transform.childCount > 0 && HedgehogCamera.Instance().AreBoundsWithinCameraView(this.boxCollider2D.bounds)) //Syncs up child objects to the platforms movement
        {
            Physics2D.SyncTransforms();
        }
    }

    /// <summary>
    /// Moves the fan in relation to its speed
    /// </summary>
    private void MoveFan()
    {
        Vector2 position = this.transform.position;
        position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.range);
        this.delta += this.hoverSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.transform.position = position;
    }

    /// <summary>
    /// Checks if the players solid box comes in contact with the solid box and if so begin the action
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = true;
        triggerAction = true;

        return triggerAction;
    }

    /// <summary>
    /// Sets the players animations and the player itself to the appopriate state
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.InFan);
        player.GetActionManager().EndCurrentAction();
        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.FanSpin);
        player.transform.parent = this.transform;
    }

    /// <summary>
    /// Makes the player float while on the fan
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        player.velocity.y = this.windSpeed;
        player.SetGrounded(false);
    }

    /// <summary>
    /// Ends the player contact action with the fan and reverts the state
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);

        if (player.transform.parent == this.transform)
        {
            player.transform.parent = null;
        }
    }

    private void OnDrawGizmos()
    {
        //Draw collider 
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }
        GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor);
        //Draw range
        Vector2 position = Application.isPlaying ? this.startPosition : (Vector2)this.transform.position;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(position + (Vector2.up * this.range), position + (Vector2.down * this.range));
        GizmosExtra.Draw2DArrow(position + (Vector2.up * this.range), 0, 0, 6);
        GizmosExtra.Draw2DArrow(position + (Vector2.down * this.range), 180, 0, 6);
    }

}
