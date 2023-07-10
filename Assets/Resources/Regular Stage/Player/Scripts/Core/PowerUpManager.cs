using UnityEngine;
/// <summary>
/// Manager class which controls access to power up abilities and general items related to all power ups
/// </summary>
public class PowerUpManager : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField]
    private ShieldPowerUpController shieldPowerUp;
    [SerializeField]
    private InvincibilityPowerUpController invincibilityPowerUp;
    [SerializeField]
    private PowerSneakersPowerUpController powerSneakersPowerUp;
    [SerializeField, LastFoldoutItem()]
    private SuperFormPowerUpController superFormPowerUp;

    [SerializeField, Tooltip("The Current power up the playerhas")]
    private SuperPowerUp currentSuperPowerUp = SuperPowerUp.None;
    [Tooltip("The parent object for all the effects ")]
    public Transform effectPivotPoint;

    private void Reset()
    {
        this.player = this.GetComponent<Player>();
        this.shieldPowerUp = this.GetComponentInChildren<ShieldPowerUpController>();
        this.invincibilityPowerUp = this.GetComponentInChildren<InvincibilityPowerUpController>();
        this.powerSneakersPowerUp = this.GetComponentInChildren<PowerSneakersPowerUpController>();
        this.superFormPowerUp = this.GetComponentInChildren<SuperFormPowerUpController>();
    }

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        if (this.player.GetPlayerState() == PlayerState.Awake && this.GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            this.GoSuperForm();
        }
    }

    private void FixedUpdate()
    {
        if (this.player.GetPlayerState() == PlayerState.Awake && this.GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            if (GMRegularStageScoreManager.Instance().GetRingCount() == 0)
            {
                this.RevertSuperForm();
                this.superFormPowerUp.RemovePowerUp();
            }
            else
            {
                if (this.player.GetPaletteController().GetPaletteIndex() < 1)
                {
                    this.player.GetPaletteController().SetPaletteIndex(1);
                }

                this.superFormPowerUp.SpawnSmallSuperSparkle();
            }
        }
    }

    /// <summary>
    /// Get a reference to the shield power up
    /// </summary>
    public ShieldPowerUpController GetShieldPowerUp() => this.shieldPowerUp;
    /// <summary>
    /// Get a reference to the invincibility Power up
    /// </summary>
    public InvincibilityPowerUpController GetInvincibilityPowerUp() => this.invincibilityPowerUp;
    /// <summary>
    /// Get a reference to the power sneakers power up
    /// </summary>
    public PowerSneakersPowerUpController GetPowerSneakersPowerUp() => this.powerSneakersPowerUp;

    /// <summary>
    /// Sets the super form of the character
    /// <param name="currentSuperPowerUp">The superform the player is to be set to </param>
    /// </summary>
    public void SetSuperPowerUp(SuperPowerUp currentSuperPowerUp) => this.currentSuperPowerUp = currentSuperPowerUp;
    /// <summary>
    /// Get the current super form of the character
    /// </summary>
    public SuperPowerUp GetSuperPowerUp() => this.currentSuperPowerUp;

    /// <summary>
    /// Actions that takes place when the character turns super
    /// </summary>
    public void GoSuperForm()
    {
        this.player.GetPaletteController().SetPaletteIndex(1);

        if (this.GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            return;
        }

        this.SetSuperPowerUp(SuperPowerUp.SuperForm);

        //With super sonic and sonic we character swap
        if (GMCharacterManager.Instance().currentCharacter == PlayableCharacter.Sonic)
        {
            GMCharacterManager.Instance().SwapCharacter(PlayableCharacter.SuperSonic);
        }

        this.invincibilityPowerUp.SetPowerUpVisbility(false);
        this.shieldPowerUp.SetPowerUpVisbility(false);

        GMAudioManager.Instance().PlayAndQueueBGM(BGMToPlay.SuperFormTheme);
        this.superFormPowerUp.BeginLargeSparkleCoroutine();
    }

    /// <summary>
    /// Actions that take place when the character reverts from the super form
    /// </summary>
    public void RevertSuperForm()
    {
        if (this.GetSuperPowerUp() == SuperPowerUp.None)
        {
            return;
        }

        this.player.GetPaletteController().SetPaletteIndex(0);
        this.SetSuperPowerUp(SuperPowerUp.None);

        //From super sonic back to sonic we character swap
        if (GMCharacterManager.Instance().currentCharacter == PlayableCharacter.SuperSonic)
        {
            GMCharacterManager.Instance().SwapCharacter(PlayableCharacter.Sonic);
        }

        this.player.GetHealthManager().SetHealthStatus(HealthStatus.Vulnerable);

        //Revert shield and invincibility visibility
        if (this.invincibilityPowerUp.PowerUpIsActive())
        {
            this.shieldPowerUp.SetPowerUpVisbility(false);
            this.invincibilityPowerUp.SetPowerUpVisbility(true);
            this.player.GetHealthManager().SetHealthStatus(HealthStatus.Invincible);
        }
        else
        {
            this.shieldPowerUp.Start();
            this.shieldPowerUp.SetPowerUpVisbility(this.shieldPowerUp.PowerUpIsActive());
        }

        GMAudioManager.Instance().StopBGM(BGMToPlay.SuperFormTheme);
    }
}
