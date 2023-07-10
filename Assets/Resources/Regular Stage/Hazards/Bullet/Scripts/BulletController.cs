using System;
using System.Collections;
using UnityEngine;
[Serializable]
/// <summary>
/// The bullet controller class controls the movement and life of the bullet based on the bulletinfo
/// </summary>
public class BulletController : HitBoxContactEvent, IPooledObject
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BoxCollider2D boxCollider2D;
    [SerializeField, Tooltip("The sprite renderer component for the bullet"), LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;
    [Tooltip("The movement information of the bullet"), SerializeField]
    private BulletData bulletInfo;
    [Tooltip("The current velocity of the bullet"), SerializeField]
    private Vector2 velocity;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.velocity = this.bulletInfo.GetStartVelocity();
    }

    public void OnObjectSpawn()
    {
        this.velocity = this.bulletInfo.GetStartVelocity();

        if (this.bulletInfo.GetDeactivationSetting() is ProjectileDeactivationSetting.LifeTime or ProjectileDeactivationSetting.OffScreenVerticallyOrLifeTime)
        {
            this.Invoke(nameof(DeactivateBullet), General.StepsToSeconds(this.bulletInfo.GetLifTimeInSteps()));//Keep the bullet alive for its life span
        }
    }

    private void FixedUpdate()
    {
        this.Move(this.velocity);
        this.ApplyGravity();

        if (this.bulletInfo.GetDeactivationSetting() is ProjectileDeactivationSetting.OffScreenVertically or ProjectileDeactivationSetting.OffScreenVerticallyOrLifeTime)
        {
            this.CheckToDeactivate();
        }
    }

    /// <summary>
    /// Moves the bullet t in the direction of its current velocity
    /// <param name="velocity">The crabmeats current velocity</param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the bullet by the current velocity

    /// <summary>
    /// Apply gravity to the shards y velocity
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.bulletInfo.GetGravity());

    /// <summary>
    /// Simply deactivates the bullet when it is below the camera view port
    /// </summary>
    private void CheckToDeactivate()
    {
        if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.spriteRenderer.bounds.max))
        {
            this.DeactivateBullet();
        }
    }

    /// <summary>
    /// Deactivate the game object
    /// </summary>
    private void DeactivateBullet()
    {
        this.CancelInvoke();
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Reflect based on the angle between the player and the bullet
    /// <param name="player">The player object to check against  </param>
    /// </summary>
    private void ReflectBullet(Player player)
    {
        this.bulletInfo.SetGravity(0);
        float angleBetweenPlayerAndBullet = General.AngleBetweenVector2(player.transform.position, this.transform.position);
        this.velocity.x = this.bulletInfo.GetReflectVelocity().x * Mathf.Cos(angleBetweenPlayerAndBullet);
        this.velocity.y = this.bulletInfo.GetReflectVelocity().y * Mathf.Sin(angleBetweenPlayerAndBullet);
        this.StartCoroutine(this.DisableProjectileTillEndOfFrame());
    }

    /// <summary>
    /// If the player comes in contact with the bullet activate its effects
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = true;

        return triggerAction;
    }

    /// <summary>
    /// Reflects the bullet as along as they are not shielded, super or flying
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        if (player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.None
            || player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            this.ReflectBullet(player);
        }
        else
        {
            player.GetHealthManager().VerifyHit(this.transform.position.x);
        }
    }

    /// <summary>
    /// Reflects the bullet if the secondary hitbox action states so
    /// </summary>
    public override void SecondaryHitBoxObjectAction(SecondaryHitBoxController secondaryHitBoxController)
    {
        if (secondaryHitBoxController.GetCanReflectBullets() && secondaryHitBoxController.GetPlayer() != null)
        {
            this.ReflectBullet(secondaryHitBoxController.GetPlayer());
        }
    }

    /// <summary>
    /// Get the bullet data
    /// </summary>
    public BulletData GetBulletData() => this.bulletInfo;

    /// <summary>
    /// Disable the projectile collider to the end of frame 
    /// This is to ensure the player doesnt get hit at the same frame they deflect a projectile from an object that deflects projectiles
    /// </summary>
    private IEnumerator DisableProjectileTillEndOfFrame()
    {
        this.boxCollider2D.enabled = false;

        yield return new WaitForEndOfFrame();

        this.boxCollider2D.enabled = true;

        yield return null;
    }
}
