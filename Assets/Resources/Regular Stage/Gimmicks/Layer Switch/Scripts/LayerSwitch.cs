using UnityEngine;
/// <summary>
/// Class that switches the layer the player is currently on based on the players direction
/// </summary>
public class LayerSwitch : TriggerContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private LayerSwitchCondition switchCondition = LayerSwitchCondition.SwitchWhenGroundedAndMoving;

    [SerializeField, LayerList]
    [Help("Regular - The layer added when the player is going right but removed when going Left \n ForceSwitch - The layer added when a force switch takes place")]
    private int layerSwap1 = 9;
    [SerializeField, LayerList]
    [Help("The layer added when the player is going left but removed when going right \n ForceSwitch - The layer removed when a force switch takes place")]
    private int layerSwap2 = 10;

    public Color debugColor = new Color(0f / 255f, 204f / 154f, 323f / 255f);

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Checks if the players solid box comes in contact with the solid box
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = true;

        return triggerAction;
    }

    /// <summary>
    /// Switch the players layer to the set layer to add and remove the layer the player is currently on
    /// <param name="player">The player object to manipulate </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (this.switchCondition == LayerSwitchCondition.Always)
        {
            this.SwitchLayer(player, true);
        }
        else
        {
            if (player.groundVelocity is < 0 or > 0)
            {
                this.SwitchLayer(player);
            }
        }
    }

    /// <summary>
    /// Changes the player layer if he is grounded
    /// <param name="player">The active player object </param>
    /// <param name="forceSwitch">The flag which determines whether to always switch </param>
    /// </summary> 
    private void SwitchLayer(Player player, bool forceSwitch = false)
    {
        Sensors sensors = player.GetSensors();

        if (forceSwitch)
        {
            sensors.AddLayerToAllCollisionMasks(this.layerSwap1);
            sensors.RemoveLayerFromAllCollisionMasks(this.layerSwap2);
            return;
        }
        if (this.switchCondition == LayerSwitchCondition.SwitchWhenGroundedAndMoving)
        {
            if (player.groundVelocity > 0)
            {
                sensors.AddLayerToAllCollisionMasks(this.layerSwap1);
                sensors.RemoveLayerFromAllCollisionMasks(this.layerSwap2);
            }
            else if (player.groundVelocity < 0)
            {
                sensors.AddLayerToAllCollisionMasks(this.layerSwap2);
                sensors.RemoveLayerFromAllCollisionMasks(this.layerSwap1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            GizmosExtra.DrawWireRect(this.boxCollider2D.bounds, this.debugColor);
        }
    }

}
