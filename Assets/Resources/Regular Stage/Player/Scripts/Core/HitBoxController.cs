using UnityEngine;
/// <summary>
/// The controller handles collision with objects that can harm the player
/// </summary>
public class HitBoxController : MonoBehaviour
{

    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField, LastFoldoutItem()]
    private BoxCollider2D boxCollider2D;
    [Tooltip("A secondary hitbox without direct relations to the player"), SerializeField]
    private SecondaryHitBoxController secondaryHitBoxController;
    [Tooltip("The layers the hit box collider can interact with"), SerializeField]
    private LayerMask collisionMask;
    [Tooltip("The size of the hit box collider when regular"), SerializeField]
    private Rect normalBounds;
    [Tooltip("The size of the hit box collider when shrunk"), SerializeField]
    private Rect shrinkBounds;
    [Tooltip("The debug colour of the collider bounds"), SerializeField]
    private Color hitBoxDebugColor = General.RGBToColour(247, 0, 255, 170);
    [Tooltip("The debug colour of the collider bounds while attacking"), SerializeField]
    private Color attackingDebugColor = General.RGBToColour(255, 77, 84, 170);

    private void Reset()
    {
        this.player = this.GetComponentInParent<Player>();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        this.normalBounds.x = this.boxCollider2D.offset.x;
        this.normalBounds.y = this.boxCollider2D.offset.y;
        this.normalBounds.width = this.boxCollider2D.size.x;
        this.normalBounds.height = this.boxCollider2D.size.y;
    }

    /// <summary>
    /// Checks for an object interaction and performs the specified action if the its true
    /// </summary>
    public HitBoxContactEventResult CheckHitBoxCollisions()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Die>())
        {
            return HitBoxContactEventResult.NoContact;
        }

        Vector2 startVelocity = this.player.velocity;
        this.UpdateHitBoxAngle(this.player.GetSpriteController().GetSpriteAngle());

        //Check secondary hitbox controllers that are attached to the hitbox like Tails' Tails or Knuckles Gloves
        if (this.secondaryHitBoxController != null && this.secondaryHitBoxController.isActiveAndEnabled)
        {
            if (this.secondaryHitBoxController.secondaryHitBoxType == SecondaryHitBoxType.Attached)
            {
                this.secondaryHitBoxController.CheckHitBoxCollisions();
            }
        }
        //AABB collisions
        Bounds bounds = this.boxCollider2D.bounds;
        bounds.center = (Vector2)this.transform.position + this.boxCollider2D.offset;
        Collider2D boxCollision = Physics2D.OverlapBox(bounds.center, new Vector2(bounds.size.x, bounds.size.y), 0, this.collisionMask);
        //Check when we hit an enemy
        if (boxCollision)
        {
            HitBoxContactEvent hitboxContactEvent = boxCollision.GetComponent<HitBoxContactEvent>();
            if (hitboxContactEvent != null)
            {
                //Check if the player is to be harmed
                if (hitboxContactEvent.HedgeIsCollisionValid(this.player, bounds))
                {
                    //If we are homing at the current object trigger an event
                    if (this.player.GetActionManager().CheckActionIsBeingPerformed<HomingAttack>())
                    {
                        HomingAttack homingAttack = this.player.GetActionManager().GetAction<HomingAttack>() as HomingAttack;
                        if (homingAttack.GetHomingAttackMode() == HomingAttackMode.Homing && homingAttack.GetCurrentTarget() == hitboxContactEvent.gameObject)
                        {
                            homingAttack.OnHitTargetObject(hitboxContactEvent);
                        }
                    }

                    hitboxContactEvent.HedgeOnCollisionEnter(this.player);
                }
            }
            else if (GMStageManager.Instance().IsInHazardLayer(boxCollision.transform.gameObject))//Generiz hazard with no necessary calculations
            {
                this.player.GetHealthManager().VerifyHit(boxCollision.transform.position.x);
            }

            return this.player.velocity == startVelocity ? HitBoxContactEventResult.ContactFoundAndVelocityNotModified : HitBoxContactEventResult.ContactFoundAndVelocityModified;
        }

        return HitBoxContactEventResult.NoContact;
    }

    /// <summary>
    /// Updates the angle of the hit box
    /// <param name="angle">The new angle of the hit box</param>
    /// </summary>
    public void UpdateHitBoxAngle(float angle) => this.transform.rotation = Quaternion.Euler(0f, 0f, angle);

    /// <summary>
    /// Set the secondary hitbox  to be monitored
    /// <param name="flightHitbox">The secondary hitbox to watch for </param>
    /// </summary>
    public void SetSecondaryHitBox(SecondaryHitBoxController secondaryHitBoxController) => this.secondaryHitBoxController = secondaryHitBoxController;

    /// <summary>
    /// Updates the current colliders bounds based on the size mode
    /// <param name="sizeMode">The new size mode of the solid box</param>
    /// </summary>
    public void UpdateHitBoxBounds(SizeMode sizeMode)
    {

        switch (sizeMode)
        {
            case SizeMode.Regular:
                this.boxCollider2D.offset = new Vector2(this.normalBounds.x, this.normalBounds.y);
                this.boxCollider2D.size = new Vector2(this.normalBounds.width, this.normalBounds.height);

                break;
            case SizeMode.Shrunk:
            case SizeMode.Gliding:
                this.boxCollider2D.offset = new Vector2(this.shrinkBounds.x, this.shrinkBounds.y);
                this.boxCollider2D.size = new Vector2(this.shrinkBounds.width, this.shrinkBounds.height);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the box collider state
    /// <param name="state">The new state of the collider</param>
    /// </summary>
    public void SetSolidBoxColliderState(bool state) => this.boxCollider2D.enabled = state;
    /// <summary>
    /// Swaps the hit box controller data with the new target hit box controller
    /// <param name="targetHitBoxController">The hit box controller to swap to</param>
    /// </summary>
    public void SwapHitBoxController(HitBoxController targetHitBoxController)
    {
        this.normalBounds = targetHitBoxController.normalBounds;
        this.shrinkBounds = targetHitBoxController.shrinkBounds;

        if (this.player.GetActionManager().currentPrimaryAction != null)
        {
            this.UpdateHitBoxBounds(this.player.GetActionManager().currentPrimaryAction.sizeMode);
        }
        else
        {
            this.UpdateHitBoxBounds(SizeMode.Regular);
        }
    }

    /// <summary>
    /// Get the secondary hitbox controller
    /// </summary>
    public SecondaryHitBoxController GetSecondaryHitBoxController() => this.secondaryHitBoxController;

    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider2D = this.GetComponent<BoxCollider2D>();
        Color debugColor = Application.isPlaying && this.player.GetAttackingState() ? this.attackingDebugColor : this.hitBoxDebugColor;
        GizmosExtra.DrawRect(this.transform, boxCollider2D, debugColor, true);

        if (Application.isPlaying == false)
        {
            this.normalBounds.x = boxCollider2D.offset.x;
            this.normalBounds.y = boxCollider2D.offset.y;
            this.normalBounds.width = boxCollider2D.size.x;
            this.normalBounds.height = boxCollider2D.size.y;
        }
    }
}
