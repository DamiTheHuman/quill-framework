using System.Collections;
using UnityEngine;
/// <summary>
/// Controls the activity of the power sneakers 
/// </summary>
public class PowerSneakersPowerUpController : HedgePowerUp
{
    [Tooltip("A flag used to determine whether the player currently has power sneakers")]
    private bool powerSneakersActive;
    [Tooltip("How long the speedshoes should last when active in steps")]
    public float powerSneakersTimerInSteps = 1200f;
    private IEnumerator PowerSneakersCountDownCoroutine;

    /// <summary>
    /// Give the player the speed shoes and begin the timer
    /// </summary>
    public void GrantPowerSneakers()
    {
        this.powerSneakersActive = true;

        if (this.PowerSneakersCountDownCoroutine == null)
        {
            this.PowerSneakersCountDownCoroutine = this.CountDownPowerSneakersTimer();
            this.StartCoroutine(this.PowerSneakersCountDownCoroutine);
        }
        else
        {
            this.StopCoroutine(this.PowerSneakersCountDownCoroutine);
            this.PowerSneakersCountDownCoroutine = this.CountDownPowerSneakersTimer();
            this.StartCoroutine(this.PowerSneakersCountDownCoroutine);
        }
    }

    /// <summary>
    /// Counts down the Speed shoes timer
    /// </summary>
    private IEnumerator CountDownPowerSneakersTimer()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.powerSneakersTimerInSteps));
        this.powerSneakersActive = false;//Reset the flag for speedshoes
        GMAudioManager.Instance().StopBGM(BGMToPlay.PowerSneakers);//End the Power sneakertheme
    }

    public override bool PowerUpIsActive() => this.powerSneakersActive;
}
