using UnityEngine;
/// <summary>
/// A Class to handle common animator actions that take place during a cutscene all prefixed with a "_" so its easy to find and in order
/// </summary>
public class PlayerCutsceneCommands : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private void Reset() => this.player = this.GetComponent<Player>();

    // Start is called before the first frame update
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Play a specific animation
    /// </summary>
    private void _PlayAnimation(string animation) => this.player.GetAnimatorManager().PlayAnimation(animation);
    /// <summary>
    /// Sets the update mode of the animator usefuly when using custom animations
    /// </summary>
    private void _SetAnimatorUpdateMode(AnimatorUpdateMode updateMode) => this.player.GetAnimatorManager().SetUpdateMode(updateMode);
    /// <summary>
    /// Call the player Jump Action
    /// </summary>
    private void _CallActionJump()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>() == false)
        {
            this.player.GetActionManager().PerformAction<Jump>();
        }
    }
    /// <summary>
    /// Call the player transform cation 
    /// </summary>
    private void _CallActionTransform() => this.player.GetActionManager().PerformAction<SuperTransform>();
    /// <summary>
    /// Revert the player super form action
    /// </summary>
    private void _CallActionRevertSuper()
    {
        this.player.GetHedgePowerUpManager().RevertSuperForm();
        ;
    }
    /// <summary>
    /// Call the player hurt state
    /// </summary>
    private void _CallActionHurt(int x)
    {
        this.player.GetComponentInChildren<Hurt>().SetHurtFallDirection(x);
        this.player.GetActionManager().PerformAction<Hurt>();
    }

    /// <summary>
    /// Call the player roll Action
    /// </summary>
    private void _CallActionRoll()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Roll>() == false)
        {
            this.player.GetActionManager().PerformAction<Roll>();
        }
    }
    /// <summary>
    /// Set the player horizontal velocity
    /// </summary>
    private void _SetPlayerHorizontalVelocity(float x) => this.player.SetBothHorizontalVelocities(x);
    /// <summary>
    /// Set the player vertical velocity
    /// </summary>
    private void _SetPlayerVerticalVelocity(float y)
    {
        if (y > 0)
        {
            this.player.SetGrounded(false);
        }

        this.player.velocity.y = y;
    }
    /// <summary>
    /// Set the Current Player Direction
    /// </summary>
    public void _SetCurrentPlayerDirection(int currentPlayerDirection)
    {
        this.player.currentPlayerDirection = currentPlayerDirection;
        this.player.GetSpriteController().SetSpriteDirection(currentPlayerDirection);
    }

    /// <summary>
    /// Set the Update Mode for the animator Manager
    /// </summary>
    public void _SetUpdateWithoutFiringTriggers(AnimatorUpdateMode updateMode) => this.player.GetAnimatorManager().SetUpdateMode(updateMode);

    /// <summary>
    /// Set the input override X Value
    /// </summary>
    private void _SetPlayerXInputOverride(int x) => this.player.GetInputManager().SetInputOverride(new Vector2(x, this.player.GetInputManager().GetInputOverride().y));
    /// <summary>
    /// Set the input override Y Value
    /// </summary>
    private void _SetPlayerYInputOverride(int y) => this.player.GetInputManager().SetInputOverride(new Vector2(this.player.GetInputManager().GetInputOverride().x, y));
    /// <summary>
    /// Set the player state
    /// </summary> 
    private void _SetPlayerState(PlayerState playerState) => this.player.SetPlayerState(playerState);

    /// <summary>
    /// Set the player gimmick state
    /// </summary> 
    private void _SetGimmickState(GimmickSubstate gimmickSubstate) => this.player.GetAnimatorManager().SwitchGimmickSubstate(gimmickSubstate);

}
