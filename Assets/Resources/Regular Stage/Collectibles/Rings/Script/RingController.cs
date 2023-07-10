using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The controller class that handles the players interaction witht he ring object
/// </summary>
public class RingController : HitBoxContactEvent, IPooledObject
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField, FirstFoldOutItem("General Ring Information")]
    private RingType currentRingType = RingType.Regular;
    [Tooltip("The amount of rings to add to the counter on retrieval"), SerializeField]
    private int ringValue = 1;
    [Tooltip("The magnetize Ring Id"), SerializeField]
    private int magnetizedRingId = 0;
    [Tooltip("The action that takes place when a magnetized ring is gotten"), SerializeField, IsDisabled]
    private Action onMagnetizedRingGotten;
    [Tooltip("The offsets for the ring sparkles"), SerializeField]
    private float sparkleOffset = 4;
    [LastFoldoutItem(), Tooltip("The audio played when the ring is collected"), SerializeField]
    private AudioClip collectedSound;

    [SerializeField, FirstFoldOutItem("Magnetized Ring Properties")]
    private Transform target;
    [Tooltip("The velocity of the ring"), SerializeField]
    private Vector2 velocity;
    [Tooltip("The acceleration of the ring when moving in the direction of the player"), SerializeField]
    private float followAcceleration = 0.1875f;
    [Tooltip("The acceleration of the ring when trying to catch up with the player"), SerializeField]
    private float catchUpAcceleration = 0.75f;
    private List<float> ringAcceleration = new List<float>();

    [Tooltip("The layers the ring can interact with when going downwards"), SerializeField, FirstFoldOutItem("Spilled Ring Properties")]
    private LayerMask groundCollisionMask = new LayerMask();
    [Tooltip("The layers the ring can interact with when moving horizontally"), SerializeField]
    private LayerMask wallCollisionMask = new LayerMask();
    [Tooltip("The layers the ring can interact with when going upwards"), SerializeField]
    private LayerMask ceilingCollisionMask = new LayerMask();
    [Tooltip("Half of the rings radius"), SerializeField]
    private float halfRingRadius = 8;
    [Tooltip("The rings life span in steps"), SerializeField]
    private float ringLifeSpan = 256;
    [Tooltip("Steps to begin fading out the rings sprite component"), SerializeField]
    private float ringStartFadeTime = 60;

    [Tooltip("The force added to the y velocity every step"), SerializeField]
    private float gravity = 0.09375f;
    [Tooltip("The amount of velocity lost on each bounce whether horizontal or vertical"), SerializeField, LastFoldoutItem()]
    private Vector2 ringBounceLoss = new Vector2(-0.25f, -0.75f);
    [SerializeField, Tooltip("Debug color of the ring")]
    private Color debugColor = new Color(1f, 0.92f, 0.016f, 0.5f);

    public override void Reset()
    {
        base.Reset();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        if (this.spriteRenderer == null)
        {
            this.Reset();
        }
    }

    protected override void Start()
    {
        base.Start();

        this.ringAcceleration.Add(this.catchUpAcceleration);
        this.ringAcceleration.Add(this.followAcceleration);
        this.transform.localScale = this.transform.lossyScale;
        this.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Actions performed when the shard is created
    /// </summary>
    public void OnObjectSpawn()
    {
        if (this.currentRingType == RingType.Spilled)
        {
            this.spriteRenderer.color = new Color(1, 1, 1, 1);
            this.CancelInvoke(nameof(BeginFadeToZero));
            this.StopAllCoroutines();
            this.Invoke(nameof(BeginFadeToZero), General.StepsToSeconds(this.ringLifeSpan - this.ringStartFadeTime));//Keep the ring alive for its life span leaving an extra second to fade away
        }
    }

    private void FixedUpdate()
    {
        if (this.currentRingType == RingType.Magnetized)
        {
            if (this.target != null)
            {
                this.FollowTarget();
            }
        }
        else if (this.currentRingType == RingType.Spilled)
        {
            this.ApplyGravity();
            this.MoveAndCollide();
        }
    }

    /// <summary>
    /// If the player comes in contact and can collect rings grant them the ring
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerContact = false;
        triggerContact = player.GetHealthManager().CanCollectRings();

        return triggerContact;
    }

    /// <summary>
    /// Give the player the ring if the collision check is passed
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.RingCollected(player.transform.position);
    }
    /// <summary>
    /// The set of actions to be performed on ring collections
    /// <param name="playerPosition">The position of the player when they touched the ring to know what side the audio should come from</param>
    /// </summary>
    private void RingCollected(Vector2 playerPosition)
    {
        GMRegularStageScoreManager.Instance().IncrementRingCount(this.ringValue);//add one to the ring count


        float zPosition = this.transform.position.z;
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, (Vector2)this.transform.position)
            .transform.localEulerAngles = Vector3.zero;
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, (Vector2)this.transform.position + new Vector2(UnityEngine.Random.Range(-this.sparkleOffset, this.sparkleOffset), UnityEngine.Random.Range(-this.sparkleOffset, this.sparkleOffset)))
            .transform.localEulerAngles = Vector3.zero;
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, (Vector2)this.transform.position + new Vector2(UnityEngine.Random.Range(-this.sparkleOffset, this.sparkleOffset), UnityEngine.Random.Range(-this.sparkleOffset, this.sparkleOffset)))
            .transform.localEulerAngles = Vector3.zero;

        if (this.currentRingType == RingType.Magnetized)
        {
            this.onMagnetizedRingGotten?.Invoke();//Call the magnetize ring gotten function if its not null
            this.onMagnetizedRingGotten = null;
        }

        OneShotSoundDirection oneShotAudioType = playerPosition.x <= this.transform.position.x ? OneShotSoundDirection.LeftEarOnly : OneShotSoundDirection.RightEarOnly;
        GMAudioManager.Instance().PlayOneShot(this.collectedSound, oneShotAudioType);
        this.gameObject.SetActive(false);//Deactivate instead of destroy
    }

    /// <summary>
    /// Sets the active type of the ring
    /// <param name="ringType">The new type of the ring</param>
    /// </summary>
    public void SetRingType(RingType ringType)
    {
        this.currentRingType = ringType;

        if (this.currentRingType == RingType.Spilled)
        {
            this.OnObjectSpawn();
        }
        else
        {
            this.StopAllCoroutines();
        }
    }

    /// <summary>
    /// Get a reference to the ring type
    /// </summary>
    public RingType GetRingType() => this.currentRingType;

    /// <summary>
    /// Sets the target for the ring to follow
    /// <param name="target">The object the ring should follow </param>
    /// </summary>
    public void SetMagnetizeTarget(Transform target)
    {
        this.target = target;
        this.gameObject.layer = 15;//after being magnetized go back to the default layer
    }

    /// <summary>
    /// Unsets the magnetize state and sets the ring to its spilled state appropriatelt
    /// </summary>
    public void UnmagnetizeRing()
    {
        this.SetRingType(RingType.Spilled);
        this.gameObject.layer = 29;//Move the item back to the collectible layer for remagnetizing
    }

    /// <summary>
    /// Set the actiont that occurs when a ring is magnetized
    /// </summary>
    public void SetOnMagnetizedRingGotten(Action onMagnetizedRingGotten) => this.onMagnetizedRingGotten = onMagnetizedRingGotten;

    /// <summary>
    /// Get the magnetize ring id
    /// </summary>
    public int GetMagnetizeRingId() => this.magnetizedRingId;

    /// <summary>
    /// Set the magnetize ring id
    /// </summary>
    public void SetMagnetizeRingId(int magnetizeId) => this.magnetizedRingId = magnetizeId;

    /// <summary>
    /// Moves the Ring in the direction of the target set
    /// </summary>
    public void FollowTarget()
    {
        //The relative posiiton of the ring against the players position 
        float relativeXPosition = Mathf.Sign(this.target.position.x - this.transform.position.x); //-1 being to the left and 1 being to the right
        float relativeYPosition = Mathf.Sign(this.target.position.y - this.transform.position.y); //-1 being to the bottom and 1 being to the top

        //Checks the relative movement by examining if the ring is going in the direction of the players relative position
        bool relativeXMovement = Mathf.Sign(this.velocity.x) == relativeXPosition;
        bool relativeYMovement = Mathf.Sign(this.velocity.y) == relativeYPosition;

        //Calculate & Apply the velocity
        this.velocity.x += this.ringAcceleration[relativeXMovement ? 1 : 0] * relativeXPosition;
        this.velocity.y += this.ringAcceleration[relativeYMovement ? 1 : 0] * relativeYPosition;

        this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(this.velocity.x, this.velocity.y, 0f);
    }

    /* 2 -- Spilled Ring --*/
    /// <summary>
    /// Sets the velocity of the ring to specified value
    /// <param name="velocity">The new velocity for the ring</param>
    /// </summary
    public void SetVelocity(Vector2 velocity) => this.velocity = velocity;

    /// <summary>
    /// Apply Ring Collisions Detection And Physics
    /// </summary>
    private void MoveAndCollide()
    {
        this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.fixedDeltaTime * new Vector3(this.velocity.x, this.velocity.y, 0);

        if (this.velocity.x > 0)
        {
            this.HandleAllCollisions(this.transform.position, Vector2.right);
        }
        else
        {
            this.HandleAllCollisions(this.transform.position, Vector2.left);
        }

        if (this.velocity.y <= 0)
        {
            this.HandleAllCollisions(this.transform.position, Vector2.down);
        }
        else
        {
            this.HandleAllCollisions(this.transform.position, Vector2.up);
        }
    }

    /// <summary>
    /// Apply gravity to the rings velocity forcing it to move downards
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= this.gravity;

    /// <summary>
    /// Handles all the rings collisions by simply passing in the direction of the ring ands its center position
    /// <param name="position">The current position of the ring</param>
    /// <param name="direction">The direction to cast the ray in </param>
    /// </summary
    private void HandleAllCollisions(Vector2 position, Vector2 direction)
    {
        float distance = this.halfRingRadius;
        Vector2 verticalSensorPosition = position;
        LayerMask collisionMask = this.groundCollisionMask;

        if (direction == Vector2.up)
        {
            collisionMask = this.ceilingCollisionMask;
        }

        if (direction == Vector2.left || direction == Vector2.right)
        {
            collisionMask = this.wallCollisionMask;
        }

        RaycastHit2D ringSensor = Physics2D.Raycast(verticalSensorPosition, direction, distance, collisionMask);
        Debug.DrawLine(verticalSensorPosition, verticalSensorPosition + (direction * distance), this.debugColor);

        if (ringSensor)
        {
            float angleInRadians = General.Vector2ToAngle(ringSensor.normal);
            float ringDirectionInDegrees = Mathf.Round((General.Vector2ToAngle(direction) * Mathf.Rad2Deg) - 180);
            ringDirectionInDegrees = ringDirectionInDegrees < 0 ? ringDirectionInDegrees + 360 : ringDirectionInDegrees;

            switch (ringDirectionInDegrees)
            {
                //Ground
                case 0:
                    this.velocity.y *= this.ringBounceLoss.y;
                    position.y = ringSensor.point.y + (this.halfRingRadius * Mathf.Cos(angleInRadians));
                    break;
                //Right Wall
                case 90:
                    this.velocity.x *= this.ringBounceLoss.x;
                    position.x = ringSensor.point.x - (this.halfRingRadius * Mathf.Sin(angleInRadians));
                    break;
                //Ceiling
                case 180:
                    this.velocity.y = 0;
                    position.y = ringSensor.point.y + (this.halfRingRadius * Mathf.Cos(angleInRadians));
                    break;
                //Left Wall
                case 270:
                    this.velocity.x *= this.ringBounceLoss.x;
                    position.x = ringSensor.point.x - (this.halfRingRadius * Mathf.Sin(angleInRadians));
                    break;
                default:
                    Debug.LogWarning("Please pass in a direction using Vector2 directions such as Vector2.Up");
                    break;
            }

            this.transform.position = position;
        }
    }

    /// <summary>
    /// Begins to fade the ring towards zero
    /// </summary>
    private void BeginFadeToZero()
    {
        if (this.gameObject.activeSelf == false)
        {
            return;
        }

        this.StartCoroutine(this.LerpColorsOverTime(this.spriteRenderer.color, new Color(1, 1, 1, 0), General.StepsToSeconds(this.ringStartFadeTime)));
    }

    /// <summary>
    /// Fades the colour alpha back towards zero in the specified time 
    /// </summary>
    private IEnumerator LerpColorsOverTime(Color startingColor, Color endingColor, float time)
    {
        float inversedTime = 1 / time;

        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            this.spriteRenderer.color = Color.Lerp(startingColor, endingColor, step);

            yield return null;
        }

        this.velocity = Vector2.zero;
        this.gameObject.SetActive(false);//Deactivate instead of destroy
    }
}
