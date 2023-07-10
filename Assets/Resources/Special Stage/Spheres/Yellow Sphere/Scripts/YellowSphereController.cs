using UnityEngine;

/// <summary>
/// A Gimmick that bounces the player on contact
/// </summary>
public class YellowSphereController : SpecialStageContactGimmick
{

    [SerializeField]
    private BoxCollider boxCollider;
    [SerializeField, Tooltip("The bounce velocity when the player touches the sphere")]
    private float bounceVelocity;
    [Tooltip("The debug colour of the collider"), SerializeField]
    private Color debugColor = new Color(1f, 0.92f, 0.016f, 0.5f);

    public override void Reset()
    {
        base.Reset();
        this.boxCollider = this.GetComponent<BoxCollider>();
    }

    /// <summary>
    /// Checks if the player is ahead of the bomb and grounded
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(SpecialStagePlayer player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if (player.GetGrounded())
        {
            triggerAction = true;
        }

        return triggerAction;
    }

    /// <summary>
    /// Make the player jump
    /// <param name="player">The player who lost </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(SpecialStagePlayer player)
    {
        player.GetActionManager().PerformAction<SpecialStageJump>();
        player.velocity.x = 0;
        player.velocity.y = -this.bounceVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        Gizmos.DrawCube(this.boxCollider.bounds.center, this.boxCollider.bounds.size);
    }
}
