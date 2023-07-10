using UnityEngine;
/// <summary>
/// Manages how the player finds what is considered a ceiling and readjusts
/// </summary>
[System.Serializable]
public class CeilingCollisionInfo : PlayerCollisionInfo
{
    public CeilingCollisionInfo(Player player) : base(player) => this.SetCurrentCollisionInfo(new CollisionInfoData());

    /// <summary>
    /// Updates the ceiling collision data
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override void Update(ref Vector2 velocity)
    {
        if (this.player.GetGrounded() == false && velocity.y > 0)
        {
            this.CheckForCollision(this.player.transform.position, ref velocity);
        }
        else
        {
            if (this.GetCurrentCollisionInfo().GetHit())
            {
                base.Update(ref velocity);
                this.SetCollisionState(CollisionState.OnCollisionExit);
            }
        }
    }

    /// <summary>
    /// While colliding we don't really want to save the collision data as adjustments will remove us from range of the collision
    /// </summary>
    public override void OnCollisionStay() => this.SetCurrentCollisionInfo(new CollisionInfoData());

    /// <summary>
    /// Performs collision detection based on what is above the player
    /// <param name="position">The current player position</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override void CheckForCollision(Vector2 position, ref Vector2 velocity)
    {
        Sensors sensors = this.player.GetSensors();
        CollisionInfoData ceilingInfo = null;

        float distance = sensors.currentBodyHeightRadius + sensors.characterBuild.sensorExtension;
        SensorData sensorData = new SensorData(sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians(), -90, sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees() - 180, distance);

        Vector2 cSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(-sensors.currentBodyWidthRadius, 0)); // C - Left Ceiling Sensor
        RaycastHit2D cSensor = Physics2D.Raycast(cSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(cSensorPosition, cSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColor);

        Vector2 dSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(sensors.currentBodyWidthRadius, 0)); // D - Right Ceiling Sensor
        RaycastHit2D dSensor = Physics2D.Raycast(dSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(dSensorPosition, dSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.rightSensorColor);

        if (cSensor && dSensor)
        {
            ceilingInfo = cSensor.point.y < dSensor.point.y ? new CollisionInfoData(cSensor, SensorHitDirectionEnum.Both) : new CollisionInfoData(dSensor, SensorHitDirectionEnum.Both);//When we get two hits we have to find the apporpriate collision info to store between the two
            ceilingInfo = this.CalculateCollisionInfoWithAtan(ceilingInfo, dSensor, cSensor);
        }
        else if (cSensor && dSensor == false)
        {
            ceilingInfo = new CollisionInfoData(cSensor, SensorHitDirectionEnum.Left);
        }
        else if (dSensor && cSensor == false)
        {
            ceilingInfo = new CollisionInfoData(dSensor, SensorHitDirectionEnum.Right);
        }
        else
        {
            ceilingInfo = new CollisionInfoData();

            return;
        }

        if (ceilingInfo.GetHit())
        {
            this.SetCurrentCollisionInfo(this.ValidateCollisionContact(ceilingInfo, position, ref velocity));
        }
    }
    /// <summary>
    /// Validates the players initial ceiling hit and acts on them
    /// <param name="ceilingInfo"> The current ceiling info identified</param>
    /// <param name="position"> The current position of the player</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override CollisionInfoData ValidateCollisionContact(CollisionInfoData ceilingInfo, Vector2 position, ref Vector2 velocity, float extensionCheck = 0)
    {
        Sensors sensors = this.player.GetSensors();

        if (ceilingInfo.GetHit().distance <= GMStageManager.Instance().GetMaxBlockSize() + Mathf.Sin(ceilingInfo.GetAngleInRadians()) && velocity.y > 0)
        {
            Vector2 currentPlayerVelocity = this.player.velocity;
            SolidContactGimmickType collisionCheck = this.CheckNewCollisionWithSolidGimmick(ceilingInfo);

            switch (collisionCheck)//Modifiy the velocity based on the collision type
            {
                case SolidContactGimmickType.ModifiesVelocityOnContact:
                case SolidContactGimmickType.BreaksOnContact:
                    velocity = this.player.velocity;

                    if (collisionCheck != SolidContactGimmickType.BreaksOnContact)
                    {
                        position = this.CalculateCollisionRepositioning(position, ceilingInfo);
                    }

                    return new CollisionInfoData();
                case SolidContactGimmickType.Normal:
                case SolidContactGimmickType.MovingPlatform:
                default:
                    if (this.player.velocity != currentPlayerVelocity)
                    {
                        velocity = this.player.velocity;
                    }

                    break;
            }

            velocity = this.ConvertVelocityOnContactToGroundVelocity(this.player.velocity, ceilingInfo);
            this.player.velocity = velocity;

            position = this.CalculateCollisionRepositioning(position, ceilingInfo);
            this.player.transform.position = position;

            return ceilingInfo;
        }

        return new CollisionInfoData();
    }

    /// <summary>
    /// Pushes the player away from the wall they are currently colliding with
    /// <param name="position"> The current position of the player</param>
    /// <param name="ceilingInfo"> The current ceiling info identified</param>
    /// </summary>
    public override Vector2 CalculateCollisionRepositioning(Vector2 position, CollisionInfoData ceilingInfo)
    {
        Sensors sensors = this.player.GetSensors();
        float bodyHeightRadius = sensors.GetSizeMode() == SizeMode.Shrunk ? sensors.characterBuild.rollingBodyHeightRadius : sensors.characterBuild.bodyHeightRadius; //For the ceiling adjust the position based on the players current height

        position.y = ceilingInfo.GetHit().point.y + 1 - (bodyHeightRadius * Mathf.Cos(this.player.velocity.y != 0 ? sensors.groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians() : 0));

        return position;
    }

    /// <summary>
    /// Convert the players air/ground velocity to the appropriate ground speeeds when attaching to walls
    /// <param name="velocity"> The current velocity of the player</param>
    /// <param name="ceilingInfo"> The current ceiling info identified</param>
    /// </summary>
    public override Vector2 ConvertVelocityOnContactToGroundVelocity(Vector2 velocity, CollisionInfoData ceilingInfo)
    {
        Sensors sensors = this.player.GetSensors();
        float roundedAngle = Mathf.RoundToInt(ceilingInfo.GetAngleInDegrees());
        float conversionVelocity = velocity.y;
        bool isCeiling = roundedAngle is >= 136 and <= 225;
        bool isSlope = roundedAngle is (>= 91 and <= 135) or (>= 226 and <= 270);

        if (isCeiling)
        {
            velocity.y = 0;
        }
        else if (isSlope)
        {
            this.player.SetGrounded(true);
            sensors.groundCollisionInfo.CalculateCollisionRepositioning(this.player.transform.position, ceilingInfo);
            conversionVelocity = velocity.y * Mathf.Sign(Mathf.Sin(ceilingInfo.GetAngleInRadians()));
            this.player.groundVelocity = conversionVelocity;
            sensors.groundCollisionInfo.SetCurrentCollisionInfo(ceilingInfo);
        }

        this.player.GetSpriteController().UpdatePlayerSpriteAngle(General.RoundToDecimalPlaces(roundedAngle, 2));

        return velocity;
    }

    public override void OnDrawGimzos()
    {
        Sensors sensors = this.player.GetSensors();
        float currentAngle = this.player.transform.eulerAngles.z;
        Vector2 position = this.player.transform.position;
        float verticalSensorDistance = sensors.currentBodyHeightRadius + sensors.characterBuild.sensorExtension; //How far the Vertical Sensors go
        float wallSensorDistance = sensors.currentPushRadius; //How far the horizontal sensors go

        SensorData ceilingSensorrData = new SensorData(currentAngle * Mathf.Deg2Rad, -90, currentAngle - 180, verticalSensorDistance);

        Vector2 cSensorPosition = General.CalculateAngledObjectPosition(position, ceilingSensorrData.GetAngleInRadians(), new Vector2(-sensors.currentBodyWidthRadius, 0)); // C - Left Ceiling Sensor
        Debug.DrawLine(cSensorPosition, cSensorPosition + (ceilingSensorrData.GetCastDirection() * verticalSensorDistance), this.leftSensorColor);

        Vector2 dSensorPosition = General.CalculateAngledObjectPosition(position, ceilingSensorrData.GetAngleInRadians(), new Vector2(sensors.currentBodyWidthRadius, 0)); // D - Right Ceiling Sensor
        Debug.DrawLine(dSensorPosition, dSensorPosition + (ceilingSensorrData.GetCastDirection() * verticalSensorDistance), this.rightSensorColor);
    }
}
