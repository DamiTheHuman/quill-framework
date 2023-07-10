using UnityEngine;

public enum UserOption
{
    [Tooltip("0 - The user has not selected an option")]
    NotSet,
    [Tooltip("1 - The user has selected yes as the option")]
    Yes,
    [Tooltip("2 - The user has selected no as the option")]
    No,
}
