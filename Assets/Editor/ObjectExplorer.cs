using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectExplorer : EditorWindow
{

    [MenuItem("Tools/Object Explorer")]
    private static void Init() => GetWindow<ObjectExplorer>();

    private readonly List<GameObject> objects = new List<GameObject>();
    private Vector2 scrollPos = Vector2.zero;
    private bool filterTop = true;
    private bool filterHidden = false;

    private void OnEnable() => this.FindObjects();

    private void AddObject(GameObject obj)
    {
        if (this.filterTop)
        {
            obj = obj.transform.root.gameObject;
        }
        if (this.filterHidden)
        {
            if ((obj.hideFlags & (HideFlags.HideInHierarchy | HideFlags.HideInInspector)) == 0)
            {
                return;
            }
        }
        if (!this.objects.Contains(obj))
        {
            this.objects.Add(obj);
        }
    }

    private void FindObjects()
    {
        GameObject[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
        this.objects.Clear();

        foreach (GameObject obj in objs)
        {
            this.AddObject(obj);
        }
    }

    private HideFlags HideFlagsButton(string aTitle, HideFlags aFlags, HideFlags aValue)
    {
        if (GUILayout.Toggle((aFlags & aValue) > 0, aTitle, "Button"))
        {
            aFlags |= aValue;
        }
        else
        {
            aFlags &= ~aValue;
        }

        return aFlags;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("find objects"))
        {
            this.FindObjects();
        }

        this.filterTop = GUILayout.Toggle(this.filterTop, "only top objects");
        this.filterHidden = GUILayout.Toggle(this.filterHidden, "only hidden objects");
        GUILayout.EndHorizontal();
        this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);

        for (int i = 0; i < this.objects.Count; i++)
        {
            GameObject obj = this.objects[i];
            if (obj == null)
            {
                continue;
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(obj.name, obj, typeof(GameObject), true);
            HideFlags flags = obj.hideFlags;
            flags = this.HideFlagsButton("HideInHierarchy", flags, HideFlags.HideInHierarchy);
            flags = this.HideFlagsButton("HideInInspector", flags, HideFlags.HideInInspector);
            flags = this.HideFlagsButton("DontSave", flags, HideFlags.DontSave);
            flags = this.HideFlagsButton("NotEditable", flags, HideFlags.NotEditable);
            obj.hideFlags = flags;
            GUILayout.Label("" + ((int)flags), GUILayout.Width(20));
            GUILayout.Space(20);
            if (GUILayout.Button("DELETE"))
            {
                DestroyImmediate(obj);
                this.FindObjects();
                GUIUtility.ExitGUI();
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
    }
}