using UnityEngine;
/// <summary>
/// The controller class that handles the players interactions with the bounce floor
/// Author - Nihil
/// Changes - Converted code to framework consistent syntax and values
/// </summary>
public class BounceFloorController : SolidContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BoxCollider2D boxCollider2D;
    [SerializeField, LastFoldoutItem()]
    private Animator animator;
    [SerializeField]
    private float bounceVelocity = 20;
    [Tooltip("The audio played when the bounce floor is touched")]
    public AudioClip bounceFloorTouchedSound;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.animator = this.GetComponent<Animator>();
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
    /// Checks if the players collider is untop of the bounce floor 
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y <= 0;

        return triggerAction;
    }

    /// <summary>
    /// Apply the bounce velocity to the player
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (player.GetGrounded())
        {
            player.SetGrounded(false);
        }
        player.velocity.y = this.bounceVelocity;
        player.GetActionManager().EndCurrentAction();
        player.GetActionManager().PerformAction<Roll>();
        this.animator.SetTrigger("Bounce");
        GMAudioManager.Instance().PlayOneShot(this.bounceFloorTouchedSound);
    }
}
