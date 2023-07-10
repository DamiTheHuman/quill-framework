using UnityEngine;
/// <summary>
/// A collapsable block which deactivates its collider and falls towards the ground once activated
///</summary>
public class CollapsableBlockController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D parentCollider2D;

    [Tooltip("The direction which the platform falls in ")]
    public CollapsableBlockType blockType = CollapsableBlockType.Regular;
    [SerializeField]
    [Tooltip("A flag which determines whether the block is currently in motion")]
    private bool activated = false;
    [SerializeField]
    [Tooltip("The current velocity of the block")]
    private Vector2 velocity;
    [SerializeField]
    [Tooltip("The amount in which velocity will be decremented by every step")]
    private float gravity = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();

        if (this.transform.parent != null)
        {
            this.parentCollider2D = this.GetComponentInParent<Collider2D>();

        }
    }

    private void FixedUpdate()
    {
        if (!this.activated)
        {
            return;
        }

        this.Move(this.velocity);
        this.ApplyGravity();
        this.CheckForDeactivation();
    }

    /// <summary>
    /// Sets the activation and begins the collapsables block movement and deactivation
    /// <param name="fallGravity">How fast the collapsable block falls towards the ground  </param>
    /// </summary>
    public void BeginCollapse(float fallGravity = 0)
    {
        if (this.activated)
        {
            return;
        }

        this.gravity = fallGravity == 0 ? this.gravity : fallGravity;

        if (this.blockType == CollapsableBlockType.RemoveCollisionWhenActivated)
        {
            this.parentCollider2D.enabled = false;
        }

        this.activated = true;
    }

    /// <summary>
    /// Move the block by its current velocity every step
    /// <param name="velocity">The current velocity of the collapsable block  </param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the collapsable block by its velocity
    /// <summary>
    /// A cvertical force thet moves the block towards the ground
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.gravity);

    /// <summary>
    /// Checks whether to deactivate the collapsable block when it is below the camera view
    /// </summary>
    private void CheckForDeactivation()
    {
        if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.spriteRenderer.bounds.max))
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the parent polygon collider
    /// </summary>
    public Collider2D GetParentCollider() => this.parentCollider2D;
}
