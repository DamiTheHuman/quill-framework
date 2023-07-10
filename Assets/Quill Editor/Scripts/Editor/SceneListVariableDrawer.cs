using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// Custom property drawer to provide a list of scenes for the player to choose from
/// </summary>
[CustomPropertyDrawer(typeof(SceneListAttribute))]
public class SceneListVariableDrawer : PropertyDrawer
{
    /// <summary>
    /// The folder the levels file is located when the game is hosted in the editor.
    /// </summary>
    private const string EDITOR_LEVELS_FILE_DIRECTORY = "Assets/GameObjects/Resources/Scenes";
    private static readonly GUIContent[] noMulti = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noAnimCont = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noBehav = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noGameObj = new GUIContent[] { new GUIContent("") };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SceneListAttribute a = (SceneListAttribute)this.attribute;

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
                Dictionary<int, string> scenes = this.LoadGameScenes();

                if (scenes == null)
                {
                    EditorGUI.Popup(position, -1, noAnimCont);
                }
                else
                {
                    int x = 0;
                    for (x = 0; x < scenes.Count; x++)
                    {
                        if (x.Equals(property.intValue))
                        {
                            break;
                        }
                    }
                    EditorGUI.BeginChangeCheck();

                    position.width = EditorGUIUtility.currentViewWidth / 1.8f;
                    position.x = EditorGUIUtility.currentViewWidth / 2.5f;

                    GUI.enabled = !a.disabled;
                    x = EditorGUI.Popup(position, x, scenes.Values.ToArray());
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
            EditorGUI.LabelField(position, label, "Scene strings");
        }
    }

    /// <summary>
    /// Load all the available scenes that are available within build
    /// </summary>
    private Dictionary<int, string> LoadGameScenes()
    {
        Dictionary<int, string> availableScenes = new Dictionary<int, string>();
        EditorBuildSettingsScene[] scenesInBuild = EditorBuildSettings.scenes;
        string path;

        for (int i = 0; i < scenesInBuild.Length; i++)
        {
            path = Path.GetFileNameWithoutExtension(scenesInBuild[i].path);
            availableScenes.Add(i, path);
        }

        return availableScenes;
    }
}

