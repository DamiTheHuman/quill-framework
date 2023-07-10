using UnityEngine;
/// <summary>
/// The controller class that handles the bolt which the player interacts with
/// Author - Originally coded by Damizean & DW
/// Update - Slight modifications to make it more personalized
/// </summary
public class ScrewBoltController : SolidContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private ScrewController screwController;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField, LastFoldoutItem()]
    private Animator animator;

    [SerializeField, Tooltip("The current velocity of the screw bolt")]
    private Vector2 velocity;
    [SerializeField, Tooltip("The gravity used to push the screw bolt downwards when falling")]
    private float gravity = 0.21875f;
    [SerializeField, Tooltip("The current direction the player is moving in on the bolt")]
    private ScrewBoltDirection screwBoltDirection = ScrewBoltDirection.None;
    [SerializeField, Tooltip("The current state of the screw bolt")]
    private ScrewBoltState screwBoltState = ScrewBoltState.Idle;

    [SerializeField, Tooltip("The snatch range of the screw bolt to snap the player to the center")]
    private float snatchRange = 32;
    [SerializeField, Tooltip("The length of the screw bolt animation")]
    private int animationLength = 4;
    [SerializeField, Tooltip("The current side of the screw bolt being displayed")]
    private float animationFrame = 0;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate()
    {
        if (this.screwBoltState == ScrewBoltState.Falling)
        {
            this.Move(this.velocity);
            this.ApplyGravity();
            this.CheckToDeactivate();
        }
    }

    /// <summary>
    /// Sets the active screw controller
    /// <param name="screwController">The screwController to be parented to </param>
    /// </summary>
    public void SetScrewController(ScrewController screwController) => this.screwController = screwController;

    /// <summary>
    /// Update the face of the screw bolt
    /// <param name="playerHorizontalVelocity">The current horzitonal velocity of the player</param>
    /// </summary>
    private void UpdateScrewBoltAnimation(float playerHorizontalVelocity)
    {
        this.animationFrame = (this.animationLength + (this.animationFrame * this.animationLength) - (playerHorizontalVelocity * 0.11f)) % this.animationLength;
        this.animationFrame /= this.animationLength;
        this.animator.Play("Spin", 0, this.animationFrame);
    }

    /// <summary>
    /// Apply gravity to the bolts y velocity
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.gravity);

    /// <summary>
    /// Move the bolt in the direction of its velocity
    /// <param name="velocity">The velocity to move the bolt by</param>
    /// </summary>
    private void Move(Vector2 velocity)
    {
        this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the corkscrew by its velocity
        if (this.transform.childCount > 0 && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer)) //Syncs up child objects to the platforms movement so the child object stays untop
        {
            Physics2D.SyncTransforms();
        }
        if (this.screwBoltState == ScrewBoltState.InUse)
        {
            this.ClampScrewBoltPosition();
        }
    }

    /// <summary>
    /// Clamp the screw bolts position to the top and bottom action points
    /// </summary>x
    private void ClampScrewBoltPosition()
    {
        Vector2 position = this.transform.position;
        position.y = Mathf.Clamp(position.y, this.screwController.GetBottomActionPoint().GetTransform().position.y, this.screwController.GetTopActionPoint().GetTransform().position.y);
        this.transform.position = position;
    }

    /// <summary>
    /// Simply deactivates the bullet when it is below the camera view port
    /// </summary>
    private void CheckToDeactivate()
    {
        if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.spriteRenderer.bounds.max))
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Lock the screw bolt direction based on the players position and input
    /// <param name="player">The player object to check against </param>
    /// </summary>x
    private void VerifyScrewBoltDirection(Player player)
    {
        if (player.GetGrounded() && player.transform.parent != this.transform)
        {
            if (this.transform.position.x > player.transform.position.x ||
                (this.transform.position.x > player.transform.position.x && player.GetInputManager().GetCurrentInput().x == 1) ||
                this.transform.position.x > player.transform.position.x + this.snatchRange)
            {
                this.screwBoltDirection = ScrewBoltDirection.Right;

                player.transform.parent = this.transform;
            }
            else if (this.transform.position.x < player.transform.position.x ||
                    (this.transform.position.x < player.transform.position.x && player.GetInputManager().GetCurrentInput().x == -1) ||
                    this.transform.position.x < player.transform.position.x - this.snatchRange)
            {
                this.screwBoltDirection = ScrewBoltDirection.Left;

                player.transform.parent = this.transform;
            }

        }
    }

    /// <summary>
    /// Set the screw bolt state to falling and prepare to move the bolt downards
    /// <param name="player">The player object to check against </param>
    /// </summary>x
    private void BeginFalling(Player player)
    {
        this.screwBoltState = ScrewBoltState.Falling;
        player.transform.parent = null;
        player.SetGrounded(false);
        this.velocity.y = CheckPlayerGroundVelocity(player) * 0.17f; //THe speed in which the corkscrew falls at initally
        this.boxCollider2D.enabled = false;//Disable collisions with the box collider
        this.animator.SetFloat("SpeedMultiplier", General.ConvertAnimationSpeed(this.animator, CheckPlayerGroundVelocity(player) * -12));
    }

    /// <summary>
    /// Locks the player position to the center of the screw
    /// <param name="player">The player object to check against </param>
    /// </summary>x
    private void LockPlayerToScrew(Player player)
    {
        if ((player.groundVelocity >= 0 && this.screwBoltDirection == ScrewBoltDirection.Right && player.transform.position.x >= this.transform.position.x) ||
             (player.groundVelocity <= 0 && this.screwBoltDirection == ScrewBoltDirection.Left && player.transform.position.x <= this.transform.position.x))
        {
            this.screwBoltState = ScrewBoltState.InUse;
            player.transform.parent = this.transform;
            player.transform.localPosition = new Vector2(0, player.transform.localPosition.y);
            player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.OnScrewBolt);
            player.SetMovementRestriction(MovementRestriction.Horizontal);
        }
    }

    /// <summary>
    /// Update the screw position based on the velocity of the player and the position of the screw
    /// <param name="player">The player object to check against </param>
    /// </summary>x
    private void UpdateScrewByPlayerVelocity(Player player)
    {
        Vector2 position = this.transform.position;

        Vector2 velocity = new Vector2(0, this.CheckPlayerGroundVelocity(player) * 0.17f);
        ;//update this
        this.Move(velocity);

        if (this.screwBoltDirection == ScrewBoltDirection.Right && velocity.y < 0)
        {
            this.screwBoltDirection = ScrewBoltDirection.Left;
        }
        else if (this.screwBoltDirection == ScrewBoltDirection.Left && velocity.y > 0)
        {
            this.screwBoltDirection = ScrewBoltDirection.Right;
        }

        this.UpdateScrewBoltAnimation(player.groundVelocity);

        if (position.y >= this.screwController.GetTopActionPoint().GetTransform().position.y && this.screwBoltDirection == ScrewBoltDirection.Right)
        {
            this.EndBoltAction(this.screwController.GetTopActionPoint().GetScrewAction(), player);
        }
        else if (position.y <= this.screwController.GetBottomActionPoint().GetTransform().position.y && this.screwBoltDirection == ScrewBoltDirection.Left)
        {
            this.EndBoltAction(this.screwController.GetBottomActionPoint().GetScrewAction(), player);
        }
    }

    /// <summary>
    /// Calculates the players ground velocity based on whether the screw movement is inverted or not
    /// <param name="player">The player object to check against </param>
    /// </summary>x
    private float CheckPlayerGroundVelocity(Player player) => this.screwController.GetInverted() ? player.groundVelocity * -1 : player.groundVelocity;

    /// <summary>
    /// The actions that take place when the bolt reaches the end action point of the corkscrew
    /// <param name="screwAction">The action to be performed at the end</param>
    /// <param name="player">The player object to check against </param>
    /// </summary>x
    private void EndBoltAction(ScrewAction screwAction, Player player)
    {
        if (screwAction == ScrewAction.Fall)
        {
            this.BeginFalling(player);
        }
        else if (screwAction == ScrewAction.ReleasePlayer)
        {
            player.transform.parent = null;
            this.screwBoltDirection = ScrewBoltDirection.None;
            this.screwBoltState = ScrewBoltState.Idle;
        }

        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        player.SetMovementRestriction(MovementRestriction.None);
    }

    /// <summary>
    /// Checks if the players collider is within the activitable range of the screw bolt
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = player.GetGrounded();

        return triggerAction;
    }

    /// <summary>
    /// Check to verify the direction of the screw bolt
    /// <param name="player">The player object to check against  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.VerifyScrewBoltDirection(player);
    }
    /// <summary>
    /// Locks the player to the screw bolt and updates the screw bolt  position based on the players velocity
    /// <param name="player">The player object to check against  </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        if (player.GetGrounded() && this.screwBoltState != ScrewBoltState.Falling)
        {
            this.LockPlayerToScrew(player);
        }

        if (this.screwBoltState == ScrewBoltState.InUse)
        {
            this.UpdateScrewByPlayerVelocity(player);
        }
    }

    /// <summary>
    /// Ends the interactions with the player 
    /// <param name="player">The player object to check against  </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        player.transform.parent = null;
        this.screwBoltDirection = ScrewBoltDirection.None;

        if (this.screwBoltState != ScrewBoltState.Falling)
        {
            this.screwBoltState = ScrewBoltState.Idle;
        }

        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        player.SetMovementRestriction(MovementRestriction.None);
    }
}
