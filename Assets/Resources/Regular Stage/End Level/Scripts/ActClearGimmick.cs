using UnityEngine;

/// <summary>
/// Base class for objects that when touched signify the act is cleared
/// </summary>
public class ActClearGimmick : ContactEvent
{
    [Tooltip("The time line triggers to be played when an act is cleared"), SerializeField]
    protected CutsceneController actClearCutscene;

    /// <summary>
    /// Get the start position
    /// </summary>
    public Vector2 GetStartPosition() => this.startPosition;

    /// <summary>
    /// Actions that take place when the act clear gimmick is activated
    /// </summary>
    protected virtual void OnActClearActivation(Player player)
    {
        GMStageManager.Instance().SetActClearGimmick(this);
        player.GetHedgePowerUpManager().RevertSuperForm();
        HedgehogCamera.Instance().SetCameraTarget(this.gameObject);
        HedgehogCamera.Instance().SetCameraMode(CameraMode.EndLevel);
        GMCutsceneManager.Instance().SetActClearCutscene(this.actClearCutscene);
        GMStageManager.Instance().SetStageState(RegularStageState.ActClear);
    }
}
