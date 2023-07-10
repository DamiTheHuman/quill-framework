using UnityEngine;
/// <summary>
/// Controls the way the monitor moves and reacts to the player movements
///  Original Author - Damizean [Sonic Worlds] 
///  Added functionality - Converted to unity and made it count backwards 
/// </summary>
public class MonitorController : SolidContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BoxCollider2D boxCollider2D;
    [SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;

    [Tooltip("The base for the monitor icon"), FirstFoldOutItem("Monitor Info"), SerializeField]
    private MonitorIconController monitorController;

    [Tooltip("The Power up to give the player when the monitor is broken"), SerializeField]
    private PowerUp powerUpToGrant = PowerUp.TenRings;
    [Tooltip("Half height of the monitor"), SerializeField]
    private float monitorHalfHeight = 16;
    [Tooltip("The offset position for the broken monitor")]
    public Vector2 brokenMonitorPositionOffset = new Vector2(0, 20);
    [Tooltip("The audio played when the power up is granted"), SerializeField]
    private AudioClip powerUpGrantSound;
    [LastFoldoutItem(), Tooltip("The audio played when the monitor is broken"), SerializeField]
    private AudioClip destroyMonitorSound;

    [Tooltip("The Layer of the players solid box"), FirstFoldOutItem("Monitor Collision Info")]
    [SerializeField, LayerList] private int solidBoxLayer = 27;
    [SerializeField, Tooltip("The current ground the monitor is on if active")]
    private CollisionInfoData currentGroundInfo;
    [Tooltip("The collision mask for the monitor"), LastFoldoutItem(), SerializeField]
    private LayerMask collisionMask;

    [Tooltip("The Layer of the players solid box"), FirstFoldOutItem("Monitor Velocities"), SerializeField]
    private Vector2 monitorVelocity;
    [Tooltip("The force that pushes the monitor towards the ground"), SerializeField]
    private float monitorGravity = 0.2734375f;
    [Tooltip("The force added to the monitor when it bounces"), LastFoldoutItem(), SerializeField]
    private float monitorBounceHeight = 2;

    [Tooltip("Whether the monitor is grounded or not"), FirstFoldOutItem("Monitor States"), SerializeField]
    private bool grounded = true;
    [Tooltip("Whether the monitor has been bumped by the player and too plummet to the ground"), SerializeField]
    private bool bumped = false;
    [Tooltip("A flag to determine whether the monitor has been bumped before"), SerializeField]
    private bool bumpedBefore = false;
    [Tooltip("A flag to determine whether the monitor was a hidden monitor"), LastFoldoutItem, SerializeField]
    private bool hiddenMonitor = false;

    [IsDisabled, SerializeField, Tooltip("The start position of the monitor")]
    private Vector2 startPositon;

    [Tooltip("The Colour of the ray casted"), FirstFoldOutItem("Debug Color"), LastFoldoutItem(), SerializeField]
    private Color debugColor = Color.red;

    public override void Reset()
    {
        base.Reset();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }
    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.monitorController.enabled = false;

        if (this.powerUpToGrant == PowerUp.ExtraLife && GMHistoryManager.Instance().GetRegularStageScoreHistory().CheckIfBrokenLifeMonitorAtPositionExists(this.startPositon))
        {
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BrokenMonitor, this.transform.position);
            this.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        this.MoveAndCollide(this.monitorVelocity);
        this.ApplyGravity();

        if (this.hiddenMonitor && GMStageManager.Instance().GetStageState() == RegularStageState.ActClear)
        {
            if (GMStageManager.Instance().GetPlayer().GetActionManager().CheckActionIsBeingPerformed<Victory>())
            {
                this.DestroyMonitor();
            }
        }
    }

    /// <summary>
    /// Checks if the players collider is within the activitable range of the monitor
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        Glide glide = player.GetActionManager().GetAction<Glide>() as Glide;
        bool isGliding = glide != null && glide.GetGlideState() == GlideState.Gliding;

        HomingAttack homingAttack = player.GetActionManager().GetAction<HomingAttack>() as HomingAttack;
        bool isHomingAttack = homingAttack != null && homingAttack.GetHomingAttackMode() == HomingAttackMode.Homing;

        if (isHomingAttack)
        {
            return true;
        }

        if (isGliding)
        {
            triggerAction = (this.TargetIsToTheLeft(solidBoxColliderBounds) && player.velocity.x >= 0) || (this.TargetIsToTheRight(solidBoxColliderBounds) && player.velocity.x <= 0);
        }
        //Check whether to collide or not
        if (player.GetAttackingState())
        {
            //From the top
            if (this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y < 0 && !player.GetGrounded())
            {
                triggerAction = true;
            }

            //From the right Side
            else if (this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheRight(solidBoxColliderBounds) && player.velocity.x < 0 && player.velocity.y <= 0)
            {
                if (player.velocity.y <= 0)
                {
                    if (player.GetGrounded() == false && solidBoxColliderBounds.center.y >= this.boxCollider2D.bounds.center.y)
                    {
                        triggerAction = true;
                    }
                    else if (player.GetGrounded())
                    {
                        triggerAction = true;
                    }
                }
            }
            //From the left Side
            else if (this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheLeft(solidBoxColliderBounds) && player.velocity.x > 0 && player.velocity.y <= 0)
            {
                if (player.GetGrounded() == false && solidBoxColliderBounds.center.y >= this.boxCollider2D.bounds.center.y)
                {
                    triggerAction = true;
                }
                else if (player.GetGrounded())
                {
                    triggerAction = true;
                }
            }
        }
        //From the bottom
        if (this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheBottom(solidBoxColliderBounds) && player.velocity.y >= 0)
        {
            if ((this.bumped == false && player.velocity.y >= 0) || this.bumped)
            {
                player.velocity.y = 0;
                this.MonitorBounce();
            }
        }

        return triggerAction;
    }
    /// <summary>
    /// Perform the monitor action when the player validates the trigger action
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        //If the player performed an aerial attack do a smooth rebound
        if ((player.GetGrounded() == false && player.velocity.y < 0) || player.GetActionManager().CheckActionIsBeingPerformed<HomingAttack>())
        {
            player.AttackRebound();
        }

        if (this.powerUpToGrant == PowerUp.ExtraLife)
        {
            GMHistoryManager.Instance().GetRegularStageScoreHistory().AddBrokenLifeMonitor(this, this.startPositon);
        }

        GMGrantPowerUpManager.Instance().DelayPowerUpGrant(player, this.powerUpToGrant, this.powerUpGrantSound);//Grant the player an item
        this.DestroyMonitor();//Get rid of it
    }
    /// <summary>
    /// Move the sensors in the direction of the monitors velocity and stick to ground
    /// <param name="velocity">The velocity to the move the object in  </param>
    /// </summary>
    private void MoveAndCollide(Vector2 velocity)
    {
        if (this.grounded == false && this.bumped)
        {
            this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the monitor based on its velocity
            this.StickToGround(this.transform.position);
        }
    }
    /// <summary>
    /// Apply gravity to the monitors y velocity
    /// </summary>
    private void ApplyGravity()
    {
        if (this.grounded == false && this.bumped && this.bumpedBefore)
        {
            this.monitorVelocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.monitorGravity);
        }
    }
    /// <summary>
    /// Sticks the monitor to the top of anything considered ground
    /// <param name="position">The current position of the monitor </param>
    /// </summary>
    private bool StickToGround(Vector2 position)
    {
        float distance = this.monitorHalfHeight;
        SensorData sensorData = new SensorData(0, -90, 0, distance);

        Vector2 monitorGroundSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), Vector2.zero); // A - Left Ground Senso

        RaycastHit2D monitorGroundSensor = Physics2D.Raycast(monitorGroundSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(monitorGroundSensorPosition, monitorGroundSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.debugColor);

        if (monitorGroundSensor)
        {
            this.currentGroundInfo = new CollisionInfoData(monitorGroundSensor, SensorHitDirectionEnum.Both);

            float angleInRadians = General.Vector2ToAngle(monitorGroundSensor.normal);
            position.y = monitorGroundSensor.point.y + (this.monitorHalfHeight * Mathf.Cos(this.currentGroundInfo.GetAngleInDegrees()));

            //As long as the object collided with is not the player  solid box the monitor is sucessfully grounded
            if (monitorGroundSensor.transform.gameObject.layer != this.solidBoxLayer)
            {
                this.grounded = true;
                this.monitorVelocity.y = 0;
            }

            SolidContactGimmick solidContactGimmick = monitorGroundSensor.transform.GetComponent<SolidContactGimmick>();

            if (solidContactGimmick != null && solidContactGimmick.solidGimmickType == SolidContactGimmickType.MovingPlatform) //Contact with a moving platform
            {
                this.transform.parent = solidContactGimmick.transform;
            }
        }
        else
        {
            this.currentGroundInfo = new CollisionInfoData();
        }


        this.transform.position = position;

        return monitorGroundSensor;
    }
    /// <summary>
    /// Nudges the monitor upwards and sets a flag to inform the controller that velocity based movement has begun
    /// </summary>
    private void MonitorBounce()
    {
        this.bumped = true;
        this.grounded = false;
        this.bumpedBefore = true;
        this.monitorVelocity.y = this.monitorBounceHeight;

        if (this.transform.parent != null)
        {
            this.transform.parent = null;
        }
    }

    /// <summary>
    /// Get the hiddenmonitor value
    /// </summary>
    public bool GetIsHiddenMonitor() => this.hiddenMonitor;

    /// <summary>
    /// Flag the monitor as a hidden one
    /// </summary>
    public void SetIsHiddenMonitor(bool hiddenMonitor) => this.hiddenMonitor = hiddenMonitor;

    /// <summary>
    /// Get the bumped status of the monitor
    /// </summary>
    public bool GetBumped() => this.bumped;

    /// <summary>
    /// Set the bumped status of the monitor
    /// </summary>
    public void SetBumped(bool bumped)
    {
        if (bumped && this.bumpedBefore == false)
        {
            this.bumpedBefore = true;
        }

        this.bumped = bumped;
    }

    /// <summary>
    /// Set the grounded value of the monitor
    /// </summary>
    public void SetGrounded(bool grounded) => this.grounded = grounded;

    /// <summary>
    /// Get the grounded value of the monitor
    /// </summary>
    public bool GetGrounded(bool grounded) => this.grounded;

    /// <summary>
    /// Get the monitor Icon controller
    /// </summary>
    public MonitorIconController GetMonitorIconController() => this.monitorController;

    /// <summary>
    /// Get the power up the monitor will grant when broken
    /// </summary>
    public PowerUp GetPowerUpToGrant() => this.powerUpToGrant;


    /// <summary>
    /// Destroy/Deactivate the monitor by  deactivting its colliders, unchilding the icon and spawning an explosion
    /// </summary>
    private void DestroyMonitor()
    {
        this.boxCollider2D.enabled = false;//Turn the collider off so the sensors can no longer collide with it
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.MonitorExplosion, this.transform.position);
        this.monitorController.enabled = true;
        this.monitorController.transform.parent = null;//Unchild the monitor icon because this object is about to be deactivated
        Vector2 brokenMonitorSpawnPosition = this.transform.position + (Vector3)this.brokenMonitorPositionOffset;
        GameObject brokenMonitor = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BrokenMonitor, brokenMonitorSpawnPosition);
        BrokenMonitorController brokenMonitorController = brokenMonitor.GetComponent<BrokenMonitorController>();
        brokenMonitorController.enabled = !(this.grounded == false && this.bumpedBefore == false);//If the monitor has never been bumped or grounded disable the script so it doesnt fall

        if (brokenMonitorController.enabled == false)
        {
            brokenMonitor.transform.position = this.transform.position;
        }

        if (this.transform.parent != null)
        {
            brokenMonitor.transform.parent = this.transform.parent;
        }

        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BrokenMonitorShardSet, this.transform.position);
        GMAudioManager.Instance().PlayOneShot(this.destroyMonitorSound);
        this.gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            Gizmos.color = this.debugColor;
            Vector2 position = this.transform.position;
            Gizmos.DrawLine(position, position - new Vector2(0, this.monitorHalfHeight));
        }
    }

}
