using UnityEngine;

/// <summary>
/// A class to store the collision info attained from the sensors
/// </summary>
[System.Serializable]
public class CollisionInfoData
{
    [Tooltip("The angle in degrees of the collision point"), SerializeField]
    private float angleInDegrees;
    [Tooltip("Raycast info regarding what was hit"), SerializeField]
    private RaycastHit2D hit;
    [Tooltip("The transform of the sensors hit"), SerializeField]
    private Transform target = null;
    [Tooltip("Info regarding what the sensors hit"), SerializeField]
    public SensorHitDirectionEnum sensorHitData = SensorHitDirectionEnum.None;

    public CollisionInfoData()
    {
        this.hit = new RaycastHit2D();
        this.angleInDegrees = 0;
        this.sensorHitData = SensorHitDirectionEnum.None;
        this.target = null;
    }

    public CollisionInfoData(RaycastHit2D hit)
    {
        this.hit = hit;
        this.angleInDegrees = General.RoundToDecimalPlaces(General.Vector2ToAngle(hit.normal) * Mathf.Rad2Deg);
        this.ClampAngleData();
        this.sensorHitData = SensorHitDirectionEnum.None;
        this.target = hit.collider.transform;
    }

    public CollisionInfoData(RaycastHit2D hit, SensorHitDirectionEnum sensorHitData)
    {
        this.hit = hit;
        this.angleInDegrees = General.RoundToDecimalPlaces(General.Vector2ToAngle(hit.normal) * Mathf.Rad2Deg);
        this.ClampAngleData();
        this.sensorHitData = sensorHitData;
        this.target = hit.collider.transform;
    }

    /// <summary>
    /// Copy core collision from another collisionInfo object
    /// </summary>
    public void SetCollisionInfo(CollisionInfoData collisionInfo)
    {
        this.hit = collisionInfo.GetHit();
        this.angleInDegrees = collisionInfo.GetAngleInDegrees();
        this.sensorHitData = collisionInfo.sensorHitData;
        this.target = collisionInfo.target;
    }

    /// <summary>
    /// Clamps the angle data appropriately
    /// </summary>
    public void ClampAngleData()
    {
        this.angleInDegrees = this.angleInDegrees >= 360 ? this.angleInDegrees - 360 : this.angleInDegrees;
        this.angleInDegrees = this.angleInDegrees < 0 ? this.angleInDegrees + 360 : this.angleInDegrees;
    }

    /// <summary>
    /// Gets the height of our collision based on the raycast
    /// </summary>
    public float GetHeight() => this.hit.point.y;

    /// <summary>
    /// Sets the collision angle in degrees
    /// </summary>
    public void SetAngleInDegrees(float angleInDegrees) => this.angleInDegrees = angleInDegrees;

    /// <summary>
    /// Gets the collision angle in degrees
    /// </summary>
    public float GetAngleInDegrees() => this.angleInDegrees;

    /// <summary>
    /// Gets the collision angle in radians
    /// </summary>
    public float GetAngleInRadians() => this.angleInDegrees * Mathf.Deg2Rad;

    /// <summary>
    /// Get the current raycast hit
    /// </summary>
    public RaycastHit2D GetHit() => this.hit;

    /// <summary>
    /// Set the raycast hit
    /// </summary>
    public void SetRaycast2DHit(RaycastHit2D hit) => this.hit = hit;

    /// <summary>
    /// Gets the sensor hit direction   
    /// </summary>
    public int GetSensorHitDirection()
    {
        int direction = 0;

        direction = this.sensorHitData switch
        {
            SensorHitDirectionEnum.None => 0,
            SensorHitDirectionEnum.Left => -1,
            SensorHitDirectionEnum.Right => 1,
            SensorHitDirectionEnum.Both => 2,
            _ => 0,
        };

        return direction;
    }

    /// <summary>
    /// Gets the the amount of hits or sensors have 
    /// </summary>
    public int GetHitCount()
    {
        int hitCount = 0;

        hitCount = this.sensorHitData switch
        {
            SensorHitDirectionEnum.None => 0,
            SensorHitDirectionEnum.Left => 1,
            SensorHitDirectionEnum.Right => 1,
            SensorHitDirectionEnum.Both => 2,
            _ => 0,
        };

        return hitCount;
    }

}
