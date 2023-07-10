using UnityEngine;
/// <summary>
/// The screw controller serves as a base point for the screw bolt to retrieve information from
/// Author - Originally coded by Damizean & DW
/// Update - Slight modifications to make it more personalized
/// </summary
public class ScrewController : MonoBehaviour
{
    [SerializeField]
    private bool inverted;
    [SerializeField]
    protected ScrewBoltController screwBoltController;
    [SerializeField]
    protected ScrewActionPointData screwTopActionPoint;
    [SerializeField]
    protected ScrewActionPointData screwBottomActionPoint;

    private void Reset() => this.screwBoltController = this.GetComponentInChildren<ScrewBoltController>();

    // Start is called before the first frame update
    private void Start()
    {
        if (this.screwBoltController == null)
        {
            this.Reset();
        }

        this.screwBoltController.SetScrewController(this);
    }

    /// <summary>
    /// Get the top screw action point
    /// </summary>
    public ScrewActionPointData GetTopActionPoint() => this.screwTopActionPoint;
    /// <summary>
    /// Get the bottom screw action point
    /// </summary>
    public ScrewActionPointData GetBottomActionPoint() => this.screwBottomActionPoint;

    /// <summary>
    /// Get the flag determining whether the screw movement is inverted
    /// </summary>
    public bool GetInverted() => this.inverted;
    private void OnValidate()
    {
        if (this.screwTopActionPoint.GetScrewAction() == ScrewAction.Fall)
        {
            Debug.LogWarning("Fall is an inappropriate type for the top of the screw!");

            this.screwTopActionPoint.SetScrewAction(ScrewAction.ReleasePlayer);
        }
    }
}

