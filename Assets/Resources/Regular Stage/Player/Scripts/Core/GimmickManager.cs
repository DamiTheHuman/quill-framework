using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class handles the players interactions with gimmicks within the stage
/// </summary>
public class GimmickManager : MonoBehaviour
{
    private Player player;
    [SerializeField, IsDisabled]
    private List<GimmickMode> groundGimmicks = new List<GimmickMode> { GimmickMode.None, GimmickMode.OnBridge, GimmickMode.OnScrewBolt };
    [Tooltip("The Active gimmick being interacted with"), SerializeField]
    private GimmickMode activeGimmickMode = GimmickMode.None;
    [Tooltip("The Angle set the gimmick sets the player at"), SerializeField]
    private float gimmickAngle;
    private void Start() => this.player = this.GetComponent<Player>();

    /// <summary>
    /// Set the current active gimmick mode
    /// <param name="activeGimmickMode">The active gimmick mode to be set</param>
    /// </summary>
    public void SetActiveGimmickMode(GimmickMode activeGimmickMode)
    {
        this.activeGimmickMode = activeGimmickMode;

        if (this.activeGimmickMode == GimmickMode.None)
        {
            this.gimmickAngle = 0;
        }
    }

    /// <summary>
    /// Get the currently active gimmick
    /// </summary>
    public GimmickMode GetActiveGimmickMode() => this.activeGimmickMode;

    /// <summary>
    /// Actions that take place when the player interacts with an air bubble
    /// </summary>
    public void AirBubble()
    {
        this.player.GetOxygenManager().ReplenishPlayerBreath();
        this.player.velocity = Vector2.zero;//Zero out the players velocity
        this.player.GetActionManager().EndCurrentAction();
        this.player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.Gulp);
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().gulp);
    }

    /// <summary>
    /// Get the gimmick angle
    /// </summary>
    public float GetGimmickAngle() => this.gimmickAngle;

    /// <summary>
    /// Set the gimmick angle
    /// </summary>
    public void SetGimmickAngle(float gimmickAngle) => this.gimmickAngle = gimmickAngle;

    /// <summary>
    /// Gets a list of active gimmick modes that don't affect the players animation on contact
    /// </summary>
    public List<GimmickMode> GetGroundedGimicks() => this.groundGimmicks;
}
