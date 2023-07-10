using UnityEngine;
/// <summary>
/// Manages how the player finds what is considered ground and sticks to the ground
/// </summary>
[System.Serializable]
public class GroundCollisionInfo : PlayerCollisionInfo
{
    [SerializeField]
    private LedgeHelper ledgeHelper = new LedgeHelper();
    [SerializeField]
    private SmoothGroundLandingHelper smoothGroundLandingHelper;
    [Tooltip("A flag that ensures the sensors rotate when coming into contact with a steep angle"), SerializeField]
    private bool dontRotate = false;
    [Tooltip("A flag that determines whether sharp decent should be detected"), SerializeField]
    private bool detectSharpDecent;
    [SerializeField]
    private Color secondarySensorColor = new Color(0.8207547f, 0.5497508f, 0.6113427f, 1);

    public GroundCollisionInfo(Player player) : base(player)
    {
        this.SetCurrentCollisionInfo(new CollisionInfoData());
        this.ledgeHelper = new LedgeHelper();
        this.ledgeHelper.Reset();
    }

    /// <summary>
    /// Updates the ground collision data
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override void Update(ref Vector2 velocity)
    {
        this.dontRotate = Mathf.Abs(this.player.groundVelocity) <= 0 && this.detectSharpDecent && this.player.currentGroundMode != GroundMode.Ceiling;

        if (this.player.GetGrounded() || velocity.y <= 0)//Ground sensors only run when the player is on the ground or looking for the ground
        {
            this.CheckForCollision(this.player.transform.position, ref velocity);

            if (this.player.GetGrounded())
            {
                base.Update(ref velocity);
            }
        }
    }

    /// <summary>
    /// Actions performed the first time a collision is made
    /// </summary>
    public override void OnCollisionEnter()
    {
        this.player.transform.position = this.CalculateCollisionRepositioning(this.player.transform.position, this.GetCurrentCollisionInfo());
        this.player.SetGrounded(true);
    }

    /// <summary>
    /// Actions performed while actively colliding with the ground
    /// </summary>
    public override void OnCollisionStay()
    {
        this.player.transform.position = this.CalculateCollisionRepositioning(this.player.transform.position, this.GetCurrentCollisionInfo());

        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.OnFlipper)
        {
            this.GetCurrentCollisionInfo().SetAngleInDegrees(this.player.GetGimmickManager().GetGimmickAngle());
        }

        this.player.GetSensors().UpdateLowCeiling(this.player.transform.position, this.GetCurrentCollisionInfo());
    }

    /// <summary>
    /// Actions performed when the player leaves the ground
    /// </summary>
    public override void OnCollisionExit() => this.player.SetGrounded(false);

    /// <summary>
    /// Checks if the player has collided with successfully with the ground and saves the collision info
    /// <param name="position">The current player position</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override void CheckForCollision(Vector2 position, ref Vector2 velocity)
    {
        Sensors sensors = this.player.GetSensors();
        CollisionInfoData groundInfo = null;

        bool onRampHelper = this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.OnRampHelper;
        float distance = sensors.currentPixelPivotPoint + (onRampHelper ? 0 : sensors.characterBuild.sensorExtension); //How far the primary ground sensors go
        SensorData sensorData = new SensorData(this.GetCurrentCollisionInfo().GetAngleInRadians(), -90, this.GetCurrentCollisionInfo().GetAngleInDegrees(), distance);

        Vector2 aSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(-sensors.currentBodyWidthRadius, 0)); // A - Left Ground Sensor
        RaycastHit2D aSensor = Physics2D.Raycast(aSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(aSensorPosition, aSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColor);

        Vector2 bSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(sensors.currentBodyWidthRadius, 0)); // B - Right Ground Sensor
        RaycastHit2D bSensor = Physics2D.Raycast(bSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(bSensorPosition, bSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.rightSensorColor);
        bool onFlatGround = aSensor.normal == Vector2.up && bSensor.normal == Vector2.up;

        //Forces the player to walk off anything larger than the currentpixelpivotpoint
        if (onFlatGround && (Mathf.Abs(position.y - bSensor.point.y) > sensors.currentPixelPivotPoint || Mathf.Abs(position.y - aSensor.point.y) > sensors.currentPixelPivotPoint))//The lower point gets nullified
        {
            if (aSensor.point.y < bSensor.point.y)
            {
                aSensor = new RaycastHit2D();
            }
            else
            {
                bSensor = new RaycastHit2D();
            }
        }

        //When walking along a bridge set the hit sensor to the highest point
        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.OnBridge && aSensor && bSensor)
        {
            if (aSensor.point.y > bSensor.point.y)
            {
                aSensor = new RaycastHit2D();
            }
            else
            {
                bSensor = new RaycastHit2D();
            }
        }

        if (aSensor && bSensor) //When we get two hits we have to find the apporpriate ground info to store based on the collision points of both sensors
        {
            groundInfo = this.FindVerticalCollisionInfo(aSensor, bSensor);

            if (this.player.GetGrounded())
            {
                groundInfo.sensorHitData = SensorHitDirectionEnum.Both;
                this.CheckForSharpDecent(aSensor, bSensor, ref this.dontRotate, ref this.detectSharpDecent);

                if ((groundInfo.GetHit() && groundInfo.GetHit().transform.gameObject.layer != 13) || groundInfo.GetHit().transform.gameObject.layer != 14)
                {
                    groundInfo = this.CalculateCollisionInfoWithAtan(groundInfo, aSensor, bSensor);
                }

                if (this.dontRotate)
                {
                    groundInfo = this.FindVerticalCollisionInfo(aSensor, bSensor);
                    groundInfo.sensorHitData = groundInfo.GetHit().point == aSensor.point ? SensorHitDirectionEnum.Left : SensorHitDirectionEnum.Right;//Ensures stick sensors cast at the right position
                }
            }
            else
            {
                groundInfo = this.FindVerticalCollisionInfo(aSensor, bSensor);
                groundInfo.sensorHitData = groundInfo.GetHit().point == aSensor.point ? SensorHitDirectionEnum.Left : SensorHitDirectionEnum.Right;
            }
        }
        else if (aSensor && bSensor == false)
        {
            if (this.player.GetGrounded()
                && this.player.GetGimmickManager().GetGimmickAngle() == 0
                && this.ledgeHelper.CheckIfLedgeExceeded(this.player, this.GetCurrentCollisionInfo(), this.GetPreviousCollisionInfo(), this.collisionMask, SensorHitDirectionEnum.Left, aSensor, velocity))
            {
                this.ledgeHelper.Reset();
                this.Reset();

                return;
            }

            groundInfo = new CollisionInfoData(aSensor, SensorHitDirectionEnum.Left);
        }
        else if (bSensor && aSensor == false)
        {
            if (this.player.GetGrounded() &&
                this.player.GetGimmickManager().GetGimmickAngle() == 0 &&
                this.ledgeHelper.CheckIfLedgeExceeded(this.player, this.GetCurrentCollisionInfo(), this.GetPreviousCollisionInfo(), this.collisionMask, SensorHitDirectionEnum.Right, bSensor, velocity))
            {
                this.ledgeHelper.Reset();
                this.Reset();

                return;
            }

            groundInfo = new CollisionInfoData(bSensor, SensorHitDirectionEnum.Right);
        }
        else
        {
            this.Reset();
            this.player.SetGrounded(false);

            return;
        }

        if (this.player.GetGrounded() == false && groundInfo.GetHit()) //Set the players grounded status if the conditions are valid
        {
            //Going through a one way platform is accurate as long as wer are not inside (hit distance != 0)
            if (GMStageManager.Instance().SensorHitOneWayLayer(groundInfo.GetHit()) && groundInfo.GetHit().distance == 0)
            {
                return;
            }

            this.SetCurrentCollisionInfo(this.ValidateCollisionContact(groundInfo, position, ref velocity));

            return;
        }

        if (this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.Sliding)
        {
            if (this.player.CalculateGroundMode(groundInfo.GetAngleInDegrees()) > 0)
            {
                groundInfo.SetAngleInDegrees(this.GetCurrentCollisionInfo().GetAngleInDegrees());
            }
        }

        this.SetCurrentCollisionInfo(groundInfo);
    }

    /// <summary>
    /// Validates the players collision with the ground for the first time while also modifying velocity where needed and interacting with gimmicks
    /// <param name="groundInfo"> The current ground info identified</param>
    /// <param name="position"> The current position of the player</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override CollisionInfoData ValidateCollisionContact(CollisionInfoData groundInfo, Vector2 position, ref Vector2 velocity, float extensionCheck = 0)
    {
        if (Mathf.Round(groundInfo.GetHit().distance) <= GMStageManager.Instance().GetMaxBlockSize() + Mathf.Sin(groundInfo.GetAngleInRadians()))
        {
            //If we only get one sensor hit while not on a ground mode curve check an extra block (16px) down and rerun the collision check ONCE
            if (groundInfo.sensorHitData != SensorHitDirectionEnum.Both && this.player.CalculateGroundMode(groundInfo.GetAngleInDegrees(), 90) > 0 && extensionCheck == 0)
            {
                extensionCheck += GMStageManager.Instance().GetMaxBlockSize();

                return this.ValidateCollisionContact(groundInfo, position, ref velocity, extensionCheck);
            }

            Vector2 currentPlayerVelocity = this.player.velocity;
            SolidContactGimmickType collisionCheck = this.CheckNewCollisionWithSolidGimmick(groundInfo);
            bool launchedInTheAir = collisionCheck != SolidContactGimmickType.NoCollision && this.player.velocity.y > 0;

            switch (collisionCheck)//Modifiy the velocity based on the collision type
            {
                case SolidContactGimmickType.ModifiesVelocityOnContact:
                case SolidContactGimmickType.BreaksOnContact:
                    velocity = this.player.velocity;

                    if (collisionCheck != SolidContactGimmickType.BreaksOnContact)
                    {
                        this.player.transform.position = this.CalculateCollisionRepositioning(position, groundInfo);
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

            velocity = this.ConvertVelocityOnContactToGroundVelocity(this.player.velocity, groundInfo);
            this.player.velocity = velocity;

            return groundInfo;
        }

        return new CollisionInfoData();
    }

    /// <summary>
    /// Convert the players air velocity to the appropriate ground speeeds
    /// <param name="groundInfo"> The current ground info identified</param>
    /// <param name="velocity"> The current velocity of the player</param>
    /// </summary>
    public override Vector2 ConvertVelocityOnContactToGroundVelocity(Vector2 velocity, CollisionInfoData groundInfo)
    {
        float roundedAngle = Mathf.RoundToInt(groundInfo.GetAngleInDegrees());

        if (roundedAngle == 360)
        {
            roundedAngle = 0;
        }

        float conversionVelocity = velocity.x;
        //TODO: Consider addinga  class that takes in the angle converts it and returns an enum with isShallow, isHalfSteep, isFullstep or none?
        bool isShallow = roundedAngle is (>= 0 and <= 23) or (>= 339 and <= 360);
        bool isHalfSteep = roundedAngle is (>= 24 and <= 45) or (>= 316 and <= 338);
        bool isFullSteep = roundedAngle is (>= 46 and <= 90) or (>= 271 and <= 315);

        if (isShallow)
        {
            conversionVelocity = velocity.x;
        }
        else if (isHalfSteep)
        {
            conversionVelocity = Mathf.Abs(velocity.x) > -velocity.y ? velocity.x : velocity.y * 0.5f * Mathf.Sign(Mathf.Sin(groundInfo.GetAngleInRadians()));
        }
        else if (isFullSteep)
        {
            conversionVelocity = Mathf.Abs(velocity.x) > -velocity.y ? velocity.x : velocity.y * Mathf.Sign(Mathf.Sin(groundInfo.GetAngleInRadians()));
        }

        this.player.groundVelocity = conversionVelocity;
        this.player.GetSpriteController().UpdatePlayerSpriteAngle(General.RoundToDecimalPlaces(roundedAngle, 2));

        return velocity;
    }

    /// <summary>
    /// Ensure that the player is untop of the terrain at all times
    /// <param name="groundInfo"> The current ground info identified</param>
    /// <param name="position"> The current position of the player</param>
    /// </summary>
    public override Vector2 CalculateCollisionRepositioning(Vector2 position, CollisionInfoData groundInfo)
    {
        Sensors sensors = this.player.GetSensors();

        float distance = sensors.currentPixelPivotPoint + 0 + sensors.characterBuild.sensorExtension;
        SensorData sensorData = new SensorData(groundInfo.GetAngleInRadians(), -90, groundInfo.GetAngleInDegrees(), distance);
        bool playerisAboveTheGround = groundInfo.GetHit().distance > sensors.currentPixelPivotPoint;
        float rayHorizontalOffset = this.CalculateRayHorizontalOffset(groundInfo);

        Vector2 stickSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(rayHorizontalOffset, 0));
        RaycastHit2D stickSensor = Physics2D.Raycast(stickSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);

        if (!stickSensor && groundInfo.sensorHitData == SensorHitDirectionEnum.Both && General.CheckIfAngleIsZero(groundInfo.GetAngleInDegrees())) //There is a gap in the maddle so recast with an offset to the A or B Sensors
        {
            rayHorizontalOffset = sensors.currentBodyWidthRadius * this.player.currentPlayerDirection;
            stickSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(rayHorizontalOffset, 0));
            stickSensor = Physics2D.Raycast(stickSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        }

        Debug.DrawLine(stickSensorPosition, stickSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.secondarySensorColor);

        if (stickSensor)
        {
            if (playerisAboveTheGround)//While the player is above the ground the conditions must be met before their position can be adjusted
            {
                float distanceBetweenSensorAndGround = Vector2.Distance(stickSensor.point, position + new Vector2(rayHorizontalOffset, 0)) - sensors.currentPixelPivotPoint;
                float distanceValue = General.RoundToDecimalPlaces(distanceBetweenSensorAndGround, 2) + (rayHorizontalOffset * Mathf.Sin(groundInfo.GetAngleInRadians()));

                if (General.RoundToDecimalPlaces(distanceValue, 2) <= 0)
                {
                    return this.player.transform.position;
                }
            }

            position.x = stickSensor.point.x - (sensors.currentPixelPivotPoint * Mathf.Sin(groundInfo.GetAngleInRadians()));
            position.y = stickSensor.point.y + (sensors.currentPixelPivotPoint * Mathf.Cos(groundInfo.GetAngleInRadians()));

            if (rayHorizontalOffset != 0)
            {
                position -= new Vector2(rayHorizontalOffset * Mathf.Cos(groundInfo.GetAngleInRadians()), rayHorizontalOffset * Mathf.Sin(groundInfo.GetAngleInRadians()));
            }
        }

        return position;
    }
    /// <summary>
    /// Finds the appropriate collision info to store between two collision points
    /// This is for collision data storage typically the highest points will be used
    /// <param name="leftSensorHit">Typically the Left "A/C" sensor </param>
    /// <param name="rightSensorHit">Typically the Right "B/D" sensor </param>
    /// </summary>
    private CollisionInfoData FindVerticalCollisionInfo(RaycastHit2D leftSensorHit, RaycastHit2D rightSensorHit)
    {
        CollisionInfoData collisionInfo = new CollisionInfoData();
        this.CalculateVerticalInfo(leftSensorHit, rightSensorHit, out float leftVerticalResult, out float rightVerticalResult, out float leftSensorAngle, out float rightSensorAngle);

        // For those pesky 25 degree + angles
        if (this.player.GetGrounded())
        {
            if (leftSensorAngle == 360)
            {
                leftSensorAngle = 0;
            }

            if (rightSensorAngle == 360)
            {
                rightSensorAngle = 0;
            }

            //Limits for this operation
            if (General.CheckAngleIsWithinRange(leftSensorAngle, 0, 45) && General.CheckAngleIsWithinRange(rightSensorAngle, 0, 45))
            {
                if (this.player.currentGroundMode == GroundMode.Floor)
                {
                    if (rightSensorAngle == 0 && leftSensorAngle != 0 && leftVerticalResult > rightVerticalResult)
                    {
                        collisionInfo = new CollisionInfoData(rightSensorHit, SensorHitDirectionEnum.Right);

                        return collisionInfo;
                    }
                    else if (leftSensorAngle == 0 && rightSensorAngle != 0 && rightVerticalResult > leftVerticalResult)
                    {
                        collisionInfo = new CollisionInfoData(leftSensorHit, SensorHitDirectionEnum.Left);

                        return collisionInfo;
                    }
                }
            }
        }
        else if (this.smoothGroundLandingHelper.IsLandingOnCurve() && this.player.GetGroundMode() == GroundMode.Floor)
        {
            CollisionInfoData groundInfo = new CollisionInfoData(rightSensorAngle == 0 ? rightSensorHit : leftSensorHit, SensorHitDirectionEnum.Both);

            return this.CalculateCollisionInfoWithAtan(groundInfo, leftSensorHit, rightSensorHit);
        }

        collisionInfo = leftVerticalResult >= rightVerticalResult ? new CollisionInfoData(leftSensorHit) : new CollisionInfoData(rightSensorHit);
        collisionInfo.sensorHitData = SensorHitDirectionEnum.Both;

        return collisionInfo;
    }


    /// <summary>
    /// Checks if the player is about to go through a sharp decent 
    /// <param name="leftSensor">The left sensor</param>
    /// <param name="rightSensor">The right sensor</param>
    /// </summary>
    private void CheckForSharpDecent(RaycastHit2D leftSensor, RaycastHit2D rightSensor, ref bool dontRotate, ref bool detectSharpDecent)
    {
        float leftSensorAngleInDegrees = General.Vector2ToAngle(leftSensor.normal) * Mathf.Rad2Deg;
        float rightSensorAngleInDegrees = General.Vector2ToAngle(rightSensor.normal) * Mathf.Rad2Deg;

        if (General.DifferenceBetween2Angles(leftSensorAngleInDegrees, rightSensorAngleInDegrees) > 45 && (leftSensorAngleInDegrees == 0 || rightSensorAngleInDegrees == 0))
        {
            dontRotate = true;
            detectSharpDecent = true;
        }
        else
        {
            detectSharpDecent = false;
            dontRotate = false;
        }
    }
    /// <summary>
    /// Calculates the vertical result scores based on the current ground mode of the player
    /// <param name="leftSensorHit"> The left sensor hit</param>
    /// <param name="rightSensorHit"> The right sensor hit </param>
    /// <param name="leftVerticalResult"> The left vertical result returned based on the ground mode</param>
    /// <param name="rightVerticalResult"> the right vertical result returned based on the ground mode</param>
    /// <param name="leftSensorAngle"> The angle returned from the left sensor hit</param>
    /// <param name="rightSensorAngle"> The angle returned from the right sensor</param>
    /// </summary>
    private void CalculateVerticalInfo(RaycastHit2D leftSensorHit, RaycastHit2D rightSensorHit, out float leftVerticalResult, out float rightVerticalResult, out float leftSensorAngle, out float rightSensorAngle)
    {
        leftSensorAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(leftSensorHit.normal)) * Mathf.Rad2Deg;
        rightSensorAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(rightSensorHit.normal)) * Mathf.Rad2Deg;

        switch (this.player.GetGroundMode())
        {
            case GroundMode.Floor:
                leftVerticalResult = leftSensorHit.point.y;
                rightVerticalResult = rightSensorHit.point.y;

                if (this.player.GetGrounded() == false)
                {
                    this.smoothGroundLandingHelper.Reset();
                    //Since we are using polygons for collision when landing on certain curves we need to perform extra checks to make sure the correct anggle is used
                    //A common prolem case is landing on curves where the left sensor is on flat ground but the right sensor is detecting a potential curve
                    //If new inconsistenciies arise with ground landing try disabling this if that helps :D
                    if ((leftSensorAngle == 0 && rightSensorAngle != 0) || (rightSensorAngle == 0 && leftSensorAngle != 0))
                    {
                        this.smoothGroundLandingHelper = new SmoothGroundLandingHelper(this.player, this.collisionMask, leftSensorHit, rightSensorHit);
                        this.smoothGroundLandingHelper.UpdateDetails();

                        leftVerticalResult = this.smoothGroundLandingHelper.currentleftVerticalResult;
                        rightVerticalResult = this.smoothGroundLandingHelper.currentRightVerticalResult;
                        leftSensorHit = this.smoothGroundLandingHelper.currentLeftSensor;
                        rightSensorHit = this.smoothGroundLandingHelper.currentRightSensor;
                    }
                }

                break;
            case GroundMode.RightWall:
                leftVerticalResult = -leftSensorHit.point.x;
                rightVerticalResult = -rightSensorHit.point.x;

                break;
            case GroundMode.Ceiling:
                leftVerticalResult = -leftSensorHit.point.y;
                rightVerticalResult = -rightSensorHit.point.y;

                break;
            case GroundMode.LeftWall:
                leftVerticalResult = leftSensorHit.point.x;
                rightVerticalResult = rightSensorHit.point.x;

                break;
            default:
                leftVerticalResult = 0;
                rightVerticalResult = 0;

                break;
        }
    }

    /// <summary>
    /// When only one of the A (left) and B (Right) chances are the middle sensor is not going to hit
    /// This offset allows us to move the stick to ground sensors to the position of the singular hitting ray
    /// while simultaneously adjusting the player object in relation to the middle point in other words(falsifying the center point)
    /// <param name="groundInfo"> The current ground info identified</param>
    /// </summary>
    private float CalculateRayHorizontalOffset(CollisionInfoData groundInfo)
    {
        if (groundInfo.sensorHitData != SensorHitDirectionEnum.Both)
        {
            return this.player.GetSensors().currentBodyWidthRadius * groundInfo.GetSensorHitDirection();
        }

        return 0;
    }

    public override void OnDrawGimzos()
    {
        Sensors sensors = this.player.GetSensors();
        float currentAngle = this.player.transform.eulerAngles.z;
        Vector2 position = this.player.transform.position;
        float verticalSensorDistance = sensors.currentBodyHeightRadius + sensors.characterBuild.sensorExtension; //How far the Vertical Sensors go
        float wallSensorDistance = sensors.currentPushRadius; //How far the horizontal sensors go
        SensorData groundSensorData = new SensorData(currentAngle * Mathf.Deg2Rad, -90, currentAngle, verticalSensorDistance);

        Vector2 aSensorPosition = General.CalculateAngledObjectPosition(position, groundSensorData.GetAngleInRadians(), new Vector2(-sensors.currentBodyWidthRadius, 0)); // A - Left Ground Sensor
        Debug.DrawLine(aSensorPosition, aSensorPosition + (groundSensorData.GetCastDirection() * verticalSensorDistance), this.leftSensorColor);

        Vector2 bSensorPosition = General.CalculateAngledObjectPosition(position, groundSensorData.GetAngleInRadians(), new Vector2(sensors.currentBodyWidthRadius, 0)); // B - Right Ground Sensor
        Debug.DrawLine(bSensorPosition, bSensorPosition + (groundSensorData.GetCastDirection() * verticalSensorDistance), this.rightSensorColor);
    }
}
