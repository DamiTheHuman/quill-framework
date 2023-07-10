using UnityEngine;

public enum ActionAvailability
{
    [Tooltip("This action will only be conisdered active while the player is in the air")]
    AirOnly,
    [Tooltip("This action will only be conisdered active while the player is grounded")]
    GroundOnly,
    [Tooltip("This move will always be considered available")]
    Always,
}
