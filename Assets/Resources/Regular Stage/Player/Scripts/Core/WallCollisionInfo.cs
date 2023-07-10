using UnityEngine;
/// <summary>
/// Manages how the player finds what is considered wall
/// </summary>
[System.Serializable]
public class WallCollisionInfo : PlayerCollisionInfo
{
    [Tooltip("What the player can climb"), SerializeField]
    private LayerMask climbableCollisionMask;
    [SerializeField, Tooltip("A flag that dermines us doing the wall collision after ground and ceiling collision"), FirstFoldOutItem("Delay Wall Collision Settings")]
    private bool delayWallCollision = false;
    [SerializeField, Tooltip("Walls with angles out of this range will be run after ground & ceiling collision"), LastFoldoutItem(), Min(15)]
    private float delayWallCollisionRange = 15f;

    public WallCollisionInfo(Player player) : base(player) => this.SetCurrentCollisionInfo(new CollisionInfoData());

    /// <summary>
    /// Updates the wall collision data
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override void Update(ref Vector2 velocity)
    {
        this.CheckForCollision(this.player.transform.position, ref velocity);

        if (this.GetDelayWallCollision())
        {
            this.SetDelayWallCollision(false);
        }

        if (this.GetCurrentCollisionInfo().GetHit())
        {
            base.Update(ref velocity);
            this.SetCollisionState(CollisionState.OnCollisionExit);
        }
    }

    /// <summary>
    /// While colliding we don't really want to save the collision data as adjustments will remove us from range of the collision
    /// </summary>
    public override void OnCollisionStay() => this.SetCurrentCollisionInfo(new CollisionInfoData());

    /// <summary>
    /// Performs collision detection based on what is to the left or right of the player
    /// <param name="position">The current player position</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override void CheckForCollision(Vector2 position, ref Vector2 velocity)
    {
        Sensors sensors = this.player.GetSensors();
        CollisionInfoData wallCollisionInfo = null;
        SensorData sensorData = new SensorData(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians(), 0, sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees(), sensors.currentPushRadius);
        Vector2 wallSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(0, sensors.currentPushRadiusOffset)); //Setup sensor position

        RaycastHit2D eSensor = Physics2D.Raycast(wallSensorPosition, -sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);// E - Left Wall Sensor
        Debug.DrawLine(wallSensorPosition, wallSensorPosition + (-sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColor);
        RaycastHit2D fSensor = Physics2D.Raycast(wallSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);// F - Right Wall Sensor
        Debug.DrawLine(wallSensorPosition, wallSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.rightSensorColor);

        float playerDirection = Mathf.Sign(Mathf.Cos(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians())); //Will be 1 normally and be -1 when the player is upside down to allow wall collisions upside down

        if (eSensor && velocity.x * playerDirection < 0)
        {
            wallCollisionInfo = new CollisionInfoData(eSensor)
            {
                sensorHitData = SensorHitDirectionEnum.Left
            };
        }
        else if (fSensor && velocity.x * playerDirection > 0)
        {
            wallCollisionInfo = new CollisionInfoData(fSensor)
            {
                sensorHitData = SensorHitDirectionEnum.Right
            };
        }
        else
        {
            this.SetCurrentCollisionInfo(new CollisionInfoData());

            return;
        }

        if (this.GetDelayWallCollision() == false)
        {
            float range = this.delayWallCollisionRange - sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees();

            if (wallCollisionInfo.sensorHitData != SensorHitDirectionEnum.None
                && !General.CheckAngleIsWithinRange(wallCollisionInfo.GetAngleInDegrees(), 90 - range, 90 + range)
                && !General.CheckAngleIsWithinRange(wallCollisionInfo.GetAngleInDegrees(), 270 - range, 270 + range))
            {
                SolidContactGimmickType collisionCheck = this.CheckNewCollisionWithSolidGimmick(wallCollisionInfo);

                if (collisionCheck == SolidContactGimmickType.NoCollision)
                {
                    wallCollisionInfo = new CollisionInfoData();
                    this.SetDelayWallCollision(true);

                    return;
                }
            }
        }

        if (wallCollisionInfo.GetHit())
        {
            this.SetCurrentCollisionInfo(this.ValidateCollisionContact(wallCollisionInfo, position, ref velocity));
        }
    }
    /// <summary>
    /// Actions performed when the player comes in contact with the wall
    /// <param name="wallCollisionInfo"> The current wall collision info identified </param>
    /// <param name="position"> The current position of the player</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// <paramref name="extensionCheck"/> the current value of the extension check </parm>
    /// </summary>
    public override CollisionInfoData ValidateCollisionContact(CollisionInfoData wallCollisionInfo, Vector2 position, ref Vector2 velocity, float extensionCheck = 0)
    {
        Sensors sensors = this.player.GetSensors();
        position = this.CalculateCollisionRepositioning(position, wallCollisionInfo);

        if (wallCollisionInfo.GetHit().distance == 0)//If this returns true it means the player is stuck inside the wall so recast from the new position recusrively
        {
            this.CheckForCollision(position, ref velocity);

            return this.GetCurrentCollisionInfo();
        }

        if (sensors.characterBuild.useWallLanding)
        {
            bool condition = this.player.GetGrounded() ? General.CheckAngleIsWithinRange(wallCollisionInfo.GetAngleInDegrees(), 60, 157.5f) : General.CheckAngleIsWithinRange(wallCollisionInfo.GetAngleInDegrees(), 150, 247.5f);

            if ((condition && Mathf.Round(wallCollisionInfo.GetAngleInDegrees()) != 270 && Mathf.Round(wallCollisionInfo.GetAngleInDegrees()) != 90)
                || (velocity.y < 0 && wallCollisionInfo.GetAngleInDegrees() == 0))
            {
                this.player.GetSensors().groundCollisionInfo.SetCurrentCollisionInfo(wallCollisionInfo);
                velocity = this.ConvertVelocityOnContactToGroundVelocity(velocity, wallCollisionInfo);
                this.player.SetGrounded(true);
                this.player.velocity = velocity;

                return new CollisionInfoData();
            }
        }

        Vector2 currentPlayerVelocity = this.player.velocity;
        SolidContactGimmickType collisionCheck = this.CheckNewCollisionWithSolidGimmick(wallCollisionInfo);

        switch (collisionCheck)
        {
            case SolidContactGimmickType.ModifiesVelocityOnContact:
            case SolidContactGimmickType.BreaksOnContact:
                velocity = this.player.velocity;

                if (collisionCheck != SolidContactGimmickType.BreaksOnContact)
                {
                    this.player.transform.position = position;
                }

                return new CollisionInfoData();
            case SolidContactGimmickType.Normal:
            case SolidContactGimmickType.MovingPlatform:
            default:

                if (this.player.velocity != currentPlayerVelocity)
                {
                    velocity = this.player.velocity;

                    return new CollisionInfoData();
                }

                this.player.groundVelocity = 0;
                this.player.velocity.x = 0;
                velocity.x = 0;

                break;
        }

        this.player.transform.position = position;
        this.OnCollisionEnter();

        return wallCollisionInfo;
    }

    /// <summary>
    /// Pushes the player away from the wall they are currently colliding with
    /// <param name="position"> The current position of the player</param>
    /// <param name="wallCollisionInfo"> The current wall collision info identified </param>
    /// </summary>
    public override Vector2 CalculateCollisionRepositioning(Vector2 position, CollisionInfoData wallCollisionInfo)
    {
        Sensors sensors = this.player.GetSensors();

        position.x = wallCollisionInfo.GetHit().point.x + (wallCollisionInfo.GetSensorHitDirection() * -1 * sensors.currentPushRadius * Mathf.Cos(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians()));
        position.y = wallCollisionInfo.GetHit().point.y + (wallCollisionInfo.GetSensorHitDirection() * -1 * sensors.currentPushRadius * Mathf.Sin(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians()));
        position.x += sensors.currentPushRadiusOffset * Mathf.Sin(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians());
        position.y -= sensors.currentPushRadiusOffset * Mathf.Cos(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians());

        return position;
    }

    /// <summary>
    /// Convert the players air/ground velocity to the appropriate ground speeeds when attaching to walls
    /// <param name="velocity"> The current velocity of the player</param>
    /// <param name="wallCollisionInfo"> The current wall collision info identified </param>
    /// </summary>
    public override Vector2 ConvertVelocityOnContactToGroundVelocity(Vector2 velocity, CollisionInfoData wallCollisionInfo)
    {
        float roundedAngle = Mathf.RoundToInt(wallCollisionInfo.GetAngleInDegrees());
        this.player.GetSpriteController().UpdatePlayerSpriteAngle(General.RoundToDecimalPlaces(roundedAngle, 2));
        float conversionVelocity = velocity.x;
        conversionVelocity *= Mathf.Sign(Mathf.Cos(wallCollisionInfo.GetAngleInRadians()));
        this.player.groundVelocity = conversionVelocity;

        return velocity;
    }

    /// <summary>
    /// Gets the collision of what the player can climb
    /// </summary>
    public LayerMask GetClimbableCollisionMask() => this.climbableCollisionMask;

    /// <summary>
    /// Set the delay wall collision
    /// <param name="delayWallCollision"> the delay wall collision state</param>
    /// </summary>
    public void SetDelayWallCollision(bool delayWallCollision) => this.delayWallCollision = delayWallCollision;

    /// <summary>
    /// Get the delay wall collision flag
    /// </summary>
    public bool GetDelayWallCollision() => this.delayWallCollision;

    public override void OnDrawGimzos()
    {
        Sensors sensors = this.player.GetSensors();
        float currentAngle = this.player.transform.eulerAngles.z;
        Vector2 position = this.player.transform.position;
        float verticalSensorDistance = sensors.currentBodyHeightRadius + sensors.characterBuild.sensorExtension; //How far the Vertical Sensors go
        float wallSensorDistance = sensors.currentPushRadius; //How far the horizontal sensors go
        SensorData wallSensorData = new SensorData(currentAngle * Mathf.Deg2Rad, -90, currentAngle - 90, wallSensorDistance);

        Vector2 wallSensorPosition = General.CalculateAngledObjectPosition(position, wallSensorData.GetAngleInRadians(), new Vector2(0, sensors.currentPushRadiusOffset)); //Setup sensor position

        Debug.DrawLine(wallSensorPosition, wallSensorPosition + (-wallSensorData.GetCastDirection() * wallSensorDistance), this.leftSensorColor);// E - Left Wall Sensor
        Debug.DrawLine(wallSensorPosition, wallSensorPosition + (wallSensorData.GetCastDirection() * wallSensorDistance), this.rightSensorColor);// F - Right Wall Sensor
    }
}
