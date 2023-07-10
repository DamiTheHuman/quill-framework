using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

internal static class EditorTypes
{
    public static Dictionary<int, List<FieldInfo>> fields = new Dictionary<int, List<FieldInfo>>(FastComparable.Default);
    /// <summary>
    ///  Get the length of all the properties within a script
    /// <param name="target"> The current target </param>
    /// <param name="objectFields"> The object fields</param>
    /// </summary>
    public static int GetAllPropertiesLength(Object target, out List<FieldInfo> objectFields)
    {
        System.Type t = target.GetType();
        int hash = t.GetHashCode();

        if (!fields.TryGetValue(hash, out objectFields))
        {
            IList<System.Type> typeTree = t.GetTypeTree();
            objectFields = target.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic)
                    .OrderByDescending(x => typeTree.IndexOf(x.DeclaringType))
                    .ToList();
            fields.Add(hash, objectFields);
        }

        return objectFields.Count;
    }
}

