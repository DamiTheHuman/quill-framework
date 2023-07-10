using UnityEngine;
/// <summary>
/// A simple gimmick which sets the player ground velocity to that of the gimmick where appropriate
/// Original Author - Naoshi
/// </summary>
public class SpeedPadController : TriggerContactGimmick
{
    public float speedPadVelocity = 16f;
    [Help("Please input the timer in steps - (timeInSeconds * 59.9999995313f)")]
    public float speedPadHorizontalControlLockTime = 16f;
    [Tooltip("The audio played when the speed pad is touched")]
    public AudioClip speedPadTouchedSound;
    /// <summary>
    /// As long as the player is grounded and moving less than the speed pad velocity set the player speed to that of the speed pad
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        //If grounded and not previously interacted with to stop the speed pad from activating multiple times in one interaction
        if (player.GetGrounded() && player.GetSolidBoxController().ContactEventIsActive(this) == false)
        {
            if (this.transform.localScale.x == -1)
            {
                triggerAction = player.groundVelocity > this.speedPadVelocity * this.transform.localScale.x;
            }
            else
            {
                triggerAction = player.groundVelocity < this.speedPadVelocity;
            }
        }

        return triggerAction;
    }

    /// <summary>
    /// Sets the player ground velocity to that of the speed pad
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        player.groundVelocity = this.speedPadVelocity * this.transform.localScale.x;
        player.GetInputManager().SetLockControls(this.speedPadHorizontalControlLockTime, InputRestriction.XAxis);
        GMAudioManager.Instance().PlayOneShot(this.speedPadTouchedSound);
        //When a player lands on a speed pad while gliding in the drop or stand up state end the action otherwise the interaction will be a bit weird
        if (player.GetActionManager().CheckActionIsBeingPerformed<Glide>())
        {
            Glide glide = player.GetActionManager().GetAction<Glide>() as Glide;

            if (glide.GetGlideState() is GlideState.Dropping or GlideState.StandUp)
            {
                player.GetActionManager().EndAction<Glide>();
            }

        }
    }

}
