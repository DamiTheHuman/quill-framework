using System.Collections;
using UnityEngine;
/// <summary>
/// The sign  post object which interacts with the player and continously moves when bumped from the button
/// Author - Nihil 
/// Changes - Slight modifications to make it work for unity and logic overhaul
/// </summary>
public class SignPostController : ActClearGimmick
{
    [SerializeField]
    private SignFaceController signFaceController;
    [Tooltip("The current state of the sign face"), FirstFoldOutItem("Sign Post Info"), SerializeField]
    private SignPostState signPostState = SignPostState.None;
    [Tooltip("Determines what the sign post considers as ground"), SerializeField]
    private LayerMask groundCollisionMask = new LayerMask();
    [LayerList, Tooltip("The layer where gimmicks are found")]
    [Help("Ensure the layer selected here is the same layer as the monitors"), SerializeField]
    private int gimmickLayer = 13;
    [SerializeField]
    private LayerMask monitorCollisionMask;
    [Tooltip("The Body Height Radius of the sign"), SerializeField]
    private float signBodyHeightRadius = 24f;
    [Tooltip("The audio clip played when the signpost is touched"), SerializeField]
    private AudioClip signPostActivatedSound = null;

    [Tooltip("The current velocity of the sign post"), FirstFoldOutItem("Sign Post Movement")]
    public Vector2 velocity;
    [Tooltip("The vertical force which moves the sign towards the ground"), SerializeField]
    private float signGravity = 0.07f;
    [Tooltip("The acceleration of the sign wheen touched"), LastFoldoutItem(), SerializeField]
    private Vector2 signAcceleration = new Vector2(1, 3);
    [Tooltip("The score value granted each time the monitor is touched"), FirstFoldOutItem("Bump Score Info"), SerializeField]
    private int scoreToGrant = 100;
    [Tooltip("The offset spawn point of the score object"), LastFoldoutItem(), SerializeField]
    private Vector2 scoreDisplayOffset = new Vector2(0, -25);
    [FirstFoldOutItem("Sign Sparkle Info"), LastFoldoutItem(), SerializeField]
    private float sparkleFrequency = 10;

    private IEnumerator spawnSignSparklesCoroutine;


    public override void Reset()
    {
        base.Reset();
        this.signFaceController = this.GetComponentInChildren<SignFaceController>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (this.signFaceController == null)
        {
            this.Reset();
        }

        this.signPostState = SignPostState.None;
        this.monitorCollisionMask |= 1 << this.gimmickLayer;
        this.startPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (this.signPostState == SignPostState.Touched)
        {
            this.MoveAndCollide(this.velocity);
            this.ApplyGravity();
            this.RestrictSignWithinBounds();
        }
    }

    /// <summary>
    /// Restricts the sign to the boundaries of the camera
    /// </summary>
    private void RestrictSignWithinBounds()
    {
        if (HedgehogCamera.Instance().PositionIsLeftOfCmaeraView(this.transform.position) || HedgehogCamera.Instance().PositionIsRightOfCameraView(this.transform.position))
        {
            this.velocity.x *= -1;
        }
    }

    /// <summary>
    /// Move the sign post in the direction of its current velocity
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
        float distance = this.signBodyHeightRadius; //How far the primary ground sensors go
        SensorData sensorData = new SensorData(0, -90, 0, distance);

        Vector2 groundSensorPosition = this.transform.position; //Left Ground Sensor
        RaycastHit2D groundSensor = Physics2D.Raycast(groundSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.groundCollisionMask);
        Debug.DrawLine(groundSensorPosition, groundSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), Color.red);

        if (groundSensor)
        {
            RaycastHit2D monitorSearchSensor = Physics2D.Raycast(groundSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance() + 8f, this.monitorCollisionMask);
            if (monitorSearchSensor)
            {
                if (this.CheckGroundedMonitorCollision(monitorSearchSensor))
                {
                    return;
                }
            }

            this.TriggerGroundAction();
            this.StickToGround(groundSensor, this.transform.position);
        }
        else
        {
            this.signPostState = SignPostState.Touched;
        }
    }

    /// <summary>
    /// Peforms grounded monitor based collision
    /// </summary>
    private bool CheckGroundedMonitorCollision(RaycastHit2D hit)
    {
        if (hit)
        {
            HiddenMonitorController hiddenMonitor = hit.transform.GetComponent<HiddenMonitorController>();
            if (hiddenMonitor != null)
            {
                float ySpawnPosition = hit.point.y + this.signBodyHeightRadius;
                Vector2 hiddenMonitorPosition = new Vector2(this.transform.position.x, ySpawnPosition) + new Vector2(-2, 32);
                hiddenMonitor.ShowMonitor(hiddenMonitorPosition);
                this.BumpSign();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Stick to the ground
    /// <param name="stickSensor">The raycast which has determines the hit position and ground data </param>
    /// <param name="position">The current position of the signpost </param>
    /// </summary>
    private void StickToGround(RaycastHit2D stickSensor, Vector2 position)
    {
        float angleInRadians = General.RoundToDecimalPlaces(General.Vector2ToAngle(stickSensor.normal));
        position.x = stickSensor.point.x - (this.signBodyHeightRadius * Mathf.Sin(angleInRadians));
        position.y = stickSensor.point.y + (this.signBodyHeightRadius * Mathf.Cos(angleInRadians));
        this.transform.position = position;
    }

    /// <summary>
    /// The actions performed when the sign post touches the ground after being launched in the air
    /// </summary>
    public void TriggerGroundAction()
    {
        this.signPostState = SignPostState.End;
        this.velocity.y = 0;
        this.velocity.x = 0;
        this.signFaceController.SetSignFaceState(SignFaceState.Depleting);
        this.Invoke(nameof(StopSpawningSparkles), General.StepsToSeconds(this.signFaceController.GetSpinDepletionTimeInSteps() - 30));
    }

    /// <summary>
    /// Apply a vertical force that moves the sign post towards the ground 
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.signGravity);

    /// <summary>
    /// Accepts the sign post collision as long as it is not in its end state
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = this.signPostState != SignPostState.End;

        return triggerAction;
    }

    /// <summary>
    /// Handle the interaction based on how the sign post is touched by the player
    /// <param name="player">The player object  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (this.signPostState == SignPostState.None)
        {
            this.signFaceController.SetPlayer(player);
            this.OnActClearActivation(player);
        }
        else if (this.velocity.y <= 0 && player.GetActionManager().CheckActionIsBeingPerformed<Jump>())
        {
            this.PushSign(player);
            GMRegularStageScoreManager.Instance().IncrementScoreCount(this.scoreToGrant);
            GMRegularStageScoreManager.Instance().DisplayScore(this.transform.position + (Vector3)this.scoreDisplayOffset, 1);
        }
    }

    /// <summary>
    /// The actions which take place on initial contact with the sign
    /// </summary>
    protected override void OnActClearActivation(Player player)
    {
        base.OnActClearActivation(player);
        this.signPostState = SignPostState.Touched;
        this.BumpSign();
        this.signFaceController.SetSignFaceState(SignFaceState.Active);
        GMAudioManager.Instance().PlayOneShot(this.signPostActivatedSound);
        this.spawnSignSparklesCoroutine = this.SpawnSignSparkles(General.StepsToSeconds(this.sparkleFrequency));
        this.StartCoroutine(this.spawnSignSparklesCoroutine);
    }

    /// <summary>
    /// Pushes the sign vertically upwards
    /// </summary>
    private void BumpSign() => this.velocity.y = this.signAcceleration.y;

    /// <summary>
    /// Push the sign horizontally based on te angle of the player
    /// <param name="player">The active player object</param>
    /// </summary>
    private void PushSign(Player player)
    {
        this.BumpSign();
        float angleBetweenSignPostAndPlayer = General.AngleBetweenVector2(this.transform.position, player.transform.position);
        this.velocity.x = Mathf.Cos(angleBetweenSignPostAndPlayer) * -this.signAcceleration.x;
    }

    /// <summary>
    /// A coroutine to spawn sparkles around the radius of the sign post
    /// <param name="frequncy">How frequently sparkles should be spawned</param>
    /// </summary>
    private IEnumerator SpawnSignSparkles(float frequncy)
    {
        while (true)
        {
            //If the sign is done spinning no sparkles
            if (this.signFaceController.signFaceState == SignFaceState.End)
            {
                break;
            }

            Vector2 spawnPosition = (Vector2)this.transform.position + new Vector2(0, this.signBodyHeightRadius / 2);
            spawnPosition.x += Random.Range(-20, 20);
            spawnPosition.y += Random.Range(-12, 12);
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, spawnPosition);

            yield return new WaitForSeconds(frequncy);
        }

        yield return null;
    }

    /// <summary>
    /// Forcibly Ends the spawn sign sprakles coroutine
    /// </summary>
    public void StopSpawningSparkles()
    {
        if (this.spawnSignSparklesCoroutine != null)
        {
            this.StopCoroutine(this.spawnSignSparklesCoroutine);
        }
    }

    /// <summary>
    /// Get the sign face controller
    /// </summary>
    public SignFaceController GetSignFaceController() => this.signFaceController;
}
