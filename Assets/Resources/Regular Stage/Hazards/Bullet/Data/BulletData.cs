using UnityEngine;

/// <summary>
/// A simple class which contains information on the bullet
/// This class makes it easier to preemptively prepare bullet data
/// </summary>

[System.Serializable]
public class BulletData : ProjectileData
{
    [Tooltip("The amount of force applied to the bullet when reflected by the player shield"), SerializeField]
    private Vector2 reflectVelocity = new Vector2(7, 7);

    /// <summary>
    /// Get the reflect velocity
    /// </summary>
    public Vector2 GetReflectVelocity() => this.reflectVelocity;

    /// <summary>
    /// Get the projectile data
    /// </summary>
    public override void CopyProjectileData(ProjectileData projectileData)
    {
        base.CopyProjectileData(projectileData);

        if (projectileData is BulletData bulletData)
        {
            this.reflectVelocity = bulletData.GetReflectVelocity();
        }
    }
}
