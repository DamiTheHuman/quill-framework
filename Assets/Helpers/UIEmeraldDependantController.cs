using UnityEngine;
using UnityEngine.UI;

public class UIEmeraldDependantController : MonoBehaviour
{
    [SerializeField, Tooltip("The minimum amount of emeralds needed before swapping occurs")]
    private int emeraldId = 1;
    [SerializeField, Tooltip("The current image")]
    private Image currentImage;
    [SerializeField, Tooltip("The sprite of the image when the conditio fails")]
    private Sprite baseSprite;
    [SerializeField, Tooltip("The sprite of the current image when available")]
    private Sprite targetSprite;

    // Start is called before the first frame update
    private void Start()
    {
        if (this.targetSprite == null)
        {
            Debug.LogError("Please set a target sprite for " + this.gameObject.name);
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update() => this.currentImage.sprite = GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds() >= this.emeraldId ? this.targetSprite : this.baseSprite;
}
