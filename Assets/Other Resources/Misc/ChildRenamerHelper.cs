using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Helper class to rename the values of child game objects to a unified string
/// </summary>
public class ChildRenamerHelper : MonoBehaviour
{
    [SerializeField, Tooltip("The new child names")]
    private string renameChildrenTo = "Child";
    [Button(nameof(RenameChildren))]
    public bool renameChildren = false;

    private void Reset()
    {
        List<Transform> directChildren = this.GetComponentsInChildren<Transform>().ToList();

        for (int x = 0; x < directChildren.Count; x++)
        {
            if (directChildren[x].transform.parent == this.transform)
            {
                this.renameChildrenTo = directChildren[x].name.Split("(")[0].Trim();

                break;
            }
        }
    }
    /// <summary>
    /// Renames the child components of all direct child transforms to follow the structure of the <see cref="renameChildrenTo"/> Value
    /// </summary>
    public void RenameChildren()
    {
        List<Transform> directChildren = this.GetComponentsInChildren<Transform>().ToList();
        int count = 0;

        for (int x = 0; x < directChildren.Count; x++)
        {
            if (directChildren[x].transform.parent == this.transform)
            {
                directChildren[x].name = this.renameChildrenTo + " (" + count + ")";
                count++;
            }
        }
    }
}
