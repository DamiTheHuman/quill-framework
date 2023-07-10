using UnityEngine;
/// <summary>
/// An object which moves the player forward on contact  without modifying the players velocity
/// Damizean - Code for Conveyor Belt Landing used
/// </summary>
public class ConveyorBeltController : SolidContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [Tooltip("Whether the conveyor belt has a lower section"), SerializeField]
    private bool hasInverseLinkSection = true;
    [Tooltip("The link objeck to clone"), SerializeField]
    private GameObject linkToClone;
    [Tooltip("The length of the conveyor belt"), SerializeField]
    private int conveyorLength = 56;
    [Tooltip("The acceleration of the player while conveyor"), SerializeField]
    private Vector2 acceleration = new Vector2(1, 0);
    [Tooltip("A list of animators that belong to this object"), SerializeField]
    private Animator[] conveyorAnimators;

    public Color primaryDebugColor = Color.blue;
    public Color secondaryDebugColor = Color.green;
    // Start is called before the first frame update
    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        this.BuildConveyorBeltLinks();
        this.conveyorAnimators = this.GetComponentsInChildren<Animator>();
        this.UpdateConveyorAnimatorDirection();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Builds the links that comprise of the top of the conveyor belt
    /// </summary>
    public void BuildConveyorBeltLinks()
    {
        Vector2 currentPlacementPosition = this.linkToClone.transform.position;
        SpriteRenderer linkSprite = this.linkToClone.GetComponent<SpriteRenderer>();
        Transform linkParent = this.linkToClone.transform.parent;

        for (int x = 1; x < this.conveyorLength; x++)
        {
            currentPlacementPosition.x += linkSprite.size.x;//place the game object  adding the sprite width to the x position
            GameObject conveyorPieceClone = Instantiate(this.linkToClone.gameObject, currentPlacementPosition, Quaternion.Euler(0, 0, 0));
            conveyorPieceClone.transform.parent = linkParent;
            conveyorPieceClone.name = this.linkToClone.name + " " + x;
            currentPlacementPosition = conveyorPieceClone.transform.position;//Update the current position
        }
        if (this.hasInverseLinkSection)
        {
            GameObject bottomSection = Instantiate(linkParent.gameObject);
            bottomSection.name = "Bottom " + linkParent.name;
            bottomSection.transform.parent = linkParent.transform.parent;
            Vector2 bottomSectionPosition = linkParent.localPosition + new Vector3(linkSprite.size.x * (this.conveyorLength + 1), 0);
            bottomSection.transform.localScale = new Vector3(-1, -1, 1);
            bottomSection.transform.localPosition = bottomSectionPosition * new Vector2(1, -1);
        }
    }

    /// <summary>
    /// Flips the conveyor belt to move in the opposite direction
    /// </summary>
    public void FlipConveyor()
    {
        this.acceleration *= -1;
        this.UpdateConveyorAnimatorDirection();
    }

    /// <summary>
    /// Updates the direction of the conveyor objects animators
    /// </summary>
    public void UpdateConveyorAnimatorDirection()
    {
        foreach (Animator animator in this.conveyorAnimators)
        {
            animator.SetFloat("Direction", Mathf.Sign(this.acceleration.x));
        }
    }

    /// <summary>
    /// Verifies the collision wiht the conveyor based off the sensor data
    /// <param name="player">The player object to check against </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = player.GetGrounded();

        return triggerAction;
    }

    /// <summary>
    /// Constantly move the player by transforming the players position directly as stated by SPG
    /// <param name="player">The player object to check against </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);

        if (player.GetGrounded() && player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform == this.transform)
        {
            player.transform.position += (Vector3)this.acceleration;
        }

        if (player.GetGrounded() == false)
        {
            if (player.transform.position.x > this.boxCollider2D.bounds.max.x || player.transform.position.x < this.boxCollider2D.bounds.max.x)
            {
                player.velocity += this.acceleration;
                player.GetSolidBoxController().RemoveActiveContactEvent(this);
            }
        }
    }

    /// <summary>
    /// Transfers the conveyors speed back to the player to provide a narual exit
    /// <param name="player">The player object to check against </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        player.velocity += this.acceleration;

        if (player.GetGrounded())
        {
            player.groundVelocity += this.acceleration.x;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)//For Debugging purposes
        {
            this.UpdateConveyorAnimatorDirection();
        }
    }

    private void OnDrawGizmos()
    {
        if (this.linkToClone != null)
        {
            Gizmos.color = this.primaryDebugColor;
            BoxCollider2D boxCollider2D = this.GetComponent<BoxCollider2D>();
            Vector2 currentPlacementPosition = this.linkToClone.transform.position;
            SpriteRenderer linkSprite = this.linkToClone.GetComponent<SpriteRenderer>();
            Bounds bounds = new Bounds
            {
                size = linkSprite.size
            };
            bounds = linkSprite.bounds;
            bounds.max += new Vector3((this.conveyorLength - 1) * linkSprite.size.x, 0);
            GizmosExtra.DrawWireRect(bounds, this.primaryDebugColor);

            if (this.hasInverseLinkSection)
            {
                bounds.center -= new Vector3(0, boxCollider2D.bounds.size.y) - new Vector3(0, linkSprite.size.y + 1);
                GizmosExtra.DrawWireRect(bounds, this.primaryDebugColor);
            }
            Gizmos.color = this.secondaryDebugColor;
            //Draw Arrow
            if (Mathf.Sign(this.acceleration.x) >= 0)
            {
                Gizmos.DrawLine(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.size.x / 3, 0), boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.size.x / 3, 0));
                GizmosExtra.Draw2DArrow(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.size.x / 3, 0), -90, 0, 8);
            }
            else
            {
                Gizmos.DrawLine(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.size.x / 3, 0), boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.size.x / 3, 0));
                GizmosExtra.Draw2DArrow(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.size.x / 3, 0), 90, 0, 8);
            }
        }
    }
}

