using System.Collections;
using UnityEngine;
/// <summary>
/// Solid Objects with this script attached will be forced to slide down the object
/// Author - Techokami
/// Changes - Converted to Unity made the slide feel more natural by not ending the state till the player touches the ground
/// </summary>
public class SlideController : SolidContactGimmick
{
    [SerializeField]
    private PolygonCollider2D polygonCollider;

    [SerializeField, Tooltip("The direction of the slide")]
    private int slideDirection = 1;
    [SerializeField, Tooltip("The slide velocity"), Min(1)]
    private int slideVelocity = 6;

    public override void Reset()
    {
        base.Reset();
        this.polygonCollider = this.GetComponent<PolygonCollider2D>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.polygonCollider == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Confirms if the player is on the ground and is colliding with the slide before activating
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if (player.GetGrounded() && solidBoxColliderBounds.center.y > player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().point.y)
        {
            if (this.slideDirection == 1 && solidBoxColliderBounds.center.x > this.polygonCollider.bounds.min.x)
            {
                triggerAction = true;
            }
            else if (solidBoxColliderBounds.center.x < this.polygonCollider.bounds.max.x)
            {
                triggerAction = true;
            }
        }

        if (triggerAction)
        {
            if (player.GetActionManager().CheckActionIsBeingPerformed<ElementalShieldAction>() && player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() == ShieldType.BubbleShield)
            {
                triggerAction = false;
            }
        }

        return triggerAction;
    }

    /// <summary>
    /// Moves the player in the direction of the slide and adjusts the players sprite appropriately
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        player.GetActionManager().EndCurrentAction();
        player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.Slide);
        player.GetSpriteController().SetSpriteAngle(0);
        player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);//Restrict movement;
        player.groundVelocity = this.slideVelocity * this.slideDirection;
        player.UpdatePlayerDirection(true);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.Sliding);
    }

    /// <summary>
    /// As long as the player is on the slide the groundvelocity velocity is set to the slide velocity
    /// <param name="player">The player object to apply the velocity to </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        player.groundVelocity = this.slideVelocity * this.slideDirection;
    }
    /// <summary>
    /// Resets the player and allows movement once the player is done colliding with the slide
    /// <param name="player">The player object to apply the velocity to </param>
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);

        if (!(player.GetSolidBoxController().HasActiveContactEvents() && player.GetSolidBoxController().IsTouchingEventOfType<SlideController>()))
        {
            player.SetGrounded(false);
        }

        this.StartCoroutine(this.EndSlideWhenGrounded(player));//Continues the lock till the player touches the ground
    }

    /// <summary>
    /// A custom fixed update for when the player is dead 
    /// This allows movement while everything else is frozen in place
    /// </summary>
    private IEnumerator EndSlideWhenGrounded(Player player)
    {

        while (true)
        {
            if (player.GetGrounded())
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        //When chaining from one slide to another no need to unset variables
        if (player.GetSolidBoxController().HasActiveContactEvents() && player.GetSolidBoxController().IsTouchingEventOfType<SlideController>())
        {
            yield return null;
        }
        else
        {
            player.SetGrounded(false);
            player.GetInputManager().SetInputRestriction(InputRestriction.None);
            player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);

            if (player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.Sliding)
            {
                player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.None);
            }
        }

        yield return null;
    }

    private void OnValidate() => this.slideDirection = this.transform.localScale.x >= 0 ? 1 : -1;

}
