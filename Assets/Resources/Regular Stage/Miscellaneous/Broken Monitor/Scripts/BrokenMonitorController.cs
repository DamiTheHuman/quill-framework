using UnityEngine;

public class BrokenMonitorController : MonoBehaviour
{
    [Tooltip("The collision mask for the broken monitor")]
    public LayerMask collisionMask;

    [Tooltip("Half height of the broken monitor")]
    public float monitorHalfHeight = 16;
    [Tooltip("The current velocity of the broken monitor")]
    public Vector2 monitorVelocity;
    [Tooltip("The force that pushes the broken monitor towards the ground")]
    public float monitorGravity = 0.2734375f;
    [Tooltip("Whether the broken monitor is grounded or not")]
    public bool grounded = false;

    [Tooltip("The Colour of the ray casted"), FirstFoldOutItem("Debug Color"), LastFoldoutItem()]
    public Color debugColor = Color.red;

    private void Start() => this.grounded = false;
    private void FixedUpdate()
    {
        this.MoveAndCollide(this.monitorVelocity);
        this.ApplyGravity();
    }

    /// <summary>
    /// Move the sensors in the direction of the monitors velocity and stick to ground
    /// <param name="velocity">The velocity to the move the object in  </param>
    /// </summary>
    private void MoveAndCollide(Vector2 velocity)
    {
        if (this.grounded == false)
        {
            this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the monitor by its velocity
            this.StickToGround(this.transform.position);
        }
    }

    /// <summary>
    /// Apply gravity to the monitors y velocity
    /// </summary>
    private void ApplyGravity()
    {
        if (this.grounded == false)
        {
            this.monitorVelocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.monitorGravity);
        }
    }

    /// <summary>
    /// Sticks the monitor to the top of anything considered ground
    /// <param name="position">The current position of the monitor </param>
    /// </summary>
    private void StickToGround(Vector2 position)
    {
        Vector2 direction = Vector2.down;
        float distance = this.monitorHalfHeight;
        Vector2 monitorGroundSensorPosition = position;
        RaycastHit2D monitorGroundSensor = Physics2D.Raycast(monitorGroundSensorPosition, direction, distance, this.collisionMask);
        Debug.DrawLine(monitorGroundSensorPosition, monitorGroundSensorPosition + (direction * distance), this.debugColor);

        if (monitorGroundSensor)
        {
            float angleInRadians = General.Vector2ToAngle(monitorGroundSensor.normal);
            position.y = monitorGroundSensor.point.y + (this.monitorHalfHeight * Mathf.Cos(angleInRadians));
            //As long as the object collided with is not the player  solid box the monitor is sucessfully grounded
            this.grounded = true;
            this.monitorVelocity.y = 0;
            SolidContactGimmick solidContactGimmick = monitorGroundSensor.transform.GetComponent<SolidContactGimmick>();
            if (solidContactGimmick != null && solidContactGimmick.solidGimmickType == SolidContactGimmickType.MovingPlatform) //Contact with a moving platform
            {
                this.transform.parent = solidContactGimmick.transform;
            }
        }

        this.transform.position = position;
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
