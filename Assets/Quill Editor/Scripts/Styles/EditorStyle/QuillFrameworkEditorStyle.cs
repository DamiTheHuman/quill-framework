using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Layout Styling for the main framework and the foldout
/// </summary>
public static class QuillFrameworkEditorStyle
{
    public static GUIStyle box;
    public static GUIStyle boxChild;
    public static GUIStyle foldout;
    public static GUIStyle button;
    public static GUIStyle text;

    /// <summary>
    /// Constructor for the style framework
    /// </summary>
    static QuillFrameworkEditorStyle()
    {
#if UNITY_EDITOR

        bool pro = EditorGUIUtility.isProSkin;
        //Fold out textures for the arrows
        Texture2D uiTex_in = Resources.Load<Texture2D>("IN foldout focus-6510");
        Texture2D uiTex_in_on = Resources.Load<Texture2D>("IN foldout focus on-5718");
        //Check the skin
        Color c_on = pro ? Color.white : new Color(1 / 255f, 10 / 255f, 24 / 255f, 1);

        //Basic GUI  text
        text = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            contentOffset = new Vector2(0, 5),
            font = Font.CreateDynamicFontFromOSFont(new[] { "Terminus (TTF) for Windows", "Calibri" }, 14)
        };

        foldout = new GUIStyle(EditorStyles.foldout)
        {
            overflow = new RectOffset(5, 0, 3, 0),
            padding = new RectOffset(15, 0, -3, 0)
        };

        foldout.active.textColor = c_on;
        foldout.active.background = uiTex_in;
        foldout.onActive.textColor = c_on;
        foldout.onActive.background = uiTex_in_on;

        foldout.focused.textColor = c_on;
        foldout.focused.background = uiTex_in;
        foldout.onFocused.textColor = c_on;
        foldout.onFocused.background = uiTex_in_on;

        foldout.hover.textColor = c_on;
        foldout.hover.background = uiTex_in;

        foldout.onHover.textColor = c_on;
        foldout.onHover.background = uiTex_in_on;

        //Design for the fold out box
        GUIStyle mainFoldStyle = new GUIStyle(GUI.skin.box);
        mainFoldStyle.normal.background = MakeTex(2, 2, new Color(76f/255f, 76f/255f, 76f/255f, 1f));
        box = mainFoldStyle;
        box.padding = new RectOffset(15, 0, 5, 0);
#endif
    }

    /// <summary>
    /// Convert the first letter of a string to uppercase
    /// <param name="text"> The string text object</param>
    /// </summary>
    public static string FirstLetterToUpperCase(this string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        char[] a = text.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return AddSpacesToSentence(new string(a));
    }

    /// <summary>
    /// Adds spaces to the sentence 
    /// <param name="text"> The text to be edited</param>
    /// </summary>
    public static string AddSpacesToSentence(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "";
        }

        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
            {
                newText.Append(' ');
            }

            newText.Append(text[i]);
        }

        return newText.ToString();
    }

    /// <summary>
    /// Get the type tree from a type object
    /// <param name="t"> The object</param>
    /// </summary>
    public static IList<Type> GetTypeTree(this Type t)
    {
        List<Type> types = new List<Type>();

        while (t.BaseType != null)
        {
            types.Add(t);
            t = t.BaseType;
        }

        return types;
    }

    /// <summary>
    /// Create a texture with the specified details
    /// <param name="width"> The width of the texture</param>
    /// <param name="height"> The height of the texture</param>
    /// <param name="colour"> The colour of the texture</param>
    /// </summary>
    public static Texture2D MakeTex(int width, int height, Color colour)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = colour;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
