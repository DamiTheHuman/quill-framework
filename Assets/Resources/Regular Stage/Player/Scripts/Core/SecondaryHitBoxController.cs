using UnityEngine;
/// <summary>
/// Secondary hitbox items like projectiles and attacks like Tails'Tails
/// </summary>
public class SecondaryHitBoxController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    protected Player player;
    [SerializeField, LastFoldoutItem()]
    protected BoxCollider2D boxCollider2D;
    [SerializeField]
    private bool canReflectBullets = false;

    [Tooltip("The current hit box mode"), Help("Attached state is used for attacks like Tails'Tails while Projectile is for attacks like bullets")]
    public SecondaryHitBoxType secondaryHitBoxType = SecondaryHitBoxType.Attached;
    [Tooltip("The layers the hit box collider can interact with")]
    public LayerMask collisionMask = new LayerMask();
    [Tooltip("The debug colour of the collider bounds")]
    public Color hitboxDebugColor = General.RGBToColour(255, 0, 255, 170);

    private void Reset()
    {
        this.collisionMask |= 1 << LayerMask.NameToLayer("Hit Box Collision Layer");
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.player = this.GetComponentInParent<Player>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate()
    {
        if (this.secondaryHitBoxType == SecondaryHitBoxType.Projectile) //Assuming its a projectile like item constantly check for collisions
        {
            this.CheckHitBoxCollisions();
        }
    }

    /// <summary>
    /// Checks for an object interaction and performs the specified action if the its true
    /// </summary>
    public void CheckHitBoxCollisions()
    {
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
                hitboxContactEvent.SecondaryHitBoxObjectAction(this);
            }
        }
    }

    /// <summary>
    /// Get the secondary hitbox type
    /// </summary>
    public SecondaryHitBoxType GetSecondaryHitBoxType() => this.secondaryHitBoxType;
    /// <summary>
    /// Sets the secondary hitbox type
    /// </summary>
    public void SetSecondaryHitBoxType(SecondaryHitBoxType hitBoxMode) => this.secondaryHitBoxType = hitBoxMode;

    /// <summary>
    /// Get the current player
    /// </summary>
    public Player GetPlayer() => this.player;

    /// <summary>
    /// Set the current player
    /// </summary>
    public void SetPlayer(Player player) => this.player = player;
    /// <summary>
    /// Get if the secondary hitbox can reflect bullets
    /// </summary>
    public bool GetCanReflectBullets() => this.canReflectBullets;

    private void OnDrawGizmos()
    {
        if (this.enabled == false)
        {
            return;
        }

        BoxCollider2D boxCollider2D = this.GetComponent<BoxCollider2D>();
        Color debugColor = this.hitboxDebugColor;
        GizmosExtra.DrawRect(this.transform, boxCollider2D, debugColor, true);
    }
}
