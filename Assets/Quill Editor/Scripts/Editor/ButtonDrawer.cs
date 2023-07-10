using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonPropertyDrawer : PropertyDrawer
{
    private MethodInfo _eventMethodInfo = null;

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        ButtonAttribute inspectorButtonAttribute = (ButtonAttribute)this.attribute;

        Rect rect = GUILayoutUtility.GetLastRect();
        Rect buttonRect = new Rect(position.x, position.y, rect.width, position.height);

        if (GUI.Button(buttonRect, label.text))
        {
            System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
            string eventName = inspectorButtonAttribute.MethodName;

            if (this._eventMethodInfo == null)
            {
                this._eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            if (this._eventMethodInfo != null)
            {
                this._eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
            }
            else
            {
                Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
            }
        }
    }
}
#endif