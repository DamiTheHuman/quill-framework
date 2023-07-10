using UnityEngine;

public enum SignPostState
{
    [Tooltip("The state when the sign has not yet been interacted with")]
    None,
    [Tooltip("The state when the sign ia currently in the air")]
    Touched,
    [Tooltip("The state when the sign has returned to its base state ")]
    End
}
