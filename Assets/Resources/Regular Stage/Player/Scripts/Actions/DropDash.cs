using System.Collections;
using UnityEngine;
/// <summary>
/// The drop dash action from mania which can only be used from a jump action
/// </summary>
public class DropDash : HedgeSubAction
{
    [Tooltip("Drop dash charge time"), SerializeField]
    private float dropDashChargeTime = 20f;
    [Tooltip("A flag to identify the end of the dropdash"), SerializeField]
    private bool dropDashFullyCharged = false;
    [Tooltip("The velocity applied at the end of the drop dash variable"), SerializeField]
    private float baseDropDashVelocity = 8;
    [Tooltip("The max velocity applied at the end of the drop dash"), SerializeField]
    private float maxDropDashVelocity = 12;
    [Tooltip("The direction the player will be launched at the end of the dash"), SerializeField]
    private int dropDashDirection = 1;
    [Tooltip("The direction the player was facing at the beginning of the dropdash"), SerializeField]
    private int startDropDashDirection = 0;
    [Tooltip("The offset position of the drop dash dust effect"), SerializeField]
    private Vector2 dropDashDustOffset = new Vector2(-13, 0);

    private IEnumerator dropDashCoroutine;
    public override void Start() => base.Start();
    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 1.1f;
        this.parentAction = this.GetComponentInParent<Jump>();
    }
    /// <summary>
    /// Can only perform the dropdash while jumping and only if the Jump sub action has not been used up
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>() && this.parentAction.usedSubAction == false
            && (this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() == ShieldType.None ||
            this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() == ShieldType.RegularShield))
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
    /// Prepares the dropdash animation and timer
    /// </summary>
    public override void OnActionStart()
    {
        this.dropDashFullyCharged = false;
        this.dropDashDirection = this.player.currentPlayerDirection;
        this.startDropDashDirection = this.dropDashDirection;
        this.dropDashCoroutine = this.DropDashCountdown();
        this.StartCoroutine(this.dropDashCoroutine);

    }
    /// <summary>
    /// After adding velocity to the jump move check for variable jump
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.player.GetInputManager().GetJumpButton().GetButtonDown())
        {
            int currentDirection = this.player.GetInputManager().GetCurrentInput().x == 0 ? this.dropDashDirection : (int)Mathf.Sign(this.player.GetInputManager().GetCurrentInput().x);
            this.dropDashDirection = currentDirection;
        }
    }
    /// <summary>
    /// Exits the dropdash when the player hits the ground
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.player.GetGrounded() || this.player.GetInputManager().GetJumpButton().GetButtonUp())
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// The commands performed at the end of the drop dash action
    /// </summary>
    public override void OnActionEnd()
    {
        this.StopCoroutine(this.dropDashCoroutine);
        //If the dropdash was fully charged at the end apply the velocity
        if (this.dropDashFullyCharged && this.player.GetGrounded())
        {
            if (this.startDropDashDirection == this.dropDashDirection)
            {
                this.player.groundVelocity = (this.player.groundVelocity / 4) + (this.baseDropDashVelocity * this.dropDashDirection);
            }
            else
            {
                if (General.CheckIfAngleIsZero(this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees()))
                {
                    this.player.groundVelocity = this.baseDropDashVelocity * this.dropDashDirection;
                }
                else
                {
                    this.player.groundVelocity = (this.player.groundVelocity / 2) + (this.baseDropDashVelocity * this.dropDashDirection);
                }
            }
            this.player.GetAnimatorManager().SwitchSubstate(SubState.Moving);
            this.player.groundVelocity = Mathf.Clamp(this.player.groundVelocity, -this.maxDropDashVelocity, this.maxDropDashVelocity);//Limit
            this.player.velocity = this.player.CalculateSlopeMovement(this.player.groundVelocity);
            HedgehogCamera.Instance().GetCameraLookHandler().BeginDashLag();
            Vector2 dustEffectPosition = (Vector2)this.player.transform.position + (this.dropDashDustOffset * this.dropDashDirection);
            GameObject dustEffect = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.DropDashDust, dustEffectPosition);
            dustEffect.transform.localScale = new Vector3(this.dropDashDirection, 1, 1);
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().spindashRelease);
            this.player.GetActionManager().PerformAction<Roll>();//Switch to the roll action at the end of the drop dash
        }
        else if (this.player.GetGrounded())
        {
            this.player.GetAnimatorManager().SwitchActionSubstate(0);
        }

        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
    }


    /// <summary>
    /// Waits until the dropdash is fully charged before notifying that the dropdash is fully charged
    /// </summary>
    private IEnumerator DropDashCountdown()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.dropDashChargeTime));
        this.dropDashFullyCharged = true;
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(this.subAnimationID);//Switch to the drop dash animation on sucess
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().dropDash);
    }
}
