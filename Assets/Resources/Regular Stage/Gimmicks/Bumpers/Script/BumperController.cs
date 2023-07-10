using UnityEngine;
/// <summary>
/// A gimmick object which bounces the player away from the gimmick based on the contact angle
/// </summary>
public class BumperController : TriggerContactGimmick
{
    [SerializeField]
    private Animator animator;
    public int scoreToGrant = 10;
    [Tooltip("The rebound velocity of the player after comming in contact with the bumper"), SerializeField]
    private Vector2 bumperReboundVelocity = new Vector2(7, 7);
    [Tooltip("The audio played when the bumper is touched")]
    public AudioClip bumperTouchedSound;

    public override void Reset()
    {
        base.Reset();
        this.animator = this.GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.animator == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Checks if the players has collided with the bumper
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = true;

        return triggerAction;
    }

    /// <summary>
    /// Apply the rebound velocity to the player based on the angle between the player and the bumper
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        float angleBetweenBumperAndPlayer = General.AngleBetweenVector2(this.transform.position, player.transform.position);
        bool isGliding = false;
        //Dont update the horizontal velocity if the player is gliding or turning
        if (player.GetActionManager().CheckActionIsBeingPerformed<Glide>())
        {
            Glide glide = player.GetActionManager().GetAction<Glide>() as Glide;
            isGliding = glide.GetGlideState() is GlideState.Gliding or GlideState.Turning;
        }
        if (isGliding == false)
        {
            player.SetHorizontalVelocity(this.bumperReboundVelocity.x * Mathf.Cos(angleBetweenBumperAndPlayer));
        }
        player.velocity.y = this.bumperReboundVelocity.y * Mathf.Sin(angleBetweenBumperAndPlayer);
        this.animator.SetTrigger("Bumped");
        //Update the score
        GMRegularStageScoreManager.Instance().IncrementScoreCount(this.scoreToGrant);
        GMRegularStageScoreManager.Instance().DisplayScore(this.transform.position, 0);
        GMAudioManager.Instance().PlayOneShot(this.bumperTouchedSound);
    }
}
