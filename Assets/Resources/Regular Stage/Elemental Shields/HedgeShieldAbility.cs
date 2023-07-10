using UnityEngine;
/// <summary>
/// Parent class where all shield abilities inherit from
/// </summary>
public class HedgeShieldAbility : MonoBehaviour, IPooledObject
{
    [SerializeField]
    protected Player player;
    [SerializeField]
    private GameObject powerUpBase;
    [SerializeField, Tooltip("Whether the shield can be used underwater or not")]
    private bool canBeActiveUnderWater = false;

    public virtual void Start()
    {
        if (this.player == null)
        {
            this.player = this.GetComponentInParent<Player>();
        }
    }

    public void OnObjectSpawn()
    {
        this.Start();
        this.OnInitializeShield();
    }

    /// <summary>
    /// Used to determine whether a shield ability can be used at theat moment
    /// </summary>
    public virtual bool CanUseShieldAbility() => true;

    /// <summary>
    /// Sets the player for the target
    /// <param name="player">The player object the shield is attached to</param>
    /// </summary>
    public virtual void SetPlayer(Player player) => this.player = player;

    /// <summary>
    /// The moment the shiled ability is spawned and the player can access its abilities
    /// </summary>
    public virtual void OnInitializeShield() { }
    /// <summary>
    /// The moment the shiled ability is activated
    /// </summary>
    public virtual void OnActivateAbility() { }
    /// <summary>
    /// The floating effects of the shield while its still active
    /// </summary>
    public virtual void OnFloatingEffect() { }
    /// <summary>
    /// On the end poiint of the ability
    /// </summary>
    public virtual void OnAbilityEnd() { }
    /// <summary>
    /// On the deinitializing of the shield
    /// </summary>
    public virtual void OnDeinitializeShield() { }

    /// <summary>
    /// Get the current power up base
    /// </summary>
    public GameObject GetPowerUpBase() => this.powerUpBase;

    /// <summary>
    /// Gets the current shield is usuable underwater value
    /// </summary>
    public bool GetCanBeUsedUnderwater() => this.canBeActiveUnderWater;
}