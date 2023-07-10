using UnityEngine;

public enum CameraType
{
    [Tooltip("The camera will apply a maximum distance when following the player")]
    Retro,
    [Tooltip("The camera will allways follow the player within the camera bounds")]
    ConstantFollow,
    [Tooltip("The camera will not move")]
    Static,
}
