using UnityEngine;
/// <summary>
/// Shield abilitiy for the flame shield which launches the  flame dash sound
/// </summary>
public class FlameShield : HedgeShieldAbility
{
    public Animator animator;
    [Tooltip("The velocity added to the player horizontally on activation"), SerializeField]
    private float flameShieldVelocity = 8;
    [SerializeField, Tooltip("The sound played when the flame shield is activated")]
    private AudioClip flameDashSound = null;
    private void Awake() => this.Start();
    public override void Start()
    {
        base.Start();

        if (this.animator == null)
        {
            this.animator = this.GetComponentInChildren<Animator>();
        }
    }

    /// <summary>
    /// Launches the player forward in the direction of their horizontal velocity and zeroes out their y velocity
    /// </summary>
    public override void OnActivateAbility()
    {
        this.player.velocity.x = this.flameShieldVelocity * this.player.currentPlayerDirection;
        this.player.velocity.y = 0;
        HedgehogCamera.Instance().GetCameraLookHandler().BeginDashLag();

        if (this.animator.gameObject.activeSelf)
        {
            this.animator.SetTrigger("Activated");
        }

        GMAudioManager.Instance().PlayOneShot(this.flameDashSound);
    }

    /// <summary>
    /// Quickly resets the animator back to idle
    /// </summary>
    public override void OnAbilityEnd()
    {
        this.animator.Play("Idle");
        this.animator.ResetTrigger("Activated");
    }

    /// <summary>
    /// Gets the flame shields velocity
    /// </summary>
    public float GetFlameShieldVelocity() => this.flameShieldVelocity;
}
