using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

internal static class EditorUIHelper
{
    /// <summary>
    ///  Whether to use a vertical layout to display content
    /// <param name="editor"> The currennt editor </param>
    /// <param name="action"> The action to perform between the layouts </param>
    /// <param name="style"> The GUI style for the layour</param>
    /// </summary>
    public static void UseVerticalLayout(this Editor editor, Action action, GUIStyle style)
    {
        EditorGUILayout.BeginVertical(style);
        action();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    ///  Whether to use a button
    /// <param name="editor"> The current editor </param>
    /// <param name="methodInfo"> The method info</param>
    /// </summary>
    public static void UseButton(this Editor editor, MethodInfo methodInfo)
    {
        Debug.Log("This method was useful");

        if (GUILayout.Button(methodInfo.Name))
        {
            methodInfo.Invoke(editor.target, null);
        }
    }
}