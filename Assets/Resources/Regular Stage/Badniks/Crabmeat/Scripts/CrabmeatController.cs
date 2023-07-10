using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A common crab like enemy which shoots bullets from its claws
/// </summary>
public class CrabmeatController : BadnikController
{
    [SerializeField]
    private Animator animator;
    [Tooltip("The active state of the crab meat")]
    [FirstFoldOutItem("Crabmeat Information"), SerializeField]
    private CrabmeatMode currentCrabmeatMode = CrabmeatMode.Idle;
    [Tooltip("A flag that checks if the crabmeat has been seen by the player before it can shoot"), SerializeField]
    private bool canShoot;
    [Tooltip("The sound that plays when the crabmeat shoots"), SerializeField]
    private AudioClip crabmeatShootSound = null;
    [Tooltip("The width of the crabmeat which effects how close it gets to the boundaries"), LastFoldoutItem(), SerializeField]
    private float crabMeatBodyWidthRadius = 20f;

    [Tooltip("The walk speed of the crab meat"), FirstFoldOutItem("Crabmeat Movement Info"), LastFoldoutItem(), SerializeField]
    private float walkSpeed = 0.5f;

    [Tooltip("The left limit of the crabmeat"), FirstFoldOutItem("Crabmeat Boundaries"), SerializeField]
    private float leftLimit = 32f;
    [Tooltip("The right limit of the crabmeat"), LastFoldoutItem(), SerializeField]
    private float rightLimit = 32f;

    [FirstFoldOutItem("Bullet Information")]
    [Tooltip("The info for the spawned bullet"), SerializeField]
    private BulletData spawnBulletInfo = new BulletData();
    [Tooltip("The position to spawn the left bullet"), SerializeField]
    private Transform leftBulletSpawnPosition = null;
    [Tooltip("The position to spawn the right bullet"), LastFoldoutItem(), SerializeField]
    private Transform rightBulletSpawnPositon = null;

    [FirstFoldOutItem("Debug parameters")]
    [Tooltip("The primary debug color for rays"), SerializeField]
    private Color debugColor = Color.red;
    [Tooltip("The secondary debug color for bullet rays"), SerializeField]
    private Color debugColor2 = Color.yellow;
    [Tooltip("The amount of loops used for visualizing the bullet path")]
    [LastFoldoutItem(), SerializeField]
    private int debugBulletIterations = 36;

    public override void Reset() => base.Reset();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        this.currentCrabmeatMode = CrabmeatMode.Walking;

        if (this.animator == null)
        {
            this.Reset();
        }

        this.canShoot = false;
    }

    private void FixedUpdate()
    {
        if (HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer) && this.canShoot == false)
        {
            this.canShoot = true;
        }

        if (this.currentCrabmeatMode == CrabmeatMode.Walking)
        {
            this.Move(this.velocity);
            this.ApplyAcceleration();
            this.UpdateDirection();
        }

        this.animator.SetInteger("State", (int)this.currentCrabmeatMode);
    }

    /// <summary>
    /// Moves the crabmeat in the direction of its current velocity
    /// <param name="velocity">The crabmeats current velocity</param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the crabmeat by the current velocity
    /// <summary>
    /// Apply acceleration to the crabmeats velocity
    /// </summary>
    private void ApplyAcceleration() => this.velocity.x = GMStageManager.Instance().ConvertToDeltaValue(this.walkSpeed) * this.currentDirection;
    /// <summary>
    /// Update the direction of the crabmeat
    /// </summary>
    private void UpdateDirection()
    {
        if ((this.transform.position.x <= this.startPosition.x - this.leftLimit + this.crabMeatBodyWidthRadius && this.currentDirection == -1) || (this.transform.position.x >= this.startPosition.x + this.rightLimit - this.crabMeatBodyWidthRadius && this.currentDirection == 1))
        {
            this.currentDirection = this.currentDirection == 1 ? -1 : 1;
            this.velocity.x = 0;
            this.PrepareToFireBullet();
        }
    }

    /// <summary>
    /// Prepares to fire bullet by updating the crabmeat state and beginning the fire animation
    /// </summary>
    private void PrepareToFireBullet()
    {
        CrabmeatMode newCrabMeatMode = this.canShoot ? CrabmeatMode.Firing : CrabmeatMode.Waiting;
        this.UpdateCrabMeatState(newCrabMeatMode);
    }

    /// <summary>
    /// Actually fires the bullet blasts from the crab meats pointers and switches to the recovery state
    /// This function is called from the animators animation event
    /// </summary>
    private void FireBullet()
    {
        BulletController leftBullet = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.Bullet, this.leftBulletSpawnPosition.position).GetComponent<BulletController>();
        BulletController rightBullet = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.Bullet, this.rightBulletSpawnPositon.position).GetComponent<BulletController>();
        BulletData rightBulletInfo = new BulletData();
        rightBulletInfo.CopyProjectileData(this.spawnBulletInfo);

        BulletData leftBulletInfo = new BulletData();
        leftBulletInfo.CopyProjectileData(this.spawnBulletInfo);

        Vector2 leftBulletStartVelocity = leftBulletInfo.GetStartVelocity();
        leftBulletStartVelocity.x *= -1;
        leftBulletInfo.SetStartVelocity(leftBulletStartVelocity);

        rightBullet.GetBulletData().CopyProjectileData(rightBulletInfo);
        leftBullet.GetBulletData().CopyProjectileData(leftBulletInfo);

        leftBullet.OnObjectSpawn();
        rightBullet.OnObjectSpawn();

        if (HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer))
        {
            GMAudioManager.Instance().PlayOneShot(this.crabmeatShootSound);
        }
    }

    /// <summary>
    /// Update the crab meat state
    /// </summary>
    private void UpdateCrabMeatState(CrabmeatMode crabmeatMode) => this.currentCrabmeatMode = crabmeatMode;
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
        //Draw Bullet level
        Vector2 positionRight = this.rightBulletSpawnPositon.position;
        Vector2 positionLeft = this.leftBulletSpawnPosition.position;
        Vector2 debugBulletVelocityRight = this.spawnBulletInfo.GetStartVelocity();
        Vector2 debugBulletVelocityLeft = this.spawnBulletInfo.GetStartVelocity() * new Vector2(-1, 1);
        List<Vector2> bulletPositionsRight = new List<Vector2>();
        List<Vector2> bulletPositionsLeft = new List<Vector2>();
        bulletPositionsRight.Add(positionRight);
        bulletPositionsLeft.Add(positionLeft);

        for (int x = 1; x < this.debugBulletIterations; x++)
        {
            //Right Bullet
            positionRight += debugBulletVelocityRight * 1;
            debugBulletVelocityRight.y -= this.spawnBulletInfo.GetGravity();
            bulletPositionsRight.Add(positionRight);
            Debug.DrawLine(bulletPositionsRight[x - 1], bulletPositionsRight[x], this.debugColor2);

            //Left Bullet
            positionLeft += debugBulletVelocityLeft * 1;
            debugBulletVelocityLeft.y -= this.spawnBulletInfo.GetGravity();
            ;
            bulletPositionsLeft.Add(positionLeft);
            Debug.DrawLine(bulletPositionsLeft[x - 1], bulletPositionsLeft[x], this.debugColor2);

        }
    }
}
