using UnityEngine;
/// <summary>
/// The actions relating to the players death
/// </summary>
public class Die : HedgePrimaryAction
{
    [Tooltip("A flag to determine whether velocity has been added post death"), SerializeField]
    private bool addedVelocity;
    public override void Start() => base.Start();
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        this.actionID = 9;
        this.primaryAnimationID = ActionSubState.Die;
        base.Reset();
    }

    /// <summary>
    /// If the player sate is dead it can begin the death action
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetHealthManager().GetHealthStatus() == HealthStatus.Death)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// If the player state dead launch the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (!this.addedVelocity)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the player X and sets the players Y velocity to the death velocity alongside locking the controls
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetAnimatorManager().SwitchGimmickSubstate(0);
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.player.GetSpriteController().SetSpriteAngle(0);
        this.player.GetSolidBoxController().UpdateSolidBoxAngle(this.player.GetSpriteController().GetSpriteAngle());
        this.player.GetHitBoxController().UpdateHitBoxAngle(this.player.GetSpriteController().GetSpriteAngle());
        this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
        this.player.GetHedgePowerUpManager().RevertSuperForm();
        this.player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.WaterRun, false);
        this.player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.SpindashDust, false);
        HedgehogCamera.Instance().SetCameraMode(CameraMode.Freeze);// Freeze the camera

        if (this.player.GetPhysicsState() == PhysicsState.Underwater && this.player.GetOxygenManager().currentOxygen <= 0)
        {
            this.Drown();
        }
        else
        {
            this.RegularDeath();
        }

        this.UpdatePlayerDetails();
        GMStageManager.Instance().SetStageState(RegularStageState.Died);
        GMStageManager.Instance().FreezePhysicsCycle();
        this.addedVelocity = true;
    }

    /// <summary>
    /// Exit the death state once the player is no longer visible
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.player.GetSpriteController().GetSpriteRenderer().bounds.max) && this.player.velocity.y < 0)
        {
            this.player.gameObject.SetActive(false);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Restore the users input after the action is completed
    /// </summary>
    public override void OnActionEnd() => GMStageHUDManager.Instance().LaunchEndDeathSequence();
    /// <summary>
    /// Actions that take place when the player dies by regularly for example a spike hazard
    /// </summary>
    private void RegularDeath()
    {
        this.player.SetBothHorizontalVelocities(0);//Zero out the horizontal velocity
        this.player.velocity.y = 0;
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
        this.player.velocity = this.player.GetPlayerPhysicsInfo().deathVelocity;

        if (GMRegularStageScoreManager.Instance().TimeOver())
        {
            GMStageManager.Instance().SetDeathReason(PlayerDeathReason.TimeOver);
        }
        else
        {
            GMStageManager.Instance().SetDeathReason(PlayerDeathReason.RegularDeath);
        }
    }

    /// <summary>
    /// Actions that take place when the player drowns
    /// </summary>
    private void Drown()
    {
        this.player.GetAnimatorManager().SetOtherAnimationSubstate(1);
        this.player.SetBothHorizontalVelocities(0);//Zero out the horizontal velocity
        this.player.velocity.y = 0;
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().drowned);
        GMStageManager.Instance().SetDeathReason(PlayerDeathReason.Drowned);
    }

    /// <summary>
    /// Update details relating to the player
    /// </summary>
    private void UpdatePlayerDetails()
    {
        this.player.SetGrounded(false);
        this.player.GetSolidBoxController().SetSolidBoxColliderEnabled(false);//Disable the solid box
        this.player.GetHitBoxController().SetSolidBoxColliderState(false);//Disable the hit box

        if (this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.None)
        {
            this.player.GetHedgePowerUpManager().GetShieldPowerUp().RemovePowerUp();
        }

        if (this.player.GetHedgePowerUpManager().GetInvincibilityPowerUp().PowerUpIsActive())
        {
            this.player.GetHedgePowerUpManager().GetInvincibilityPowerUp().RemovePowerUp();
        }
    }
}
