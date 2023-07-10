using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Updates the value of the set animator key to that of the current stage/scene act
/// </summary>
public class UIActDependantController : MonoBehaviour
{
    [SerializeField, Tooltip("The current image")]
    private Image currentImage;
    [SerializeField, Tooltip("The list of sprites")]
    private List<Sprite> actSprites;

    private void Update() => this.UpdateActImage();
    /// <summary>
    /// Update the image of the act based on the current scene act value
    /// </summary>
    public void UpdateActImage()
    {
        int act = (int)GMSceneManager.Instance().GetCurrentSceneData().GetActNumber() - 1;
        act = Mathf.Clamp(act, 0, this.actSprites.Count - 1);
        this.currentImage.sprite = this.actSprites[act];
    }
}
