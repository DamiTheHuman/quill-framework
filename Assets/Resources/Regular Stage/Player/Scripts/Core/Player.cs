using System.Collections;
using UnityEngine;
/// <summary>
/// The main player class where movement is calculated and controlled (not performed see Sensors.cs)
/// </summary>
[RequireComponent(typeof(Sensors), typeof(InputManager), typeof(AnimatorManager))]
public class Player : MonoBehaviour
{
    [Help("This determins the main state of the player and how interactive they can be by user input")]
    [SerializeField, Tooltip("Sets whether the player object should be uninteractive")]
    private PlayerState playerState = PlayerState.Awake;
    [Help("You can double click the field to change the player speeds while in base form or attach your own")]
    [SerializeField]
    private CharacterPhysicsScriptableObject physicsInfo = null;
    [Help("The superform version of Physcs Info")]
    [SerializeField]
    private CharacterPhysicsScriptableObject superPhysicsInfo = null;
    [SerializeField]
    /// <summary>
    /// The action audio for the player
    /// </summary>
    private PlayerActionAudio playerActionAudio = null;
    [FirstFoldOutItem("Dependencies")]
    [SerializeField]
    private Sensors sensors;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private SpriteController spriteController;
    [SerializeField]
    private AnimatorManager animatorManager;
    [SerializeField]
    private ActionManager actionManager;
    [SerializeField]
    private SolidBoxController solidBoxController;
    [SerializeField]
    private HitBoxController hitBoxController;
    [SerializeField]
    private HealthManager healthManager;
    [SerializeField]
    private OxygenManager oxygenManager;
    [SerializeField]
    private GimmickManager gimmickManager;
    [SerializeField]
    private SpriteEffectsController spriteEffectsController;
    [SerializeField]
    private FXPaletteController paletteController;
    [SerializeField]
    private PowerUpManager hedgePowerUpManager;
    [SerializeField]
    private AfterImageController afterImageController;
    [SerializeField, LastFoldoutItem()]
    private DustPuffController dustPuffController;

    [Tooltip("The speed the player is moving at horizontally and vertically"), FirstFoldOutItem("Active Velocities")]
    public Vector2 velocity;
    [Tooltip("The speed the player is moving at on the ground"), LastFoldoutItem]
    public float groundVelocity;

    [Tooltip("The players angle on the ground"), FirstFoldOutItem("General Player Information")]
    public float playerAngle;
    //Player States
    [Tooltip("Whether the player is grounded or not")]
    [SerializeField]
    private bool grounded;
    public GroundMode currentGroundMode = GroundMode.Floor;
    public MovementRestriction movementRestriction = MovementRestriction.None;

    [SerializeField]
    private bool attacking = false;
    [Tooltip("The current direction the player is facing "), DirectionList, LastFoldoutItem]
    public int currentPlayerDirection = 1;

    [FirstFoldOutItem("Physics State"), SerializeField]
    private PhysicsState physicsState = PhysicsState.Basic;
    [Tooltip("Current Deceleration applied to the players speed")]
    [FirstFoldOutItem("Current - Ground Movement"), SerializeField]
    public float currentDeceleration;
    [Tooltip("Current acceleration applied to the players speed")]
    [SerializeField]
    public float currentAcceleration;
    [SerializeField]
    public float currentFriction;
    [SerializeField, LastFoldoutItem()]
    public float currentTopSpeed;

    [SerializeField, FirstFoldOutItem("Current - Air Movement")]
    public float currentAirAcceleration;
    private float currentRollingFriction;
    private float currentRollingDeceleration;
    [SerializeField, LastFoldoutItem]
    public float currentGravity;

    [SerializeField, FirstFoldOutItem("Current - Jump Variables")]
    public float currentJumpVelocity;
    [SerializeField, LastFoldoutItem]
    public float currentJumpReleaseThreshold;

    [FirstFoldOutItem("Current - Roll Variables"), LastFoldoutItem()]
    public float currentMinThreshold;

    [Tooltip(" Extra speed on ground contact depending on the angle"), FirstFoldOutItem("Landing Conversion Factors")]
    public float landingConversionFactor = 1f;
    [Tooltip("Extra speed on ground contact depending on the angle when rolling"), LastFoldoutItem]
    public float rollingLandingConversionFactor = 0.7f;
    [Tooltip("How long to lock the players controls in m/s")]
    [Help("Please input the timer in steps - (timeInSeconds * 59.9999995313f)"), FirstFoldOutItem("Horizontal Control Lock")]
    public float horizontalControlLockTime = 30f;
    [Tooltip("Check if the horizontal cotrol lock is active"), LastFoldoutItem]
    public bool horizontalControlLock = false;
    private IEnumerator horizontalControlLockCoroutine;
    [SerializeField, FirstFoldOutItem("Debug Post Gravity Multipliers")]
    private bool applyWaterExit;
    [SerializeField, LastFoldoutItem]
    private bool applyWaterEntry;
    [Tooltip("Prepares the victory animation for when the player lands"), SerializeField, FirstFoldOutItem("Extra")]
    private bool beginVictoryActionOnGroundContact;

    private void Reset()
    {
        this.sensors = this.GetComponent<Sensors>();
        this.inputManager = this.GetComponent<InputManager>();
        this.animatorManager = this.GetComponent<AnimatorManager>();
        this.actionManager = this.GetComponent<ActionManager>();
        this.hedgePowerUpManager = this.GetComponent<PowerUpManager>();
        this.gimmickManager = this.GetComponent<GimmickManager>();
        this.spriteController = this.GetComponentInChildren<SpriteController>();
        this.solidBoxController = this.GetComponentInChildren<SolidBoxController>();
        this.spriteEffectsController = this.GetComponentInChildren<SpriteEffectsController>();
        this.afterImageController = this.GetComponentInChildren<AfterImageController>();
        this.dustPuffController = this.GetComponentInChildren<DustPuffController>();
        this.healthManager = this.GetComponent<HealthManager>();
        this.hitBoxController = this.GetComponentInChildren<HitBoxController>();
        this.oxygenManager = this.GetComponent<OxygenManager>();
        this.paletteController = this.GetComponentInChildren<FXPaletteController>();
    }

    public void Start()
    {
        if (this.sensors == null)
        {
            this.Reset();
        }

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.RegularStage && this.GetPlayerState() == PlayerState.Awake)
        {
            Debug.Log("Forcing player to sleep as current scene is not a stage");
            this.SetPlayerState(PlayerState.Sleep);
        }

        if (this.GetPlayerState() == PlayerState.Sleep)
        {
            this.gameObject.SetActive(false);

            return;
        }

        this.sensors.MoveAndCollide(Vector2.zero);
        this.OnStart();
    }
    /// <summary>
    /// Sets the player position to the ground if available and sets the camera on the player
    /// </summary>
    public void OnStart()
    {
        if (HedgehogCamera.Instance().GetCameraTarget() == null)
        {
            HedgehogCamera.Instance().SetCameraTarget(this.gameObject);
        }

        GMStageManager.Instance().OnPlayerInitialize();//Inform the GM the player has been initialized
        GMStageHUDManager.Instance().SetLifeIconMaterial(this.GetSpriteController().GetSpriteRenderer().sharedMaterial);//update the HUD material to match the player
    }

    private void Update()
    {
        if (this.GetPlayerState() == PlayerState.Sleep)
        {
            this.gameObject.SetActive(false);
            return;
        }
    }
    public void FixedUpdate()
    {
        if (this.GetPlayerState() == PlayerState.Sleep)
        {
            return;
        }

        this.ClampVelocityApproximations();
        this.UpdatePhysicsValues();
        this.GetHitBoxController().CheckHitBoxCollisions();
        this.sensors.MoveAndCollide(this.velocity);//Actually moves the player and performs solid box (for gimmicks) and sensor collision

        if (this.GetPlayerState() == PlayerState.Awake)
        {
            this.GetHealthManager().CheckForBelowBoundaryDeath();
        }

        this.GetAfterImageController().UpdateRecentObjectPosition(this.transform.position, this.GetSpriteController().GetSpriteAngle());//Update the positions of the player

        if (this.gimmickManager.GetActiveGimmickMode() == GimmickMode.OnCorkscrew)
        {
            this.SetGrounded(true);
        }

        this.animatorManager.UpdatePlayerAnimations();
        this.UpdatePlayerGroundMode();
        this.spriteController.UpdatePlayerSpriteAngle(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees());

        if (this.GetGrounded())
        {
            this.CalculateGroundMovement();
        }
        else
        {
            this.CalculateAirMovement();
        }

        if (this.applyWaterEntry || this.applyWaterExit)
        {
            this.ApplyPostGravityMultipliers();
        }

        this.velocity = this.LimitVelocity(this.velocity);
        this.UpdatePlayerDirection();

        if (this.beginVictoryActionOnGroundContact && this.grounded)
        {
            this.actionManager.PerformAction<Victory>();
        }

        this.actionManager.UpdateActionLifeCycle();
        this.CheckHorizontalControlLock();
        this.CheckShields();
        this.spriteController.CallSpriteAddOnLifeCycle();
        this.GetAfterImageController().UpdateAfterImages();
        this.GetDustPuffController().CheckToSpawnDustPuffs();


        if (this.gimmickManager.GetActiveGimmickMode() == GimmickMode.OnHandle)
        {
            this.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Begin the start health update coroutine
    /// </summary>
    public void StartDeathUpdate()
    {
        this.currentGravity = this.physicsInfo.basicGravity;
        this.StartCoroutine(this.UnscaledFixedUpdate());
    }

    /// <summary>
    /// A custom fixed update for when the player is dead 
    /// This allows movement while everything else is frozen in place
    /// </summary>
    private IEnumerator UnscaledFixedUpdate()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (Time.timeScale != 0)
            {
                break;
            }
            this.sensors.MoveAndCollide(this.velocity);
            this.CalculateAirMovement();
            this.velocity = this.LimitVelocity(this.velocity);
            this.actionManager.UpdateActionLifeCycle();

            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    /// <summary>
    /// Updates the physics values of the player object based on their specified conditions
    /// </summary>
    private void UpdatePhysicsValues()
    {
        bool hasPowerSneakers = this.GetHedgePowerUpManager().GetPowerSneakersPowerUp().PowerUpIsActive();
        CharacterPhysicsScriptableObject currentPhysicsInfo = this.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm ? this.superPhysicsInfo : this.physicsInfo;
        this.currentDeceleration = this.actionManager.CheckActionIsBeingPerformed<Roll>() ? currentPhysicsInfo.basicRollingDeceleration : currentPhysicsInfo.basicDeceleration;
        this.currentAcceleration = this.actionManager.CheckActionIsBeingPerformed<Roll>() ? 0 : currentPhysicsInfo.basicAcceleration;
        this.currentFriction = this.actionManager.CheckActionIsBeingPerformed<Roll>() ? currentPhysicsInfo.basicRollingFriction : currentPhysicsInfo.basicFriction;
        this.currentAirAcceleration = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicAirAcceleration : currentPhysicsInfo.underwaterAirAcceleration;
        this.currentGravity = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicGravity : currentPhysicsInfo.underwaterGravity;
        this.currentGravity = this.actionManager.CheckActionIsBeingPerformed<Hurt>() ? this.physicsInfo.knockBackGravity : this.currentGravity;
        this.currentJumpVelocity = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicJumpVelocity : currentPhysicsInfo.underwaterJumpVelocity;
        this.currentJumpReleaseThreshold = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicJumpReleaseThreshold : currentPhysicsInfo.underwateJumpReleaseThreshold;
        this.currentRollingFriction = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicRollingFriction : currentPhysicsInfo.underwaterRollingFriction;
        this.currentRollingDeceleration = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicRollingDeceleration : currentPhysicsInfo.underwaterRollingDeceleration;
        this.currentTopSpeed = this.physicsState == PhysicsState.Basic ? currentPhysicsInfo.basicTopSpeed : currentPhysicsInfo.underwaterTopSpeed;
        this.currentMinThreshold = currentPhysicsInfo.basicMinRollThreshold;
        //Power Sneakers active and not underwater or super
        if (hasPowerSneakers && this.physicsState != PhysicsState.Underwater && this.GetHedgePowerUpManager().GetSuperPowerUp() != SuperPowerUp.SuperForm)
        {
            this.currentAcceleration = currentPhysicsInfo.speedShoesAcceleration;
            this.currentDeceleration = currentPhysicsInfo.speedShoesDeceleration;
            this.currentFriction = currentPhysicsInfo.speedShoesFriction;
            this.currentTopSpeed = currentPhysicsInfo.speedShoesTopSpeed;
            this.currentAirAcceleration = currentPhysicsInfo.speedShoesAirAcceleration;
            this.currentRollingFriction = currentPhysicsInfo.speedShoesRollingFriction;
            this.currentRollingDeceleration = currentPhysicsInfo.speedShoesRollingDeceleration;
        }

        if (this.actionManager.CheckActionIsBeingPerformed<Fly>())
        {
            Fly fly = this.GetActionManager().GetAction<Fly>() as Fly;
            this.currentGravity = fly.CalculateFlightGravity();
        }

        if (this.actionManager.CheckActionIsBeingPerformed<Glide>())
        {
            this.currentAcceleration = currentPhysicsInfo.glideAcceleration;
            this.currentGravity = currentPhysicsInfo.glideGravity;
            this.currentFriction = currentPhysicsInfo.glideSlideFriction;
        }
    }

    /// <summary>
    /// Calculate the ground movement of the player based on input, terrain and other factors
    /// </summary>
    private void CalculateGroundMovement()
    {
        this.ApplySlopeFactor();
        this.ApplyDeceleration();
        this.ApplyAcceleration();
        this.ApplyFriction();
        this.groundVelocity = this.LimitGroundVelocity(this.groundVelocity);
        this.velocity = this.CalculateSlopeMovement(this.groundVelocity);
    }
    /// <summary>
    /// Calculate the air movement of the player based on drag, gravity and other factors
    /// </summary>
    private void CalculateAirMovement()
    {
        if (this.GetActionManager().CheckActionIsBeingPerformed<Glide>())
        {
            Glide glide = this.GetActionManager().GetAction<Glide>() as Glide;
            glide.ApplyAirHorizontalMovement();
            glide.ApplyGravity(this.ApplyGravity);
            glide.LimitGlideVelocity();

            return;
        }
        else if (this.GetActionManager().CheckActionIsBeingPerformed<Climb>())
        {
            Climb climb = this.GetActionManager().GetAction<Climb>() as Climb;

            if (climb.GetClimbState() != ClimbState.Drop)
            {
                return;
            }
        }

        this.ApplyAirHorizontalMovement();
        this.ApplyAirDrag();

        if (this.actionManager.CheckActionIsBeingPerformed<HomingAttack>())
        {
            HomingAttack homingAttack = this.GetActionManager().GetAction<HomingAttack>() as HomingAttack;
            if (homingAttack.GetHomingAttackMode() == HomingAttackMode.Homing)
            {
                return;
            }
        }

        this.ApplyGravity();

    }
    /// <summary>
    /// Apply slope factor to the players ground velocity pushing them down steep slopes where possible
    /// </summary>
    private void ApplySlopeFactor()
    {
        bool walkingOnSlope = Mathf.Round(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees()) is >= 45 and <= 315;
        float currentSlopeFactor = this.physicsInfo.slopeFactor;

        if (walkingOnSlope || this.horizontalControlLock)
        {
            if (this.actionManager.CheckActionIsBeingPerformed<Roll>())
            {
                float sinAngle = Mathf.Sin(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians());
                bool goingUphill = (sinAngle >= 0f && this.groundVelocity >= 0f) || (sinAngle <= 0f && this.groundVelocity <= 0);
                currentSlopeFactor = goingUphill ? this.physicsInfo.basicSlopeRollUpwards : this.physicsInfo.basicSlopeRollDownards;
            }

            currentSlopeFactor = GMStageManager.Instance().ConvertToDeltaValue(currentSlopeFactor);
            this.groundVelocity -= currentSlopeFactor * Mathf.Sin(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians()); //apply the slope factor
        }
    }

    /// <summary>
    /// Reduce the players ground velocity based on the current decelerationValue
    /// </summary>
    private void ApplyDeceleration()
    {
        float deltaDeceleration = GMStageManager.Instance().ConvertToDeltaValue(this.currentDeceleration);

        if (this.inputManager.GetCurrentInput().x == 1)
        {
            if (this.groundVelocity < 0)
            {
                this.groundVelocity += deltaDeceleration;
                if (this.groundVelocity >= 0)
                {
                    this.groundVelocity = this.currentDeceleration;
                }
            }
        }
        else if (this.inputManager.GetCurrentInput().x == -1)
        {
            if (this.groundVelocity > 0)
            {
                this.groundVelocity -= deltaDeceleration;
                if (this.groundVelocity <= 0)
                {
                    this.groundVelocity = -this.currentDeceleration;
                }
            }
        }
    }

    /// <summary>
    /// Increase the players ground velocity based on the current acceleration value
    /// </summary>
    private void ApplyAcceleration()
    {
        //While rolling no acceleration can be applied
        if (this.actionManager.CheckActionIsBeingPerformed<Roll>())
        {
            return;
        }

        this.groundVelocity = this.IncrementHorizontalVelocity(this.groundVelocity, this.currentAcceleration, this.currentTopSpeed);
    }

    /// <summary>
    /// Apply friction to the players ground velocity based on the set conditions
    /// </summary>
    private void ApplyFriction()
    {
        float deltaFriction = GMStageManager.Instance().ConvertToDeltaValue(this.currentFriction);

        if (this.inputManager.GetCurrentInput().x == 0 || this.actionManager.CheckActionIsBeingPerformed<Roll>())
        {
            //When no input is found friction is subtracted from ground velocity based on the sign of ground velocity
            this.groundVelocity -= Mathf.Min(Mathf.Abs(this.groundVelocity), deltaFriction) * Mathf.Sign(this.groundVelocity);

        }
    }

    /// <summary>
    /// Limits the players ground velocity
    /// <param name="currentGroundVelocity">The speed the player is moving at on the ground currently</param>
    /// </summary>
    public float LimitGroundVelocity(float currentGroundVelocity) => Mathf.Clamp(currentGroundVelocity, -this.physicsInfo.velocityLimits.x, this.physicsInfo.velocityLimits.x);

    /// <summary>
    /// Converts the ground velocity to the actual velocity applied taking slopes into account
    /// <param name="currentGroundVelocity">The speed the player is moving at on the ground currently</param>
    /// </summary>
    public Vector2 CalculateSlopeMovement(float currentGroundVelocity)
    {
        if ((this.gimmickManager.GetActiveGimmickMode() == GimmickMode.OnCorkscrew) && this.gimmickManager.GetGimmickAngle() != 0)
        {
            this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().SetAngleInDegrees(this.gimmickManager.GetGimmickAngle());
        }

        return this.SlopeFormula(currentGroundVelocity, this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians());
    }

    /// <summary>
    /// Formula for calculatng slopes based on the velocity and angle passed 
    /// <param name="currentGroundVelocity">The speed the player is moving at on the ground currently </param>
    /// <param name="angleInRadians">The angle of the ground in radians </param>
    /// </summary>
    public Vector2 SlopeFormula(float currentGroundVelocity, float angleInRadians)
    {
        Vector2 newVelocity;
        newVelocity.x = currentGroundVelocity * Mathf.Cos(angleInRadians);
        newVelocity.y = currentGroundVelocity * Mathf.Sin(angleInRadians);

        return newVelocity;
    }


    /// <summary>
    /// Check whether to lock the players controls when they slide off a slope
    /// </summary>
    private void CheckHorizontalControlLock()
    {
        float roundedAngle = Mathf.Round(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees());

        if (Mathf.Abs(this.groundVelocity) < this.physicsInfo.basicFall && this.currentGroundMode != 0 && this.gimmickManager.GetActiveGimmickMode() != GimmickMode.OnCorkscrew)
        {
            if (this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetHit() && roundedAngle >= 90 && roundedAngle <= 270)
            {
                this.SetGrounded(false);//Unground The player
                this.SetGroundMode(0);//Return the player back to the grounded ground mode
                this.groundVelocity = 0;//Set the ground speed back to 0
            }

            this.BeginHorizontalControlLockCoroutine();
        }
    }

    /// <summary>
    /// Apply acceleration to the player body while in the air
    /// </summary>
    private void ApplyAirHorizontalMovement() => this.velocity.x = this.IncrementHorizontalVelocity(this.velocity.x, this.currentAirAcceleration, this.currentTopSpeed);

    /// <summary>
    /// Increments horizontal velocity based on the parameters passed and the users input
    /// This calculation is used for both air horizontal velocity(<see cref="velocity.x"/>) and ground velocity (<see cref="groundVelocity"/>)
    /// <param name="horizontalVelocity"> The current horizontal velocity to update </param>
    /// <param name="acceleration"> The acceleration used to increment the horizontal velocity </param>
    /// <param name="topSpeed"> The highest velocity that can be attained while using the input to increment speed as long as the top speed isnt exceeded </param>
    /// </summary>
    private float IncrementHorizontalVelocity(float horizontalVelocity, float acceleration, float topSpeed)
    {
        acceleration = GMStageManager.Instance().ConvertToDeltaValue(acceleration);

        if (this.inputManager.GetCurrentInput().x == 1) //As long as the player is not going above top speed i.e by running down a slope already apply acceleration
        {
            if ((horizontalVelocity < topSpeed) && (horizontalVelocity >= 0 || this.GetGrounded() == false))
            {
                horizontalVelocity += acceleration;

                if (horizontalVelocity >= topSpeed)
                {
                    horizontalVelocity = topSpeed;
                }
            }

        }
        else if (this.inputManager.GetCurrentInput().x == -1) //As long as the player is not going above top speed i.e by running down a slope already apply acceleration
        {
            if ((horizontalVelocity > -topSpeed) && (horizontalVelocity <= 0 || this.GetGrounded() == false))
            {
                horizontalVelocity -= acceleration;

                if (horizontalVelocity <= -topSpeed)
                {
                    horizontalVelocity = -topSpeed;
                }
            }
        }

        return horizontalVelocity;
    }

    /// <summary>
    /// Apply air drag tot he players air horizontal movement based on the following conditions:
    /// First Y must be negative
    /// Second velocity.y must be less than airDragConditions.y
    ///  Lastly velocity.x must be greater than airDragConditions.x
    /// </summary>
    private void ApplyAirDrag()
    {
        if (this.velocity.y > 0f && this.velocity.y < this.physicsInfo.basicAirDragCondition.y)
        {
            if (Mathf.Abs(this.velocity.x) >= this.physicsInfo.basicAirDragCondition.x)
            {
                this.velocity.x *= this.physicsInfo.basicAirDrag;
            }
        }
    }

    /// <summary>
    /// Apply a vertical force that moves the player towards the ground 
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.currentGravity);//Apply Gravity

    /// <summary>
    /// Applies post gravity multipliers based on whether the player entered or exited water
    /// As stated by the SPG guide this function should be called after gravity is applied
    /// </summary>
    private void ApplyPostGravityMultipliers()
    {
        if (this.applyWaterEntry)
        {
            this.ApplyWaterEntryMultiplier();
        }

        if (this.applyWaterExit)
        {
            this.ApplyWaterExitMultipler();
            this.GetOxygenManager().ReplenishPlayerBreath();
        }
    }

    /// <summary>
    /// Limit the players velocity against the characters respective velocity limit relative to the terrain
    /// <param name="currentVelocity">The speed the player is moving at horizontally and vertically currently </param>
    /// </summary>
    public Vector2 LimitVelocity(Vector2 currentVelocity)
    {
        Vector2 limits = this.CalculateSlopeMovement(this.physicsInfo.velocityLimits.x);
        limits.x = this.physicsInfo.velocityLimits.x + Mathf.Cos(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians());
        limits.y = this.physicsInfo.velocityLimits.y + Mathf.Sin(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians());
        currentVelocity.x = Mathf.Clamp(currentVelocity.x, -limits.x, limits.x);
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, -limits.y, limits.y);

        if (Mathf.Abs(currentVelocity.x) <= 0.0001)
        {
            currentVelocity.x = 0;
        }
        if (Mathf.Abs(currentVelocity.y) <= 0.0001)
        {
            currentVelocity.y = 0;
        }

        return currentVelocity;
    }

    /// <summary>
    /// Get the grounded state
    /// </summary>
    public bool GetGrounded() => this.grounded;

    /// <summary>
    /// Set the grounded state
    /// <param name="grounded"> The flag for the grounded state</param>
    /// </summary>
    public void SetGrounded(bool grounded)
    {
        if (grounded == false)
        {
            this.sensors.groundCollisionInfo.Reset();
            this.currentGroundMode = GroundMode.Floor;
        }

        this.grounded = grounded;
    }

    /// <summary>
    /// Updates the player direction based on the the players velocity
    /// <param name="forceDirectionUpdate"> A flag to force the players direction to update regardless of the players state</param>
    /// </summary>
    public void UpdatePlayerDirection(bool forceDirectionUpdate = false)
    {
        if (this.inputManager.GetCurrentInput().x == this.GetHorizontalVelocityDirection() || this.GetGrounded() == false || forceDirectionUpdate)
        {
            if ((this.GetHorizontalVelocity() != 0 && this.GetGrounded()) || this.GetGrounded() == false)
            {
                //When grounded use the horizontal velocity to verify the flip
                if (this.GetGrounded() || forceDirectionUpdate)
                {
                    this.currentPlayerDirection = this.GetHorizontalVelocityDirection();
                }
                //When un grounded and the users input is not 0 flip based on the users input
                else if (this.inputManager.GetCurrentInput().x != 0)
                {
                    this.currentPlayerDirection = (int)this.inputManager.GetCurrentInput().x;
                }
                else if (this.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.InTube && this.GetHorizontalVelocityDirection() != 0)
                {
                    this.currentPlayerDirection = this.GetHorizontalVelocityDirection();
                }
                //Flip the player based on the current player direction
                if (this.spriteController.GetSpriteDirection() != this.currentPlayerDirection)
                {
                    this.spriteController.SetSpriteDirection(this.currentPlayerDirection);
                }
            }
        }
    }

    /// <summary>
    /// Sets the current players direction value
    /// </summary>
    public void SetPlayerDirection(int direction)
    {
        this.currentPlayerDirection = direction;
        this.GetSpriteController().SetSpriteDirection(this.currentPlayerDirection);
    }

    /// <summary>
    /// Check if the use of shields are allowed based on the players state
    /// </summary>
    private void CheckShields()
    {
        if (this.physicsState == PhysicsState.Underwater && this.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.None)
        {
            if (!this.GetHedgePowerUpManager().GetShieldPowerUp().CheckIfShieldCanBeUsedUnderwater())
            {
                this.GetHedgePowerUpManager().GetShieldPowerUp().RemovePowerUp();
            }
        }
    }

    /// <summary>
    /// Limits the players position to the horizontal boundaries of the stage
    /// </summary>
    public void ClampPositionToStageBounds()
    {
        Vector2 position = this.transform.position;
        CameraBounds stageBounds = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActiveStageBounds();

        if (position.x - this.sensors.characterBuild.pushRadius < stageBounds.GetLeftBorderPosition() && this.GetHorizontalVelocity() <= 0)
        {
            position.x = stageBounds.GetLeftBorderPosition() + this.sensors.characterBuild.bodyWidthRadius;
            this.SetHorizontalVelocity(0);
        }
        else if (position.x + this.sensors.characterBuild.pushRadius > stageBounds.GetRightBorderPosition() && this.GetHorizontalVelocity() >= 0)
        {
            position.x = stageBounds.GetRightBorderPosition() - this.sensors.characterBuild.bodyWidthRadius;
            this.SetHorizontalVelocity(0);
        }

        this.transform.position = position;
    }


    /// <summary>
    /// Checks if there is abocve the top camera bounds
    /// </summary>
    public bool PlayerIsAboveTopBounds()
    {
        Vector2 position = this.transform.position;
        CameraBounds stageBounds = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActiveStageBounds();

        return position.y + this.sensors.characterBuild.bodyHeightRadius > stageBounds.GetTopBorderPosition();
    }

    /// <summary>
    /// Due to floating point precision approximate the velocities
    /// </summary>
    public void ClampVelocityApproximations()
    {
        this.velocity.x = Mathf.Approximately(this.velocity.x, 0) ? 0 : this.velocity.x;
        this.velocity.y = Mathf.Approximately(this.velocity.y, 0) ? 0 : this.velocity.y;
        this.groundVelocity = Mathf.Approximately(this.groundVelocity, 0) ? 0 : this.groundVelocity;
    }

    /// <summary>
    /// Begins the HorizontalControlLock Coroutine
    /// </summary>
    private void BeginHorizontalControlLockCoroutine()
    {
        if (this.horizontalControlLockCoroutine != null)
        {
            this.StopCoroutine(this.horizontalControlLockCoroutine);
        }

        this.horizontalControlLockCoroutine = this.UpdateHorizontalControlLock();
        this.StartCoroutine(this.horizontalControlLockCoroutine);//Begin the lock
    }

    /// <summary>
    /// Locks the playeres directional input for the specified time
    /// </summary>
    private IEnumerator UpdateHorizontalControlLock()
    {
        this.inputManager.SetInputRestriction(InputRestriction.XAxis);
        this.horizontalControlLock = true;

        yield return new WaitUntil(() => this.grounded);//Ensures the lock timer does not start till the player is grounded
        yield return new WaitForSeconds(General.StepsToSeconds(this.horizontalControlLockTime));
        this.inputManager.SetInputRestriction(InputRestriction.None);
        this.horizontalControlLock = false;
    }

    /// <summary>
    /// Updates the player ground mode based on the current ground info
    /// </summary>
    public void UpdatePlayerGroundMode()
    {
        int mode = this.CalculateGroundMode(this.sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees(), 90);
        this.SetGroundMode((GroundMode)mode);
    }

    /// <summary>
    /// Calculation to find the currect ground mode of the player
    ///EXAMPLE 1 : MODE KEYS when divider is 90 and range is 4 Floor = 0 ,RightWall = 1 ,Ceiling = 2 ,LeftWall = 3 
    /// <param name="angle"> The current angle</param>
    /// <param name="divider"> How many possible dividents from a range of 360</param>
    /// </summary>
    public int CalculateGroundMode(float angle, float divider = 90)
    {
        float range = 360 / divider;
        float mode = Mathf.Round(angle / divider) % range;

        return (int)mode;
    }

    /// <summary>
    /// Get the ground mode
    /// </summary>
    public GroundMode GetGroundMode() => this.currentGroundMode;

    /// <summary>
    /// Set the ground mode
    /// <param name="groundMode"> The new ground mode of the player </param>
    /// </summary>
    public void SetGroundMode(GroundMode groundMode) => this.currentGroundMode = groundMode;

    /// <summary>
    /// Return the direction the horizontal velocity is going in
    /// </summary>
    public int GetHorizontalVelocityDirection() => this.GetHorizontalVelocity() != 0 ? (int)Mathf.Sign(this.GetHorizontalVelocity()) : 0;

    /// <summary>
    /// Return the horizontal velocity based on the players grounded status
    /// </summary>
    public float GetHorizontalVelocity() => this.grounded ? this.groundVelocity : this.velocity.x;

    /// <summary>
    /// Set the players horizontal velocity based on the players grounded status
    /// <param name="horizontalVelocity"> The amount of velocity to be applied </param>
    /// </summary>
    public void SetHorizontalVelocity(float horizontalVelocity)
    {
        if (this.GetGrounded())
        {
            this.groundVelocity = horizontalVelocity;
        }
        else
        {
            this.velocity.x = horizontalVelocity;
        }
    }
    /// <summary>
    /// Sets both the players horizontal velocity to the same value
    /// <param name="horizontalVelocity"> The amount of velocity to be applied </param>
    /// </summary>
    public void SetBothHorizontalVelocities(float horizontalVelocity)
    {
        this.groundVelocity = horizontalVelocity;
        this.velocity.x = horizontalVelocity;
    }

    /// <summary>
    /// Rebound the players velocity after performing an attack
    /// </summary>
    public void AttackRebound()
    {
        if (this.velocity.y < 0)
        {
            this.velocity.y = Mathf.Abs(this.velocity.y);
        }

        this.SetGrounded(false);
    }

    /// <summary>
    /// Set the attacking of the player
    /// <param name="attacking"> the new attacking state of the player</param>
    /// </summary>
    public void SetAttackingState(bool attacking) => this.attacking = attacking;

    /// <summary>
    /// Get the attacking of the player
    /// </summary>
    public bool GetAttackingState() => this.attacking;

    /// <summary>
    /// Set the physics state of the player
    /// <param name="physicsState"> the new physics state of the player </param>
    /// </summary>
    public void SetPhysicsState(PhysicsState physicsState) => this.physicsState = physicsState;

    /// <summary>
    /// Get the current physics state
    /// </summary>
    public PhysicsState GetPhysicsState() => this.physicsState;

    /// <summary>
    /// Sets the players velocity based on the currently active movement restriction
    /// <param name="velocity"> The current velocity of the player </param>
    /// </summary>
    public Vector2 ApplyMovementRestrictions(Vector2 velocity)
    {
        switch (this.movementRestriction)
        {
            case MovementRestriction.Horizontal:
                velocity.x = 0;

                break;
            case MovementRestriction.Vertical:
                velocity.y = 0;

                break;
            case MovementRestriction.Both:
                velocity = Vector2.zero;

                break;
            case MovementRestriction.None:

                break;
            default:
                break;
        }
        return velocity;
    }
    /// <summary>
    /// Sets the movement restriction to the value set
    /// <param name="movementRestriction"> The movement restriction to be set</param>
    /// </summary>
    public void SetMovementRestriction(MovementRestriction movementRestriction) => this.movementRestriction = movementRestriction;

    /// <summary>
    /// Returns the value of the current movement restriction
    /// </summary>
    public MovementRestriction GetMovementRestriction() => this.movementRestriction;

    /// <summary>
    /// Prepare to add the velocity that effects the player when they enter the water after gravity is applied
    /// </summary>
    public void PrepareWaterEntryMultiplier()
    {
        this.applyWaterEntry = true;

        if (this.applyWaterExit)
        {
            this.applyWaterExit = false;
        }
    }

    /// <summary>
    /// Prepare to add the velocity that effects the player when they exit the water after gravity is applied
    /// </summary>
    public void PrepareWaterExitMultiplier() => this.applyWaterExit = true;

    /// <summary>
    /// Apply the Water Entry Multiplier
    /// </summary>
    public void ApplyWaterEntryMultiplier()
    {
        this.velocity *= this.physicsInfo.underWaterEntryMultiplier;
        this.applyWaterEntry = false;
    }
    /// <summary>
    /// Apply the water exit multiplier
    /// </summary>
    public void ApplyWaterExitMultipler()
    {
        this.velocity *= this.physicsInfo.underWaterExitMultiplier;
        this.applyWaterExit = false;
    }

    /// <summary>
    /// Get the <see cref=">this.performVcitoryActionOnGrounded"/> variable
    /// </summary>
    public bool GetBeginVictoryActionOnGroundContac() => this.beginVictoryActionOnGroundContact;

    /// <summary>
    /// Sets the variable that makes the victory action become performed as soon as the player is grounded
    /// <param name="performVcitoryActionOnGrounded"> Begin the victory action on ground contact </param>
    /// </summary>
    public void SetBeginVictoryActionOnGroundContact(bool performVcitoryActionOnGrounded) => this.beginVictoryActionOnGroundContact = performVcitoryActionOnGrounded;


    /// <summary>
    /// Get a reference to the players sensors
    /// </summary>
    public Sensors GetSensors() => this.sensors;

    /// <summary>
    /// Get a reference to the players Input
    /// </summary>
    public InputManager GetInputManager() => this.inputManager;

    /// <summary>
    /// Get a reference to the players animator Manager
    /// </summary>
    public AnimatorManager GetAnimatorManager() => this.animatorManager;

    /// <summary>
    /// Get a reference to the players action Manager
    /// </summary>
    public ActionManager GetActionManager() => this.actionManager;

    /// <summary>
    /// Get a reference to the players gimmick data
    /// </summary>
    public GimmickManager GetGimmickManager() => this.gimmickManager;

    /// <summary>
    /// Get a reference to the players health manager
    /// </summary>
    public HealthManager GetHealthManager() => this.healthManager;

    /// <summary>
    /// Get a reference to the players oxygen
    /// </summary>
    public OxygenManager GetOxygenManager() => this.oxygenManager;

    /// <summary>
    /// Get a reference to the players sprite controller
    /// </summary>
    public SpriteController GetSpriteController() => this.spriteController;

    /// <summary>
    /// Get a reference to the players solid box controller
    /// </summary>
    public SolidBoxController GetSolidBoxController() => this.solidBoxController;

    /// <summary>
    /// Get a reference to the players hitbox controller
    /// </summary>
    public HitBoxController GetHitBoxController() => this.hitBoxController;

    /// <summary>
    /// Get a reference to the sprite effects controller
    /// </summary>
    public SpriteEffectsController GetSpriteEffectsController() => this.spriteEffectsController;

    /// <summary>
    /// Get a reference to the players afterimage controller
    /// </summary>
    public AfterImageController GetAfterImageController() => this.afterImageController;

    /// <summary>
    /// Get a reference to the players dust puff controller
    /// </summary>
    public DustPuffController GetDustPuffController() => this.dustPuffController;

    /// <summary>
    /// Get a reference to the palette controller
    /// </summary>
    public FXPaletteController GetPaletteController() => this.paletteController;

    /// <summary>
    /// A reference to the player power up manager
    /// </summary>
    public PowerUpManager GetHedgePowerUpManager() => this.hedgePowerUpManager;

    /// <summary>
    /// A reference to the physics setting depending on whether they are super or not
    /// </summary>
    public CharacterPhysicsScriptableObject GetPlayerPhysicsInfo() => this.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm ? this.superPhysicsInfo : this.physicsInfo;

    /// <summary>
    /// A reference to the physics setting relating to the player
    /// </summary>
    public CharacterPhysicsScriptableObject GetBasePhysicsInfo() => this.physicsInfo;

    /// <summary>
    /// Gets the super physics info of the player
    /// </summary/>
    public CharacterPhysicsScriptableObject GetSuperPhysicsInfo() => this.superPhysicsInfo;

    /// <summary>
    /// A reference to the audio files relating to the players actions
    /// </summary>
    public PlayerActionAudio GetPlayerActionAudio() => this.playerActionAudio;

    /// <summary>
    /// Sets the player state
    /// </summary/>
    public void SetPlayerState(PlayerState playerState)
    {
        //Renable the input master if it was disabled during a cutscen
        if (this.playerState == PlayerState.Cutscene && playerState != PlayerState.Cutscene)
        {
            this.inputManager.enabled = true;
            this.inputManager.SetInputRestriction(InputRestriction.None);
        }

        this.playerState = playerState;

        //Disable player input when in cutscene mode
        if (this.playerState == PlayerState.Cutscene)
        {
            this.inputManager.SetInputRestriction(InputRestriction.All);
            this.inputManager.enabled = false;
        }
    }

    /// <summary>
    /// Gets the player state
    /// </summary/>
    public PlayerState GetPlayerState() => this.playerState;

    /// <summary>
    /// Swaps the player data with another player object
    /// <param name="targetPlayerData">The player data to swap towards</param>
    /// </summary>
    public void SwapPlayerData(Player targetPlayerData)
    {
        this.physicsInfo = targetPlayerData.physicsInfo;
        this.superPhysicsInfo = targetPlayerData.superPhysicsInfo;

        if (targetPlayerData.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            this.GetHedgePowerUpManager().GoSuperForm();
        }
        else if (targetPlayerData.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.None)
        {
            if (this.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm)
            {
                this.GetHedgePowerUpManager().RevertSuperForm();
            }
        }

        this.UpdatePhysicsValues();

        if (this.spriteController.GetSpriteAddOnController() != null)
        {
            this.spriteController.GetSpriteAddOnController().gameObject.SetActive(false);
            this.spriteController.GetSpriteAddOnController().transform.parent = null;
            this.spriteController.SetSpriteAddOnController(null);
        }

        this.spriteController.CheckForAddOns();
    }
}
