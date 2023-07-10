using UnityEngine;
/// <summary>
/// Controls the interactions between shield powerups and the player object
/// </summary>
public class ShieldPowerUpController : HedgePowerUp
{
    [Tooltip("A reference to the elemental shieldAbilities"), SerializeField]
    private ElementalShieldAction elementalShieldAction;
    [SerializeField, Tooltip("The hedge shield ability that is active")]
    private HedgeShieldAbility hedgeShieldAbility;

    [Tooltip("The shield the player currently has"), SerializeField]
    private ShieldType currentShieldType = ShieldType.None;
    public override void Start()
    {
        base.Start();
        this.elementalShieldAction = this.hedgePowerUpManager.GetComponentInChildren<ElementalShieldAction>();

        if (this.elementalShieldAction != null)
        {
            this.elementalShieldAction.SetShieldAbility(null);
            if (this.activePowerUp != null)
            {
                this.UpdateCurrentShieldDetails(this.currentShieldType, this.activePowerUp);
                this.UpdateElementalShieldAbilities();
                this.RetriveAllPowerUpSpriteData();
                this.SetPowerUpVisbility(this.hedgePowerUpManager.GetSuperPowerUp() != SuperPowerUp.SuperForm && this.player.GetHedgePowerUpManager().GetInvincibilityPowerUp().PowerUpIsActive() == false);
            }
        }
    }

    /// <summary>
    /// Get the currently active shield
    /// </summary>
    public ShieldType GetShieldType() => this.currentShieldType;

    /// <summary>
    /// Sets the current shield type
    /// <param name="activeShieldType">The new active shield value</param>
    /// </summary>
    public void SetShieldType(ShieldType activeShieldType) => this.currentShieldType = activeShieldType;

    /// <summary>
    /// Sets the current shield and updates the specified details
    /// <param name="activeShield">The type of the current shield</param>
    /// <param name="shieldObject">The object relating to the shield being granted</param>
    /// </summary>
    public void GrantShield(ShieldType activeShield, GameObject shieldObject)
    {
        //When a new shield is granted, remove the old one
        if (activeShield != ShieldType.None && this.activePowerUp != null)
        {
            if (this.player.GetActionManager().CheckActionIsBeingPerformed<ElementalShieldAction>())
            {
                this.player.GetActionManager().EndAction<ElementalShieldAction>();
            }

            this.RemovePowerUp();
        }


        this.UpdateCurrentShieldDetails(activeShield, shieldObject);
        this.UpdateElementalShieldAbilities();
        this.RetriveAllPowerUpSpriteData();

        //Super forms show no shield visibility
        if (this.hedgePowerUpManager.GetSuperPowerUp() == SuperPowerUp.SuperForm || this.hedgePowerUpManager.GetInvincibilityPowerUp().PowerUpIsActive())
        {
            this.SetPowerUpVisbility(false);
        }

    }

    /// <summary>
    /// Update the details of the current shield
    /// <param name="activeShield">The type of the current shield</param>
    /// <param name="shieldObject">The object relating to the shield being granted</param>
    /// </summary>
    private void UpdateCurrentShieldDetails(ShieldType activeShield, GameObject shieldObject)
    {
        this.currentShieldType = activeShield;
        this.activePowerUp = shieldObject;
        this.activePowerUp.transform.parent = this.hedgePowerUpManager.effectPivotPoint;
        this.activePowerUp.transform.localPosition = Vector3.zero;
        this.activePowerUp.transform.eulerAngles = this.hedgePowerUpManager.effectPivotPoint.transform.eulerAngles;
        this.activePowerUp.transform.localScale = this.hedgePowerUpManager.effectPivotPoint.transform.localScale;
        this.hedgeShieldAbility = this.activePowerUp.GetComponent<HedgeShieldAbility>();
        this.hedgeShieldAbility.GetPowerUpBase().SetActive(true);

        if (this.hedgeShieldAbility != null)
        {
            this.hedgeShieldAbility.SetPlayer(this.player);
        }
    }

    /// <summary>
    /// Updates the elementalshield abilities to contain the players new abilities
    /// </summary>
    private void UpdateElementalShieldAbilities()
    {
        if (this.hedgeShieldAbility == null || this.elementalShieldAction == null)
        {
            return;
        }

        this.elementalShieldAction.SetShieldAbility(this.hedgeShieldAbility);
        this.elementalShieldAction.GetShieldAbility().Start();
        this.elementalShieldAction.GetShieldAbility().GetPowerUpBase().SetActive(this.hedgePowerUpManager.GetInvincibilityPowerUp().GetActivePowerUpGameObject() == null);
    }

    /// <summary>
    /// Removes the currently active shield
    /// </summary>
    public override void RemovePowerUp()
    {
        if (this.activePowerUp == null)
        {
            return;
        }

        base.RemovePowerUp();

        if (this.elementalShieldAction != null)
        {
            this.elementalShieldAction.DeinitializeShieldAbility();
        }

        this.hedgeShieldAbility.GetPowerUpBase().SetActive(true);
        this.hedgeShieldAbility = null;
        this.currentShieldType = ShieldType.None;
    }

    /// <summary>
    ///Checks if the active shield can be used underwater
    /// </summary>
    public bool CheckIfShieldCanBeUsedUnderwater()
    {
        if (this.hedgeShieldAbility != null)
        {
            return this.hedgeShieldAbility.GetCanBeUsedUnderwater();
        }

        return true;
    }

    /// <summary>
    /// Checks if the current player already has the shield to be granted
    /// <param name="powerUpToGrant">The power up to grant</param>
    /// </summary>
    public bool AlreadyHasShield(PowerUp powerUpToGrant)
    {
        bool result = false;

        switch (powerUpToGrant)
        {
            case PowerUp.RegularShield:
                result = this.currentShieldType == ShieldType.RegularShield;

                break;
            case PowerUp.BubbleShield:
                result = this.currentShieldType == ShieldType.BubbleShield;

                break;
            case PowerUp.FlameShield:
                result = this.currentShieldType == ShieldType.FlameShield;

                break;
            case PowerUp.ElectricShield:
                result = this.currentShieldType == ShieldType.ElectricShield;

                break;
            case PowerUp.TenRings:
                break;
            case PowerUp.Invincibility:
                break;
            case PowerUp.ExtraLife:
                break;
            case PowerUp.PowerSneakers:
                break;
            case PowerUp.Super:
                break;
            case PowerUp.Eggman:
                break;
            case PowerUp.Random:
                break;
            default:
                break;
        }

        return result;
    }

    /// <summary>
    /// Overrides the pwer up visibility based on the shield type to only turn off the sprite renderers
    /// <param name="value">The value for the power up visibility</param>
    /// </summary>
    public override void SetPowerUpVisbility(bool value)
    {
        if (this.activePowerUp == null)
        {
            return;
        }

        if (this.hedgeShieldAbility != null)
        {
            this.hedgeShieldAbility.GetPowerUpBase().SetActive(value);
        }
    }
}
