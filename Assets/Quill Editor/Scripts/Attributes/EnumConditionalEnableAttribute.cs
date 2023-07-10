using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Attribute to select a single layer.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class EnumConditionalEnableAttribute : PropertyAttribute
{
    public string targetEnum;
    public List<string> serializableClasses = new List<string>();
    public List<int> activeEnumIndexes = new List<int>();

    public EnumConditionalEnableAttribute(string targetName, int state)
    {
        this.targetEnum = targetName;
        this.activeEnumIndexes.Add(state);
    }

    public EnumConditionalEnableAttribute(string targetName, int state, string parents)
    {
        this.targetEnum = targetName;
        this.activeEnumIndexes.Add(state);

        if (parents != "")
        {
            this.serializableClasses = parents.Split(',').ToList();

            for (int x = 0; x < this.serializableClasses.Count; x++)
            {
                this.serializableClasses[x] = this.serializableClasses[x].Trim();
            }
        }
    }

    public EnumConditionalEnableAttribute(string targetName, params int[] states)
    {
        this.targetEnum = targetName;
        this.activeEnumIndexes = states.ToList();
    }

    public EnumConditionalEnableAttribute(string targetName, string parents, params int[] states)
    {
        this.targetEnum = targetName;
        this.activeEnumIndexes = states.ToList();


        if (parents != "")
        {
            this.serializableClasses = parents.Split(',').ToList();

            for (int x = 0; x < this.serializableClasses.Count; x++)
            {
                this.serializableClasses[x] = this.serializableClasses[x].Trim();
            }
        }
    }

}
