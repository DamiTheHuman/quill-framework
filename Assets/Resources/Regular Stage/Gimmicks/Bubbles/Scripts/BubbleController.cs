using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// Handles the spawning of bubbles small, medium or large for visuals or to replenish the players health depending on the type
/// Author - Lake Feperd
/// </summary>
public class BubbleController : TriggerContactGimmick, IPooledObject
{
    [SerializeField, Tooltip("The sprite renderer of the bubble")]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Bounds waterBoundaries;
    [Tooltip("The individual timestep taken to move each bubbe vertically"), EnumConditionalEnable("bubbleType", 2)]
    public bool bubbleCanBeInteractedWith;
    [Tooltip("The individual timestep taken to move each bubbe vertically")]
    public float bubbleTimeStep = 0.02f;
    [Tooltip("The type of bubble being interactid with")]
    public BubbleType bubbleType = BubbleType.Small;
    [Tooltip("The speed in which the bubble moves vertically every time step")]
    public float verticalBubbleSpeed = 1f;
    [Tooltip("The speed in which the bubble moves downwards when bubble vertical movement is positive")]
    public float downWardBubbleSpeed = 0.2f;
    [Tooltip("The vertical direction of the bubble")]
    public float bubbleVerticalMovement;
    [Tooltip("The horizoontal delta of the bubble")]
    public float delta;
    [Tooltip("The speed in which the horizontal delta is incremented by")]
    public float deltaIncremenet = 2.5f;
    [Tooltip("The horizontal range of the bubble")]
    public float horizontalRange = 4;
    [Tooltip("The random range for the X position of the bubble")]
    public int randomHorizontalSpawnRange = 11;
    [Tooltip("The additional offset granted to a newly spawned bubble")]
    public int additionalSpawnOffset = 5;

    public void OnObjectSpawn()
    {
        this.delta = 0;
        this.startPosition = this.transform.position;
        this.startPosition.x -= this.additionalSpawnOffset + Random.Range(0, this.randomHorizontalSpawnRange);
        this.bubbleVerticalMovement = 1;
        this.bubbleCanBeInteractedWith = false;
        this.waterBoundaries = new Bounds();
        this.spriteRenderer.enabled = false;
    }

    private void OnEnable()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.ExtraBubbleMovementCoroutine(this.bubbleTimeStep));
    }

    private void FixedUpdate()
    {
        this.MoveBubble();
        this.DeactivateBubble();
    }
    /// <summary>
    /// Deactivates the bubble if they exceed the water boundaries
    /// </summary>
    private void DeactivateBubble()
    {
        if (this.waterBoundaries != null && this.transform.position.y > this.waterBoundaries.max.y)
        {
            this.StopAllCoroutines();
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the basic movement of the bubble
    /// </summary>
    private void MoveBubble()
    {
        Vector2 pos = this.transform.position;
        //If the bubble is above the waterlevel
        if (this.bubbleType != BubbleType.Large)
        {
            if (this.bubbleVerticalMovement < 0)
            {
                pos.y -= this.downWardBubbleSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;  //Moves the bubble vertically downars
                this.bubbleVerticalMovement++;
            }
            this.bubbleVerticalMovement = Mathf.Clamp(this.bubbleVerticalMovement, Mathf.NegativeInfinity, 1);
        }
        //Moves the bubble horizontally 
        pos.x = GMStageManager.Instance().ConvertToDeltaValue(this.startPosition.x + (this.horizontalRange * Mathf.Sin(this.delta * Mathf.Deg2Rad)));
        this.delta += this.deltaIncremenet;
        this.delta = General.ClampDegreeAngles(this.delta);

        //If water cant be found after 3 increments deactivate the bubble
        if (this.delta > this.deltaIncremenet * 3 && this.spriteRenderer.enabled == false)
        {
            this.DeactivateBubble();
        }

        this.transform.position = pos;
    }

    /// <summary>
    /// The basic vertical movement of the bubble every timestep
    /// </summary>
    private void VerticalBubbleMovement() => this.transform.position += new Vector3(0, this.verticalBubbleSpeed * GMStageManager.Instance().GetPhysicsMultiplier()) * Time.deltaTime;

    private IEnumerator ExtraBubbleMovementCoroutine(float bubbleTimeStep)
    {
        while (true)
        {
            yield return new WaitForSeconds(bubbleTimeStep);

            if (this.bubbleVerticalMovement != 1)
            {
                continue;
            }

            this.VerticalBubbleMovement();
        }
    }
    /// <summary>
    /// Allows the bubble to interactable with the player
    /// </summary>
    public void SetBubbleInteractionActive()
    {
        if (this.bubbleType == BubbleType.Large)
        {
            this.bubbleCanBeInteractedWith = true;
        }
    }

    /// <summary>
    /// Checks if the player contacts a large bubble while not on the ground to replenish the player breath
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = this.bubbleType == BubbleType.Large && player.GetGrounded() == false && player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.BubbleShield && this.bubbleCanBeInteractedWith;

        return triggerAction;
    }
    /// <summary>
    /// Replenish the players breath and burst the bubble
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BubbleBurst, this.transform.position);
        this.gameObject.SetActive(false);
        player.GetGimmickManager().AirBubble();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.waterBoundaries != new Bounds())
        {
            return;
        }

        if (collision.gameObject.layer == GMWaterLevelManager.Instance().GetWaterLevelLayer())
        {
            this.waterBoundaries = collision.bounds;//Get access to the boundaries of the water layer to know when to destroy the bubble dynamically
            this.spriteRenderer.enabled = true;
        }
    }

}
