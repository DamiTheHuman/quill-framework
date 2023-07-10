using UnityEngine;

public enum CollapsableBlockType
{
    [Tooltip("The regular collapse type which simply plummte towards the ground")]
    Regular,
    [Tooltip("This collapse type removes the parent collider when it falls useful for split blocks")]
    RemoveCollisionWhenActivated
}
