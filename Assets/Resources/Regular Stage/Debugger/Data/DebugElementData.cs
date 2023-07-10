using TMPro;
/// <summary>
/// Handle data for debug elements used
/// </summary>
[System.Serializable]
public class DebugElementData
{
    public DebugElementNames tag;
    public TextMeshPro uiObject;

    /// <summary>
    /// Play an animation by its name in the animator
    /// <param name="name">The tag of the UI elemeent</param>
    /// <param name="uiObject">The text ui element to place the text</param>
    /// </summary>
    public DebugElementData(DebugElementNames name, TextMeshPro uiObject)
    {
        this.tag = name;
        this.uiObject = uiObject;
    }

    /// <summary>
    /// Set the text displayed on the ui element
    /// <param name="value">The value of the debug element ui text field</param>
    /// </summary>
    public void SetUiText<T>(T value)
    {
        if (value == null)
        {
            this.uiObject.text = this.tag + ":" + "N/A";
            return;
        }

        this.uiObject.text = this.tag + ":" + value.ToString();
    }
}
