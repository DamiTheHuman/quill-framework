using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Custom property drawer to provide a list of 2D horizontal directions
/// </summary>
[CustomPropertyDrawer(typeof(DirectionListAttribute))]
public class DirectionListVariableDrawer : PropertyDrawer
{
    private static readonly GUIContent[] noMulti = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noAnimCont = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noBehav = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noGameObj = new GUIContent[] { new GUIContent("") };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DirectionListAttribute a = (DirectionListAttribute)this.attribute;

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            GUI.enabled = true;
            position.size = new Vector2(EditorGUIUtility.currentViewWidth, position.size.y);
            position.width = EditorGUIUtility.currentViewWidth / 1.2f;

            EditorGUI.LabelField(position, label.text);
            position.x += position.width;
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.Popup(position, -1, noMulti);
            }
            else if (property.serializedObject.targetObject)
            {
                Behaviour behaviour = property.serializedObject.targetObject as Behaviour;
                GUI.enabled = true;
                Dictionary<int, string> directions = this.LoadDirections();

                if (directions == null)
                {
                    EditorGUI.Popup(position, -1, noAnimCont);
                }
                else
                {
                    int x = 0;
                    foreach (KeyValuePair<int, string> entry in directions)
                    {
                        if (entry.Key != property.intValue)
                        {
                            x++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    EditorGUI.BeginChangeCheck();

                    position.width = EditorGUIUtility.currentViewWidth / 1.8f;
                    position.x = EditorGUIUtility.currentViewWidth / 2.5f;

                    GUI.enabled = !a.disabled;
                    x = EditorGUI.Popup(position, x, directions.Values.ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.intValue = EditorGUI.LayerField(position, label, x);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    GUI.enabled = true;
                }

            }
        }
        else
        {
            EditorGUI.LabelField(position, label, "Horizontal direction strings");
        }
    }

    /// <summary>
    /// Load all the available directions within the build
    /// </summary>
    private Dictionary<int, string> LoadDirections()
    {
        Dictionary<int, string> availableHorizontalDirections = new Dictionary<int, string>
        {
            { 1, "Right" },
            { -1, "Left" }
        };

        return availableHorizontalDirections;
    }
}
