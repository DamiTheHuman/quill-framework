using UnityEngine;
/// <summary>
/// Helper class to find potential ledges that the player will fall off when exceeded
/// </summary>
[System.Serializable]
public class LedgeHelper
{
    [Tooltip("The current direction of the ledge")]
    public LedgeDirection ledgeDirection = LedgeDirection.None;
    [SerializeField, Tooltip("The current position of what has been identified as a ledge")]
    private Vector2 activeLedgePosition;
    [SerializeField, FirstFoldOutItem("Previous info"), Tooltip("The previous position of what has been identified as a ledge")]
    private Vector2 previousLedgePosition;
    [SerializeField, Tooltip("Any angle above this number is considered a ledge when either the player or ledge is at 0 degrees")]
    private float maxLedgeAngleLeniency = 65f;

    public void Reset()
    {
        this.previousLedgePosition = this.activeLedgePosition;
        this.activeLedgePosition = Vector2.negativeInfinity;
        this.ledgeDirection = LedgeDirection.None;
    }

    /// <summary>
    /// Checks if the sensor collision point exceeds the position of the ledge
    /// <param name=sensorHitData"> The collision type of the sensor being checked</param>
    /// <param name="collisionPoint"> The collision point of the sensors to be checked against the active Ledge Point</param>
    /// </summary>
    public bool CheckIfLedgeExceeded(Player player, CollisionInfoData collisionInfo, CollisionInfoData previousCollisionInfo, LayerMask collisionMask, SensorHitDirectionEnum sensorHitData, RaycastHit2D collisionPoint, Vector2 velocity)
    {
        bool validLedge = false;
        this.activeLedgePosition = this.CheckForLedges(player, collisionInfo, previousCollisionInfo, collisionMask, sensorHitData, ref validLedge);

        if (validLedge == false)
        {
            return false;
        }

        //If there is no ledge return false
        bool ledgePointExceeded = false;
        float xSignVelocity = velocity.x == 0 || Mathf.Abs(velocity.x) < Mathf.Abs(velocity.y) ? 0 : Mathf.Sign(General.RoundToDecimalPlaces(velocity.x));
        float ySignVelocity = velocity.y == 0 || Mathf.Abs(velocity.y) < Mathf.Abs(velocity.x) ? 0 : Mathf.Sign(General.RoundToDecimalPlaces(velocity.y));

        switch (sensorHitData)
        {
            case SensorHitDirectionEnum.Right:
                if ((collisionPoint.point.x < this.activeLedgePosition.x && xSignVelocity == -1) || (collisionPoint.point.y > this.activeLedgePosition.y && ySignVelocity == 1))
                {
                    ledgePointExceeded = true;
                }
                //Due to the length of the sensors its is possible while on a slope for the player to be angled and detect a ledge at 0 which is in anomaly and should be counted as exceeding the ledge
                else if (player.currentGroundMode == GroundMode.LeftWall && General.RoundToDecimalPlaces(General.Vector2ToAngle(collisionPoint.normal)) == 0)
                {
                    ledgePointExceeded = true;
                }

                break;
            case SensorHitDirectionEnum.Left:
                if ((collisionPoint.point.x > this.activeLedgePosition.x && xSignVelocity == 1) || (collisionPoint.point.y < this.activeLedgePosition.y && ySignVelocity == -1))
                {
                    ledgePointExceeded = true;
                }
                //Due to the length of the sensors its is possible while on a slope for the player to be angled and detect a ledge at 0 which is in anomaly and should be counted as exceeding the ledge
                else if (player.currentGroundMode == GroundMode.RightWall && General.RoundToDecimalPlaces(General.Vector2ToAngle(collisionPoint.normal)) == 0)
                {
                    ledgePointExceeded = true;
                }

                break;
            case SensorHitDirectionEnum.None:
            case SensorHitDirectionEnum.Both:
            default:
                break;
        }

        if (ledgePointExceeded)
        {
            this.activeLedgePosition = Vector2.negativeInfinity;
        }

        return ledgePointExceeded;
    }

    /// <summary>
    /// Checks for the position of ledges by the players previous position
    /// ** LOGIC **
    /// To check if the player is about to run off an ledge when only a singular sensor is hit cast a ray from the opposite side of the current ray to the active ray
    /// This assumes that the start sensor ray will run into a ledge at some point as it reaches the initial spawn point
    /// Once the point is hit a ledge is found and its true position can be found by casting another to from the start Positions Y and ledge position x towards the ledge position basically intersecting this is important for angles
    /// After this once the player exceeds the ledge direction he has fallen off the ledge
    /// An alternative is to use no sensor extensions but this can be inconsistent or set a trigger on the slope to set the sensor extensios to 0 when on a specific ramp
    /// But I believe this physics alternative is better as it keeps everything in one place
    /// ** END LOGIC **
    /// <param name=sensorHitData"> The collision type of the sensor being checked</param>
    /// </summary>
    private Vector2 CheckForLedges(Player player, CollisionInfoData groundInfo, CollisionInfoData previousCollisionInfo, LayerMask collisionMask, SensorHitDirectionEnum sensorHitData, ref bool validLedge)
    {
        Sensors sensors = player.GetSensors();
        //Check if the player is on a curve and not a ledge based on off a middle sensor hits
        float distance2 = sensors.currentPixelPivotPoint + sensors.characterBuild.sensorExtension;
        SensorData curveSensorData = new SensorData(groundInfo.GetAngleInRadians(), -90, groundInfo.GetAngleInDegrees(), distance2);
        Vector2 curveSensorPosition = General.CalculateAngledObjectPosition(sensors.transform.position, curveSensorData.GetAngleInRadians(), new Vector2(0, 0));
        RaycastHit2D curveSensor = Physics2D.Raycast(curveSensorPosition, curveSensorData.GetCastDirection(), curveSensorData.GetCastDistance(), collisionMask);
        Debug.DrawLine(curveSensorPosition, curveSensorPosition + (curveSensorData.GetCastDirection() * curveSensorData.GetCastDistance()), Color.grey);

        if (curveSensor)
        {
            validLedge = false;

            return Vector2.negativeInfinity;
        }

        Vector2 position = sensors.previousPosition;
        Vector2 direction = General.AngleToVector(previousCollisionInfo.GetAngleInDegrees() * Mathf.Deg2Rad); //Set the ray direction
        float distance = sensors.characterBuild.bodyWidthRadius * 2;
        float ledgeSensorMinOffset = -(sensors.characterBuild.playerPixelPivotPoint + 0.01f);
        Vector2 ledgeSensorStartPosition = General.CalculateAngledObjectPosition(position, previousCollisionInfo.GetAngleInRadians(), new Vector2(-sensors.characterBuild.bodyWidthRadius, ledgeSensorMinOffset));

        RaycastHit2D ledgeSensor = sensorHitData == SensorHitDirectionEnum.Right ?
            Physics2D.Linecast(ledgeSensorStartPosition, ledgeSensorStartPosition + (direction * distance), collisionMask) :
            Physics2D.Linecast(ledgeSensorStartPosition + (direction * distance), ledgeSensorStartPosition, collisionMask);
        Debug.DrawLine(ledgeSensorStartPosition, ledgeSensorStartPosition + (direction * distance), Color.black);

        //Traverse eastwards or westwards from hit point based on the hit data and angle of the ledge
        float angle = General.RoundToDecimalPlaces(General.Vector2ToAngle(ledgeSensor.normal) * Mathf.Rad2Deg);
        SensorData sensorData = new SensorData(angle * Mathf.Deg2Rad, 270, angle, distance);
        RaycastHit2D trueLedgePosition = sensorHitData == SensorHitDirectionEnum.Right ?
          Physics2D.Linecast(ledgeSensor.point + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), ledgeSensor.point, collisionMask) :
          Physics2D.Linecast(ledgeSensor.point, ledgeSensor.point + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), collisionMask);

        Vector2 activeLedgePosition = new Vector2(ledgeSensor.point.x, ledgeSensor.point.y);

        validLedge = trueLedgePosition;

        if (validLedge) //Allow some leniency when coming in contact with ledges
        {
            float ledgeAngle = General.RoundToDecimalPlaces(General.Vector2ToAngle(ledgeSensor.normal) * Mathf.Rad2Deg);

            if (ledgeAngle == 0)
            {
                if (previousCollisionInfo.GetAngleInDegrees() < this.maxLedgeAngleLeniency || previousCollisionInfo.GetAngleInDegrees() > 360 - this.maxLedgeAngleLeniency)
                {
                    activeLedgePosition = Vector2.negativeInfinity;
                    validLedge = false;//Falsify the ledge data
                }
            }
            else if (previousCollisionInfo.GetAngleInDegrees() == 0)
            {
                if (ledgeAngle < this.maxLedgeAngleLeniency || ledgeAngle > 360 - this.maxLedgeAngleLeniency)
                {
                    activeLedgePosition = Vector2.negativeInfinity;
                    validLedge = false;//Falsify the ledge data
                }
            }
        }

        this.SetActiveLedgePosition(activeLedgePosition);
        this.UpdateLedgeDirection(player);

        return this.activeLedgePosition;
    }

    /// <summary>
    /// Update the ledge direction
    /// </summary>
    private void UpdateLedgeDirection(Player player)
    {
        if (this.activeLedgePosition.Equals(Vector2.negativeInfinity))
        {
            this.ledgeDirection = LedgeDirection.None;
        }
        else if (activeLedgePosition.x < player.transform.position.x)
        {
            this.ledgeDirection = LedgeDirection.Left;
        }
        else
        {
            this.ledgeDirection = LedgeDirection.Right;
        }
    }

    /// <summary>
    /// Gets the active ledge position
    /// </summary>
    private void SetActiveLedgePosition(Vector2 activeLedgePosition)
    {
        this.previousLedgePosition = activeLedgePosition;
        this.activeLedgePosition = activeLedgePosition;
    }

    /// <summary>
    /// Gets the active ledge position
    /// </summary>
    public Vector2 GetActiveLedgePosition() => this.activeLedgePosition;

    /// <summary>
    /// Set the position of the active ledge
    /// </summary>
    public void SetActiveLedgePositon(Vector2 activeLedgePosition) => this.activeLedgePosition = activeLedgePosition;

    /// <summary>
    /// Checks if the ledge helper has an active ledge
    /// </summary>
    public bool HasActiveLedge() => this.GetActiveLedgePosition().Equals(Vector2.negativeInfinity);
}
