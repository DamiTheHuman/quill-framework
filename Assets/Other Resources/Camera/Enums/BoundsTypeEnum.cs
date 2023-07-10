using UnityEngine;

public enum BoundsType
{
    [Tooltip("For bounds that follow the target object")]
    RelativeBounds,
    [Tooltip("For bounds that encompass the zone and thus must remain the same throught")]
    AbsoluteBounds,
}
