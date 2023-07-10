using UnityEngine;
/// <summary>
/// This refers to rings gotten specifically in special stages
/// </summary>
public class SpecialStageRingController : SpecialStageContactGimmick
{

    [SerializeField]
    private BoxCollider boxCollider;
    [Tooltip("The amount of rings to add to the counter on retrieval"), SerializeField]
    private int ringValue = 1;
    [Tooltip("The offsets for the ring sparkles"), SerializeField]
    private float sparkleOffset = 4;
    [Tooltip("The audio played when the ring is collected"), SerializeField]
    private AudioClip collectedSound;
    [SerializeField, Tooltip("Debug color of the ring")]
    private Color debugColor = new Color(1f, 0.92f, 0.016f, 0.5f);
    public override void Reset()
    {
        base.Reset();
        this.boxCollider = this.GetComponent<BoxCollider>();
    }

    /// <summary>
    /// If the player comes in contact within reasonable bounds of the special stage ring
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(SpecialStagePlayer player, Bounds solidBoxColliderBounds)
    {
        bool triggerContact = false;

        if (solidBoxColliderBounds.center.y < this.boxCollider.bounds.center.y)
        {
            triggerContact = true;
        }
        return triggerContact;
    }

    /// <summary>
    /// Give the player the ring if the collision check is passed
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(SpecialStagePlayer player) => this.RingCollected(player.transform.position);
    /// <summary>
    /// The set of actions to be performed on ring collections
    /// <param name="playerPosition">The position of the player when they touched the ring to know what side the audio should come from</param>
    /// </summary>
    private void RingCollected(Vector2 playerPosition)
    {
        GMSpecialStageScoreManager.Instance().IncrementRingCount(this.ringValue);//add one to the ring count
        GameObject sparkle1 = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position);
        GameObject sparkle2 = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position + new Vector3(Random.Range(-this.sparkleOffset, this.sparkleOffset), 0, Random.Range(-this.sparkleOffset, this.sparkleOffset)));
        GameObject sparkle3 = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position + new Vector3(Random.Range(-this.sparkleOffset, this.sparkleOffset), 0, Random.Range(-this.sparkleOffset, this.sparkleOffset)));
        OneShotSoundDirection oneShotAudioType = playerPosition.x <= this.transform.position.x ? OneShotSoundDirection.LeftEarOnly : OneShotSoundDirection.RightEarOnly;
        sparkle1.transform.eulerAngles = this.transform.eulerAngles;
        sparkle2.transform.eulerAngles = this.transform.eulerAngles;
        sparkle3.transform.eulerAngles = this.transform.eulerAngles;
        GMAudioManager.Instance().PlayOneShot(this.collectedSound, oneShotAudioType);
        this.gameObject.SetActive(false);//Deactivate instead of destroy
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        Gizmos.DrawCube(this.boxCollider.bounds.center, this.boxCollider.bounds.size);
    }
}
