using UnityEngine;
#if UNITY_EDITOR
#endif

/// <summary>
/// This attribute can only be applied to fields because its
/// associated PropertyDrawer only operates on fields (either
/// public or tagged with the [SerializeField] attribute) in
/// the target MonoBehaviour.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class ButtonAttribute : PropertyAttribute
{
    public static float kDefaultButtonWidth = 80;

    public readonly string MethodName;

    public float ButtonWidth { get; set; } = kDefaultButtonWidth;

    public ButtonAttribute(string MethodName) => this.MethodName = MethodName;
}