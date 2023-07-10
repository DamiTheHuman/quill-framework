using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(IsDisabledAttribute))]
public class IsDisabledAttributeDrawer : PropertyDrawer
{

    /// <summary>
    /// Set the attribute to disabled
    /// Fill attribute needed
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        IsDisabledAttribute isDisabled = (IsDisabledAttribute)this.attribute;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label, true);
        EditorGUI.EndDisabledGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);

}
