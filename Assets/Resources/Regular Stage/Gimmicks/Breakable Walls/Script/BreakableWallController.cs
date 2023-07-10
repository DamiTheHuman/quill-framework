using UnityEngine;
/// <summary>
/// A Wall that can be broken by the players actions
/// </summary>
public class BreakableWallController : SolidContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [Tooltip("The direction the wall can be broken from"), SerializeField]
    private BreakableDirection breakableDirection = BreakableDirection.Horizontal;
    [Help("The object set here is spawned when the wall is successfully broken")]
    [Tooltip("The object spawned when the wall is broken"), SerializeField]
    private ObjectToSpawn breakableWallToSpawn = ObjectToSpawn.BreakableShardSet;
    [Tooltip("The prefab to instantiate when the wall is broken"), SerializeField]
    private GameObject customShardSetPrefab;
    [Tooltip("Allows the object to be broken while super or if the player can ram through walls e.g knuckles"), SerializeField]
    private bool canBeRammedThrough = true;
    [Tooltip("Whether the player must be attacking to break this wall"), SerializeField]
    private bool mustBeAttackedThrough = true;
    [Tooltip("Only allows breakage if the collision happened while the player is in the air"), SerializeField]
    [EnumConditionalEnable("wallType", (int)BreakableDirection.Bottom, (int)BreakableDirection.Vertical, (int)BreakableDirection.Top)]
    private bool playerMustBeInTheAir = false;
    [Tooltip("The direction the wall was broken into"), SerializeField]
    private int breakDirection = 1;
    [SerializeField]
    private Color debugColour = Color.red;

    [SerializeField, IsDisabled()]
    private GameObject instatiatedShardSet;

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

        if (this.customShardSetPrefab != null)
        {
            this.instatiatedShardSet = Instantiate(this.customShardSetPrefab);
            this.instatiatedShardSet.transform.position = this.transform.position;
            this.instatiatedShardSet.transform.parent = GMSpawnManager.Instance().GetOtherObjectsPile().transform;
            this.instatiatedShardSet.SetActive(false);
        }
    }

    /// <summary>
    /// Checks to destroy the wall if attacked into
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        //Check if the player is build different and can just break through the walls by touching them
        bool playerCanBreakWalls = player.GetPlayerPhysicsInfo().canRamThroughWalls || player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm;
        bool mustbeAttackedThrough = this.mustBeAttackedThrough && player.GetAttackingState();

        switch (this.breakableDirection)
        {
            case BreakableDirection.Horizontal:

                //If the player is not attacking and the wall can't be rammed through return
                if (this.canBeRammedThrough && !playerCanBreakWalls)
                {
                    if (this.mustBeAttackedThrough && player.GetAttackingState() == false)
                    {
                        break;
                    }
                }

                if (Mathf.Abs(player.velocity.x) > 0 && this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds))
                {
                    this.breakDirection = this.TargetIsToTheRight(solidBoxColliderBounds) && player.velocity.x < 0 && player.velocity.y <= 0 ? 1 : -1;

                    if (player.GetPlayerPhysicsInfo().canRamThroughWalls)
                    {
                        triggerAction = true;
                    }


                    if (!player.GetGrounded())
                    {
                        //Check if the player is performing the flame shield
                        if (player.GetActionManager().CheckActionIsBeingPerformed<ElementalShieldAction>() && player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() == ShieldType.FlameShield)
                        {
                            triggerAction = true;
                        }
                    }
                    else
                    {
                        triggerAction = true;

                    }
                }

                break;
            case BreakableDirection.Vertical:
                if (this.mustBeAttackedThrough && player.GetAttackingState() == false)
                {
                    break;
                }

                if (this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds)
                    && (this.TargetIsToTheTop(solidBoxColliderBounds) || this.TargetIsToTheBottom(solidBoxColliderBounds))
                    && Mathf.Abs(player.velocity.y) > 0)
                {
                    if (this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y < 0)
                    {
                        if (player.GetAttackingState())
                        {
                            triggerAction = true;
                            player.AttackRebound();
                        }
                    }
                    else if (this.TargetIsToTheBottom(solidBoxColliderBounds) && player.velocity.y > 0)
                    {

                        triggerAction = true;
                    }
                }

                break;
            case BreakableDirection.Bottom:
                if (this.mustBeAttackedThrough && player.GetAttackingState() == false)
                {
                    break;
                }

                if (player.velocity.y > 0 && this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds))
                {
                    if (player.GetGrounded() == false)
                    {
                        triggerAction = true;
                    }
                }

                break;
            case BreakableDirection.Top:
                if(this.mustBeAttackedThrough && player.GetAttackingState() == false)
                {
                    break;
                }

                if (this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y < 0)
                {
                    if (player.GetGrounded() == false)
                    {
                        triggerAction = true;
                    }
                }

                break;
            default:
                break;
        }


        return triggerAction;
    }

    /// <summary>
    /// Deactivates the wall on contact and creates spawned pieces
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (this.breakableDirection == BreakableDirection.Top)
        {
            player.AttackRebound();
        }

        this.BreakWall();
    }

    /// <summary>
    /// Spawns the set of shard pieces
    /// </summary>
    private void BreakWall()
    {
        GameObject shardSet = this.instatiatedShardSet != null ? this.instatiatedShardSet : GMSpawnManager.Instance().SpawnGameObject(this.breakableWallToSpawn, this.transform.position);
        shardSet.SetActive(true);

        foreach (BreakableWallShardController projectileInfo in shardSet.GetComponentsInChildren<BreakableWallShardController>())
        {

            Vector2 startVelocity = projectileInfo.projectileData.GetStartVelocity();
            startVelocity.x *= this.breakDirection;
            projectileInfo.projectileData.SetStartVelocity(startVelocity);
            projectileInfo.Start();
        }

        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }

        GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColour);
    }
}
