using UnityEngine;

/// <summary>
/// A hazardous gimmick that damages the plauer on contact
/// </summary>
public class BombController : SpecialStageContactGimmick
{
    [SerializeField]
    private BoxCollider boxCollider;
    [Tooltip("The debug colour of the collider"), SerializeField]
    private Color debugColor = General.RGBToColour(255, 77, 84, 170);
    [SerializeField, Tooltip("Audio played when the player runs into a bomb")]
    private AudioClip bombDestroyAudioClip;

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

        if (player.GetHealthManager().GetHealthStatus() == HealthStatus.Vulnerable && solidBoxColliderBounds.center.y < this.boxCollider.bounds.center.y && player.GetGrounded())
        {
            triggerAction = true;
        }

        return triggerAction;
    }

    /// <summary>
    /// Identify the player has gotten hit
    /// <param name="player">The player who lost </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(SpecialStagePlayer player)
    {
        bool hit = player.GetHealthManager().VerifyHit();

        if (hit == false)
        {
            return;
        }

        GameObject explosition = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.MonitorExplosion, this.transform.position);
        explosition.transform.eulerAngles = this.transform.eulerAngles;
        this.gameObject.SetActive(false);
        GMAudioManager.Instance().PlayOneShot(this.bombDestroyAudioClip);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        Gizmos.DrawCube(this.boxCollider.bounds.center, this.boxCollider.bounds.size);
    }
}
