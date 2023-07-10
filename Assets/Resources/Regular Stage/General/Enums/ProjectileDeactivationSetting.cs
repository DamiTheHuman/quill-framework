using UnityEngine;

public enum ProjectileDeactivationSetting
{
    [Tooltip("Offscreen Vertically = 0 - Destroys the projectile when it is off screen")]
    OffScreenVertically,
    [Tooltip("Lifetime = 1 - Destroys the projectile when its lifetime has run out")]
    LifeTime,
    [Tooltip("Offscreen Vertically Or LifeTime = 1 - Destroys the projectile when its of screen or its lifetime has run out")]
    OffScreenVerticallyOrLifeTime
}
