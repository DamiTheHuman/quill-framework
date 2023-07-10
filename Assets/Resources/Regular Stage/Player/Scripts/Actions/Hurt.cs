using System.Collections;
using UnityEngine;
/// <summary>
/// Performs the hurt action when the player gets hit
/// </summary>
public class Hurt : HedgePrimaryAction
{
    [Tooltip("The direction to make the player fall in"), SerializeField]
    private int hurtFallDirection = 0;
    [Tooltip("How long to flash the player sprite for"), SerializeField]
    private float flashDuration = 120f;
    [Tooltip("How many steps to update the transparency"), SerializeField]
    private float flashTransparencyUpdateStep = 4f;
    private IEnumerator hurtFlashCoroutine;

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 8;
    }
    /// <summary>
    /// Play the hurt animation and knock the player backwards
    /// </summary>
    public override void OnActionStart()
    {
        //Makes the spike hurt clip play instead of the regular hurt clip if the player contacts a spike
        AudioClip audioClipToPlay = this.player.GetSolidBoxController().IsTouchingEventOfType<SpikeController>() ? this.player.GetPlayerActionAudio().spikeHurt : this.player.GetPlayerActionAudio().hurt;
        GMAudioManager.Instance().PlayOneShot(audioClipToPlay);
        this.player.GetSpriteController().SetSpriteAngle(0);
        this.player.GetSolidBoxController().UpdateSolidBoxAngle(this.player.GetSpriteController().GetSpriteAngle());
        this.player.GetHitBoxController().UpdateHitBoxAngle(this.player.GetSpriteController().GetSpriteAngle());
        this.player.GetActionManager().EndCurrentAction();//Forcefully end the current action
        this.player.GetAnimatorManager().SwitchActionSubstate(ActionSubState.Hurt);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
        this.player.SetGrounded(false);

        if (this.hurtFallDirection == 0)
        {
            this.hurtFallDirection = this.player.currentPlayerDirection * -1;
        }//If no direction is set just fall in the opposite direction of the player
        this.player.velocity = this.player.GetPlayerPhysicsInfo().knockBackVelocity * new Vector2(this.hurtFallDirection, 1f);//Get the jump velocity in relation the current ground angle
    }
    /// <summary>
    /// Exit the hurt action when the player hits the ground
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded())
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// End the hurt action and animation then use a coroutine to flash the player sprite
    /// </summary>
    public override void OnActionEnd()
    {
        if (this.hurtFlashCoroutine != null)
        {
            this.StopCoroutine(this.BeginFlashing());
        }

        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.SetBothHorizontalVelocities(0);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.hurtFlashCoroutine = this.BeginFlashing();
        this.StartCoroutine(this.hurtFlashCoroutine);
    }
    /// <summary>
    /// Sets the direction for the player to fall in
    /// <param name="hurtFallDirection">The X direction the player falls in</param>
    /// </summary>
    public void SetHurtFallDirection(int hurtFallDirection) => this.hurtFallDirection = hurtFallDirection != 0 ? hurtFallDirection : 1;

    /// <summary>
    /// Waits until the dropdash is fully charged before notifying that the dropdash is fully charged
    /// </summary>
    private IEnumerator BeginFlashing()
    {

        this.player.GetSpriteController().SetTransparency(255);

        for (int x = 0; x < this.flashDuration / this.flashTransparencyUpdateStep; x++)
        {
            if (this.player.GetSpriteController().GetTransparency() != 0)
            {
                this.player.GetSpriteController().SetTransparency(0);
            }
            else
            {
                this.player.GetSpriteController().SetTransparency(255);
            }
            yield return new WaitForSeconds(General.StepsToSeconds(this.flashTransparencyUpdateStep));
        }
        if (this.player.GetHealthManager().GetHealthStatus() is not HealthStatus.Invincible and not HealthStatus.Death)
        {
            this.player.GetHealthManager().SetHealthStatus(HealthStatus.Vulnerable);//Return the player to a vulnerable state on end as long as they are not invincible or dead
        }
    }
}
