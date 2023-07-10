using UnityEngine;
/// <summary>
/// A trigger which activates the boss fight
/// Author - Nihil - Core Framework
/// </summary>
public class BossTrigger : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [Tooltip("The boundaries of the boss zone"), SerializeField]
    private CameraBounds bossFightBounds = new CameraBounds();
    [Tooltip("The boss to be activated when the player crosses the middle point"), SerializeField]
    private Boss activeBoss;
    [Tooltip("The debug boundaries of the zone"), SerializeField]
    private Color debugColor = Color.red;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.activeBoss.SetBossTrigger(this);
    }

    /// <summary>
    /// Checks if the players comes in contact with the trigger
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerGimmick = false;
        triggerGimmick = this.activeBoss.GetBossPhase() == BossPhase.Idle;

        return true;
    }

    /// <summary>
    /// Set the camera to boss mode
    /// <param name="player">The player object to come in contact with the trigger </param>
    /// </summary
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        GMStageManager.Instance().SetBoss(this.activeBoss);
        HedgehogCamera.Instance().SetCameraMode(CameraMode.BossMode);
    }

    /// <summary>
    /// Waits for the player to cross the middle of the trigger before spawning the boss
    /// <param name="player">The player object to come in contact wit the trigger </param>
    /// </summary
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);

        if (player.transform.position.x >= this.bossFightBounds.GetCenterPosition().x && this.activeBoss.GetBossPhase() == BossPhase.Idle)
        {
            this.activeBoss.OnActivation(this);
        }
    }

    /// <summary>
    /// Get the bounds for the boss fight
    /// </summary
    public CameraBounds GetBossFightBounds() => this.bossFightBounds;

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }

        Gizmos.color = Color.red;

        if (this.activeBoss && this.activeBoss.GetBossPhase() == BossPhase.Idle)
        {
            GizmosExtra.Draw2DArrow(this.bossFightBounds.GetCenterPosition(), 90);
            GizmosExtra.Draw2DArrow(this.bossFightBounds.GetCenterPosition(), 270);
            Gizmos.DrawLine(new Vector2(this.bossFightBounds.GetCenterPosition().x, this.bossFightBounds.GetTopBorderPosition()), new Vector2(this.bossFightBounds.GetCenterPosition().x, this.bossFightBounds.GetBottomBorderPosition()));
        }

        Gizmos.DrawLine(this.bossFightBounds.GetTopLeftBorderPosition(), this.bossFightBounds.GetTopRightBorderPosition());//Top Horizontal Bounds
        Gizmos.DrawLine(this.bossFightBounds.GetBottomLeftBorderPosition(), this.bossFightBounds.GetBottomRightBorderPosition());//Bottom Horizontal Bounds
        Gizmos.DrawLine(this.bossFightBounds.GetTopLeftBorderPosition(), this.bossFightBounds.GetBottomLeftBorderPosition());//Left Vertical Bounds
        Gizmos.DrawLine(this.bossFightBounds.GetTopRightBorderPosition(), this.bossFightBounds.GetBottomRightBorderPosition());//Right Vertical Bound
    }
}
