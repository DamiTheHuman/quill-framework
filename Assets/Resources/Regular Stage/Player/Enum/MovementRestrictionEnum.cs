using UnityEngine;

public enum MovementRestriction
{
    [Tooltip("Velocity will be added as expected")]
    None,
    [Tooltip("Velocity will not be added on the X Axis")]
    Horizontal,
    [Tooltip("Velocity will not be added on the X Axis")]
    Vertical,
    [Tooltip("Velocity will not be added on the X and Y Axis")]
    Both
}
