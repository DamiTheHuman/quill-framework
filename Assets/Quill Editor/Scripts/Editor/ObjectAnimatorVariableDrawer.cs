using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using an = UnityEditor.Animations;
#endif

[CustomPropertyDrawer(typeof(ObjectAnimatorAttribute))]
public class ObjectAnimatorVariableDrawer : PropertyDrawer
{
    private static readonly GUIContent[] noMulti = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noAnimator = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noAnimCont = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noBehav = new GUIContent[] { new GUIContent("") };
    private static readonly GUIContent[] noGameObj = new GUIContent[] { new GUIContent("") };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (property.propertyType == SerializedPropertyType.String)
        {
            GUI.enabled = false;

            //EditorGUI.PropertyField(position, property);
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

                if (!behaviour)
                {
                    EditorGUI.Popup(position, -1, noBehav);
                }
                else if (behaviour.gameObject)
                {
                    GUI.enabled = true;
                    Animator search = null;
                    //Check the area its being called first if null check its parent for animator
                    if (behaviour.GetComponent<Animator>() != null)
                    {
                        search = behaviour.GetComponent<Animator>();
                    }
                    else if (behaviour.transform.parent != null)
                    {
                        if (behaviour.transform.parent.GetComponent<Animator>() != null)
                        {
                            search = behaviour.transform.parent.GetComponent<Animator>();
                        }
                        else if (behaviour.transform.parent.GetComponentInChildren<Animator>() != null)
                        {
                            search = behaviour.transform.parent.GetComponentInChildren<Animator>();
                        }
                    }
                    Animator animator = search;

                    if (animator)
                    {
                        string[] anims = ObjectAnimatorVariabless.animatorVariables(animator);
                        if (anims == null)
                        {
                            EditorGUI.Popup(position, -1, noAnimCont);
                        }
                        else
                        {
                            int x = 0;
                            for (x = 0; x < anims.Length; x++)
                            {
                                if (anims[x].Equals(property.stringValue))
                                {
                                    break;
                                }
                            }
                            EditorGUI.BeginChangeCheck();
                            position.x = EditorGUIUtility.currentViewWidth / 2.45f;
                            position.width = EditorGUIUtility.currentViewWidth / 1.9f;
                            x = EditorGUI.Popup(position, x, anims);
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (anims[x] == "---")
                                {
                                    property.stringValue = "";
                                }
                                else
                                {
                                    property.stringValue = anims[x];
                                }
                                property.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.Popup(position, -1, noAnimator);
                    }
                }
                else
                {
                    EditorGUI.Popup(position, -1, noGameObj);
                }
            }
        }
        else
        {
            EditorGUI.LabelField(position, label, "Animator strings");
        }
    }
}

public static class ObjectAnimatorVariabless
{
    public static string[] animatorVariables(Animator animator)
    {
        if (animator)
        {
            if (animator.runtimeAnimatorController)
            {
                an.AnimatorController controller = animator.runtimeAnimatorController as an.AnimatorController;

                if (controller == null)
                {
                    return null;
                }
                else
                {
                    string[] animationParameter = new string[controller.parameters.Length + 1];
                    animationParameter[0] = "---";
                    for (int i = 1; i < animationParameter.Length; i++)
                    {
                        animationParameter[i] = controller.parameters[i - 1].name;
                    }
                    return animationParameter;
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
