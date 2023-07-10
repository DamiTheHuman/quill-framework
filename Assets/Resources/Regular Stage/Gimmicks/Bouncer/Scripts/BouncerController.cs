using UnityEngine;
/// <summary>
/// A simple gimmick that pushes the player in the direction of the bouncer
/// Original Author - Nihil 
/// </summary>
public class BouncerController : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [SerializeField, Tooltip("The velocity the player will be bounced off")]
    private Vector2 bounceVelocity = new Vector2(5, 5);
    [Tooltip("The audio played when the bouncer is touched"), SerializeField]
    private AudioClip bouncerTouchedSound;

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
    /// Checks if the player comes in contact with the bouncer
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds);

        return triggerAction;
    }

    /// <summary>
    /// Bounces the player in the direction of the bouncers
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.velocity.x = this.bounceVelocity.x * this.transform.localScale.x; //The further away the more force
        player.velocity.y = this.bounceVelocity.y;
        player.SetGrounded(false);
        player.GetActionManager().EndCurrentAction();
        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.Bouncer);//Play the bouncer animation
        GMAudioManager.Instance().PlayOneShot(this.bouncerTouchedSound);
    }
}
