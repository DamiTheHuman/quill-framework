using System.Collections;
using UnityEngine;
/// <summary>
/// Handles the fly action of the player
/// </summary>
public class Fly : HedgeSubAction
{
    private Jump jump;
    private SAOTailsController tailsController;
    [SerializeField]
    private int tiredSubAnimationID = 4;
    [SerializeField]
    private FlightState flightState = FlightState.None;
    [Tooltip("Whether the upwards flight gravity should be applied"), SerializeField]
    private bool flyUpwards = false;
    [SerializeField, Tooltip("Checks if the player can fly up")]
    private bool canFlyUp = false;
    private IEnumerator delayFirstFlightCoroutine;
    private IEnumerator flightCoroutine;

    public override void Start()
    {
        base.Start();
        this.jump = this.GetComponentInParent<Jump>();
        this.tailsController = this.player.GetComponentInChildren<SAOTailsController>();
    }

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 1.3f;
        this.subAnimationID = SubActionSubState.Flying;
        this.jump = this.GetComponentInParent<Jump>();
        this.parentAction = this.jump;

        if (this.tailsController != null)
        {
            this.tailsController.GetTailsHitBox().gameObject.SetActive(false);
        }
        if (this.flightCoroutine != null)
        {
            this.StopCoroutine(this.flightCoroutine);
        }
    }

    /// <summary>
    /// Can only perform the fly action while jumping and only if the Jump sub action has not been used up
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>() && this.parentAction.usedSubAction == false)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs the jump button again before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Prepares the flight state and the begins the flight countdown
    /// </summary>
    public override void OnActionStart()
    {
        if (this.flightCoroutine != null)
        {
            this.StopCoroutine(this.flightCoroutine);
        }

        this.flightState = FlightState.Flying;

        if (this.tailsController != null)
        {
            this.player.GetHitBoxController().SetSecondaryHitBox(this.tailsController.GetTailsHitBox());
            this.tailsController.GetTailsHitBox().gameObject.SetActive(true);
        }
        this.canFlyUp = false;
        this.player.GetSensors().SetSizeMode(SizeMode.Regular);
        this.player.SetAttackingState(false);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(this.subAnimationID);
        this.flightCoroutine = this.FlightCountdown();
        this.StartCoroutine(this.flightCoroutine);
        this.delayFirstFlightCoroutine = this.DelayFirstFlight();
        this.StartCoroutine(this.delayFirstFlightCoroutine);

        if (this.jump.GetRollJump())
        {
            this.jump.SetRollJump(false);
            this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        }
    }

    /// <summary>
    /// While the player presses the Jump button add to the velocity
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.player.velocity.y > 1 && this.flyUpwards)
        {
            this.flyUpwards = false;
        }

        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed() && this.flightState == FlightState.Flying && this.player.velocity.y < 1 && this.canFlyUp)
        {
            this.flyUpwards = true;
        }

        if ((this.player.GetBeginVictoryActionOnGroundContac() || this.player.PlayerIsAboveTopBounds()) && this.player.velocity.y > 0)
        {
            this.player.velocity.y = 0;
        }
    }

    /// <summary>
    /// Exits the flight state when the player hits the ground
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// The commands performed at the end of the fly action
    /// </summary>
    public override void OnActionEnd()
    {
        if (this.flightCoroutine != null)
        {
            this.StopCoroutine(this.flightCoroutine);
        }

        this.canFlyUp = false;
        this.flightState = FlightState.None;
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
        this.DisableTailsHitBox();
    }

    /// <summary>
    /// Calculates the gravity for the players flight abilities
    /// </summary>
    public float CalculateFlightGravity()
    {
        if (this.flyUpwards && this.player.GetSensors().ceilingCollisionInfo.GetCurrentCollisionInfo().sensorHitData == SensorHitDirectionEnum.None && this.flightState == FlightState.Flying)
        {
            return this.player.GetPlayerPhysicsInfo().flightUpwardsGravity;
        }
        else
        {
            this.flyUpwards = false;

            return this.player.GetPlayerPhysicsInfo().flightBaseGravity;
        }
    }

    /// <summary>
    /// Disable the tails hitbox
    /// </summary>
    private void DisableTailsHitBox()
    {
        if (this.tailsController == null)
        {
            return;
        }

        this.tailsController.GetTailsHitBox().gameObject.SetActive(false);
        this.player.GetHitBoxController().SetSecondaryHitBox(null);
    }

    /// <summary>
    /// Countdown the flight timer till to find out when the move has ended
    /// </summary>
    private IEnumerator FlightCountdown()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.player.GetPlayerPhysicsInfo().maxFlightDuration));
        this.flightState = FlightState.Tired;
        this.player.GetAnimatorManager().GetAnimator().SetInteger(this.player.GetAnimatorManager().GetSecondaryActionSubstateHash(), this.tiredSubAnimationID);
        this.DisableTailsHitBox();

        yield return null;
    }

    /// <summary>
    /// Waits a couple of frames before the player can fly upwards
    /// </summary>
    private IEnumerator DelayFirstFlight()
    {
        this.canFlyUp = false;
        yield return new WaitForSeconds(General.StepsToSeconds(this.player.GetPlayerPhysicsInfo().firstFlightDelayDuration));
        this.canFlyUp = true;

        yield return null;
    }
}
