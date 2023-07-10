using UnityEngine;
/// <summary>
///A controller class for the water related objects within the stage
/// </summary>
public class WaterController : TriggerContactGimmick
{
    private BoxCollider2D boxCollider2D;
    [Tooltip("The Layer of the water")]
    [SerializeField, LayerList]
    private int waterLayer = 28;
    [Tooltip("The water waves to tile the surface of the water with"), SerializeField]
    private GameObject waterSurfaceBlock;
    [Tooltip("List of extra water surface blocks"), SerializeField, Min(0)]
    private int extraWaterSurfaceBlocks = 0;
    [Tooltip("The slowest we can be to enter and stick to to the surface of the water"), SerializeField]
    private float minWaterRunSpeed = 3f;

    protected override void Start()
    {
        base.Start();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.waterSurfaceBlock.transform.parent.parent = this.transform;//This makes the surface move just incase this object is controlled by the WaterLevelManager
        this.TileWaterSurface();
    }

    /// <summary>
    /// Tile the surface of the water of the water sprites
    /// </summary>
    public void TileWaterSurface()
    {
        SpriteRenderer waterSurfaceSprite = this.waterSurfaceBlock.GetComponent<SpriteRenderer>();
        this.waterSurfaceBlock.transform.position = new Vector3(this.boxCollider2D.bounds.min.x + (waterSurfaceSprite.size.x / 2), this.boxCollider2D.bounds.max.y);

        int numberOfSuraceBlocksToSpawn = Mathf.CeilToInt(this.boxCollider2D.bounds.size.x / waterSurfaceSprite.bounds.size.x);
        Vector2 currentBlockPlacePosition = waterSurfaceSprite.transform.position;//Set the current position to the start block

        for (int x = 1; x < numberOfSuraceBlocksToSpawn + this.extraWaterSurfaceBlocks; x++)
        {
            //Move by the sprite bounds
            currentBlockPlacePosition.x += waterSurfaceSprite.size.x;
            GameObject waterSurfaceClone = Instantiate(this.waterSurfaceBlock.gameObject, currentBlockPlacePosition, Quaternion.Euler(0, 0, 0));
            waterSurfaceClone.transform.localScale = new Vector2(1, 1);
            waterSurfaceClone.transform.parent = this.waterSurfaceBlock.transform.parent;
            waterSurfaceClone.name = this.waterSurfaceBlock.name + " (" + (x + 1) + ")";
        }
    }

    /// <summary>
    /// On Collision with this object start handling water collision
    /// <param name="player">The current active player object</param>
    /// <param name="solidBoxColliderBounds">The bounds of the players solid box</param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = true;

        return triggerAction;
    }

    /// <summary>
    /// While the player is colliding with the surface check for water running and water entry
    /// <param name="player">The current active player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.HandlerWaterRunning(player);

        if (player.GetSolidBoxController().PreviousContactEventOfType<WaterController>() || player.GetSolidBoxController().IsTouchingEventOfType<WaterController>())
        {
            return;
        }

        if (player.transform.position.y < this.boxCollider2D.bounds.max.y)
        {
            this.HandleWaterEntry(player);
        }
    }

    /// <summary>
    /// While the player is colliding with the surface constantly check for water running
    /// Because water running is available the player might not be in the water yet so check for water entry
    /// <param name="player">The current active player object</param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        this.HandlerWaterRunning(player);

        if (player.transform.position.y < this.boxCollider2D.bounds.max.y && player.GetPhysicsState() != PhysicsState.Underwater)
        {
            this.HandleWaterEntry(player);
        }
    }

    /// <summary>
    ///Removes the player sufrace where available and sets the water exit flag
    /// <param name="player">The current active player object</param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        this.RemoveSurfaceFromPlayerGroundLayer(player);

        if (player.GetSolidBoxController().NextContactEventOfType<WaterController>() || player.GetSolidBoxController().IsTouchingEventOfType<WaterController>())
        {
            return;
        }

        this.HandleWaterExit(player);
    }

    /// <summary>
    /// Handles the player interaction while running on the surface of the water
    /// <param name="player">The current active player object</param>
    /// </summary>
    private void HandlerWaterRunning(Player player)
    {
        if (player.GetPhysicsState() != PhysicsState.Underwater)
        {
            if (player.GetGrounded() && player.GetSensors().groundCollisionInfo.IsLayerIsInCollisionMask(this.waterLayer) == false)
            {
                if (player.GetGroundMode() == GroundMode.Floor && Mathf.Abs(player.groundVelocity) > this.minWaterRunSpeed && player.transform.position.y >= this.boxCollider2D.bounds.max.y)
                {
                    this.AddSurfaceToPlayerGroundLayer(player);

                    return;
                }
            }
            else if (player.GetGrounded() == false || Mathf.Abs(player.groundVelocity) < this.minWaterRunSpeed)
            {
                this.RemoveSurfaceFromPlayerGroundLayer(player);
            }
        }
    }

    /// <summary>
    /// Adds the water surface to the players ground layer
    /// <param name="player">The current active player object</param>
    /// </summary>
    private void AddSurfaceToPlayerGroundLayer(Player player)
    {
        player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.WaterRun, true);
        player.GetSensors().groundCollisionInfo.AddToCollisionMask(this.waterLayer);
    }

    /// <summary>
    /// Removes the water surface from the players ground layer
    /// <param name="player">The current active player object</param>
    /// </summary>
    private void RemoveSurfaceFromPlayerGroundLayer(Player player)
    {
        player.GetSpriteEffectsController().ToggleEffect(SpriteEffectToggle.WaterRun, false);
        player.GetSensors().groundCollisionInfo.RemoveFromCollisionMask(this.waterLayer);
    }

    /// <summary>
    /// Manage the player interaction as they enter the water
    /// <param name="player">The current active player object</param>
    /// </summary>
    private void HandleWaterEntry(Player player)
    {
        if (player.GetPhysicsState() == PhysicsState.Basic)
        {
            player.PrepareWaterEntryMultiplier();
            player.SetPhysicsState(PhysicsState.Underwater);

            if (Time.timeSinceLevelLoad > Time.fixedDeltaTime && this.ShouldSpawnWaterSplash(player))
            {
                Vector2 splashPosition = new Vector2(player.transform.position.x, this.boxCollider2D.bounds.max.y);
                GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.WaterSplash, splashPosition);
            }
        }
    }

    /// <summary>
    /// Manage the player interaction as they exit the water
    /// <param name="player">The current active player object</param>
    /// </summary>
    private void HandleWaterExit(Player player)
    {
        if (player.GetPhysicsState() == PhysicsState.Underwater)
        {
            player.PrepareWaterExitMultiplier();


            if (Time.timeSinceLevelLoad > Time.fixedDeltaTime && this.ShouldSpawnWaterSplash(player))
            {
                Vector2 splashPosition = new Vector2(player.transform.position.x, this.boxCollider2D.bounds.max.y);
                GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.WaterSplash, splashPosition);
            }

            player.SetPhysicsState(PhysicsState.Basic);
        }
    }


    /// <summary>
    /// Settings that determine whether a water splash should be spawned
    /// </summary>
    private bool ShouldSpawnWaterSplash(Player player) => player.GetSolidBoxController().GetBoxCollider2D().bounds.center.y > this.boxCollider2D.bounds.max.y;
}
