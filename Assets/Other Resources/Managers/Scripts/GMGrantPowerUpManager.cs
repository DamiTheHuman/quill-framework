using System;
using System.Collections;
using UnityEngine;
/// <summary>
///Handles the way power ups and other bonuses are granted to the player
/// </summary>
public class GMGrantPowerUpManager : MonoBehaviour
{
    [Tooltip("Checks whether the superform is available"), SerializeField]
    private bool superFormAvailable = false;
    [Help("Please input the timer in steps - (timeInSeconds * 59.9999995313f)")]
    public float stepsTillPowerupGrant = 31;

    /// <summary>
    /// The single instance of the Power Up level manager
    /// </summary>
    private static GMGrantPowerUpManager instance;

    // Start is called before the first frame update
    private void Start() => instance = this;
    private void Update()
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage)
        {
            this.UpdateSuperFormAvailable();
        }
    }

    /// <summary>
    /// Get a reference to the static instance of the power up  manager
    /// </summary>
    public static GMGrantPowerUpManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMGrantPowerUpManager>();
        }

        return instance;
    }

    /// <summary>
    /// Implements a delay before the player is granted a power up
    /// <param name="player">The player object</param>
    /// <param name="powerUpToGrant">The power up to be granted </param>
    /// <param name="powerUpGrantSound">The sound clip played when the audio is granted </param>
    /// </summary>
    public void DelayPowerUpGrant(Player player, PowerUp powerUpToGrant, AudioClip powerUpGrantSound) => this.StartCoroutine(this.DelayItemGrantCoroutine(player, powerUpToGrant, this.stepsTillPowerupGrant, powerUpGrantSound));

    /// <summary>
    /// Coroutine that waits a while before granting the player the specified powerup
    /// <param name="player">The player object</param>
    /// <param name="powerUpToGrant">The power up to be granted </param>
    /// <param name="delayInSteps">The delay before a power up is granted</param>
    /// <param name="powerUpGrantSound">The sound clip played when the audio is granted </param>
    /// </summary>
    private IEnumerator DelayItemGrantCoroutine(Player player, PowerUp powerUpToGrant, float delayInSteps, AudioClip powerUpGrantSound)
    {
        yield return new WaitForSeconds(General.StepsToSeconds(delayInSteps));
        //Grant Item
        this.GrantPowerUp(player, powerUpToGrant);

        if (powerUpGrantSound != null)
        {
            GMAudioManager.Instance().PlayOneShot(powerUpGrantSound);
        }
    }

    /// <summary>
    /// Grants the player the specified power up
    /// <param name="player">The player object </param>
    /// <param name="powerUpToGrant">The power up to grant</param>
    /// </summary>
    public void GrantPowerUp(Player player, PowerUp powerUpToGrant)
    {
        if (player.GetPlayerState() == PlayerState.Sleep)
        {
            return;
        }

        //Check if the player already has the shield being set
        if (player.GetHedgePowerUpManager().GetShieldPowerUp().AlreadyHasShield(powerUpToGrant))
        {
            return;
        }

        if (player.GetHedgePowerUpManager().GetInvincibilityPowerUp().PowerUpIsActive() && powerUpToGrant == PowerUp.Invincibility)
        {
            player.GetHedgePowerUpManager().GetInvincibilityPowerUp().BeginInvincibilityTimer();

            return;
        }

        GameObject powerupGameObject;
        switch (powerUpToGrant)
        {
            case PowerUp.TenRings:
                GMRegularStageScoreManager.Instance().IncrementRingCount(10);

                break;
            case PowerUp.ExtraLife:
                GMRegularStageScoreManager.Instance().IncrementLifeCount(1);

                break;
            case PowerUp.RegularShield:
                powerupGameObject = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RegularShield, player.transform.position);
                player.GetHedgePowerUpManager().GetShieldPowerUp().GrantShield(ShieldType.RegularShield, powerupGameObject);

                break;
            case PowerUp.BubbleShield:
                powerupGameObject = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BubbleShield, player.transform.position);
                player.GetHedgePowerUpManager().GetShieldPowerUp().GrantShield(ShieldType.BubbleShield, powerupGameObject);

                break;
            case PowerUp.FlameShield:
                powerupGameObject = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.FlameShield, player.transform.position);
                player.GetHedgePowerUpManager().GetShieldPowerUp().GrantShield(ShieldType.FlameShield, powerupGameObject);

                break;
            case PowerUp.ElectricShield:
                powerupGameObject = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.ElectricShield, player.transform.position);
                player.GetHedgePowerUpManager().GetShieldPowerUp().GrantShield(ShieldType.ElectricShield, powerupGameObject);

                break;
            case PowerUp.Invincibility:
                powerupGameObject = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.InvincibilityTrail, player.transform.position);
                player.GetHedgePowerUpManager().GetInvincibilityPowerUp().GrantInvincibility(powerupGameObject);
                GMAudioManager.Instance().PlayAndQueueBGM(BGMToPlay.InvincibilityTheme);//Queue the theme
                break;
            case PowerUp.PowerSneakers:
                player.GetHedgePowerUpManager().GetPowerSneakersPowerUp().GrantPowerSneakers();
                GMAudioManager.Instance().PlayAndQueueBGM(BGMToPlay.PowerSneakers);//Queue the theme
                break;
            case PowerUp.Eggman:
                player.GetHealthManager().VerifyHit(player.transform.position.x);

                break;
            case PowerUp.Super:
                GMRegularStageScoreManager.Instance().IncrementRingCount(50);
                player.GetActionManager().PerformAction<SuperTransform>();

                break;
            case PowerUp.Random:
                PowerUp randomPowerUp = (PowerUp)UnityEngine.Random.Range(0, Enum.GetNames(typeof(PowerUp)).Length);
                this.GrantPowerUp(player, randomPowerUp);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Update the super form availbility
    /// </summary>
    private void UpdateSuperFormAvailable()
    {
        if (GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds() == 7 && GMRegularStageScoreManager.Instance().GetRingCount() >= 50)
        {
            if (GMStageManager.Instance().GetPlayer().GetActionManager().CheckActionIsBeingPerformed<Jump>() && GMStageManager.Instance().GetPlayer().GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.None)
            {
                this.SetSuperFormAvailable(true);
                return;
            }
        }

        this.SetSuperFormAvailable(false);
    }

    /// <summary>
    /// Sets the super form availability
    /// <param name="superFormAvailable">The new value for the super form Availibility </param>
    /// </summary>
    public void SetSuperFormAvailable(bool superFormAvailable) => this.superFormAvailable = superFormAvailable;

    /// <summary>
    /// Gets the availibilty of the superform
    /// </summary>
    public bool GetSuperFormAvailable() => this.superFormAvailable;
}
