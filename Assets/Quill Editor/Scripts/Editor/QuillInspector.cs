using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Object), true, isFallback = true)]
[CanEditMultipleObjects]
public class QuillInspector : Editor
{
    //===============================//
    // Members
    //===============================//F

    private Dictionary<string, CacheFoldProp> cacheFolds = new Dictionary<string, CacheFoldProp>();
    private List<SerializedProperty> props = new List<SerializedProperty>();
    private List<MethodInfo> methods = new List<MethodInfo>();
    private bool initialized;

    //===============================//
    // Logic
    //===============================//

    private void OnEnable() => this.initialized = false;

    private void OnDisable()
    {
        //if (Application.wantsToQuit)
        //if (applicationIsQuitting) return;
        //	if (Toolbox.isQuittingOrChangingScene()) return;

        if (this.target != null)
        {
            foreach (KeyValuePair<string, CacheFoldProp> c in this.cacheFolds)
            {
                EditorPrefs.SetBool(string.Format($"{c.Value.atr.name}{c.Value.props[0].name}{this.target.name}"), c.Value.expanded);
                c.Value.Dispose();
            }
        }
    }

    public override bool RequiresConstantRepaint() => EditorFramework.needToRepaint;

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        this.Setup();

        //If no props are found just draw the default inspector otherwise
        if (this.props.Count == 0)
        {
            this.DrawDefaultInspector();

            return;
        }
        else
        {
            //Draw the custom inspector
            this.DrawAstroInspector();
        }
    }

    /// <summary>
    /// Set up the caching for the script
    /// </summary>
    private void Setup()
    {
        EditorFramework.currentEvent = Event.current;

        if (!this.initialized)
        {
            //	SetupButtons();
            List<FieldInfo> objectFields;
            FirstFoldOutItemAttribute prevFold = default;
            int length = this.GetVariableFields(this.target.GetType()).Count;
            objectFields = this.GetVariableFields(this.target.GetType());

            for (int i = 0; i < length; i++)
            {
                #region FOLDERS
                CacheFoldProp c;

                if (Attribute.GetCustomAttribute(objectFields[i], typeof(FirstFoldOutItemAttribute)) is not FirstFoldOutItemAttribute currentFold)
                {
                    //If the current fold is null check the previos fld
                    if (prevFold != null)
                    {
                        //If the previous cache name is not empty add its content to the array and start a new fold
                        if (!this.cacheFolds.TryGetValue(prevFold.name, out c))
                        {
                            this.cacheFolds.Add(prevFold.name, new CacheFoldProp { atr = prevFold, types = new HashSet<string> { objectFields[i].Name } });
                        }
                        //Add its existing content to the fold
                        else
                        {
                            c.types.Add(objectFields[i].Name);
                            //
                            if (Attribute.GetCustomAttribute(objectFields[i], typeof(LastFoldoutItemAttribute)) is LastFoldoutItemAttribute lastfoldoutProp)
                            {
                                prevFold = default;
                                continue;
                            }

                        }
                    }

                    continue;
                }
                //Set the previous fold to the current fold
                prevFold = currentFold;
                //Necessary for the first fold to be added correctly
                if (!this.cacheFolds.TryGetValue(currentFold.name, out c))
                {
                    bool expanded = EditorPrefs.GetBool(string.Format($"{currentFold.name}{objectFields[i].Name}{this.target.name}"), false);
                    this.cacheFolds.Add(currentFold.name, new CacheFoldProp { atr = currentFold, types = new HashSet<string> { objectFields[i].Name }, expanded = expanded });
                }

                #endregion
            }

            SerializedProperty property = this.serializedObject.GetIterator();
            bool next = property.NextVisible(true);
            if (next)
            {
                do
                {
                    this.HandleFoldProp(property);
                } while (property.NextVisible(false));
            }

            this.initialized = true;
        }
    }

    public List<FieldInfo> GetVariableFields(Type type)
    {
        BindingFlags bindingFlags = BindingFlags.DeclaredOnly | // This flag excludes inherited variables.
                            BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;
        List<FieldInfo> text = new List<FieldInfo>();
        //If its a childed object get the parents values too
        if (type.BaseType != null)
        {
            text.AddRange(type.BaseType.GetFields(bindingFlags));
        }

        text.AddRange(type.GetFields(bindingFlags));

        return text;
    }

    private void DrawAstroInspector()
    {
        Header();
        Body();
        this.serializedObject.ApplyModifiedProperties();

        /// <summary>
        /// Create the header of the foldout view
        /// </summary>
        void Header()
        {
            using (new EditorGUI.DisabledScope("m_Script" == this.props[0].propertyPath))
            {
                EditorGUILayout.Space();
                //Set the first field property hich is usually the script
                EditorGUILayout.PropertyField(this.props[0], true);
                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Create the body of the view
        /// </summary>
        void Body()
        {
            //Space between the top script and the other fields
            EditorGUILayout.Space();
            List<SerializedProperty> cacheValues = new List<SerializedProperty>();
            List<KeyValuePair<string, CacheFoldProp>> cachePair = new List<KeyValuePair<string, CacheFoldProp>>();
            //Start the loop from 1 to skip the script field as a prop
            for (int i = 1; i < this.props.Count; i++)
            {
                foreach (KeyValuePair<string, CacheFoldProp> pair in this.cacheFolds)
                {
                    bool setContent = false;
                    for (int x = 0; x < pair.Value.props.Count; x++)
                    {
                        if (pair.Value.props[x] == this.props[i])
                        {
                            if (this.props.Contains(pair.Value.props[x]))
                            {
                                //Debug.Log(pair.Value);
                                setContent = true;
                                cacheValues.Add(this.props[i]);
                                break;
                            }

                        }
                        else
                        {
                            if (!this.props.Contains(pair.Value.props[x]))
                            {
                                break;
                            }
                        }
                    }
                    
                    if (setContent)
                    {
                        if (cachePair.Contains(pair) == false)
                        {
                            cachePair.Add(pair);
                            this.UseVerticalLayout(() => Foldout(pair.Value), QuillFrameworkEditorStyle.box);
                            EditorGUI.indentLevel = 0;
                        }
                    }

                }

                if (!cacheValues.Contains(this.props[i]))
                {
                    EditorGUILayout.PropertyField(this.props[i], true);
                }

            }

            EditorGUILayout.Space();

            if (this.methods == null)
            {
                return;
            }

            foreach (MethodInfo memberInfo in this.methods)
            {
                this.UseButton(memberInfo);
            }
        }
        /// <summary>
        /// Set up the foldout content and cache the content and display the content if its contains child nodes
        /// </summary>
        void Foldout(CacheFoldProp cache)
        {
            //the description text if needed
            string description = cache.atr.description != "" ? " - " + cache.atr.description : "";
            //Display the expanded text based on its attributes
            cache.expanded = EditorGUILayout.Foldout(cache.expanded, cache.atr.name + description, true,
                    QuillFrameworkEditorStyle.foldout);
            //Check whether the foldout is expanded and if so display its content
            if (cache.expanded)
            {
                EditorGUI.indentLevel = 1;
                for (int i = 0; i < cache.props.Count; i++)
                {
                    this.UseVerticalLayout(() => Child(i), QuillFrameworkEditorStyle.text);
                }
            }
            /// <summary>
            /// Load the child content in the appropriate style
            /// </summary>
            void Child(int i) => EditorGUILayout.PropertyField(cache.props[i], new GUIContent(cache.props[i].name.FirstLetterToUpperCase()), true);
        }
    }

    /// <summary>
    /// Handle the fold property for the foldouts
    /// </summary>
    public void HandleFoldProp(SerializedProperty prop)
    {
        bool shouldBeFolded = false;

        foreach (KeyValuePair<string, CacheFoldProp> pair in this.cacheFolds)
        {
            if (pair.Value.types.Contains(prop.name))
            {
                SerializedProperty pr = prop.Copy();
                shouldBeFolded = true;
                pair.Value.props.Add(pr);
                this.props.Add(pr);
            }
        }

        if (shouldBeFolded == false)
        {
            SerializedProperty pr = prop.Copy();
            this.props.Add(pr);
        }
    }

    private class CacheFoldProp
    {
        public HashSet<string> types = new HashSet<string>();
        public List<SerializedProperty> props = new List<SerializedProperty>();
        public FirstFoldOutItemAttribute atr;
        public bool expanded;

        public void Dispose()
        {
            this.props.Clear();
            this.types.Clear();
            this.atr = null;
        }
    }
}
