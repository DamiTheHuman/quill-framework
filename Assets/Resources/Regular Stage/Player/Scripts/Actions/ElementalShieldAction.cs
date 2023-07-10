using UnityEngine;
/// <summary>
/// rename to action
/// An actin that controls the players interactions with the elemental shields that are currently active
/// </summary>
public class ElementalShieldAction : HedgeSubAction
{
    [SerializeField, Tooltip("The currentn active shield ability")]
    private HedgeShieldAbility currentShieldAction;
    public override void Start()
    {
        base.Start();
        this.Reset();
    }

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 0.1f;
        this.parentAction = this.GetComponentInParent<Jump>();
    }

    /// <summary>
    /// Get the current shield ability
    /// </summary>
    public HedgeShieldAbility GetShieldAbility() => this.currentShieldAction;
    /// <summary>
    /// Initialize the current shield action
    /// <param name="shieldAbility"> The shield ability to be set active</param>
    /// </summary>
    public void SetShieldAbility(HedgeShieldAbility shieldAbility)
    {
        this.currentShieldAction = shieldAbility;

        if (shieldAbility != null)
        {
            shieldAbility.OnInitializeShield();//Play some audio if available
            shieldAbility.SetPlayer(this.player);
        }
    }

    /// <summary>
    /// Deinitializes the shield ability based on certain circumstance like being in water
    /// </summary
    public void DeinitializeShieldAbility()
    {
        if (this.currentShieldAction != null)
        {
            this.currentShieldAction.OnDeinitializeShield();
            this.currentShieldAction = null;
            if (this.player.GetActionManager().currentSubAction is ElementalShieldAction)
            { this.player.GetActionManager().currentSubAction = null; }
        }
    }

    /// <summary>
    /// Can only consider the action if the shield is active but always perform its floating effect
    /// </summary>
    public override bool CanPerformAction()
    {

        if (this.currentShieldAction != null && this.parentAction.usedSubAction == false && this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.RegularShield && this.currentShieldAction.CanUseShieldAbility())
        {
            this.currentShieldAction.OnFloatingEffect();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs the jump button again before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed() && this.player.GetGrounded() == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Prepares the dropdash animation and timer
    /// </summary>
    public override void OnActionStart() => this.currentShieldAction.OnActivateAbility();

    /// <summary>
    /// Exits the shield ability when the player touches the ground again
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded() && this.currentShieldAction != null)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// The actions performed when the player is back on the ground
    /// </summary>
    public override void OnActionEnd()
    {
        this.currentShieldAction.OnAbilityEnd();

        if (this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() == ShieldType.BubbleShield)
        {
            this.parentAction.usedSubAction = false;//For instances such as the bubble shield on end it can be reused immedietly
        }
    }
}

