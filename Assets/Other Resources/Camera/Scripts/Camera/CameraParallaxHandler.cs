using System.Collections.Generic;
using UnityEngine;

public class CameraParallaxHandler : MonoBehaviour
{
    [Tooltip("A list of all the backgrounds responsible for following the camera")]
    private List<ParallaxController> parallaxBackgrounds = new List<ParallaxController>();

    /// <summary>
    /// Registers a background with parallax properties to the list of active backgrounds
    /// <param name="parallaxController"/> The background to be registered </param>
    /// </summary>
    public void RegisterBackGround(ParallaxController parallaxController) => this.parallaxBackgrounds.Add(parallaxController);

    /// <summary>
    /// Updates the positions of all the backgrounds registered to this camera
    /// </summary>
    public void UpdateParallaxBackgroundPositions()
    {
        foreach (ParallaxController parallaxController in this.parallaxBackgrounds)
        {
            parallaxController.UpdateParallaxPosition();
        }
    }
}
