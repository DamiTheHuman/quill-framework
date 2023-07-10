using UnityEngine;
/// <summary>
/// The teleporter from Sky Sanctuary zone
/// Author  - AZU and Damizien -in Sonic Worlds
/// Changes - Converted to unity with some minor tweaks for customization
/// </summary>
public class TeleporterController : SolidContactGimmick
{
    private Player activePlayer;
    [SerializeField]
    private BeamOfLightController beamOfLight;
    [SerializeField, Tooltip("The position the beam of light is spawned at")]
    private Transform beamOfLightSpawnPosition = null;

    [SerializeField, Tooltip("The speed the sprite fades in and out with")]
    private float spriteFadeSpeed = 2;
    [SerializeField, Tooltip("The transform which is set as the teleport point")]
    private Transform teleportToPoint = null;
    [SerializeField, Tooltip("The speed of the player when elevating and moving towards the center point")]
    private Vector2 elevateSpeed = new Vector2(1, 0.8f);
    [SerializeField, Tooltip("The speed in which the player will move towards the target")]
    private float moveToTeleportPointspeed = 10;

    [SerializeField, Tooltip("The current state of the teleporter")]
    private TeleporterState teleporterState = TeleporterState.Idle;
    [SerializeField, Tooltip("The current timer of the action being performed by the teleporter")]
    private int teleportTimer;

    public AudioClip chargeUpTeleporterSound;
    public AudioClip fireTeleporterSound;

    private void FixedUpdate()
    {
        if (this.activePlayer != null)
        {
            switch (this.teleporterState)
            {
                case TeleporterState.Elevate:
                    this.ElevatePlayer(this.activePlayer.transform.position);
                    if (this.activePlayer.transform.position.x != this.transform.position.x)
                    {
                        this.MoveTowardsCenter(this.activePlayer.transform.position);
                    }
                    this.FadeOutPlayer();

                    if (this.teleportTimer % 10 == 0)
                    {
                        this.activePlayer.GetInputManager().VibrateController(0.3f, 0.4f, 1f);
                    }

                    if (this.teleportTimer == 128)
                    {
                        this.teleporterState = TeleporterState.SearchForStop;
                        this.teleportTimer = 0;
                        GMAudioManager.Instance().PlayOneShot(this.fireTeleporterSound);
                    }

                    break;
                case TeleporterState.SearchForStop:
                    this.MoveToTeleportPoint(this.activePlayer.transform.position);
                    this.WatchEndPoint(this.activePlayer.transform.position);

                    break;
                case TeleporterState.Ending:
                    this.ElevatePlayer(this.activePlayer.transform.position);
                    this.FadeInPlayer();

                    if (this.teleportTimer == 112)
                    {
                        this.EndTeleportAction();
                    }

                    break;
                case TeleporterState.Idle:
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Moves the player upwards in the same speed
    /// <param name="position">The current player position</param>
    /// </summary>
    public void ElevatePlayer(Vector2 position)
    {
        this.teleportTimer++;
        position.y += this.elevateSpeed.y * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.activePlayer.transform.position = position;
    }

    /// <summary>
    /// Moves the player towards the center point of the teleporter
    /// <param name="position">The current player position</param>
    /// </summary>
    public void MoveTowardsCenter(Vector2 position)
    {
        position.x = Mathf.MoveTowards(position.x, this.transform.position.x, this.elevateSpeed.x * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
        this.activePlayer.transform.position = position;
    }

    /// <summary>
    /// Fades the player colors and shields out
    /// </summary>
    private void FadeOutPlayer()
    {
        this.activePlayer.GetSpriteController().GetSpriteRenderer().color = General.RGBToColour(255, 255, 255, Mathf.RoundToInt(255 - (this.teleportTimer * this.spriteFadeSpeed)));
        Color currentSpriteColor = this.activePlayer.GetSpriteController().GetSpriteRenderer().color;

        if (this.activePlayer.GetHedgePowerUpManager().GetShieldPowerUp() != null)
        {
            this.activePlayer.GetHedgePowerUpManager().GetShieldPowerUp().SetPowerUpSpritesTransparency(currentSpriteColor.a);
        }
        if (this.activePlayer.GetHedgePowerUpManager().GetInvincibilityPowerUp() != null)
        {
            this.activePlayer.GetHedgePowerUpManager().GetInvincibilityPowerUp().SetPowerUpSpritesTransparency(currentSpriteColor.a);
        }
    }

    /// <summary>
    /// Moves the player towards the target set position
    /// <param name="position">The current player position</param>
    /// </summary>
    private void MoveToTeleportPoint(Vector2 position)
    {
        this.teleportTimer++;
        position = Vector2.MoveTowards(position, this.teleportToPoint.position, this.moveToTeleportPointspeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
        this.activePlayer.transform.position = position;
    }

    /// <summary>
    /// Waits until the player reaches the target end point before ending the action
    /// </summary>
    private void WatchEndPoint(Vector2 position)
    {
        if (position.y == this.teleportToPoint.position.y)
        {
            this.teleportTimer = 0;
            this.teleporterState = TeleporterState.Ending;
        }
    }

    /// <summary>
    /// Fades the player colors and shields back in
    /// </summary>
    private void FadeInPlayer()
    {
        this.activePlayer.GetSpriteController().GetSpriteRenderer().color = General.RGBToColour(255, 255, 255, Mathf.RoundToInt(this.teleportTimer * this.spriteFadeSpeed));
        Color currentSpriteColor = this.activePlayer.GetSpriteController().GetSpriteRenderer().color;

        if (this.activePlayer.GetHedgePowerUpManager().GetShieldPowerUp() != null)
        {
            this.activePlayer.GetHedgePowerUpManager().GetShieldPowerUp().SetPowerUpSpritesTransparency(currentSpriteColor.a);
        }
        if (this.activePlayer.GetHedgePowerUpManager().GetInvincibilityPowerUp() != null)
        {
            this.activePlayer.GetHedgePowerUpManager().GetInvincibilityPowerUp().SetPowerUpSpritesTransparency(currentSpriteColor.a);
        }
    }

    /// <summary>
    /// The actions that take place when the teleport action has ended
    /// </summary>
    private void EndTeleportAction()
    {
        this.activePlayer.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        this.teleporterState = TeleporterState.Idle;
        this.activePlayer.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.activePlayer.SetMovementRestriction(MovementRestriction.None);
        this.activePlayer.velocity = Vector2.zero;
        this.activePlayer.GetAnimatorManager().SwitchGimmickSubstate(0);
        this.teleportTimer = 0;
        //Ensure the appropriate color is set
        this.activePlayer.GetSpriteController().GetSpriteRenderer().color = General.RGBToColour(255, 255, 255, 255);
        Color currentSpriteColor = this.activePlayer.GetSpriteController().GetSpriteRenderer().color;

        if (this.activePlayer.GetHedgePowerUpManager().GetShieldPowerUp() != null)
        {
            this.activePlayer.GetHedgePowerUpManager().GetShieldPowerUp().SetPowerUpSpritesTransparency(currentSpriteColor.a);
        }
        if (this.activePlayer.GetHedgePowerUpManager().GetInvincibilityPowerUp() != null)
        {
            this.activePlayer.GetHedgePowerUpManager().GetInvincibilityPowerUp().SetPowerUpSpritesTransparency(currentSpriteColor.a);
        }

        this.activePlayer = null;
        this.beamOfLight.SetActivated(false);
    }

    /// <summary>
    /// Checks if the player is standing on the teleporter and inputs upwards
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if (player.GetGrounded() && player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InTeleporter)
        {
            if (player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform == this.transform && player.GetInputManager().GetCurrentInput().y == 1)
            {
                triggerAction = true;
            }
        }

        return triggerAction;
    }

    /// <summary>
    /// Begin the telport action
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.teleportTimer = 0;
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.InTeleporter);
        this.teleporterState = TeleporterState.Elevate;
        player.SetGrounded(false);
        player.GetActionManager().EndCurrentAction();
        player.velocity = Vector2.zero;
        player.GetInputManager().SetInputRestriction(InputRestriction.All);//Stop recieving player input
        player.SetMovementRestriction(MovementRestriction.Both);//Restrict both movement
        //Play Audio
        this.beamOfLight = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BeamOfLight, this.beamOfLightSpawnPosition.position).GetComponent<BeamOfLightController>();
        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.Teleporter);
        this.activePlayer = player;
        GMAudioManager.Instance().PlayOneShot(this.chargeUpTeleporterSound);
    }
}
