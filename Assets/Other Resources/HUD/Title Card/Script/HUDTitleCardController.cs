using UnityEngine;
/// <summary>
/// Controls the events of the Title Card when available
/// </summary>
public class HUDTitleCardController : MonoBehaviour
{
    /**
    /// If you are wondering what is disabling this on start of a stage its in the <see  cref="GMStageHUDManager"> Start Function</see>
    */

    /// <summary>
    /// Begin updating the player score and allow movement
    /// </summary>
    public void BeginScoreUpdate()
    {
        if (GMStageManager.Instance().GetStageState() != RegularStageState.ActClear)
        {
            GMStageManager.Instance().SetStageState(RegularStageState.Running);
        }
    }

    /// <summary>
    /// Deactivates the title card
    /// </summary>
    public void DeactivateTitleCard() => this.gameObject.SetActive(false);
}
