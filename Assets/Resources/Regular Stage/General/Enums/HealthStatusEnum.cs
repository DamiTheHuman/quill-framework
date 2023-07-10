using UnityEngine;

public enum HealthStatus
{
    [Tooltip("The objecct can get hit")]
    Vulnerable,
    [Tooltip("The object cannot be hit")]
    Invulnerable,
    [Tooltip("The object is invincible")]
    Invincible,
    [Tooltip("The object is dead")]
    Death,
}
