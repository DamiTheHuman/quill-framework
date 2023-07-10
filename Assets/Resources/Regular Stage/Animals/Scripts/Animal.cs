using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// Parent class for animal objects
///  Original Author - Nihil [Core Framework]
/// </summary>
public class Animal : MonoBehaviour, IPooledObject
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Animator animator;
    [LastFoldoutItem, SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("The collision mask of the animal object")]
    private LayerMask collisionMask = new LayerMask();
    [Tooltip("The current state of the animals"), FirstFoldOutItem("Animal Information")]
    public AnimalState animalState = AnimalState.Spawned;
    [SerializeField, Tooltip("The body width radius of the animal")]
    private float animalBodyWidthRadius = 4;
    [SerializeField]
    [Tooltip("The body height radius of the animal")]
    private float animalBodyHeightRadius = 10;
    [Tooltip("The initial direction the animal is currently facing"), LastFoldoutItem()]
    public int directionFacing = 1;

    [Tooltip("The current velocity of the animal"), FirstFoldOutItem("Animal Movement")]
    public Vector2 velocity;
    [Tooltip("The velocity given to the animal on initial spawn")]
    public Vector2 startVelocity = new Vector2(0, 6);
    [SerializeField, Tooltip("The force used to bring the animal to the ground")]
    private float gravity = 0.15f;
    [Tooltip("A flag that determines whether to apply gravity to the object")]
    public bool applyGravity = true;
    [Tooltip("The acceleration applied to the animal based on their movement condition"), LastFoldoutItem()]
    public Vector2 acceleration = new Vector2(2, 3);

    [SerializeField, FirstFoldOutItem("Debug Colors")]
    private Color leftSensorColor = Color.red;
    [SerializeField, LastFoldoutItem()]
    private Color rightSensorColor = Color.green;

    public virtual void Reset()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (this.animator == null)
        {
            this.Reset();
        }
    }

    public void OnObjectSpawn()
    {
        this.velocity = this.startVelocity;
        this.applyGravity = true;
        this.animalState = AnimalState.Spawned;
        this.UpdateAnimationState(0);
    }

    public virtual void FixedUpdate()
    {
        this.MoveAndCollide(this.velocity);

        if (this.applyGravity)
        {
            this.ApplyGravity();
        }

        this.UpdateAnimalDirection();
        this.DisableAnimal();
    }

    /// <summary>
    /// Apply a vertical force that moves the animal towards the ground 
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.gravity);

    /// <summary>
    /// Move the animal in the direction of the animals current velocity
    /// <param name="velocity">The players current velocity</param>
    /// </summary>
    private void MoveAndCollide(Vector2 velocity)
    {
        this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);

        if (velocity.y <= 0)
        {
            this.GroundCollisionCheck(this.transform.position);
        }
    }

    /// <summary>
    /// Performs collision detection based on what is beneath the animal
    /// <param name="position">The current position of the animal</param>
    /// </summary>
    private void GroundCollisionCheck(Vector3 position)
    {
        float distance = this.animalBodyHeightRadius; //How far the primary ground sensors go
        SensorData sensorData = new SensorData(0, -90, 0, distance);
        Vector2 aSensorPosition = this.transform.position - new Vector3(this.animalBodyWidthRadius, 0); //Left Ground Sensor
        RaycastHit2D aSensor = Physics2D.Raycast(aSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.collisionMask);
        Debug.DrawLine(aSensorPosition, aSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColor);

        Vector2 bSensorPosition = this.transform.position + new Vector3(this.animalBodyWidthRadius, 0);//Right Ground Sensor
        RaycastHit2D bSensor = Physics2D.Raycast(bSensorPosition, sensorData.GetCastDirection(), distance, this.collisionMask);
        Debug.DrawLine(bSensorPosition, bSensorPosition + (sensorData.GetCastDirection() * distance), this.rightSensorColor);

        if (aSensor || bSensor)
        {
            this.velocity.y = 0;
            this.TriggerGroundAction();
        }
    }

    /// <summary>
    /// The action called when the animals sensors detect the ground
    /// </summary>
    public virtual void TriggerGroundAction()
    {
        if (this.animalState == AnimalState.Spawned)
        {
            this.animalState = AnimalState.Action;
            this.OnInitialGroundContact();
            this.UpdateAnimationState(1);
            this.CalculateRandomDirection();
        }
    }

    /// <summary>
    /// The first time the animal interacts with the ground
    /// </summary>
    public virtual void OnInitialGroundContact() { }
    /// <summary>
    /// Calculate the random direction to move the animal in
    /// </summary>
    public void CalculateRandomDirection()
    {
        this.directionFacing = Mathf.Clamp(Random.Range(0, 2) + 1, 1, 2);

        if (this.directionFacing == 2)
        {
            this.directionFacing = -1;
        }
    }

    /// <summary>
    /// Destroy the animal when it  is out of the camera's horizontal field of view
    /// </summary>
    public virtual void DisableAnimal()
    {
        if (HedgehogCamera.Instance().PositionIsLeftOfCmaeraView(this.spriteRenderer.bounds.min) || HedgehogCamera.Instance().PositionIsRightOfCameraView(this.spriteRenderer.bounds.max))
        {
            this.gameObject.SetActive(false);//Disable;
        }
    }

    /// <summary>
    /// Update the animals animation state
    /// </summary>
    public void UpdateAnimationState(int value) => this.animator.SetInteger("State", value);

    /// <summary>
    /// Sets the direcition the animal should look in
    /// </summary>
    public virtual void UpdateAnimalDirection()
    {
        Vector3 scale = Vector3.one;
        scale.x *= this.directionFacing;
        this.transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        float distance = this.animalBodyHeightRadius; //How far the primary ground sensors go
        SensorData sensorData = new SensorData(0, -90, 0, distance);

        Vector2 aSensorPosition = this.transform.position - new Vector3(this.animalBodyWidthRadius, 0); //Left Ground Sensor
        Debug.DrawLine(aSensorPosition, aSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.leftSensorColor);

        Vector2 bSensorPosition = this.transform.position + new Vector3(this.animalBodyWidthRadius, 0); //Right Ground Sensor
        Debug.DrawLine(bSensorPosition, bSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.rightSensorColor);
    }

}
