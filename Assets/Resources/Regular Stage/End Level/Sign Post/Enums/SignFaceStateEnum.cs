using UnityEngine;

public enum SignFaceState
{
    [Tooltip("The state when the animal is initially spawned")]
    None,
    [Tooltip("The state when the sign face is rotating")]
    Active,
    [Tooltip("The state when the sign is actively depleting its speed towards 0")]
    Depleting,
    [Tooltip("The state when the sign face has finished rotating and is playing the animation")]
    End,
}
