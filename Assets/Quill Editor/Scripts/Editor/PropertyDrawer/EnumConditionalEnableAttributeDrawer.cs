using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(EnumConditionalEnableAttribute))]

public class EnumConditionalEnableAttributeDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty propertyDrawer, GUIContent label)
    {
        EnumConditionalEnableAttribute enumConditionalEnabled = (EnumConditionalEnableAttribute)this.attribute;
        SerializedProperty t1 = propertyDrawer.serializedObject.FindProperty(enumConditionalEnabled.targetEnum);

        if (enumConditionalEnabled.serializableClasses.Count != 0 && t1 == null) //Used for enums that are part of a serializable class
        {
            if (!enumConditionalEnabled.serializableClasses.Contains(propertyDrawer.propertyPath.Split('.')[0]))
            {
                GUI.enabled = true;
                EditorGUI.PropertyField(position, propertyDrawer, label, true);
                GUI.enabled = true;
                return;
            }
            else
            {
                for (int x = 0; x < enumConditionalEnabled.serializableClasses.Count; x++)
                {
                    if (propertyDrawer.propertyPath.Split('.')[0] == enumConditionalEnabled.serializableClasses[x])
                    {
                        SerializedProperty t2 = propertyDrawer.serializedObject.FindProperty(enumConditionalEnabled.serializableClasses[x].Trim() + "." + enumConditionalEnabled.targetEnum);

                        bool newState = true;
                        bool oldState = GUI.enabled;

                        if (t2 == null)
                        {
                            Debug.LogWarning("[DisableIf] Invalid Property Name for Attribute.", propertyDrawer.serializedObject.targetObject);
                        }
                        else
                        {
                            newState = false;

                            foreach (int activeIndex in  enumConditionalEnabled.activeEnumIndexes)
                            {
                                if (t2.enumValueIndex == activeIndex)
                                {
                                    newState = t2.enumValueIndex == activeIndex; //Check if the indexes match if not disable it with the flag

                                    break;
                                }
                            }
                        }

                        GUI.enabled = newState;
                        EditorGUI.PropertyField(position, propertyDrawer, label, true);
                        GUI.enabled = oldState;
                    }
                }
            }
        }
        else
        {
            bool newState = true;
            bool oldState = GUI.enabled;

            if (t1 == null)
            {
                Debug.LogWarning("[DisableIf] Invalid Property Name for Attribute.", propertyDrawer.serializedObject.targetObject);
            }
            else
            {
                newState = false;

                foreach (int activeIndex in enumConditionalEnabled.activeEnumIndexes)
                {
                    if (t1.enumValueIndex == activeIndex)
                    {
                        newState = t1.enumValueIndex == activeIndex; //Check if the indexes match if not disable it with the flag

                        break;
                    }
                }
            }

            GUI.enabled = newState;
            EditorGUI.PropertyField(position, propertyDrawer, label, true);
            GUI.enabled = oldState;
        }
    }
}
