using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(LayerListAttribute))]
internal class LayerListAttributeEditor : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
        // One line of  oxygen free code.
        property.intValue = EditorGUI.LayerField(position, label, property.intValue);
}