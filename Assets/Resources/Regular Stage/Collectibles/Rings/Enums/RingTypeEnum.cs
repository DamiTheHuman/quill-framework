using UnityEngine;

public enum RingType
{
    [Tooltip("Regular floating ring = 0")]
    Regular,
    [Tooltip("Magnetized towards a target typically the player = 1")]
    Magnetized,
    [Tooltip("Spilled and fading = 2")]
    Spilled,
}
