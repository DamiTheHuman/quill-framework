using UnityEngine;

public class DirectionListAttribute : PropertyAttribute
{
    public bool disabled = false;
    public DirectionListAttribute(bool disabled = false) => this.disabled = disabled;
}
