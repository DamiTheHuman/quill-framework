using UnityEngine;

public class SceneListAttribute : PropertyAttribute
{
    public bool disabled = false;
    public SceneListAttribute(bool disabled = false) => this.disabled = disabled;
}