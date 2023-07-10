using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DebugItemData
{
    [HideInInspector, SerializeField]
    private string name;

    [SerializeField]
    private List<GameObject> variants = new List<GameObject>();
    /// <summary>
    /// Set the name of the debug item
    /// </summary>
    public void SetName(string name) => this.name = name;

    /// <summary>
    /// Gets a list of variants
    /// </summary>
    public List<GameObject> GetVariants() => this.variants;
}
