using UnityEngine;
/// <summary>
/// The starpost checkpoint type
/// </summary>
public class StarPostController : CheckpointController
{
    [SerializeField]
    private StarPostHeadController starPostHeadController;
    [Tooltip("The audio played when the spring is touched")]
    public AudioClip starPostTouchedSound;
    public override void Reset() => this.starPostHeadController = this.GetComponentInChildren<StarPostHeadController>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        if (this.starPostHeadController == null)
        {
            this.Reset();
        }
        if (this.CheckPointIsActive())
        {
            this.starPostHeadController.AlreadyActive();
        }
    }

    /// <summary>
    /// Activate the checkpoint on contact with the solid box
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = this.starPostHeadController.GetActivated() == false;

        return triggerAction;
    }

    /// <summary>
    /// Set the activated flag to begin rotation of the starpost head
    /// <param name="player">The player object  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.starPostHeadController.SetActivated(true);
        GMAudioManager.Instance().PlayOneShot(this.starPostTouchedSound);
        this.RegistorCheckPointPosition();
    }
}
