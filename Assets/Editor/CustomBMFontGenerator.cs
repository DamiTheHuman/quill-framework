using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CustomBMFontGenerator
{
    [MenuItem("Assets/Custom/Create Custom BMFont")]
    private static void CreateCustomFont()
    {
        TextAsset fontDataText = Selection.activeObject as TextAsset;

        if (fontDataText == null)
        {
            return;
        }

        List<CharacterInfo> characterInfoList = new List<CharacterInfo>();
        int scaleW = 0;
        int scaleH = 0;
        int lineHeight = 0;

        string[] fontDataLines = fontDataText.text.Trim().Split('\n');

        for (int i = 0; i < fontDataLines.Length; i++)
        {
            string line = fontDataLines[i];

            if (line.StartsWith("common "))
            {
                Dictionary<string, string> commonData = ParseDataLine(line);

                scaleW = int.Parse(commonData["scaleW"]);
                scaleH = int.Parse(commonData["scaleH"]);
                lineHeight = int.Parse(commonData["lineHeight"]);
            }
            else if (line.StartsWith("char "))
            {
                Dictionary<string, string> charData = ParseDataLine(line);

                int id = int.Parse(charData["id"]);
                int x = int.Parse(charData["x"]);
                int y = int.Parse(charData["y"]);
                int width = int.Parse(charData["width"]);
                int height = int.Parse(charData["height"]);
                int xoffset = int.Parse(charData["xoffset"]);
                int yoffset = int.Parse(charData["yoffset"]);
                int xadvance = int.Parse(charData["xadvance"]);

                CharacterInfo ci = new CharacterInfo
                {
                    index = id,
                    advance = xadvance
                };

                Rect uv = new Rect
                {
                    x = (float)x / scaleW,
                    y = (float)(scaleH - y - height) / scaleH,
                    width = (float)width / scaleW,
                    height = (float)height / scaleH
                };

                ci.uvBottomLeft = new Vector2(uv.x, uv.y);
                ci.uvTopRight = ci.uvBottomLeft + new Vector2(uv.width, uv.height);

                Rect vert = new Rect
                {
                    x = xoffset,
                    y = -yoffset,
                    width = width,
                    height = -height
                };

                ci.minX = (int)vert.x;
                ci.minY = (int)(vert.y + vert.height);
                ci.maxX = (int)(vert.x + vert.width);
                ci.maxY = (int)vert.y;

                characterInfoList.Add(ci);
            }
        }

        string directory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(fontDataText));
        string path = directory + "/" + fontDataText.name + ".fontsettings";

        Font font = AssetDatabase.LoadAssetAtPath<Font>(path);
        bool isNewFont = false;

        if (font == null)
        {
            font = new Font();
            isNewFont = true;
        }

        font.characterInfo = characterInfoList.ToArray();
        SetLineSpacing(font, lineHeight);

        if (isNewFont)
        {
            AssetDatabase.CreateAsset(font, path);
        }
        else
        {
            AssetDatabase.SaveAssets();
        }
    }

    private static Dictionary<string, string> ParseDataLine(string line)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        string[] properties = line.Split(' ');

        foreach (string property in properties)
        {
            string[] keyValue = property.Split('=');

            if (keyValue.Length == 2)
            {
                data.Add(keyValue[0], keyValue[1]);
            }
        }

        return data;
    }

    private static void SetLineSpacing(Font font, int lineSpacing)
    {
        SerializedObject fontObject = new SerializedObject(font);

        SerializedProperty property = fontObject.FindProperty("m_LineSpacing");
        property.floatValue = lineSpacing;

        fontObject.ApplyModifiedProperties();
    }
}