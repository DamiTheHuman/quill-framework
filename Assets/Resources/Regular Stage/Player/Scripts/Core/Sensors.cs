using UnityEngine;
/// <summary>
/// This class handles collision and movement of the player objects takes place it is also where collision data is stored
/// </summary>
[RequireComponent(typeof(Player))]
public class Sensors : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [Help("You can double click this field to see info relating to the player sizes etc. These values also effect sensors placement and adjustements on contact")]
    public CharacterInfoScriptableObject characterBuild;
    [SerializeField]
    private SizeMode currentSizeMode = SizeMode.Regular;

    [Tooltip("The collision data retrieved from the ground sensors")]
    public GroundCollisionInfo groundCollisionInfo;
    [Tooltip("The collision data retrieved from the ceiling sensors")]
    public CeilingCollisionInfo ceilingCollisionInfo;
    [Tooltip("The collision data retrieved from the wall sensors")]
    public WallCollisionInfo wallCollisionInfo;

    [Tooltip("The position of the player in the last frame")]
    public Vector2 previousPosition;

    [FirstFoldOutItem("Current Character Information"), Tooltip("The current height of the character which changes when rolling")]
    public float currentHeight = 40f;
    [Tooltip("The current width of the character which changes when rolling")]
    public float currentWidth = 20f;
    [Tooltip("The current pixel pivot point of the player")]
    public float currentPixelPivotPoint = 15f;
    [Tooltip("The current body height radius of the character which changes when rolling")]
    public float currentBodyHeightRadius = 20f;
    [Tooltip("The current body width radius of the character which changes when rolling")]
    public float currentBodyWidthRadius = 8f;
    [Tooltip("The current push radius of the character which changes when on sloped ground")]
    public float currentPushRadius = 10;
    [LastFoldoutItem(), Tooltip("The current push radius offset which changes when on sloped ground")]
    public float currentPushRadiusOffset = 0;

    [SerializeField, FirstFoldOutItem("Low Ceiling")]
    private bool lowCeiling;
    [SerializeField, LastFoldoutItem()]
    private Color lowCeilingSensorColor = new Color(0.8207547f, 0.5497508f, 0.6113427f, 1);

    private void Reset() => this.player = this.GetComponent<Player>();

    // Start is called before the first frame update
    private void Start()
    {
        this.currentHeight = this.characterBuild.characterHeight;
        this.currentWidth = this.characterBuild.characterWidth;
        this.currentBodyHeightRadius = this.characterBuild.bodyHeightRadius;
        this.currentBodyWidthRadius = this.characterBuild.bodyWidthRadius;
        this.currentPushRadius = this.characterBuild.pushRadius;
        this.currentPushRadiusOffset = this.characterBuild.pushRadiusOffsetOnFlatGround;

        if (this.player == null)
        {
            this.Reset();
        }

        this.groundCollisionInfo.SetPlayer(this.player);
        this.wallCollisionInfo.SetPlayer(this.player);
        this.ceilingCollisionInfo.SetPlayer(this.player);
    }

    /// <summary>
    /// Move the player gameobject and also perform collision detection
    /// <param name="velocity">The players current velocity</param>
    /// <param name="highSpeedMovement">A flag to determine whether the function is being called through predictive methods</param>
    /// </summary>
    public void MoveAndCollide(Vector2 velocity, bool highSpeedMovement = false)
    {
        //If we hit something like an enemy or boss that causes velocity to change, reset the velocity
        if (this.player.GetHitBoxController().CheckHitBoxCollisions() == HitBoxContactEventResult.ContactFoundAndVelocityModified)
        {
            velocity = this.player.velocity;
        }

        float delta = GMPauseMenuManager.Instance().GameIsPaused() == false ? Time.deltaTime : Time.unscaledDeltaTime;

        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.InTeleporter)
        {
            return;
        }

        /// Note this may not cover all use cases if issues are arises set limits to the player speed in the <see cref="CharacterPhysicsScriptableObject"/>
        if (this.characterBuild.allowHighSpeedMovement && highSpeedMovement == false)
        {
            if (Vector2.Distance(velocity, Vector2.zero) > GMStageManager.Instance().GetMaxBlockSize())
            {
                float distance = Vector2.Distance(Vector2.zero, delta * GMStageManager.Instance().GetPhysicsMultiplier() * (Vector3)velocity);

                if (Mathf.Abs(distance) > GMStageManager.Instance().GetMaxBlockSize())//If the player is moving more than the GMStageManager.Instance().GetMaxBlockSize() pixels in a direction
                {
                    if (this.player.GetGrounded())
                    {
                        this.PerformHighSpeedGroundMovement(velocity, distance);
                    }
                    else
                    {
                        this.PerformHighSpeedAirMovement(velocity, distance);
                    }

                    return;
                }
            }
        }

        this.previousPosition = this.transform.position;
        this.transform.position += (Vector3)this.player.ApplyMovementRestrictions(velocity * GMStageManager.Instance().GetPhysicsMultiplier()) * delta;//Move the player by the current velocity

        //If the player is dead no collision detection
        if (this.player.GetHealthManager().GetHealthStatus() == HealthStatus.Death)
        {
            return;
        }

        this.UpdateCurrentCharacterInfo();

        if (this.player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InTube)
        {
            this.PerformCollisionDetection(ref velocity);
        }

        if (!highSpeedMovement)
        {
            this.player.velocity = velocity;
        }

        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Die>() == false)
        {
            this.player.GetSolidBoxController().CheckCollisionWithContactEvent<TriggerContactGimmick>();
            this.player.GetSolidBoxController().CheckCollisionWithContactEvent<SolidContactGimmick>();
        }

        this.transform.localRotation = Quaternion.Euler(0f, 0f, this.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees());
        this.player.ClampPositionToStageBounds();
    }

    /// <summary>
    /// Handles the player movement at incredibly high speeds exceeding 16 pixels per step while grounded
    /// This is  used to help avoid clipping to level terrain and missing interacts with gimmicks alongside mainting footing on high speed slopes
    /// <param name="velocity">The players current velocity</param>
    /// <param name="distance">How far the player is to be moved in pixels</param>
    /// </summary>
    private void PerformHighSpeedGroundMovement(Vector2 velocity, float distance)
    {
        HighSpeedMovement highSpeedMovement = new HighSpeedMovement();
        HighSpeedMovement.HighSpeedData highSpeedData = new HighSpeedMovement.HighSpeedData();
        int iterations = (int)((distance / GMStageManager.Instance().GetMaxBlockSize()) + 1);

        highSpeedData.initialVelocity = velocity;
        highSpeedData.targetDistance = distance;
        highSpeedData.velocityDirection = (Vector2.zero + (velocity * Time.fixedDeltaTime)).normalized;
        highSpeedData.currentMoveDistance = distance;
        highSpeedData.delta = 0;
        highSpeedData.angleBeforeUnground = this.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees();
        highSpeedData.startedGrounded = this.player.GetGrounded();//Determines whether movement started while on the ground

        for (int x = 0; x < iterations; x++)
        {
            highSpeedData.oldVelocity = highSpeedData.newVelocity;
            highSpeedData.delta = x;
            Vector2 highSpeedVelocity = highSpeedMovement.CalculateHighSpeedMovement(highSpeedData, this.player);
            this.MoveAndCollide(highSpeedVelocity, true);
            this.player.UpdatePlayerGroundMode();
            this.player.GetSpriteController().UpdatePlayerSpriteAngle(this.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees());

            if (this.player.GetGrounded())
            {
                highSpeedData.angleBeforeUnground = this.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees();
            }

            //If the players inital velocity changes end the loop like if they hit a spring, ceiling ,wall etc
            if (velocity != this.player.velocity || highSpeedData.currentMoveDistance == 0)
            {
                break;
            }

        }
        //Hand off the velocity if the player started grounded but ends up ungrounded
        if (this.player.GetGrounded() == false && highSpeedData.startedGrounded)
        {
            //Chances are we hit a gimmick so return
            if (this.player.GetSolidBoxController().HasActiveContactEvents() && this.player.GetSolidBoxController().HasGimmickInState(CollisionState.OnCollisionEnter))
            {
                return;
            }

            this.player.velocity = this.player.SlopeFormula(this.player.groundVelocity, highSpeedData.angleBeforeUnground * Mathf.Deg2Rad);
            velocity = this.player.velocity;
        }
    }

    /// <summary>
    /// Handles the player movement at incredibly high speeds exceeding 16 pixels per step while in the air
    /// This is  used to help avoid clipping to level terrain and missing interacts with gimmicks alongside mainting footing on high speed slopes
    /// <param name="velocity">The players current velocity</param>
    /// <param name="distance">How far the player is to be moved in pixels</param>
    /// </summary>
    private void PerformHighSpeedAirMovement(Vector2 velocity, float distance)
    {
        HighSpeedMovement highSpeedMovement = new HighSpeedMovement();
        HighSpeedMovement.HighSpeedData highSpeedData = new HighSpeedMovement.HighSpeedData();
        int iterations = (int)((distance / GMStageManager.Instance().GetMaxBlockSize()) + 1);

        highSpeedData.initialVelocity = velocity;
        highSpeedData.targetDistance = distance;
        highSpeedData.velocityDirection = (Vector2.zero + (velocity * Time.fixedDeltaTime)).normalized;
        highSpeedData.currentMoveDistance = distance;
        highSpeedData.delta = 0;
        highSpeedData.angleBeforeUnground = this.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees();

        for (int x = 0; x < iterations; x++)
        {
            highSpeedData.oldVelocity = highSpeedData.newVelocity;
            highSpeedData.delta = x;
            Vector2 highSpeedVelocity = highSpeedMovement.CalculateHighSpeedMovement(highSpeedData, this.player);

            this.MoveAndCollide(highSpeedVelocity, true);

            //If the players inital velocity changes end the loop like if they hit a spring, ceiling ,wall etc or no more to move
            if (velocity != this.player.velocity || highSpeedData.currentMoveDistance == 0)
            {
                break;
            }
        }
    }
    /// <summary>
    /// Updates the character info based on an array of conditions such as rolling or what slope they are on
    /// </summary>
    private void UpdateCurrentCharacterInfo()
    {
        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.OnCorkscrew && this.player.GetGimmickManager().GetGimmickAngle() != 0)
        {
            this.groundCollisionInfo.GetCurrentCollisionInfo().SetAngleInDegrees(this.player.GetGimmickManager().GetGimmickAngle());
        }

        bool rollingState = this.player.GetActionManager().CheckActionIsBeingPerformed<Roll>() || this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>();

        this.currentPushRadiusOffset = this.player.GetGrounded() && this.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees() == 0 ? this.characterBuild.pushRadiusOffsetOnFlatGround + (this.characterBuild.bodyHeightRadius - this.characterBuild.playerPixelPivotPoint) : this.characterBuild.pushRadiusOffsetOnSlopes;
        this.currentBodyWidthRadius = this.currentSizeMode == SizeMode.Shrunk && rollingState ? this.characterBuild.rollingBodyWidthRadius : this.characterBuild.bodyWidthRadius;
        this.currentBodyHeightRadius = this.currentSizeMode == SizeMode.Shrunk ? this.characterBuild.rollingBodyHeightRadius : this.characterBuild.bodyHeightRadius;
        this.currentPixelPivotPoint = this.characterBuild.playerPixelPivotPoint;

        if (this.currentSizeMode == SizeMode.Gliding)
        {
            this.currentBodyHeightRadius = this.characterBuild.glidingBodyHeightRadius;
        }
    }
    /// <summary>
    /// Peform Collision detection with the terian and solid gimmick objects
    /// <param name="velocity">The players current velocity</param>
    /// </summary>
    private void PerformCollisionDetection(ref Vector2 velocity)
    {
        if (this.player.GetActionManager().GetAction<Climb>())
        {
            Climb climb = this.player.GetActionManager().GetAction<Climb>() as Climb;

            if (climb != null && climb.GetClimbState() == ClimbState.PullUp)
            {
                return;
            }
        }

        this.wallCollisionInfo.CheckForCollision(this.transform.position, ref velocity);
        this.ceilingCollisionInfo.Update(ref velocity);
        this.groundCollisionInfo.Update(ref velocity);

        //We want to delay the wall sensor activation sometimes if we run into a slope, this leaves room for the ground sensors to reposition so more slopes are scalable
        if (this.wallCollisionInfo.GetDelayWallCollision())
        {
            this.wallCollisionInfo.Update(ref velocity);
        }
    }

    /// <summary>
    /// Identifies if a ceiling is too close to the player while grounded
    /// <param name="position"> The current position of the player</param>
    /// <param name="collisionInfo"> The current ground info</param>
    /// </summary>
    public void UpdateLowCeiling(Vector2 position, CollisionInfoData collisionInfo)
    {
        float angleInRadians = collisionInfo.GetAngleInRadians();
        float sensorAngle = angleInRadians * Mathf.Rad2Deg;//The angle the sensors will be cast in

        Vector2 direction = General.AngleToVector(sensorAngle * Mathf.Deg2Rad); //Set the ray direction
        Vector2 lowCeilingCheckRange = new Vector2(this.characterBuild.bodyWidthRadius * 2, this.characterBuild.lowCeilingYRange);
        Vector2 lowCeilingSensorPosition = General.CalculateAngledObjectPosition(position, angleInRadians, new Vector2(-this.characterBuild.bodyWidthRadius, lowCeilingCheckRange.y));
        RaycastHit2D lowCeilingSensor = Physics2D.Raycast(lowCeilingSensorPosition, direction, lowCeilingCheckRange.x, this.ceilingCollisionInfo.GetCollisionMask());

        Debug.DrawLine(lowCeilingSensorPosition, lowCeilingSensorPosition + (direction * lowCeilingCheckRange.x), this.lowCeilingSensorColor);

        this.lowCeiling = lowCeilingSensor;
    }

    /// <summary>
    /// Get the low ceiling value
    /// </summary>
    public bool GetLowCeiling() => this.lowCeiling;

    /// <summary>
    /// Update all solid box controllers with the appropriate size mode
    /// <param name="sizeMode">The updated size mode value</param>
    /// </summary>
    public void SetSizeMode(SizeMode sizeMode)
    {
        this.player.GetSolidBoxController().UpdateSolidBoxBounds(sizeMode);
        this.player.GetHitBoxController().UpdateHitBoxBounds(sizeMode);
        this.currentSizeMode = sizeMode;
    }

    /// <summary>
    /// Get the current size mode
    /// </summary>
    public SizeMode GetSizeMode() => this.currentSizeMode;

    /// <summary>
    /// Adds a Layer to the LayerMask by layer name
    /// <param name="layer"> The layer to add</param>
    /// </summary>
    public void AddLayerToAllCollisionMasks(int layer)
    {
        this.groundCollisionInfo.AddToCollisionMask(layer);
        this.ceilingCollisionInfo.AddToCollisionMask(layer);
        this.wallCollisionInfo.AddToCollisionMask(layer);
    }

    /// <summary>
    /// Removes a layer from the current LayerMask by layername
    /// <param name="layer"> The layer to remove </param>
    /// </summary>
    public void RemoveLayerFromAllCollisionMasks(int layer)
    {
        this.groundCollisionInfo.RemoveFromCollisionMask(layer);
        this.ceilingCollisionInfo.RemoveFromCollisionMask(layer);
        this.wallCollisionInfo.RemoveFromCollisionMask(layer);
    }

    /// <summary>
    /// Swaps the sensor data with another sensor object
    /// <param name="sensorsToSwapTo">The sensor data to swap towards</param>
    /// </summary>
    public void SwapSensorsData(Sensors sensorsToSwapTo) => this.characterBuild = sensorsToSwapTo.characterBuild;

    /// <summary>
    /// Allows the high speed movement when going at super speeds
    /// </summary>
    public bool GetAllowHighSpeedMovement() => this.characterBuild.allowHighSpeedMovement;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            if (this.GetComponent<CharacterData>() != null)
            {
                this.UpdateCurrentCharacterInfo();
            }

            this.groundCollisionInfo.OnDrawGimzos();
            this.ceilingCollisionInfo.OnDrawGimzos();
            this.wallCollisionInfo.OnDrawGimzos();
        }
    }
}
