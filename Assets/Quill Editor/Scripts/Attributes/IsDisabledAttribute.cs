using UnityEngine;

public class IsDisabledAttribute : PropertyAttribute
{
    public bool isDisabled = true;

    public IsDisabledAttribute(bool disabled = true) => this.isDisabled = disabled;
}
