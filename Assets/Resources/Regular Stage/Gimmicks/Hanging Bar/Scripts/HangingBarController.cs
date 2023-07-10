using UnityEngine;

public class HangingBarController : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    public Vector2 playerPositionOnBar = new Vector2(0, -20);
    public Color debugColor = Color.red;

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
    }

    /// <summary>
    /// Begins the process of childing if the player is within activatable range of the handle, not hanging and moving downards
    /// <param name="player">The player object interacting with the gimmick </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if ((solidBoxColliderBounds.center.x > this.boxCollider2D.bounds.min.x) && (solidBoxColliderBounds.center.x < this.boxCollider2D.bounds.max.x))
        {
            triggerAction = player.velocity.y < 0 && player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.OnHandle;
        }

        return triggerAction;
    }

    /// <summary>
    /// Update the players gimmick status and child them to the handle for consistent movement
    /// <param name="player">The player object interacting with the gimmick </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.OnHandle);
        player.transform.parent = this.transform;
        player.transform.localPosition = new Vector2(player.transform.localPosition.x, this.playerPositionOnBar.y);//Center the player on the object
        player.SetMovementRestriction(MovementRestriction.Both);
        player.GetActionManager().EndCurrentAction();
        player.SetBothHorizontalVelocities(0);
        player.velocity.y = 0;
        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.Hanging);
        GMAudioManager.Instance().PlayOneShot(player.GetPlayerActionAudio().grab);
    }

    /// <summary>
    /// Wait for the player to input the jump button  or get hurt to then release then from the bar
    /// <param name="player">The player object interacting with the gimmick </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        if (player.GetActionManager().CheckActionIsBeingPerformed<Die>() || player.GetActionManager().CheckActionIsBeingPerformed<Hurt>())
        {
            this.HedgeOnCollisionExit(player);
        }
        else if (player.GetInputManager().GetJumpButton().GetButtonPressed())
        {
            this.HedgeOnCollisionExit(player);
            player.GetActionManager().PerformAction<Jump>();
        }
    }

    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        player.GetAnimatorManager().SwitchGimmickSubstate(0);
        player.SetMovementRestriction(MovementRestriction.None);
        player.transform.parent = null;
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
    }

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D)
        {
            GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor);
        }
    }
}
