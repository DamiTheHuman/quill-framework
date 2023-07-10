using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// An action that allows the player to glide through the air
/// </summary>
public class Glide : HedgeSubAction
{
    [SerializeField]
    private SecondaryHitBoxController knucklesGloveHitBox;
    private Jump jump;
    [SerializeField]
    public GlideState glideState = GlideState.None;
    [SerializeField, DirectionList]
    private int glideDirection = 1;
    [SerializeField, DirectionList, FirstFoldOutItem("Turning"), Tooltip("The direction to turn to")]
    private int turnDirection = 1;
    [SerializeField, Tooltip("The current turn delta")]
    private float currentTurnDelta = 0;
    [SerializeField, Tooltip("The target turn delta")]
    private float targetTurnDelta = 0;
    [SerializeField, Tooltip("The players horizontal velocity before they started turning")]
    private float xVelocityBeforeTurn = 0;
    [Tooltip("How long in frames it takes to turn around")]
    private float turnTimerInFrames = 15;
    [LastFoldoutItem(), Tooltip("How long it takes before turning around")]
    private float currentTurningSwitchTimerInFrames;
    [SerializeField, Tooltip("The Offset applied to the solid box when sliding")]
    private Vector2 slidingSolidBoxOffset = new Vector2(0, -15f);

    public override void Start()
    {
        base.Start();
        this.jump = this.GetComponentInParent<Jump>();
    }
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 1.4f;
        this.subAnimationID = SubActionSubState.GlidingOrFlyingTired;
        this.jump = this.GetComponentInParent<Jump>();
        this.parentAction = this.jump;
        this.glideState = GlideState.None;
        this.glideDirection = 1;
        this.turnDirection = 1;
        this.xVelocityBeforeTurn = 0;
        this.currentTurnDelta = 0;
        this.targetTurnDelta = 0;
    }
    /// <summary>
    /// Can only perform the glide action while jumping and only if the Jump sub action has not been used up
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
    /// Sets the player to the gliding state and adjusts the velocity
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetInputManager().GetJumpButton().SetButtonPressed(false);//Exhaust the jump button pressed
        this.player.GetSpriteController().SetSpriteAngle(0);
        this.player.GetSpriteController().transform.eulerAngles = Vector3.zero;
        this.player.GetSensors().SetSizeMode(SizeMode.Gliding);
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(this.subAnimationID);
        this.glideDirection = this.player.currentPlayerDirection;
        this.turnDirection = this.glideDirection;
        this.SetGlideState(GlideState.Gliding);
        this.player.velocity.x = this.player.GetPlayerPhysicsInfo().glideStartVelocity * this.player.currentPlayerDirection;

        if (this.player.velocity.y > 0)
        {
            this.player.velocity.y = 0;
        }

        this.FetchGloveHitBox();
    }
    /// <summary>
    /// Perform different actions based on the players current glide state
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.glideState is GlideState.Gliding or GlideState.Turning)
        {
            if (this.player.GetInputManager().GetJumpButton().GetButtonUp() || this.player.GetBeginVictoryActionOnGroundContac())//Jump Button Released switch to drop
            {
                this.SwitchToDropState();
            }

            if (this.player.PlayerIsAboveTopBounds() && this.player.velocity.y > 0)
            {
                this.player.velocity.y = 0;
            }

            if (this.player.GetGrounded()) //On Ground contact
            {
                //On shallow ground switch to the slide state otherwise end the action
                float currentAngleInDegrees = this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees();
                bool angleIsShallow = currentAngleInDegrees is <= 45 or >= 315;
                this.SetGlideState(angleIsShallow ? GlideState.Sliding : GlideState.None);
                this.player.SetPlayerDirection(this.turnDirection);
                this.player.GetAnimatorManager().GetAnimator().Update(Time.deltaTime);
            }
        }

        this.knucklesGloveHitBox.enabled = this.glideState == GlideState.Gliding;
        switch (this.glideState)
        {
            case GlideState.Gliding:
                this.HandleGlidingState();
                break;
            case GlideState.Turning:
                this.HandleTurningState();
                break;
            case GlideState.Dropping:
                this.HandleDropState();
                break;
            case GlideState.Sliding:
                this.HandleSlideState();
                break;
            case GlideState.StandUp:
                break;
            case GlideState.None:
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Exits the glide state when the glide state is set to none
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.glideState == GlideState.None)
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// The commands performed at the end of the glide action
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
        this.player.GetAnimatorManager().SwitchGimmickSubstate(0);
        this.player.GetAnimatorManager().SetOtherAnimationSubstate(0);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.player.GetSolidBoxController().SetExtraSolidBoxOffset(Vector2.zero);
        this.RemoveGloveHitBox();
        this.SetGlideState(GlideState.None);
    }

    /// <summary>
    /// Fetch the extra hitbox that allows knuckles to deflect bullet with his gloves and attach it out current <see cref="HitBoxController"/>
    /// </summary>
    private void FetchGloveHitBox()
    {
        this.knucklesGloveHitBox = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.KnucklesGloveHitbox, this.transform.position).GetComponent<SecondaryHitBoxController>();
        this.knucklesGloveHitBox.transform.parent = this.player.GetSpriteController().transform;
        this.knucklesGloveHitBox.transform.localPosition = Vector3.zero;
        this.knucklesGloveHitBox.SetPlayer(this.player);
        this.knucklesGloveHitBox.transform.localScale = new Vector3(1, 1, 1);
        this.knucklesGloveHitBox.transform.eulerAngles = Vector3.zero;
        this.knucklesGloveHitBox.enabled = true;
        this.player.GetHitBoxController().SetSecondaryHitBox(this.knucklesGloveHitBox);
    }

    /// <summary>
    /// Remove the active glove hitbox controller
    /// </summary>
    private void RemoveGloveHitBox()
    {
        this.player.GetHitBoxController().SetSecondaryHitBox(null);
        this.knucklesGloveHitBox.gameObject.transform.parent = null;
        this.knucklesGloveHitBox.gameObject.SetActive(false);
        this.knucklesGloveHitBox = null;
    }
    /// <summary>
    /// Apply a vertical force that moves the player towards the ground while gliding
    /// <param name="defaultApplyGravity">The default apply gravity action used when falling towards the ground so we dont have to replicate logic </param>
    /// </summary>
    public void ApplyGravity(Action defaultApplyGravity)
    {
        if (this.glideState is GlideState.Gliding or GlideState.Turning)
        {
            this.player.velocity.y = Mathf.MoveTowards(this.player.velocity.y, -this.player.GetPlayerPhysicsInfo().glideTopVelocity.y, GMStageManager.Instance().ConvertToDeltaValue(this.player.currentGravity));
            return;
        }
        defaultApplyGravity();
    }

    /// <summary>
    /// Update the players horizontal velocity while not turning
    /// </summary>
    public void ApplyAirHorizontalMovement()
    {
        if (this.glideState != GlideState.Gliding)
        {
            return;
        }

        this.player.velocity.x += this.player.GetPlayerPhysicsInfo().glideAcceleration * this.player.currentPlayerDirection;
    }
    /// <summary>
    /// Limit the players glide velocity so they dont exceed the value set
    /// </summary>
    public void LimitGlideVelocity() => this.player.velocity.x = Mathf.Clamp(this.player.velocity.x, -this.player.GetPlayerPhysicsInfo().glideTopVelocity.x, this.player.GetPlayerPhysicsInfo().glideTopVelocity.x);
    /// <summary>
    /// Set the current glide state of the action and update the animation where available
    /// <param name="glideState">The new glide state for the glide action </param>
    /// </summary>
    public void SetGlideState(GlideState glideState)
    {
        if (this.glideState == glideState)
        {
            return;
        }

        this.glideState = glideState;

        if (glideState != GlideState.None)
        {
            int index = (int)glideState;
            this.player.GetAnimatorManager().SetOtherAnimationSubstate(index);
            if (glideState == GlideState.StandUp)
            {
                this.OnSwitchToStandUpState();
            }
            else if (glideState == GlideState.Sliding)
            {
                this.OnSwitchToSlidingState();
            }
        }
        else
        {
            this.player.GetAnimatorManager().SetOtherAnimationSubstate(0);
        }
    }
    /// <summary>
    /// Get the current glide state
    /// </summary>
    public GlideState GetGlideState() => this.glideState;

    /// <summary>
    /// Handles the Glide action while the player is in the glide state
    /// </summary>
    private void HandleGlidingState()
    {
        if (this.player.GetInputManager().GetCurrentInput().x != 0 && this.player.GetInputManager().GetCurrentInput().x != this.glideDirection)
        {
            this.xVelocityBeforeTurn = this.player.velocity.x;
            this.TurnAround();
            return;
        }

        this.player.SetPlayerDirection(this.glideDirection);
    }

    /// <summary>
    /// Handles the player turning state
    /// </summary>
    private void HandleTurningState()
    {
        if (this.player.GetInputManager().GetCurrentInput().x != 0 && this.player.GetInputManager().GetCurrentInput().x != this.turnDirection)
        {
            this.currentTurningSwitchTimerInFrames = this.turnTimerInFrames;//Set a timer so the player can change their mind mid turn without affecting momentum
            this.TurnAround();
            return;
        }

        this.UpdateTurnDelta();
        this.CountDownTurnSwitchTimer();
        this.player.velocity.x = this.currentTurningSwitchTimerInFrames == 0 ? Mathf.Abs(this.xVelocityBeforeTurn) * Mathf.Cos(this.currentTurnDelta * Mathf.Deg2Rad) : this.player.velocity.x;

        if (this.currentTurnDelta == this.targetTurnDelta) // Once we reach the target delta the turn is complete
        {
            this.glideDirection = this.turnDirection;
            this.SetGlideState(GlideState.Gliding);
            this.currentTurningSwitchTimerInFrames = 0;
            return;
        }

        this.UpdateTurningAnimation(this.currentTurnDelta);
        this.player.SetPlayerDirection(-this.turnDirection);
    }

    /// <summary>
    /// Actions performed while the player is dropping
    /// </summary>
    private void HandleDropState()
    {
        this.player.GetSensors().SetSizeMode(SizeMode.Regular);
        this.player.SetAttackingState(false);

        if (this.player.GetGrounded())
        {
            this.player.groundVelocity = 0;
            this.player.velocity.x = 0;
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().land);
            this.SetGlideState(GlideState.StandUp);
        }
    }

    /// <summary>
    /// Actions performed while the player is sliding
    /// </summary>
    private void HandleSlideState()
    {
        this.player.GetInputManager().SetInputRestriction(InputRestriction.XAxis);

        if (this.player.GetGrounded() == false) //If the player slides off ledge switch back to the drop state
        {
            this.SetGlideState(GlideState.Dropping);
            this.player.GetSolidBoxController().SetExtraSolidBoxOffset(Vector2.zero);
        }
        else if (this.player.groundVelocity == 0)
        {
            this.player.GetSolidBoxController().SetExtraSolidBoxOffset(Vector2.zero);
            this.SetGlideState(GlideState.StandUp);
        }
    }

    /// <summary>
    /// Increment the curren turn delta as the player turns towards the target
    /// </summary>
    private void UpdateTurnDelta() => this.currentTurnDelta = Mathf.MoveTowards(this.currentTurnDelta, this.targetTurnDelta, General.RoundToDecimalPlaces(this.player.GetPlayerPhysicsInfo().glideTurnSpeed));

    /// <summary>
    /// Countdown the turn switch timer
    /// </summary>
    private void CountDownTurnSwitchTimer() => this.currentTurningSwitchTimerInFrames = Mathf.MoveTowards(this.currentTurningSwitchTimerInFrames, 0, General.RoundToDecimalPlaces(1));

    /// <summary>
    /// Make the player turn around in the direction of the current input
    /// </summary>
    private void TurnAround()
    {
        this.currentTurnDelta = (int)this.player.GetInputManager().GetCurrentInput().x >= 0 ? 180 : 0;
        this.targetTurnDelta = (int)this.player.GetInputManager().GetCurrentInput().x >= 0 ? 0 : 180;
        this.turnDirection *= -1;// Flip the turn direction
        this.SetGlideState(GlideState.Turning);
    }

    /// <summary>
    /// Update the index of the glide turn animation so its always in sync with the turn progress
    /// <param name="turnProgress">How far the player has gone through the turn</param>
    /// </summary
    private void UpdateTurningAnimation(float turnProgress)
    {
        turnProgress -= 180;

        //So the animation plays correcly when going from left to right
        if (this.targetTurnDelta == 180)
        {
            turnProgress += 180;
        }

        float animationFrameIndex = (int)Mathf.Abs(turnProgress * 2 / 90) % 4;
        animationFrameIndex /= 4 - 1;
        this.player.GetAnimatorManager().PlayAnimation("Glide Turn", animationFrameIndex);
    }

    /// <summary>
    /// Start the drop state for the player from  gliding
    /// </summary>
    private void SwitchToDropState()
    {
        this.SetGlideState(GlideState.Dropping);
        this.player.velocity.x *= this.player.GetPlayerPhysicsInfo().glideDropVelocityMultiplier;
        this.player.SetPlayerDirection(this.turnDirection);
    }

    /// <summary>
    /// Actions performed when the slide state is switched to 
    /// </summary>
    private void OnSwitchToSlidingState() => this.player.GetSolidBoxController().SetExtraSolidBoxOffset(this.slidingSolidBoxOffset);

    /// <summary>
    /// Actions performed when the stand up state is switched to
    /// </summary>
    private void OnSwitchToStandUpState()
    {
        this.player.GetSolidBoxController().SetExtraSolidBoxOffset(Vector2.zero);
        this.StartCoroutine(this.WaitTillStandingCoroutine());
    }

    /// <summary>
    /// Locks the players input till they stand upright
    /// </summary>
    private IEnumerator WaitTillStandingCoroutine()
    {
        yield return new WaitForEndOfFrame();
        this.player.GetSensors().SetSizeMode(SizeMode.Regular);
        this.player.SetAttackingState(false);

        while (this.glideState == GlideState.StandUp && this.player.GetAnimatorManager().GetCurrentAnimationNormalizedTime() < 1)
        {
            yield return new WaitForFixedUpdate();
        }

        this.glideState = GlideState.None;

        yield return null;
    }
}
