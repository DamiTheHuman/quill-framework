using UnityEngine;
[System.Serializable]
/// <summary>
/// A helper class that helps adjust the vertical info more accurately when comparing ground sensor data on awkward curves
/// Awkward curves can arise form poor polygon collision set up or polygon collision just retruning awkard values when at certain curves
/// </summary>
public class SmoothGroundLandingHelper
{
    LedgeHelper ledgeHelper = new LedgeHelper();
    [SerializeField, Tooltip("The collision mask to be used on ground checks")]
    public LayerMask collisionMask;
    [SerializeField, Tooltip("The current player")]
    public Player player;
    [SerializeField, Tooltip("The current left sensor hit")]
    public RaycastHit2D currentLeftSensor;
    [SerializeField, Tooltip("The current right sensor hit")]
    public RaycastHit2D currentRightSensor;
    [SerializeField, Tooltip("The vertical result which defaultts to 0 on when ledges are found")]
    public float currentleftVerticalResult = 0;
    [SerializeField, Tooltip("The vertical result which defaultts to 0 on when ledges are found")]
    public float currentRightVerticalResult = 0;
    [SerializeField, Tooltip("Whether we have a ledge to the right")]
    private bool isLedgeToTheLeft = false;
    [SerializeField, Tooltip("Whether we have a ledge to the left")]
    private bool isLedgeToTheRight = false;
    [SerializeField, Tooltip("The active direction of the ledge")]
    private LedgeDirection ledgeDirection = LedgeDirection.None;
    [SerializeField, Tooltip("The angle of our left sensor based on the normal")]
    public float currentLeftSensorAngle = 0;
    [SerializeField, Tooltip("The angle of our right sensor based on the normal")]
    public float currentRightSesnsorAngle = 0;

    public SmoothGroundLandingHelper(Player player, LayerMask collisionMask, RaycastHit2D currentLeftSensor, RaycastHit2D currentRightSensor)
    {
        this.player = player;
        this.collisionMask = collisionMask;
        this.currentLeftSensor = currentLeftSensor;
        this.currentRightSensor = currentRightSensor;
        this.currentLeftSensorAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(currentLeftSensor.normal) * Mathf.Rad2Deg);
        this.currentRightSesnsorAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(currentRightSensor.normal) * Mathf.Rad2Deg);
        this.isLedgeToTheLeft = false;
        this.isLedgeToTheRight = false;
        this.ledgeDirection = LedgeDirection.None;
        this.currentleftVerticalResult = currentLeftSensor.point.y;
        this.currentRightVerticalResult = currentRightSensor.point.y;
    }

    /// <summary>
    /// Resets the helper data to defaults
    /// </summary>
    public void Reset()
    {
        this.currentLeftSensor = new RaycastHit2D();
        this.currentRightSensor = new RaycastHit2D();
        this.currentRightSesnsorAngle = 0;
        this.ledgeDirection = LedgeDirection.None;
        this.isLedgeToTheLeft = false;
        this.isLedgeToTheRight = false;
        this.currentleftVerticalResult = 0;
        this.currentRightVerticalResult = 0;
    }

    /// <summary>
    /// Begins a check that updates this class with values that that ensures a smooth landing can be performed when the player lands on obscure ledges
    /// </summary>
    public void UpdateDetails()
    {
        this.UpdateSensorDataBasedOnLedges();

        Vector2 position = this.player.transform.position;
        Sensors sensors = this.player.GetSensors();

        if (this.isLedgeToTheLeft || this.isLedgeToTheRight)
        {
            if (this.isLedgeToTheLeft)
            {
                this.currentleftVerticalResult = Mathf.NegativeInfinity;
                this.ledgeDirection = LedgeDirection.Left;
            }
            else
            {
                this.currentRightVerticalResult = Mathf.NegativeInfinity;
                this.ledgeDirection = LedgeDirection.Right;
            }
        }
    }

    /// <summary>
    /// If there is is no ledge to either point we can assume its a curve like structure or flat ground
    /// Remove the hit checks if any issues arise as a first base
    /// </summary>
    public bool IsLandingOnCurve() => (this.isLedgeToTheLeft == false && this.currentLeftSensor) && (this.isLedgeToTheRight == false && this.currentRightSensor);

    /// <summary>
    /// Updates the passed in result data depending on if a ledge was found on either side and if theere was a collision
    /// This ground check is more strict than the check in <see cref="GroundCollisionInfo.CheckForCollision"/> as instead of using sensor extension which is 16 by default it uses half a max block size  which is 8 (16/2) by default
    /// </summary>
    private void UpdateSensorDataBasedOnLedges()
    {
        Vector2 position = this.CalculateBasePosition(out float extension);

        Sensors sensors = this.player.GetSensors();
        float distance = sensors.currentPixelPivotPoint + GMStageManager.Instance().GetMaxBlockSize() + extension; //How far the primary ground sensors go
        SensorData sensorData = new SensorData(0, -90, 0, distance);

        Vector2 newLeftSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(-sensors.characterBuild.bodyWidthRadius, 0));
        RaycastHit2D newLeftSensorHit = Physics2D.Raycast(newLeftSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);

        Vector2 newRightSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(sensors.characterBuild.bodyWidthRadius, 0));
        RaycastHit2D newRightSensorHit = Physics2D.Raycast(newRightSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);

        this.isLedgeToTheLeft = this.HasLedgeAtHorizontalPosition(newLeftSensorHit, SensorHitDirectionEnum.Left);
        this.isLedgeToTheRight = this.HasLedgeAtHorizontalPosition(newRightSensorHit, SensorHitDirectionEnum.Right);

        this.currentLeftSensorAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(newLeftSensorHit.normal)) * Mathf.Rad2Deg;
        this.currentRightSesnsorAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(newRightSensorHit.normal)) * Mathf.Rad2Deg;

        if (this.isLedgeToTheLeft == false && this.isLedgeToTheRight == false && (this.currentLeftSensorAngle == 0 || this.currentRightSesnsorAngle == 0))
        {
            this.RecheckLedgePositionsAtBaseAngles(position, newLeftSensorHit, newRightSensorHit);
        }

        if ((newLeftSensorHit && !newRightSensorHit) || (newRightSensorHit && !newLeftSensorHit))
        {
            return;
        }

        if (this.currentLeftSensorAngle == 0 && this.currentRightSesnsorAngle != 0)
        {
            this.currentLeftSensor = newLeftSensorHit;
        }
        else if (this.currentRightSesnsorAngle == 0 && this.currentLeftSensorAngle != 0)
        {
            this.currentRightSensor = newRightSensorHit;
        }
    }

    /// <summary>
    /// Calculates the base position in which sensors will be sent out from while also adding an extension when needed
    /// <param name="extension"> How much to extend the sensors by if needed </param>
    /// </summary>
    private Vector2 CalculateBasePosition(out float extension)
    {
        Vector2 position = this.player.transform.position;
        extension = 0;

        if (this.currentLeftSensor && this.currentRightSensor)
        {
            if (this.currentLeftSensorAngle == 0)
            {
                position = this.player.GetSensors().groundCollisionInfo.CalculateCollisionRepositioning(this.player.transform.position, new CollisionInfoData(this.currentLeftSensor, SensorHitDirectionEnum.Left));
            }
            else if (this.currentRightSesnsorAngle == 0)
            {
                position = this.player.GetSensors().groundCollisionInfo.CalculateCollisionRepositioning(this.player.transform.position, new CollisionInfoData(this.currentRightSensor, SensorHitDirectionEnum.Right));
            }

            extension = 8;
        }
        else if (this.currentLeftSensor)
        {
            position = this.player.GetSensors().groundCollisionInfo.CalculateCollisionRepositioning(this.player.transform.position, new CollisionInfoData(this.currentLeftSensor, SensorHitDirectionEnum.Left));
        }
        else if (this.currentRightSensor)
        {
            position = this.player.GetSensors().groundCollisionInfo.CalculateCollisionRepositioning(this.player.transform.position, new CollisionInfoData(this.currentRightSensor, SensorHitDirectionEnum.Right));
        }

        return position;
    }

    /// <summary>
    /// Returns what the player position would be if the collision info passed in was used
    /// <param name="sensorHit"> The raycast sensor hit results</param>
    /// <param name="sensorHitData"> The sensor hit data signifying the direction the hit occured in</param>
    /// </summary>
    private Vector2 CalculatePlayerPositionAtCollisionInfo(RaycastHit2D sensorHit, SensorHitDirectionEnum sensorHitData)
    {
        CollisionInfoData collisionInfoData = new CollisionInfoData(sensorHit, sensorHitData);

        return this.player.GetSensors().groundCollisionInfo.CalculateCollisionRepositioning(this.player.transform.position, collisionInfoData);
    }

    /// If either angle is at 0 and we have no ledges at either side We still need to confirm whether we are on a ledge or not
    /// To do this we need to perform the check against the angle at 0 degrees which signifies flat ground
    /// We then need to get the player supposed position at that sensor point and then check for angles
    /// This will make sure we are not at an angle when checking for ledges which can obscure the results of the check
    /// <param name="position"> The base position to check for ledges at</param>
    /// <param name="leftSensorHit"> The left sensor hit data</param>
    /// <param name="rightSensorHit"> The right sensor hit</param>
    private void RecheckLedgePositionsAtBaseAngles(Vector2 position, RaycastHit2D leftSensorHit, RaycastHit2D rightSensorHit)
    {
        Vector2 basePosition = position;

        if (this.currentLeftSensorAngle == 0)
        {
            basePosition = this.CalculatePlayerPositionAtCollisionInfo(leftSensorHit, SensorHitDirectionEnum.Left);
        }
        else if (this.currentRightSesnsorAngle == 0)
        {
            basePosition = this.CalculatePlayerPositionAtCollisionInfo(rightSensorHit, SensorHitDirectionEnum.Right);
        }

        this.isLedgeToTheRight = this.HasLedgeAtHorizontalPosition(rightSensorHit, SensorHitDirectionEnum.Right, basePosition);
        this.isLedgeToTheLeft = this.HasLedgeAtHorizontalPosition(leftSensorHit, SensorHitDirectionEnum.Left, basePosition);
    }

    /// <summary>
    /// Method call to <see cref="HasLedgeAtHorizontalPosition(RaycastHit2D, SensorHitDirectionEnum, Vector2)"/> when no override position is found
    /// </summary>
    private bool HasLedgeAtHorizontalPosition(RaycastHit2D sensorHit, SensorHitDirectionEnum sensorHitData) => this.HasLedgeAtHorizontalPosition(sensorHit, sensorHitData, Vector2.negativeInfinity);

    /// <summary>
    /// Takes in a raycast hit and determines if there is a ladge to the left or right of the player
    /// This is done by faking the player position and performing a ledge check similar to that performed in <see cref="GroundCollisionInfo.CheckForCollision"/> when only one sensor has a hit
    /// <param name="sensorHit"> The raycast sensor hit results</param>
    /// <param name="sensorHitData"> The sensor hit data signifying the direction the hit occured in</param>
    /// <param name="positionOverride"> The position override if the value is <see cref="Vector2.negativeInfinity"/> it calculates the position based on the sensor hit</param>
    /// </summary>
    private bool HasLedgeAtHorizontalPosition(RaycastHit2D sensorHit, SensorHitDirectionEnum sensorHitData, Vector2 positionOverride)
    {
        if (!sensorHit)
        {
            return true;
        }

        CollisionInfoData collisionInfoData = new CollisionInfoData(sensorHit, sensorHitData);
        Vector2 prePosition = this.player.transform.position;
        Vector2 checkPosition = positionOverride.Equals(Vector2.negativeInfinity) ? this.CalculatePlayerPositionAtCollisionInfo(sensorHit, sensorHitData) : positionOverride;
        int direction = sensorHitData == SensorHitDirectionEnum.Right ? 1 : -1;

        this.player.transform.position = checkPosition + new Vector2(GMStageManager.Instance().GetMaxBlockSize() * direction, 0);
        this.ledgeHelper.CheckIfLedgeExceeded(this.player, collisionInfoData, collisionInfoData, this.collisionMask, sensorHitData, sensorHit, this.player.velocity);
        this.player.transform.position = prePosition;

        return !this.ledgeHelper.HasActiveLedge();
    }
}
