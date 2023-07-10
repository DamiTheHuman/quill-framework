using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Shield abilitiy for the electric shield abillity which performs the electric jump and magnetizes rings
/// </summary
public class ElectricShield : HedgeShieldAbility
{
    [Tooltip("The amount of force to add to the players vertical velocity"), SerializeField]
    private float electricShieldVelocity = 5.5f;
    [Tooltip("The Layer for magnetizable objects such as rings")]
    [SerializeField, LayerList] private int ringLayer = 29;
    [Tooltip("The radius which the rings must be in to be magnetized")]
    private float attractionRadius = 64;
    [Tooltip("A collection of results containing the unmagnetized rings that have been collided with")]
    private Collider2D[] results = new Collider2D[100];
    [Tooltip("A list of all the rings currently attracted to the electric shield"), SerializeField]
    private List<RingController> attractedRings = new List<RingController>();
    [SerializeField, Tooltip("The sound played when the electric shield is activated")]
    private AudioClip electricJumpSound = null;
    [SerializeField]
    private Color debugColor = Color.cyan;
    [SerializeField, IsDisabled]
    private LayerMask collisionMask;

    [SerializeField, IsDisabled, Tooltip("The ring id set for the next magnetized ring")]
    private int currentMaxRingId = 0;
    public override void Start()
    {
        base.Start();
        this.collisionMask |= 1 << this.ringLayer; //Add the ring layer to the collision mask
    }

    /// <summary>
    /// When the shield is first granted
    /// </summary>
    public override void OnInitializeShield()
    {
        base.OnInitializeShield();
        this.attractedRings.RemoveAll(ring => ring == null);
    }

    /// <summary>
    /// Launches the players upwards by the electricShieldVelocity unaffecting the players current x velocity 
    /// </summary>
    public override void OnActivateAbility()
    {
        this.player.velocity.y = this.electricShieldVelocity;
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.ElectricShieldSpark, this.player.transform.position).SetActive(true);
        GMAudioManager.Instance().PlayOneShot(this.electricJumpSound);
    }

    /// <summary>
    /// While acive allow for the attraction of rings
    /// </summary>
    public void FixedUpdate()
    {
        int hits = Physics2D.OverlapCircleNonAlloc(this.player.transform.position, this.attractionRadius, this.results, this.collisionMask);

        for (int i = 0; i < hits; i++)
        {
            RingController ringController = this.results[i].gameObject.GetComponent<RingController>();
            if (ringController.GetRingType() == RingType.Magnetized)
            {
                continue;
            }

            ringController.SetRingType(RingType.Magnetized);
            ringController.SetMagnetizeTarget(this.player.transform);
            ringController.SetMagnetizeRingId(this.currentMaxRingId);
            this.attractedRings.Add(ringController);

            //Set the ring to remove itself from the list when the magnetized ring is gotten
            int currentMaxRingId = this.currentMaxRingId;
            ringController.SetOnMagnetizedRingGotten(() => this.attractedRings.RemoveAll(ring => ring.GetMagnetizeRingId() == currentMaxRingId));

            this.currentMaxRingId++;
        }
    }

    /// <summary>
    /// On Electric shield deactivation set attracted active rings to spilled
    /// </summary>
    public override void OnDeinitializeShield()
    {
        this.currentMaxRingId = 0;
        if (this.attractedRings == null)
        {
            return;
        }

        foreach (RingController ringController in this.attractedRings)
        {
            if (ringController.enabled)
            {
                ringController.UnmagnetizeRing();
            }
        }

        this.attractedRings.Clear();
    }

    /// <summary>
    /// Gets the electric shields velocity
    /// </summary>
    public float GetElectricShieldVelocity() => this.electricShieldVelocity;


    private void OnDrawGizmos()
    {
        Gizmos.color = this.debugColor;
        GizmosExtra.Draw2DCircle(this.transform.position, this.attractionRadius, this.debugColor);
    }
}
