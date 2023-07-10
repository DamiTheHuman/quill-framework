using UnityEngine;

public enum CameraMode
{
    [Tooltip("The camera will aim to follow the target within its bounds")]
    FollowTarget,
    [Tooltip("The camera is frozen in place for a set amount of time not to be confused with the NONE state")]
    Freeze,
    [Tooltip("The camera will focus on the sign and stay within its boundaries")]
    EndLevel,
    [Tooltip("The camera will focus on the boss zone and stay within its boundaries")]
    BossMode,
    [Tooltip("The camera will behave like its a special stage")]
    SpecialStage,
    [Tooltip("The camera will do nothing")]
    None,
}
