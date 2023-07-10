using UnityEngine;

[System.Serializable]
public class ScrewActionPointData
{
    [SerializeField]
    private Transform transform;
    [SerializeField]
    private ScrewAction screwAction = ScrewAction.ReleasePlayer;

    /// <summary>
    /// Get the transform relating to the screw action point
    /// </summary>
    public Transform GetTransform() => this.transform;

    /// <summary>
    /// Get the screw action when the player reaches the point in the screw
    /// </summary>
    public ScrewAction GetScrewAction() => this.screwAction;

    /// <summary>
    /// Set the screw action when the player reaches the point in the screw
    /// </summary>
    public void SetScrewAction(ScrewAction screwAction) => this.screwAction = screwAction;
}
