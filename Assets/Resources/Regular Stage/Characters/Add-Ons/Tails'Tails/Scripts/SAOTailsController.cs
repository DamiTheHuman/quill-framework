using UnityEngine;
/// <summary>
/// Handles the behaviour of Tails tails based on the players movement
/// Author of <see cref="CalculateTailsSpriteAngle(Player)"/> is Orbinaut Framework's Triangly
/// </summary>
public class SAOTailsController : HedgeSpriteAddOnController
{
    [Tooltip("The current substate of the Tails'Tails"), SerializeField]
    private int currentSubstate = 0;
    [Tooltip("The hitbox for Tails'Tails when flying"), SerializeField]
    private SecondaryHitBoxController flightHitBox;
    [FirstFoldOutItem("Animation Information")]
    [Tooltip("The Speed multiplier for the walk animation")]
    public float walkSpeedMultiplier = 6f;
    [Tooltip("The Speed multiplier for the jog animation")]
    public float jogSpeedMultiplier = 9f;
    [Tooltip("The Speed multiplier for the run animation")]
    public float runSpeedMultiplier = 10f;
    [Tooltip("The Speed multiplier for the dash animation")]
    public float dashSpeedMultiplier = 12.5f;
    [Tooltip("The Speed multiplier for the spindash animation"), LastFoldoutItem]
    public float spinDashMultiplier = 6f;
    private float currentSpeedMultiplier;
    private int speedMultiplierHash;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.SwitchSubstate(0);
        this.flightHitBox.gameObject.SetActive(false);
        this.speedMultiplierHash = Animator.StringToHash("SpeedMultiplier");
    }

    public override void UpdateAddOnInfo()
    {
        this.transform.localScale = new Vector3(1, 1, 0);
        this.UpdateAnimationSpeeds();
        this.currentSubstate = this.CalculateNewSubstate();
        this.SwitchSubstate(this.currentSubstate);
        base.UpdateAddOnInfo();
        this.animator.SetFloat(this.speedMultiplierHash, General.ConvertAnimationSpeed(this.animator, this.currentSpeedMultiplier));
    }

    /// <summary>
    /// Calculates the value for the substate the Tails'Tails should be in based on the player states
    /// </summary>
    public int CalculateNewSubstate()
    {
        Player player = this.GetPlayer();
        int substate = player.GetAnimatorManager().GetSubstate();
        int actionSubstate = player.GetAnimatorManager().GetActionSubState();
        int secondaryActionSubstate = player.GetAnimatorManager().GetSecondaryActionSubstate();
        int gimmickSubstate = player.GetAnimatorManager().GetGimmickSubstate();
        int newSubstate = 0;
        this.spriteAngle = player.GetSpriteController().GetSpriteAngle();
        switch (substate)
        {
            case (int)SubState.Idle:
                newSubstate = 0;

                break;
            case (int)SubState.Moving:
            case (int)SubState.Aerial:
                newSubstate = -1;


                break;
            default:
                break;
        }
        switch (actionSubstate)
        {
            case (int)ActionSubState.LookUp:
            case (int)ActionSubState.Crouch:
                newSubstate = 0;

                break;
            case (int)ActionSubState.Skidding:
            case (int)ActionSubState.Pushing:
                newSubstate = 1;

                break;
            case (int)ActionSubState.Spindash:
                newSubstate = 2;

                break;
            case (int)ActionSubState.Jump:
            case (int)ActionSubState.Roll:
                newSubstate = 3;

                if (actionSubstate == 1)
                {
                    switch (secondaryActionSubstate)
                    {
                        case (int)SubActionSubState.Flying:
                        case (int)SubActionSubState.GlidingOrFlyingTired:
                        case (int)SubActionSubState.SuperTransform:
                            newSubstate = -1;
                            break;
                        default:
                            break;
                    }
                    this.spriteAngle = this.CalculateTailsSpriteAngle(player);
                }
                else if (actionSubstate == (int)ActionSubState.Roll)
                {
                    this.spriteAngle = this.CalculateTailsSpriteAngle(player);
                }
                break;
            case (int)ActionSubState.LedgeBalancing:
            case (int)ActionSubState.Die:
            case (int)ActionSubState.Victory:
                newSubstate = -1;

                break;
            default:
                if (player.GetAnimatorManager().AnimationIsPlaying("Skidding Loop"))
                {
                    newSubstate = 1;
                }

                break;
        }
        switch (gimmickSubstate)
        {
            case (int)GimmickSubstate.Hanging:
                newSubstate = 1;

                break;
            case (int)GimmickSubstate.SpringTwirl:
            case (int)GimmickSubstate.SpringDiagonal:
            case (int)GimmickSubstate.CorkscrewSpin:
            case (int)GimmickSubstate.Gulp:
            case (int)GimmickSubstate.Riding:
                newSubstate = -1;

                break;
            default:
                break;
        }

        return newSubstate;
    }

    /// <summary>
    /// Update the animation speed of the tails
    /// </summary>
    private void UpdateAnimationSpeeds()
    {
        Player player = this.GetPlayer();
        float absoluteGroundVelocity = Mathf.Abs(player.groundVelocity);
        this.currentSpeedMultiplier = 20 + (Mathf.Abs(player.groundVelocity) * this.walkSpeedMultiplier);

        if (absoluteGroundVelocity >= 6 && absoluteGroundVelocity < 14 && player.GetGrounded())
        {
            this.currentSpeedMultiplier = 20 + (absoluteGroundVelocity * this.jogSpeedMultiplier);
        }
        else if (absoluteGroundVelocity >= 14 && absoluteGroundVelocity < 15 && player.GetGrounded())
        {
            this.currentSpeedMultiplier = 20 + (absoluteGroundVelocity * this.runSpeedMultiplier);
        }
        else if (absoluteGroundVelocity >= 15 && player.GetGrounded())
        {
            this.currentSpeedMultiplier = 30 + (absoluteGroundVelocity * this.dashSpeedMultiplier);
        }
        else if (player.GetActionManager().CheckActionIsBeingPerformed<Spindash>())
        {
            float spinRev = ((Spindash)player.GetActionManager().currentPrimaryAction).currentSpinRev;
            float minSpinRev = ((Spindash)player.GetActionManager().currentPrimaryAction).minSpindashRev;
            this.currentSpeedMultiplier = Mathf.Abs(minSpinRev + Mathf.Floor(spinRev / 2)) * this.spinDashMultiplier;
        }
    }

    /// <summary>
    /// Calculate The angle of the tails sprite based on positioning
    /// <param name="player">the new player to calculate the angle from</param>
    /// </summary>
    private float CalculateTailsSpriteAngle(Player player)
    {
        float angle = 0;

        if (this.currentSubstate == -1)
        {
            return angle;
        }
        if (player.currentPlayerDirection == 1)
        {
            angle = General.AngleBetweenVector2(player.transform.position, (Vector2)player.transform.position + player.velocity);
        }
        else
        {
            angle = General.AngleBetweenVector2((Vector2)player.transform.position + player.velocity, player.transform.position);
        }

        return angle * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Get a reference to the tails hitbox
    /// </summary>
    public SecondaryHitBoxController GetTailsHitBox() => this.flightHitBox;
}
