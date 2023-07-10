using UnityEngine;
/// <summary>
/// Shield ability for the bubble shield which bounces the player based on the surface they collide with
/// </summary>
public class BubbleShield : HedgeShieldAbility
{
    [Tooltip("Checks whether to restrict the bubble shield"), SerializeField]
    private bool bubbleShieldCooldown = false;
    [Tooltip("The inital velocity added on shield activation forcing the player downwards"), SerializeField]
    private float bubbleBounceDropSpeed = 8f;
    [Tooltip("The velocity added to the player on recovery after hitting the ground with the shield"), SerializeField]
    private float bubbleBounceReboundSpeed = 7.5f;
    [Tooltip("A flag to check wether bubble recovery from hitting the ground has been initiated"), SerializeField]
    private bool bubbleRecoveryInitiated = false;

    [Tooltip("The Animator object for the base of the bubble shield"), SerializeField]
    private Animator bubbleShieldBase = new Animator();
    [Tooltip("The Animator object for the aura of the bubble shield"), SerializeField]
    private Animator bubbleShieldAura = new Animator();
    [SerializeField, Tooltip("The sound played when the electric shield is activated")]
    private AudioClip bubbleShieldBounceSound = null;

    public override void Start() => base.Start();
    public override void OnDeinitializeShield()
    {
        base.OnDeinitializeShield();
        this.bubbleShieldCooldown = false;
    }

    public override void OnInitializeShield()
    {
        base.OnInitializeShield();
        this.bubbleShieldCooldown = false;
    }

    /// <summary>
    /// Stops the player from spamming the bubble shield bounce ability 
    /// </summary>
    public override bool CanUseShieldAbility()
    {
        if (this.bubbleShieldCooldown)
        {
            if (this.player.velocity.y < 0)
            {
                this.bubbleShieldCooldown = false;
            }
            return false;
        }

        return true;
    }

    /// <summary>
    /// Zeroes out the players horizontal velocity and plummets them towards the ground
    /// </summary>
    public override void OnActivateAbility()
    {
        this.player.velocity.x = 0;//Zero out the players velocity
        this.player.velocity.y = -this.bubbleBounceDropSpeed;//Drop to the ground

        if (this.gameObject.activeSelf)
        {
            this.bubbleShieldAura.SetBool("Activated", true);
            this.bubbleShieldBase.SetBool("Activated", true);
        }

        this.bubbleShieldCooldown = true;
    }

    /// <summary>
    /// The commands performed at the end of the bubble bounce which is when the player reaquites with the ground
    /// </summary>
    public override void OnAbilityEnd()
    {
        if (this.player.GetGrounded())
        {
            this.InitiateBubbleBounceRecovery();
        }

        if (this.gameObject.activeSelf)
        {
            this.bubbleShieldBase.SetTrigger("Recovery");
            this.bubbleShieldAura.gameObject.SetActive(false);
            this.bubbleShieldAura.SetBool("Activated", false);
            this.bubbleShieldBase.SetBool("Activated", false);
        }
    }

    private void Update() => this.ResetBubbleAnimation();

    /// <summary>
    /// Resets the bubble animation to default after the player comes in contact with the ground 
    /// This is only performed after a sucessful bounce
    /// </summary>
    private void ResetBubbleAnimation()
    {
        if (this.bubbleRecoveryInitiated && this.player.GetGrounded())
        {
            this.bubbleRecoveryInitiated = false;
            this.bubbleShieldAura.gameObject.SetActive(true);
            this.bubbleShieldAura.Play("Idle");
            this.bubbleShieldBase.Play("Idle");
        }
    }

    /// <summary>
    /// Perform a bubble recovery bounce on the jump actions end i.e when the player hits the ground
    /// </summary>
    private void InitiateBubbleBounceRecovery()
    {
        float angleInRadians = this.player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInRadians();
        this.player.velocity.x = -this.bubbleBounceReboundSpeed * Mathf.Sin(angleInRadians);
        this.player.velocity.y = this.bubbleBounceReboundSpeed * Mathf.Cos(angleInRadians);
        this.player.SetGrounded(false);
        this.bubbleRecoveryInitiated = true;
        GMAudioManager.Instance().PlayOneShot(this.bubbleShieldBounceSound);
    }
}
