using UnityEngine;
/// <summary>
/// Data pertaining to a singular sensor object and how it is cast
/// </summary>
[System.Serializable]
public class SensorData
{
    [SerializeField, Tooltip("The angle in radians")]
    private float angleInRadians = 0;
    [SerializeField, Tooltip("The cast angle")]
    private float castAngle = 0;
    [SerializeField, Tooltip("The cast direction")]
    private Vector2 castDirection = Vector2.zero;
    [SerializeField, Tooltip("The raycast distance")]
    private float castDistance;

    public SensorData(float angleInRadians, float angleOffset, float castDirection, float castDistance)
    {
        this.angleInRadians = angleInRadians;
        this.castAngle = (angleInRadians * Mathf.Rad2Deg) + angleOffset;
        this.castDirection = General.AngleToVector((castDirection + angleOffset) * Mathf.Deg2Rad);
        this.castDistance = castDistance;
    }

    /// <summary>
    /// Get the angle in radians
    /// </summary>
    public float GetAngleInRadians() => this.angleInRadians;

    /// <summary>
    /// Get the cast direction
    /// </summary>
    public Vector2 GetCastDirection() => this.castDirection;

    /// <summary>
    /// Get the cast distance
    /// </summary>
    public float GetCastDistance() => this.castDistance;
}
