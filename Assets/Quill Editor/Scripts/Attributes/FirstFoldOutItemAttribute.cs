using UnityEngine;
/// <summary>
///  Create the serializable attribute that can be used
/// </summary>
public class FirstFoldOutItemAttribute : PropertyAttribute
{
    public string name;
    public string description;
    /// <summary>Adds the property to the specified foldout group.</summary>
    /// <param name="name">Name of the foldout group.</param>
    /// <param name="description">The extra description for the beginning of the fold</param>
    public FirstFoldOutItemAttribute(string name, string description = "")
    {
        this.name = name;
        this.description = description;
    }
}
