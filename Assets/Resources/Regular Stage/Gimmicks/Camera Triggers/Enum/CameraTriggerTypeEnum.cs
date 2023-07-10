using UnityEngine;

public enum CameraTriggerType
{
    [Tooltip("When this trigger type is exceeded the camera y bounds is changed")]
    Regular,
    [Tooltip("When this trigger type is exceeded the camera left bounds the left zonebounds is set to this objects x position")]
    StageEnding,
}
