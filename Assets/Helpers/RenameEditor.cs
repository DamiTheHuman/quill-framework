using UnityEngine;
/// <summary>
/// Renames all the child transforms to the same value and index
/// </summary>
public class RenameEditor : MonoBehaviour
{
    [SerializeField, Button("Rename")]
    private bool rename;
    public void Rename()
    {
        foreach (Transform transform in this.GetComponentsInChildren<Transform>())
        {
            if (transform != this.transform)
            {
                transform.name = this.name + " " + transform.GetSiblingIndex();
            }
        }
    }
}
