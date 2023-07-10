using UnityEngine;
/// <summary>
/// Identifies an integral point of the corkscrew to handle collisions with players solid box and respond
/// </summary>
public class CorkscrewIntegralPointController : TriggerContactGimmick
{
    [SerializeField]
    private CorkscrewController corkscrewController;
    [SerializeField]
    private LayerMask collisionMask;
    [SerializeField]
    private CorkscrewSpinDirection corkScrewStartDirection = CorkscrewSpinDirection.Left;
    [SerializeField]
    private Vector2 integralSensorOffset = new Vector2(0, 21);
    [SerializeField]
    private float integralOffsetHeight = 48;
    [SerializeField]
    private Color debugColor = Color.red;
    private float offsetStartSprinProgress = 0;

    /// <summary>
    /// Set the main corkscrew controller and whether its the start or end point
    /// <param name="corkscrewController">The main corkscrew controller  </param>
    /// <param name="corkScrewStart">The corkscrew start point  </param>
    /// </summary>
    public void SetCorkScrewController(CorkscrewController corkscrewController, CorkscrewSpinDirection corkScrewStart)
    {
        this.corkscrewController = corkscrewController;
        this.corkScrewStartDirection = corkScrewStart;
        this.collisionMask |= 1 << corkscrewController.solidBoxLayer;
    }

    /// <summary>
    /// Checks if the players has collided within the range of the corkscrew
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = player.GetGrounded() || this.corkscrewController.onCorkscrew;
        this.offsetStartSprinProgress = Vector3.Magnitude(solidBoxColliderBounds.center) - Vector3.Magnitude(this.transform.position);

        return triggerAction;
    }

    /// <summary>
    /// Place the player on the corkscrew or end the players action with corkscrew based on which side the player collides with
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (this.corkscrewController.onCorkscrew == false && Mathf.Abs(player.groundVelocity) > 0)
        {
            this.corkscrewController.BeginCorkScrewSpin(player, this.corkScrewStartDirection, this.offsetStartSprinProgress);
        }
    }

    /// <summary>
    /// End the corkscrew spin when the player exits the boundaries of the end collider
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);

        if (this.corkscrewController.onCorkscrew)
        {
            if (this.corkScrewStartDirection == CorkscrewSpinDirection.Left && player.groundVelocity < 0)
            {
                this.corkscrewController.EndCorkscrewSpin();
            }
            else if (this.corkScrewStartDirection == CorkscrewSpinDirection.Right && player.groundVelocity > 0)
            {
                this.corkscrewController.EndCorkscrewSpin();
            }
            this.offsetStartSprinProgress = 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        BoxCollider2D boxCollider2D = this.GetComponent<BoxCollider2D>();
        GizmosExtra.DrawResponsiveLine(this.transform.position, this.transform.eulerAngles.z - 90, this.integralSensorOffset.y, this.integralOffsetHeight);
        GizmosExtra.DrawRect(this.transform, boxCollider2D, this.debugColor, true);
    }
}
