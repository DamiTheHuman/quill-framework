using System.Collections;
using UnityEngine;
/// <summary>
/// Controls the interactions between the invincibility powerups, the player and other powerup objects
/// </summary>
public class InvincibilityPowerUpController : HedgePowerUp
{
    [Tooltip("How long the invincibility power up lasts in steps")]
    public float invinciblityTimerInSteps = 1200f;
    private IEnumerator InvincibilityCoroutine;

    /// <summary>
    /// Gives the player invincibility status and sets up the invincibility trail renderer to the player
    /// <param ref name="invincibilityGameObject"> The gameobject relating to the invincibility being granted </param>
    /// </summary>
    public void GrantInvincibility(GameObject invincibilityGameObject)
    {
        this.activePowerUp = invincibilityGameObject;
        this.activePowerUp.GetComponent<InvincibilityTrailRenderer>().SetTarget(this.hedgePowerUpManager.effectPivotPoint, this.player);

        if (this.hedgePowerUpManager.GetShieldPowerUp().GetActivePowerUpGameObject() != null)
        {
            this.hedgePowerUpManager.GetShieldPowerUp().SetPowerUpVisbility(false);
        }
        if (this.hedgePowerUpManager.GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            this.activePowerUp.SetActive(false);
        }

        this.player.GetHealthManager().SetHealthStatus(HealthStatus.Invincible);//Make the player invincible
        this.BeginInvincibilityTimer();
        this.RetriveAllPowerUpSpriteData();

        if (this.hedgePowerUpManager.GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            this.SetPowerUpVisbility(false);
        }
    }

    /// <summary>
    /// Begins the countdown of the invincibility timer
    /// </summary>
    public void BeginInvincibilityTimer()
    {
        if (this.InvincibilityCoroutine == null)
        {
            this.InvincibilityCoroutine = this.CountDownInvincibility();
            this.StartCoroutine(this.InvincibilityCoroutine);
        }
        else
        {
            this.StopCoroutine(this.InvincibilityCoroutine);
            this.InvincibilityCoroutine = this.CountDownInvincibility();
            this.StartCoroutine(this.InvincibilityCoroutine);
            if (this.activePowerUp.activeSelf)
            {
                this.activePowerUp.GetComponent<InvincibilityTrailRenderer>().BroadcastMessage(InvincibilityTrailRenderer.resetFadoutFunctionMessage);
            }
        }
    }

    /// <summary>
    /// Remove the power up but also re instantiate a shield if one was previously available
    /// </summary>
    public override void RemovePowerUp()
    {
        base.RemovePowerUp();
        this.player.GetHealthManager().SetHealthStatus(HealthStatus.Vulnerable);
        //Restore the shield if the player previously had on as long as the player isnt super
        if (this.hedgePowerUpManager.GetShieldPowerUp().GetActivePowerUpGameObject() != null && this.hedgePowerUpManager.GetSuperPowerUp() == SuperPowerUp.None)
        {
            this.hedgePowerUpManager.GetShieldPowerUp().SetPowerUpVisbility(true);
        }
        GMAudioManager.Instance().StopBGM(BGMToPlay.InvincibilityTheme);//End the Invincibility theme
    }

    /// <summary>
    /// Counts down the Invincibility Timer
    /// </summary>
    private IEnumerator CountDownInvincibility()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.invinciblityTimerInSteps - 60));

        if (this.activePowerUp.activeSelf)
        {
            this.activePowerUp.GetComponent<InvincibilityTrailRenderer>().BroadcastMessage(InvincibilityTrailRenderer.fadeOutFunctionMessage);
        }
        yield return new WaitForSeconds(General.StepsToSeconds(60));
        this.RemovePowerUp();
    }
}
