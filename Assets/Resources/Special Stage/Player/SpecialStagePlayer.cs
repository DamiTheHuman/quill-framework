using UnityEngine;
/// <summary>
/// Duplicate of <see cref="Player"/> but for special stages
/// </summary>
[RequireComponent(typeof(SpecialStageSensors), typeof(InputManager), typeof(SpecialStageAnimatorManager))]
public class SpecialStagePlayer : MonoBehaviour
{
    [Help("This boolean is used mainly to define a player object spawned by the GMCharacterManager script")]
    [SerializeField, Tooltip("Sets whether the player object should be uninteractive")]
    private bool sleep = false;
    [Help("You can double click the field to change the player physics info, in this class we mainly just need the player tag")]
    [SerializeField]
    private CharacterPhysicsScriptableObject physicsInfo = null;
    [FirstFoldOutItem("Dependencies"), SerializeField]
    private SpecialStageSensors sensors;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private SpecialStageHealthManager healthManager;
    [SerializeField]
    private SpecialStageSpriteController spriteController;
    [SerializeField]
    private SpecialStageAnimatorManager animatorManager;
    [SerializeField]
    private SpecialStageActionManager actionManager;
    [SerializeField, LastFoldoutItem()]
    private SpecialStageSolidBoxController solidBoxController;
    [SerializeField]
    private bool attacking = false;
    [Tooltip("The speed the player is moving at")]
    public Vector2 velocity;

    public GroundMode currentGroundMode = GroundMode.Floor;

    /// <summary>
    /// The action audio for the player
    /// </summary>
    [SerializeField]
    private PlayerActionAudio playerActionAudio = null;
    [Tooltip("Whether the player is grounded or not"), SerializeField]
    private bool grounded;

    private void Reset()
    {
        this.sensors = this.GetComponent<SpecialStageSensors>();
        this.inputManager = this.GetComponent<InputManager>();
        this.spriteController = this.GetComponentInChildren<SpecialStageSpriteController>();
        this.animatorManager = this.GetComponent<SpecialStageAnimatorManager>();
        this.actionManager = this.GetComponent<SpecialStageActionManager>();
        this.healthManager = this.GetComponent<SpecialStageHealthManager>();
        this.solidBoxController = this.GetComponentInChildren<SpecialStageSolidBoxController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (this.sensors == null)
        {
            this.Reset();
        }

        if (this.sleep)
        {
            this.gameObject.SetActive(false);

            return;
        }

        this.grounded = true;
    }

    private void Update()
    {
        if (this.sleep)
        {
            this.gameObject.SetActive(false);

            return;
        }
    }

    private void FixedUpdate()
    {
        if (this.sleep)
        {
            return;
        }

        if (GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.EmeraldReached)
        {
            this.velocity = Vector2.zero;
        }

        this.sensors.MoveAndCollide(this.velocity);
        this.transform.position = this.ClampHorizontalPositionToPivotBounds();

        if (this.grounded == false)
        {
            this.ApplyAirHorizontalMovement();
            if (this.CheckForGrounded() == false)
            {
                GMSpecialStageManager.Instance().GetSpecialStageSlider().SetAngle(Mathf.Asin(this.transform.position.x / GMSpecialStageManager.Instance().GetSpecialStageSlider().GetRange()) * Mathf.Rad2Deg);
                this.ApplyGravity();
            }
        }

        this.animatorManager.UpdatePlayerAnimations();
        this.spriteController.UpdatePlayerSpriteLifeCycle();
        this.actionManager.UpdateActionLifeCycle();

        if (this.grounded)
        {
            this.UpdatePlayerGroundMode();
        }
    }

    /// <summary>
    /// Checks if the player is grounded
    /// </summary>
    private bool CheckForGrounded()
    {
        if (Vector3.Distance(this.transform.position, GMSpecialStageManager.Instance().GetSpecialStageSlider().GetPlayerParent().position) < 12 && this.velocity.y > 0)
        {
            this.velocity.y = 0;
            this.grounded = true;
            this.transform.parent = GMSpecialStageManager.Instance().GetSpecialStageSlider().GetPlayerParent();
            this.transform.localPosition = new Vector3();
            this.velocity.x = 0;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Apply a force that pulls the player towards the ground while calculating the angle
    /// </summary>
    private void ApplyGravity() => this.velocity.y += this.physicsInfo.specialStageCurrentGravity;
    /// <summary>
    /// Clamps the players horizontal position to the bounds
    /// </summary>
    private Vector3 ClampHorizontalPositionToPivotBounds()
    {
        Vector3 position = this.transform.position;
        float maxPosX = GMSpecialStageManager.Instance().GetSpecialStageSlider().GetPositionAtAngle(90).x;
        position.x = Mathf.Clamp(position.x, -maxPosX, maxPosX);

        return position;
    }

    /// <summary>
    /// Apply acceleration to the player body while in the air
    /// </summary>
    private void ApplyAirHorizontalMovement() => this.velocity.x = this.IncrementHorizontalVelocity(this.velocity.x, this.physicsInfo.specialStageAirAcceleration, this.physicsInfo.basicTopSpeed);
    /// <summary>
    /// Increments horizontal velocity While the player is in the air
    /// <param name="horizontalVelocity"> The current horizontal velocity to update </param>
    /// <param name="acceleration"> The acceleration used to increment the horizontal velocity </param>
    /// <param name="topSpeed"> The highest velocity that can be attained while using the input to increment speed as long as the top speed isnt exceeded </param>
    /// </summary>
    private float IncrementHorizontalVelocity(float horizontalVelocity, float acceleration, float topSpeed)
    {
        acceleration = GMStageManager.Instance().ConvertToDeltaValue(acceleration);

        if (this.inputManager.GetCurrentInput().x == 1) //As long as the player is not going above top speed i.e by running down a slope already apply acceleration
        {
            horizontalVelocity += acceleration;
            if (horizontalVelocity >= topSpeed)
            {
                horizontalVelocity = topSpeed;
            }
        }
        else if (this.inputManager.GetCurrentInput().x == -1) //As long as the player is not going above top speed i.e by running down a slope already apply acceleration
        {
            horizontalVelocity -= acceleration;
            if (horizontalVelocity <= -topSpeed)
            {
                horizontalVelocity = -topSpeed;
            }
        }

        return horizontalVelocity;
    }

    /// <summary>
    /// Updates the player ground mode based on the current ground info
    /// </summary>
    private void UpdatePlayerGroundMode()
    {
        int mode = this.CalculateGroundMode(GMSpecialStageManager.Instance().GetSpecialStageSlider().GetAngle(), 90);
        this.SetGroundMode((GroundMode)mode);
    }

    /// <summary>
    /// Calculation to find the currect ground mode of the player
    ///EXAMPLE 1 : MODE KEYS when divider is 90 and range is 4 Floor = 0 ,RightWall = 1 ,Ceiling = 2 ,LeftWall = 3 
    /// <param name="angle"> The current angle</param>
    /// <param name="divider"> How many possible dividents from a range of 360</param>
    /// </summary>
    public int CalculateGroundMode(float angle, float divider)
    {
        float range = 360 / divider;
        float mode = Mathf.Round(angle / divider) % range;

        return (int)mode;
    }

    /// <summary>
    /// Get the ground mode
    /// </summary>
    public GroundMode GetGroundMode() => this.currentGroundMode;

    /// <summary>
    /// Set the ground mode
    /// <param name="groundMode"> The new ground mode of the player </param>
    /// </summary>
    public void SetGroundMode(GroundMode groundMode) => this.currentGroundMode = groundMode;

    /// <summary>
    /// Get the grounded state
    /// </summary>
    public bool GetGrounded() => this.grounded;

    /// <summary>
    /// Set the grounded state
    /// <param name="grounded"> The flag for the grounded state</param>
    /// </summary>
    public void SetGrounded(bool grounded) => this.grounded = grounded;

    /// <summary>
    /// Set the attacking of the player
    /// <param name="attacking"> the new attacking state of the player</param>
    /// </summary>
    public void SetAttackingState(bool attacking) => this.attacking = attacking;

    /// <summary>
    /// Get the attacking of the player
    /// </summary>
    public bool GetAttackingState() => this.attacking;

    /// <summary>
    /// Gets the sleep variable
    /// </summary
    public bool GetSleep() => this.sleep;

    /// <summary>
    /// Sets the sleep variable
    /// </summary
    public void SetSleep(bool sleep) => this.sleep = sleep;

    /// <summary>
    /// Get a reference to the players action manager
    /// </summary>
    public SpecialStageActionManager GetActionManager() => this.actionManager;
    /// <summary>
    /// Get a reference to the players animator Manager
    /// </summary>
    public SpecialStageAnimatorManager GetAnimatorManager() => this.animatorManager;
    /// <summary>
    /// Get a reference to the players sprite controller
    /// </summary>
    public SpecialStageSpriteController GetSpriteController() => this.spriteController;
    /// <summary>
    /// Get a reference to the players Input
    /// </summary>
    public InputManager GetInputManager() => this.inputManager;
    /// <summary>
    /// Get a reference to the special stage health manager
    /// </summary>
    public SpecialStageHealthManager GetHealthManager() => this.healthManager;

    /// <summary>
    /// A reference to the audio files relating to the players actions
    /// </summary>
    public PlayerActionAudio GetPlayerActionAudio() => this.playerActionAudio;
    /// <summary>
    /// A reference to the physics setting relating to the player
    /// </summary>
    public CharacterPhysicsScriptableObject GetPlayerPhysicsInfo() => this.physicsInfo;

}
