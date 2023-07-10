using UnityEngine;
/// <summary>
/// Manages the interaction when a player comes in contact wiht a balloon like object
/// </summary>
public class BalloonController : TriggerContactGimmick
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private Animator animator;

    [SerializeField, LastFoldoutItem()]
    private AudioClip popSound;

    [SerializeField, Tooltip("The velocity of the player on contact"), FirstFoldOutItem("Balloon Settings")]
    private float balloonVelocity = 7;
    [Tooltip("Signifies that the balloon like object will never deactivate its collider post collision"), SerializeField]
    private bool infinite;
    [SerializeField, LastFoldoutItem()]
    private GimmickSubstate gimmickSubstate = GimmickSubstate.None;

    [SerializeField, FirstFoldOutItem("Debug"), LastFoldoutItem()]
    private Color debugColour = new Color(0, 0.5900204f, 1, 0.5f);

    public override void Reset()
    {
        this.TryGetComponent(out Animator animator);
        this.TryGetComponent(out BoxCollider2D boxCollider2D);

    }

    protected override void Start()
    {
        base.Start();

        if (this.animator == null || this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.SetAnimatorState(0);
    }

    /// <inheritdoc>
    /// <see cref="ContactEvent.HedgeOnCollisionEnter(Player)"/>
    /// </inheritdoc>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        this.SetCollisionState(CollisionState.OnCollisionEnter);
        player.velocity.y = this.balloonVelocity;
        player.SetGrounded(false);

        if (this.popSound != null)
        {
            GMAudioManager.Instance().PlayOneShot(this.popSound);
        }

        if (this.gimmickSubstate != GimmickSubstate.None)
        {
            player.GetAnimatorManager().SwitchSubstate(0);
            player.GetAnimatorManager().SwitchGimmickSubstate(this.gimmickSubstate);
        }

        this.boxCollider2D.enabled = this.infinite;
        this.SetAnimatorState(1);
    }


    /// <summary>
    /// Set the animator state value
    /// <param name="state">The new animator value for the state field</param>
    /// </summary>
    private void SetAnimatorState(int state)
    {
        if (this.animator == null)
        {
            return;
        }

        this.animator.SetInteger("State", state);
    }

    /// <summary>
    /// Disables the Balloon controller usually called by the animator after the 'Pop' like animation
    /// </summary>
    public void DisableBalloonController() => this.gameObject.SetActive(false);

    private void OnDrawGizmos()
    {
        if (this.boxCollider2D == null)
        {
            this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        }

        GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColour);
    }
}
