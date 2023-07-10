using UnityEngine;

public class BreakableWallShardController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BreakableShardSetController breakableShardSetController;
    [Tooltip("The sprite renderer component for the projectile"), SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;
    [Tooltip("The movement information of the projectile")]
    public ProjectileData projectileData;
    [Tooltip("The current velocity of the projectile")]
    public Vector2 velocity;
    private Vector2 localStartPosition;

    private void Reset()
    {
        this.breakableShardSetController = this.GetComponentInParent<BreakableShardSetController>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        if (this.breakableShardSetController == null || this.spriteRenderer == null)
        {
            this.Reset();
        }

        this.localStartPosition = this.transform.localPosition;
    }

    public void Start() => this.velocity = this.projectileData.GetStartVelocity();

    public void OnObjectSpawn()
    {
        this.velocity = this.projectileData.GetStartVelocity();

        if (this.projectileData.GetDeactivationSetting() is ProjectileDeactivationSetting.LifeTime or ProjectileDeactivationSetting.OffScreenVerticallyOrLifeTime)
        {
            this.Invoke(nameof(DeactivateProjectile), General.StepsToSeconds(this.projectileData.GetLifTimeInSteps()));//Keep the projectile alive for its life span
        }
    }

    private void FixedUpdate()
    {
        this.Move(this.velocity);
        this.ApplyGravity();

        if (this.projectileData.GetDeactivationSetting() is ProjectileDeactivationSetting.OffScreenVertically or ProjectileDeactivationSetting.OffScreenVerticallyOrLifeTime)
        {
            this.CheckToDeactivate();
        }
    }

    /// <summary>
    /// Moves the projectile in the direction of its current velocity
    /// <param name="velocity">The crabmeats current velocity</param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the projectile by the current velocity

    /// <summary>
    /// Apply gravity to the shards y velocity
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.projectileData.GetGravity());
    /// <summary>
    /// Simply deactivates the projectile when it is below the camera view port
    /// </summary>
    private void CheckToDeactivate()
    {
        if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.spriteRenderer.bounds.max))
        {
            this.DeactivateProjectile();
        }
    }

    /// <summary>
    /// Deactivate the game object
    /// </summary>
    private void DeactivateProjectile()
    {
        if (this.breakableShardSetController != null)
        {
            this.breakableShardSetController.DeactivatedShard();
        }
        else
        {
            Debug.LogError("No ShardSetController found on BreakableWallShard!");
        }

        this.CancelInvoke();
        this.transform.localPosition = this.localStartPosition;
        this.gameObject.SetActive(false);
    }
}
