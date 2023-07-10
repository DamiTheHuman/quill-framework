using System.Collections;
using UnityEngine;
/// <summary>
/// A simple motobug energy which moves only when on screen and based on the terrian its currently
/// </summary
public class MotobugController : BadnikController
{
    [SerializeField]
    private Animator animator;
    private IEnumerator motoBugWaitCoroutine;
    private IEnumerator spawnExhaustCoroutine;
    [Tooltip("The active state of the moto bug"), FirstFoldOutItem("Motobug Info"), SerializeField]
    private MotobugMode currentMotoBugMode = MotobugMode.Walking;
    [Tooltip("What the motobug can currently collide with"), SerializeField]
    private LayerMask collisionMask = new LayerMask();
    [Tooltip("The current motobug body width radius"), SerializeField]
    private float currentBodyWidthRadius = 16;
    [Tooltip("The current motobug body height radius"), SerializeField]
    private float currentBodyHeightRadius = 16;
    [Tooltip("How much the senser length is extended by from the body width radius"), LastFoldoutItem(), SerializeField]
    private float sensorExtension = 16;

    [Tooltip("The current ground collision info"), FirstFoldOutItem("Collision Information"), LastFoldoutItem(), SerializeField]
    private CollisionInfoData currentGroundInfo;

    [Tooltip("How much the motobug waits before turning around"), FirstFoldOutItem("Movement Info"), SerializeField]
    private float waitTimeInSteps = 60;
    [Tooltip("The walk speed of the motobug"), LastFoldoutItem(), SerializeField]
    private float walkSpeed = 1f;

    [Tooltip("The left limit of the motobug"), FirstFoldOutItem("Moto Bug Boundariees"), SerializeField]
    private Vector2 leftLimit = new Vector2(100, 0);
    [Tooltip("The right limit of the motobug"), LastFoldoutItem(), SerializeField]
    private Vector2 rightLimit = new Vector2(100, 0);

    [Tooltip("The position smoke exhaust will be spawned at"), FirstFoldOutItem("Smoke Exhaust Parameters"), SerializeField]
    private Transform exhaustSpawnPosition = null;
    [Tooltip("How frequent smoke ehaust will be spawned"), LastFoldoutItem(), SerializeField]
    private float exhaustSpawnFrequencyInSteps = 30f;

    [FirstFoldOutItem("Sensor Colours"), SerializeField]
    private Color debugColor = Color.red;
    [SerializeField]
    public Color leftSensorColour = General.RGBToColour(0, 240, 0);
    [LastFoldoutItem(), SerializeField]
    public Color rightSensorColour = General.RGBToColour(56, 255, 162);

    public override void Reset()
    {
        base.Reset();
        this.animator = this.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.animator == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate()
    {
        if (this.currentMotoBugMode == MotobugMode.Walking && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer))
        {
            this.MoveAndCollide(this.velocity);
            this.ApplyAccelerationAndSlopeMovement();
            this.UpdateDirection();

        }

        this.CheckSpawnExhaust();
        this.animator.SetInteger("State", (int)this.currentMotoBugMode);
    }

    /// <summary>
    /// Moves the motogbugs in the direction of its current velocity
    /// <param name="velocity">The motobugmeats current velocity</param>
    /// </summary>
    private void MoveAndCollide(Vector2 velocity)
    {
        this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the motobugmeat by the current velocity
        this.ManageGroundCollisions(this.transform.position);

        if (this.currentGroundInfo.sensorHitData != SensorHitDirectionEnum.None)
        {
            this.StickToGround(this.transform.position);
        }
    }

    /// <summary>
    /// Manages the ground information based on the terrain the motobug is currently on
    /// <param name="position"> The current position of the motobug</param>
    /// </summary>
    private void ManageGroundCollisions(Vector2 position)
    {
        float distance = this.currentBodyHeightRadius + this.sensorExtension; //How far the ground sensors go 
        SensorData sensorData = new SensorData(this.currentGroundInfo.GetAngleInRadians(), -90, this.currentGroundInfo.GetAngleInDegrees(), distance);

        Vector2 leftSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(-this.currentBodyWidthRadius, 0)); // A - Left Ground Sensor
        RaycastHit2D leftSensor = Physics2D.Raycast(leftSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(leftSensorPosition, leftSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColour);

        Vector2 rightSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(this.currentBodyWidthRadius, 0)); // B - Right Ground Sensor
        RaycastHit2D rightSensor = Physics2D.Raycast(rightSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(rightSensorPosition, rightSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.rightSensorColour);

        if (leftSensor && rightSensor) //When booth sensors hit
        {
            this.currentGroundInfo.sensorHitData = SensorHitDirectionEnum.Both;
            float atanAngleInRadians = Mathf.Atan2(rightSensor.point.y - leftSensor.point.y, rightSensor.point.x - leftSensor.point.x);
            float atanAngleInDegrees = Mathf.Rad2Deg * atanAngleInRadians;
            atanAngleInDegrees = atanAngleInDegrees < 0 ? atanAngleInDegrees += 360 : atanAngleInDegrees;
            this.currentGroundInfo.SetAngleInDegrees(General.RoundToDecimalPlaces(atanAngleInDegrees));
        }
        else if (leftSensor && rightSensor == false)
        {
            this.currentGroundInfo = new CollisionInfoData(leftSensor, SensorHitDirectionEnum.Left);
        }
        else if (rightSensor && leftSensor == false)
        {
            this.currentGroundInfo = new CollisionInfoData(rightSensor, SensorHitDirectionEnum.Right);
        }
        else
        {
            this.currentGroundInfo = new CollisionInfoData();
        }
    }

    /// <summary>
    /// Ensure that the motobug is untop of the terrain at all times
    /// <param name="position"> The current position of the player</param>
    /// </summary>
    public void StickToGround(Vector2 position)
    {
        float distance = this.currentBodyHeightRadius + this.sensorExtension; //How far the ground sensors go 
        SensorData sensorData = new SensorData(this.currentGroundInfo.GetAngleInRadians(), -90, this.currentGroundInfo.GetAngleInDegrees(), distance);
        float rayHorizontalOffset = 0;

        Vector2 stickSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(rayHorizontalOffset, 0));
        RaycastHit2D stickSensor = Physics2D.Raycast(stickSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(stickSensorPosition, stickSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColour);

        if (stickSensor)
        {
            position.x = stickSensor.point.x - (this.currentBodyHeightRadius * Mathf.Sin(this.currentGroundInfo.GetAngleInRadians()));
            position.y = stickSensor.point.y + (this.currentBodyHeightRadius * Mathf.Cos(this.currentGroundInfo.GetAngleInRadians()));

            if (rayHorizontalOffset != 0)
            {
                position -= new Vector2(rayHorizontalOffset * Mathf.Cos(this.currentGroundInfo.GetAngleInRadians()), rayHorizontalOffset * Mathf.Sin(this.currentGroundInfo.GetAngleInRadians()));
            }
        }

        this.transform.position = position; //Move the player object
    }

    /// <summary>
    /// Apply acceleration to the motobugs velocity relative to the terrain
    /// </summary>
    private void ApplyAccelerationAndSlopeMovement()
    {
        float groundVelocity = this.walkSpeed * this.currentDirection;
        this.velocity.x = groundVelocity * Mathf.Cos(this.currentGroundInfo.GetAngleInRadians());
        this.velocity.y = groundVelocity * Mathf.Sin(this.currentGroundInfo.GetAngleInRadians());
    }

    /// <summary>
    /// Update the direction of the motobugmeat
    /// </summary>
    private void UpdateDirection()
    {
        if ((this.transform.position.x <= this.startPosition.x - this.leftLimit.x + this.currentBodyWidthRadius && this.currentDirection == -1) || (this.transform.position.x >= this.startPosition.x + this.rightLimit.x - this.currentBodyWidthRadius && this.currentDirection == 1))
        {
            this.velocity = Vector2.zero;
            this.Flip();
        }
    }

    /// <summary>
    /// Turn the motobug around after the set time
    /// </summary>
    private void Flip()
    {
        if (this.motoBugWaitCoroutine == null)
        {
            this.motoBugWaitCoroutine = this.MotoBugWait();
            this.StartCoroutine(this.motoBugWaitCoroutine);
        }

        this.SetMotoBugMode(MotobugMode.Idle);
    }

    /// <summary>
    /// Sets the new state for the motobug
    /// <param name="motoBugMode">The new state of the motobug</param>
    /// </summary>
    public void SetMotoBugMode(MotobugMode motoBugMode) => this.currentMotoBugMode = motoBugMode;

    /// <summary>
    /// Actions performed when the turn around animation is completed
    /// This is called via animator animation events
    /// </summary>
    private void TurnAroundEnd()
    {
        this.SetMotoBugMode(MotobugMode.Walking);
        this.currentDirection = this.currentDirection == 1 ? -1 : 1;
        this.transform.localScale = new Vector3(this.currentDirection, 1, 1);
        this.motoBugWaitCoroutine = null;
    }

    /// <summary>
    /// Make the motobug wait for the specified amount of time before turning and moving towards its new direction
    /// </summary>
    private IEnumerator MotoBugWait()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.waitTimeInSteps));
        this.SetMotoBugMode(MotobugMode.Turning);

        yield return null;
    }

    /// <summary>
    /// Checks whether to spawn smoke exhaust depending on the current state of the motobug
    /// </summary>
    private void CheckSpawnExhaust()
    {
        if (this.currentMotoBugMode == MotobugMode.Walking && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer))
        {
            if (this.spawnExhaustCoroutine == null)
            {
                this.spawnExhaustCoroutine = this.SpawnExhaust();
                this.StartCoroutine(this.spawnExhaustCoroutine);
            }
        }
        else if (this.spawnExhaustCoroutine != null)
        {
            this.StopCoroutine(this.spawnExhaustCoroutine);
            this.spawnExhaustCoroutine = null;
        }
    }

    /// <summary>
    /// Make the motobug wait for the specified amount of time before turning and moving towards its new direction
    /// </summary>
    private IEnumerator SpawnExhaust()
    {
        while (true)
        {
            yield return new WaitForSeconds(General.StepsToSeconds(this.exhaustSpawnFrequencyInSteps));
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.MotobugExhaust, this.exhaustSpawnPosition.position);

        }
    }

    private void OnValidate() => this.currentDirection = (int)Mathf.Sign(this.transform.localScale.x);
    private void OnDrawGizmos()
    {
        Vector2 debugPosition = Application.isPlaying ? (Vector3)this.startPosition : this.transform.position;
        Vector2 pos1 = debugPosition;
        Vector2 pos2 = debugPosition;
        Gizmos.color = this.debugColor;

        pos1.x -= this.leftLimit.x;
        pos2.x += this.rightLimit.x;
        Gizmos.DrawLine(pos1, pos2);
        GizmosExtra.Draw2DArrow(pos1, 90);
        GizmosExtra.Draw2DArrow(pos2, 270);

        if (Application.isPlaying == false)
        {
            //Draw Debug Sensors
            float currentAngle = this.transform.eulerAngles.z;
            Vector2 position = this.transform.position;
            float distance = this.currentBodyHeightRadius + this.sensorExtension; //How far the Vertical Sensors go
            SensorData groundSensorData = new SensorData(currentAngle * Mathf.Deg2Rad, -90, currentAngle, distance);

            Vector2 leftSensorPosition = General.CalculateAngledObjectPosition(position, groundSensorData.GetAngleInRadians(), new Vector2(-this.currentBodyWidthRadius, 0)); // A - Left Ground Sensor
            Debug.DrawLine(leftSensorPosition, leftSensorPosition + (groundSensorData.GetCastDirection() * groundSensorData.GetCastDistance()), this.leftSensorColour);

            Vector2 rightSensorPosition = General.CalculateAngledObjectPosition(position, groundSensorData.GetAngleInRadians(), new Vector2(this.currentBodyWidthRadius, 0)); // B - Right Ground Sensor
            Debug.DrawLine(rightSensorPosition, rightSensorPosition + (groundSensorData.GetCastDirection() * groundSensorData.GetCastDistance()), this.rightSensorColour);
        }
    }
}
