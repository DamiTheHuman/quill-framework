using UnityEngine;
public enum DoorState
{
    [Tooltip("This state when the door is not being moved")]
    Idle,
    [Tooltip("The state when the door is being closed")]
    Closing,
    [Tooltip("This state when the door is being opened")]
    Opening,
}
