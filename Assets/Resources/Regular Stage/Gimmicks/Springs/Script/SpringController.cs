using UnityEngine;
/// <summary>
/// The controller class that handles the players interactions ith springs
/// </summary>
public class SpringController : SolidContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    protected BoxCollider2D boxCollider2D;
    [SerializeField, LastFoldoutItem]
    protected Animator animator;
    [Tooltip("The bouncy side of the spring"), FirstFoldOutItem("Spring Settings")]
    public TriggerEffectSide bouncySide = TriggerEffectSide.Top;
    [Tooltip("An additional bouncy side found in diagonal springs"), EnumConditionalEnable("springType", (int)SpringType.Diagonal)]
    public TriggerEffectSide secondaryBouncySide = TriggerEffectSide.Right;

    public SpringAnimation springAnimation = SpringAnimation.Twirl;
    [Tooltip("The velocity applied to the player on contact with the spring")]
    public float springVelocity = 16;
    [Tooltip("Affects the default angle of the spring object")]
    public SpringType springType = SpringType.Regular;
    [Tooltip("How long to lock the players controls in m/s")]
    [Help("Please input the timer in steps - (timeInSeconds * 59.999999)")]
    public float springControlLockTime = 16f;
    [SerializeField]
    protected bool upsideDownSpring;
    [Tooltip("The audio played when the spring is touched"), LastFoldoutItem()]
    public AudioClip touchedSound;
    [Tooltip("The Debug Color of the spring"), FirstFoldOutItem("Debug Settings")]
    public Color debugColor = Color.red;
    [Tooltip("The Debug Color of the active sides of the spring")]
    public Color debugActiveColor = Color.green;
    [Tooltip("The line width of the debug side")]
    public float activeColorWidth = 4;
    [LastFoldoutItem()]
    public bool autoUpdate = true;
    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.animator = this.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Checks if the players collider is within the activitable range of the bouncy side 
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        HomingAttack homingAttack = player.GetActionManager().GetAction<HomingAttack>() as HomingAttack;
        bool isHomingAttack = homingAttack != null && homingAttack.GetHomingAttackMode() == HomingAttackMode.Homing;

        if (isHomingAttack)
        {
            return true;
        }

        triggerAction = this.CheckBouncySide(player, solidBoxColliderBounds, this.bouncySide) || (this.springType == SpringType.Diagonal && this.CheckBouncySide(player, solidBoxColliderBounds, this.secondaryBouncySide));

        return triggerAction;
    }

    /// <summary>
    /// Checks the players range within the bouncy side
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// <param name="sideToCheck">The bouncy side to check against  </param>
    /// </summary>
    public bool CheckBouncySide(Player player, Bounds solidBoxColliderBounds, TriggerEffectSide sideToCheck)
    {
        if (player.GetSolidBoxController().InteractingWithContactEventOfType<SpringController>(this.gameObject))
        {
            return false;
        }

        switch (sideToCheck)
        {
            case TriggerEffectSide.Top:
                return this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheTop(solidBoxColliderBounds) && player.velocity.y <= 0;
            case TriggerEffectSide.Right:
                return this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheRight(solidBoxColliderBounds) && player.velocity.x <= 0;
            case TriggerEffectSide.Bottom:
                return this.TargetBoundsAreWithHorizontalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheBottom(solidBoxColliderBounds) && player.velocity.y >= 0;
            case TriggerEffectSide.Left:
                return this.TargetBoundsAreWithinVerticalBounds(this.boxCollider2D.bounds, solidBoxColliderBounds) && this.TargetIsToTheLeft(solidBoxColliderBounds) && player.velocity.x >= 0;
            case TriggerEffectSide.Always:
                return true;
            default:
                break;
        }

        return false;
    }

    /// <summary>
    /// Apply the bounce velocity to the player based on the rotation of the spring object
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        float offsetAngle = this.springType == SpringType.Regular ? 0 : 45;//Offset the angle by 45 for diagonal springs
        Vector2 objectVelocity = General.TransformEulerToVector2(this.springVelocity, this.transform, offsetAngle);

        if (player.GetActionManager().CheckActionIsBeingPerformed<HomingAttack>())
        {
            player.GetActionManager().EndCurrentAction();
        }

        if (objectVelocity.y != 0)
        {
            player.GetSpriteController().SetSpriteAngle(0);
            player.GetActionManager().ClearActions();
            player.GetAnimatorManager().SwitchSubstate(0);
            player.GetAnimatorManager().SwitchGimmickSubstate((GimmickSubstate)(int)this.springAnimation);
            player.velocity.y = objectVelocity.y;
            player.SetGrounded(false);
        }

        if (objectVelocity.x != 0)
        {
            if (objectVelocity.y == 0)
            {
                //Set a HCL like lock on the players input
                InputManager inputManager = player.GetComponent<InputManager>();
                inputManager.SetLockControls(this.springControlLockTime, InputRestriction.XAxis);

                if (player.GetGrounded() && this.upsideDownSpring)//Makes room for upside down springs
                {
                    objectVelocity.x *= -1;
                }
            }
            player.SetBothHorizontalVelocities(objectVelocity.x); //Sets the main velocity based on the roation of the spring and statuf of the player
            player.UpdatePlayerDirection(true);//Force a direction update
        }
        else
        {
            player.GetInputManager().SetInputRestriction(InputRestriction.None);
        }

        if (this.animator != null)
        {
            this.animator.SetTrigger("Bounce");
        }

        GMAudioManager.Instance().PlayOneShot(this.touchedSound);
    }

    public virtual void OnDrawGizmos()
    {
        if (this.boxCollider2D == null || GMStageManager.Instance() == null)
        {
            return;
        }

        float offsetAngle = this.springType == SpringType.Regular ? 0 : 45;//Offset the angle by 45 for diagonal springs
        float angleInDegrees = Mathf.RoundToInt(this.transform.eulerAngles.z);
        Gizmos.color = this.debugColor;
        Gizmos.DrawLine(this.boxCollider2D.bounds.center, (Vector2)this.boxCollider2D.bounds.center + General.TransformEulerToVector2(this.springVelocity * GMStageManager.Instance().GetPhysicsMultiplier() / 16, this.transform, offsetAngle));
        GizmosExtra.Draw2DArrow(this.boxCollider2D.bounds.center, angleInDegrees - offsetAngle, this.springVelocity * GMStageManager.Instance().GetPhysicsMultiplier() / 16);
        GizmosExtra.DrawBoxCollider2D(this.transform, this.boxCollider2D, Color.white);

        if (this.autoUpdate && Application.isPlaying == false)
        {
            TriggerEffectSide newBouncySide = TriggerEffectSide.Always;

            //Auto calculates the bouncy side
            if (this.springType == SpringType.Regular)
            {
                newBouncySide = (TriggerEffectSide)(angleInDegrees / 90);
            }
            else if (this.springType == SpringType.Diagonal)
            {
                if (angleInDegrees >= 180)
                {
                    newBouncySide = TriggerEffectSide.Bottom;
                }
                else
                {
                    newBouncySide = TriggerEffectSide.Top;
                }

                if (angleInDegrees is <= 180 and >= 90)
                {
                    this.secondaryBouncySide = TriggerEffectSide.Left;
                }
                else
                {
                    this.secondaryBouncySide = TriggerEffectSide.Right;
                }
            }
            if (this.bouncySide != newBouncySide)
            {
                this.bouncySide = newBouncySide;
                General.SetDirty(this);
            }
        }

        Gizmos.color = this.debugActiveColor;

        switch (this.bouncySide)
        {
            case TriggerEffectSide.Top:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.max, new Vector2(this.boxCollider2D.bounds.min.x, this.boxCollider2D.bounds.max.y), Gizmos.color);

                break;
            case TriggerEffectSide.Right:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.max, new Vector2(this.boxCollider2D.bounds.max.x, this.boxCollider2D.bounds.min.y), Gizmos.color);

                break;
            case TriggerEffectSide.Bottom:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.min, new Vector2(this.boxCollider2D.bounds.max.x, this.boxCollider2D.bounds.min.y), Gizmos.color);

                break;
            case TriggerEffectSide.Left:
                GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.min, new Vector2(this.boxCollider2D.bounds.min.x, this.boxCollider2D.bounds.max.y), Gizmos.color);

                break;
            case TriggerEffectSide.Always:
                GizmosExtra.DrawWireRect(this.boxCollider2D.bounds, Gizmos.color);

                break;
            default:
                break;
        }

        if (this.springType == SpringType.Diagonal)
        {
            switch (this.secondaryBouncySide)
            {
                case TriggerEffectSide.Top:
                    GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.max, new Vector2(this.boxCollider2D.bounds.min.x, this.boxCollider2D.bounds.max.y), Gizmos.color);
                    break;
                case TriggerEffectSide.Right:
                    GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.max, new Vector2(this.boxCollider2D.bounds.max.x, this.boxCollider2D.bounds.min.y), Gizmos.color);
                    break;
                case TriggerEffectSide.Bottom:
                    GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.min, new Vector2(this.boxCollider2D.bounds.max.x, this.boxCollider2D.bounds.min.y), Gizmos.color);
                    break;
                case TriggerEffectSide.Left:
                    GizmosExtra.DrawAAPolyLine(this.activeColorWidth, this.boxCollider2D.bounds.min, new Vector2(this.boxCollider2D.bounds.min.x, this.boxCollider2D.bounds.max.y), Gizmos.color);
                    break;
                case TriggerEffectSide.Always:
                    GizmosExtra.DrawWireRect(this.boxCollider2D.bounds, Gizmos.color);
                    break;
                default:
                    break;
            }
        }
    }
}
