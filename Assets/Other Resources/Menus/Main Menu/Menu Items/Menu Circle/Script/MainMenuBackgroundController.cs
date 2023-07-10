using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Manages the background elements of the main menu
/// </summary>
public class MainMenuBackgroundController : MonoBehaviour
{
    [Tooltip("The save node which determines the color of the background elements"), SerializeField]
    public GameObject saveNode;
    [Tooltip("The list of background elements which are affected by the save node"), SerializeField]
    public List<BackgroundElement> backgroundElements;

    [System.Serializable]
    public class BackgroundElement
    {
        [Tooltip("The element image affected by the color set of the current node")]
        public Image elementImage;
        [Tooltip("The start color of the element image"), HideInInspector]
        public Color startColor;
        [Tooltip("The color of the element image when on the save node")]
        public Color saveNodeColor;
    }

    private void Start()
    {
        foreach (BackgroundElement backgroundElement in this.backgroundElements)
        {
            if (backgroundElement.elementImage == null)
            {
                return;
            }

            backgroundElement.startColor = backgroundElement.elementImage.color;
        }
    }

    /// <summary>
    /// When the menu is updated change the colors based on whether we are on the save node or not
    /// </summary>
    public void OnMenuUpdate(GameObject activeMenu)
    {
        foreach (BackgroundElement backgroundElement in this.backgroundElements)
        {
            //Check if the player is on the save node or not to know what color to select
            Color targetColor = activeMenu == this.saveNode ? backgroundElement.saveNodeColor : backgroundElement.startColor;
            backgroundElement.elementImage.color = targetColor;
        }
    }
}
