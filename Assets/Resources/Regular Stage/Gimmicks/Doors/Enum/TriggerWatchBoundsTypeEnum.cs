using UnityEngine;

public enum TriggerWatchBoundsType
{
    [Tooltip("On contact with this side the door will open")]
    ActivivatableSide,
    [Tooltip("This side will not be open the door but if the player exceeds it bounds it will close")]
    NonActivitableSide,
}
