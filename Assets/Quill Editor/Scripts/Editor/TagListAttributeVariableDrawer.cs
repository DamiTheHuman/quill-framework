using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(TagListAttribute))]
public class TagListAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
        // One line of  oxygen free code.
        property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
}
