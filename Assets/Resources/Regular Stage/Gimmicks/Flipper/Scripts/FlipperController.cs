using UnityEngine;
/// <summary>
/// This gimmick handles the interaction the player has with flippers which activate when the jump button is input
/// </summary>
public class FlipperController : SolidContactGimmick
{
    [SerializeField]
    private Animator animator;
    [Tooltip("The angle the player will be forced to slide down with"), SerializeField]
    public float slideAngle = 337.5f;
    [Tooltip("The ground velocity of the player when sliding"), SerializeField]
    public float slideVelocity = 1;
    [Tooltip("The velocity multipliers to the player when the jump button is activated"), SerializeField]
    private Vector2 actionVelocity = new Vector2(0.125f, 10f);
    [Tooltip("The audio played when the flipper is activated")]
    public AudioClip flipperActivatedSound;
    [SerializeField]
    private Color debugColor = Color.red;

    public override void Reset()
    {
        base.Reset();
        this.animator = this.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.animator == null)
        {
            this.animator = this.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Checks if the player is grounded and what the players has considered to be ground is this object
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = player.GetGrounded() && player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform == this.transform && player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.OnFlipper;

        return triggerAction;
    }

    /// <summary>
    /// Update the players gimmick manager state to flip rolling
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.OnFlipper);
        player.GetGimmickManager().SetGimmickAngle(this.transform.localScale.x == 1 ? this.slideAngle : 360 - this.slideAngle);
        player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);//Restrict player Horizontal input
        player.groundVelocity = this.transform.localScale.x == 1 ? this.slideVelocity : -this.slideVelocity;
        Roll rolll = player.GetActionManager().GetComponentInChildren<Roll>();
        player.GetActionManager().PerformAction<Roll>();
        int rollDirection = this.transform.localScale.x == 1 ? 1 : -1;
        player.UpdatePlayerDirection(true);
    }

    /// <summary>
    /// Continously Increment the players speed and wait for the player to activate the jump button
    /// <param name="player">The player object in contact with the flipper  </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        if (player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.OnFlipper)
        {
            return;
        }

        player.groundVelocity = this.transform.localScale.x == 1 ? this.slideVelocity : -this.slideVelocity;

        if (player.GetInputManager().GetJumpButton().GetButtonDown())
        {
            this.FlipperAction(player);
        }
    }

    /// <summary>
    ///End contact with the player by updating the players gimmick state and allowing movement to the players movement
    /// <param name="player">The player object in contact with the flipper  </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);

        if (player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.OnFlipper)
        {
            return;
        }

        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        player.GetGimmickManager().SetGimmickAngle(0);
        player.SetGrounded(false);
        player.GetInputManager().SetInputRestriction(InputRestriction.None);//Unrestrict player input

        if (player.velocity.y <= 0) //Slid off the bumper
        {
            player.GetActionManager().EndCurrentAction();
        }
    }

    /// <summary>
    /// The actions performed when the player is to be launched in the air
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    private void FlipperAction(Player player)
    {
        player.velocity.x = (player.transform.position.x - this.transform.position.x) * this.actionVelocity.x; //The further away the more force
        player.velocity.y = this.actionVelocity.y;
        player.SetGrounded(false);
        this.animator.SetTrigger("Flip");//Play the flip animation
        GMAudioManager.Instance().PlayOneShot(this.flipperActivatedSound);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        float debugAngle = this.transform.localScale.x == 1 ? this.slideAngle : 360 - this.slideAngle;
        Gizmos.DrawLine(this.transform.position, General.CalculateAngledObjectPosition(this.transform.position, debugAngle * Mathf.Deg2Rad, new Vector2(0, this.actionVelocity.y / 8)));
        GizmosExtra.Draw2DArrow(this.transform.position, debugAngle, this.actionVelocity.y / 8);
    }

}
