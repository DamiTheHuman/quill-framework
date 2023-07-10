using UnityEngine;

[System.Serializable]
public class ProjectileData
{
    [Tooltip("The condition used to destroy the projectile"), SerializeField]
    protected ProjectileDeactivationSetting deactivationSetting = ProjectileDeactivationSetting.OffScreenVerticallyOrLifeTime;
    [SerializeField]
    [Tooltip("The start velocity of the projectile")]
    protected Vector2 startVelocity;
    [SerializeField]
    [Tooltip("The gravity applied to the projectile affecting its y velocity")]
    protected float gravity = 0.6f;
    [Tooltip("The lifetime of the projectile in steps before it is deactivated"), SerializeField]
    protected float lifeTimeInSteps = 120;

    /// <summary>
    /// Get the deactivation setting
    /// </summary>
    public ProjectileDeactivationSetting GetDeactivationSetting() => this.deactivationSetting;

    /// <summary>
    /// Get the start velocity
    /// </summary>
    public Vector2 GetStartVelocity() => this.startVelocity;

    /// <summary>
    /// Set the start velocity
    /// </summary>
    public void SetStartVelocity(Vector2 startVelocity) => this.startVelocity = startVelocity;

    /// <summary>
    /// Get the gravity value
    /// </summary>
    public float GetGravity() => this.gravity;

    /// <summary>
    /// Set the gravity value
    /// </summary>
    public void SetGravity(float gravity) => this.gravity = gravity;

    /// <summary>
    /// Get the liftime in steps
    /// </summary>
    public float GetLifTimeInSteps() => this.lifeTimeInSteps;

    /// <summary>
    /// Copy Projectile data from another <see cref="ProjectileData"/>
    /// </summary>
    public virtual void CopyProjectileData(ProjectileData projectileInfo)
    {
        this.startVelocity = projectileInfo.startVelocity;
        this.gravity = projectileInfo.gravity;
        this.lifeTimeInSteps = projectileInfo.lifeTimeInSteps;
        this.deactivationSetting = projectileInfo.deactivationSetting;
    }
}
