using UnityEngine;

/// <summary>
/// Simple controller for the tornada
/// </summary>
public class TornadoController : PlatformController
{

    [SerializeField, Tooltip("The Animator for the Tornado's body")]
    private Animator animator;
    [SerializeField, Tooltip("The Animator for the pilot")]
    private Animator pilotAnimator;
    [SerializeField, Tooltip("The flame object ")]
    private GameObject flame;
    [SerializeField, Tooltip("The Velocity of the tornado")]
    private Vector2 velocity;

    [SerializeField]
    private TornadoMovement tornadoMovement = TornadoMovement.Straight;
    [SerializeField]
    private bool showFlame;
    [SerializeField]
    private float delta;
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float range = 4;

    private int velocityXHash;
    private int velocityYHash;


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

        this.velocityXHash = Animator.StringToHash("VelocityX");
        this.velocityYHash = Animator.StringToHash("VelocityY");
    }

    private void FixedUpdate()
    {
        switch (this.tornadoMovement)
        {
            case TornadoMovement.Straight:
                this.Move(this.velocity);
                break;
            case TornadoMovement.SineWave:
                this.SineWaveMovement();
                break;
        }

        if (this.flame != null)
        {
            this.flame.SetActive(this.showFlame);
        }

        this.UpdateAnimatorParameters();
        this.SyncChildPositions();
    }

    /// <summary>
    /// Adjusts the tornados Y velocity in sine wave like manner
    /// </summary>
    private void SineWaveMovement()
    {
        Vector2 position = this.transform.position;
        position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.range);
        this.delta += this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.velocity.y = this.transform.position.y - position.y;
        this.transform.position = position;
        this.Move(new Vector2(this.velocity.x, 0));
    }

    /// <summary>
    /// Update the animator parameters based on the velocity
    /// </summary>
    private void UpdateAnimatorParameters()
    {
        this.animator.SetFloat(this.velocityXHash, this.velocity.x);
        this.animator.SetFloat(this.velocityYHash, this.velocity.y);
    }

    /// <summary>
    /// Moves the tornado in the direction of set velocity
    /// <param name="velocity">The tornado's current  velocity</param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);

}
