using System;
using UnityEngine;

public class BuzzBomberController : BadnikController
{
    [SerializeField]
    private Animator animator;
    [Tooltip("What the buzz bomber considers an enemy"), FirstFoldOutItem("Buzz Bomber Info"), SerializeField]
    private LayerMask targetCollisionMask = new LayerMask();
    [Tooltip("The sound that plays when the buzzbomber shoots"), SerializeField]
    private AudioClip buzzBomberShootSound = null;
    [Tooltip("The active state of the buzz bomber"), LastFoldoutItem(), SerializeField]
    private BuzzBomberMode currentBuzzBomberMode = BuzzBomberMode.Idle;

    [Tooltip("The left limit of the buzzBomber"), FirstFoldOutItem("Buzz Bomber Boundaries"), SerializeField]
    private float leftLimit = 32f;
    [Tooltip("The right limit of the buzzBomber"), LastFoldoutItem(), SerializeField]
    private float rightLimit = 32f;
    [Tooltip("The flight speed of the buzz bomber"), FirstFoldOutItem("Buzz Bomber Movement"), LastFoldoutItem(), SerializeField]
    private float flightSpeed = 4f;

    [Tooltip("The info for the spawned bullet"), FirstFoldOutItem("Bullet Information"), SerializeField]
    private BulletData spawnBulletInfo = new BulletData();
    [Tooltip("The position to single bullet spawned from the bu "), SerializeField]
    private Transform bulletSpawnPosition = null;
    [Range(0, 360), Tooltip("The angle the bullet is launched in"), SerializeField]
    private float bulletAngleInDegrees = 315f;
    [LastFoldoutItem(), Tooltip("The distance to check for enemies"), SerializeField]
    private float fireRange = 160;

    [FirstFoldOutItem("Debug parameters"), SerializeField]
    private Color debugColor = Color.red;
    [LastFoldoutItem(), SerializeField]
    private Color debugColor2 = Color.yellow;
    public override void Reset()
    {
        base.Reset();
        this.animator = this.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        this.currentBuzzBomberMode = BuzzBomberMode.Idle;

        if (this.animator == null)
        {
            this.animator = this.GetComponent<Animator>();
        }

        this.animator.SetInteger("State", (int)this.currentBuzzBomberMode);
    }

    private void FixedUpdate()
    {
        if (HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer) == false && (this.currentBuzzBomberMode != BuzzBomberMode.Escape || this.currentBuzzBomberMode == BuzzBomberMode.Firing))
        {
            if (this.currentBuzzBomberMode == BuzzBomberMode.Flying)
            {
                this.UpdateBuzzBomberState(BuzzBomberMode.Idle);//Of screen and not targeting or firing switch back to idle
            }
        }
        else if (this.currentBuzzBomberMode is not BuzzBomberMode.Firing and not BuzzBomberMode.Escape)
        {
            this.UpdateBuzzBomberState(BuzzBomberMode.Flying);
        }
        if (this.currentBuzzBomberMode is BuzzBomberMode.Flying or BuzzBomberMode.Escape)
        {
            this.Move(this.velocity);
            this.ApplyAcceleration();
            if (this.currentBuzzBomberMode != BuzzBomberMode.Escape)
            { this.SearchForPlayer(this.transform.position); }
            this.UpdateDirection();
        }

        this.animator.SetInteger("State", (int)this.currentBuzzBomberMode);
    }


    /// <summary>
    /// Moves the buzzBomber in the direction of its current velocity
    /// <param name="velocity">The buzzBombers current velocity</param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the buzzBomber by the current velocity
    /// <summary>
    /// Apply acceleration to the Buzz Bombers velocity
    /// </summary>
    private void ApplyAcceleration() => this.velocity.x = GMStageManager.Instance().ConvertToDeltaValue(this.flightSpeed) * this.currentDirection;
    /// <summary>
    /// Update the direction of the BuzzB omber
    /// </summary>
    private void UpdateDirection()
    {
        if ((this.transform.position.x <= this.startPosition.x - this.leftLimit && this.currentDirection == -1) || (this.transform.position.x >= this.startPosition.x + this.rightLimit && this.currentDirection == 1))
        {
            this.velocity.x = 0;
            this.Flip();
        }
    }

    /// <summary>
    /// Checks if the player is within firing range
    /// </summary>
    private void SearchForPlayer(Vector3 position)
    {
        float castAngle = this.currentDirection == 1 ? this.bulletAngleInDegrees : this.bulletAngleInDegrees - 90;
        float distance = this.fireRange; //How far the ground sensors go 
        SensorData sensorData = new SensorData(castAngle * Mathf.Deg2Rad, 0, castAngle, distance);

        Vector2 stickSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(0, 0));
        RaycastHit2D stickSensor = Physics2D.Raycast(stickSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.targetCollisionMask);

        if (stickSensor)
        {
            this.PrepareToFireBullet();
        }

        this.transform.position = position; //Move the player object
    }

    /// <summary>
    /// Prepares to fire bullet by updating the buzzBomber state and beginning the fire animation
    /// </summary>
    private void PrepareToFireBullet() => this.UpdateBuzzBomberState(BuzzBomberMode.Firing);
    /// <summary>
    /// Actually fires the bullet blasts from buzz bombers bullet spawn position
    /// This function is called from the animators animation event
    /// </summary>
    private void FireBullet()
    {
        BulletController buzzBomberBullet = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.Bullet, this.bulletSpawnPosition.position).GetComponent<BulletController>();
        BulletData buzzBomberBulletInfo = new BulletData();

        buzzBomberBulletInfo.CopyProjectileData(this.spawnBulletInfo);

        Vector2 startVelocity = buzzBomberBulletInfo.GetStartVelocity();
        startVelocity.x *= this.currentDirection;

        buzzBomberBulletInfo.SetStartVelocity(startVelocity);
        buzzBomberBullet.GetBulletData().CopyProjectileData(buzzBomberBulletInfo);
        buzzBomberBullet.OnObjectSpawn();
        GMAudioManager.Instance().PlayOneShot(this.buzzBomberShootSound);
    }

    /// <summary>
    /// Update the buzz bomber state
    /// </summary>
    private void UpdateBuzzBomberState(BuzzBomberMode buzzBomberMode) => this.currentBuzzBomberMode = buzzBomberMode;
    /// <summary>
    /// Turn the buzzbomber around around to the direction of its velocity
    /// </summary>
    private void Flip()
    {
        this.currentDirection = this.currentDirection == 1 ? -1 : 1;
        this.transform.localScale = new Vector3(this.currentDirection, 1, 1);

        if (this.currentBuzzBomberMode == BuzzBomberMode.Escape)
        {
            this.UpdateBuzzBomberState(BuzzBomberMode.Flying);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 debugPosition = Application.isPlaying ? (Vector3)this.startPosition : this.transform.position;
        Vector2 pos1 = debugPosition;
        Vector2 pos2 = debugPosition;
        Gizmos.color = this.debugColor;

        pos1.x -= this.leftLimit;
        pos2.x += this.rightLimit;
        Gizmos.DrawLine(pos1, pos2);
        GizmosExtra.Draw2DArrow(pos1, 90);
        GizmosExtra.Draw2DArrow(pos2, 270);
        //Debug Angle

        Gizmos.color = this.debugColor2;
        float castAngle = this.currentDirection == 1 ? this.bulletAngleInDegrees : this.bulletAngleInDegrees - 90;
        float distance = this.fireRange; //How far the ground sensors go 
        SensorData sensorData = new SensorData(castAngle * Mathf.Deg2Rad, 0, castAngle, distance);

        Vector2 buzzBomberBulletPosition = General.CalculateAngledObjectPosition(this.transform.position, sensorData.GetAngleInRadians(), new Vector2(0, 0));
        Debug.DrawLine(buzzBomberBulletPosition, buzzBomberBulletPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.debugColor2);
        GizmosExtra.Draw2DArrow(buzzBomberBulletPosition, castAngle - 90, sensorData.GetCastDistance());
    }
}

