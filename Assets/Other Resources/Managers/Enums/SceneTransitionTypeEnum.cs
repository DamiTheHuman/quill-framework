using UnityEngine;

public enum SceneTransitionType
{
    [Tooltip("Fades out the entire screen with blit")]
    Fade,
    [Tooltip("Plays nothing when the scene is moved")]
    None,
}
