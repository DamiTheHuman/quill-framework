using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the Bridge moving it up and down based on the players location  on the blocks
/// Author - DAMIZEAN [Sonic Worlds]  just converted to unity :D, Bridge Smooth Delta - Triangly [Orbinaut Framework]
/// </summary>
public class BridgeController : SolidContactGimmick
{
    [SerializeField]
    private BoxCollider2D boxCollider2D;
    [Tooltip("The length of the bridge")]
    [Range(4, 16), FirstFoldOutItem("Bridge Initialization Info")]
    public int bridgeLength = 12;
    [Tooltip("The block object to clone")]
    public BridgeBlockController blockToClone;
    [Tooltip("A reference to the blocks built with this bridge"), SerializeField, LastFoldoutItem()]
    private List<BridgeBlockController> bridgeBlocks = new List<BridgeBlockController>();
    [Tooltip("The current block the player is standing on"), FirstFoldOutItem("Active Bridge Info")]
    public float tensionPoint;
    [Tooltip("How far the player is from the center of the bridge")]
    public float distanceFromCenter;
    [SerializeField, Tooltip("How fast the bridge depresses on contact with the player")]
    private float depressionSpeed = 1f;
    [SerializeField, Tooltip("The max depression based on where the player is currently standing"), LastFoldoutItem()]
    private float maxDepression = 0;

    [FirstFoldOutItem("Bridge Smooth Delta")]
    [Tooltip("current bridge smooth delta"), SerializeField]
    private float currentBridgeSmoothDelta = 0;
    [LastFoldoutItem()]
    [Tooltip("The increment for bridge smooth delta"), SerializeField]
    private float bridgeSmoothIncrementa = 5.625f;

    [FirstFoldOutItem("Bridge Handles")]
    [Tooltip("The left bridge handle"), SerializeField]
    private Transform leftBridgeHandle;
    [LastFoldoutItem]
    [Tooltip("The right bridge handle"), SerializeField]
    private Transform rightBridheHandle;
    public Color debugColor = General.RGBToColour(230, 129, 0);

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        this.BuildBridgeBlocks();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate() => this.UpdateBridgeSmoothDelta();
    /// <summary>
    /// Determines if the player is on the bridge
    /// </summary>
    public bool OnBridge() => this.tensionPoint > 0;
    /// <summary>
    /// Get the current smooth delta of the bridge
    /// </summary>
    public float GetBridgeSmoothDelta() => this.currentBridgeSmoothDelta;
    /// <summary>
    /// Builds the Bridge Blocks by duplicating the first bridge block object based on the bridgeLength variable
    /// </summary>
    public void BuildBridgeBlocks()
    {
        this.bridgeBlocks.Add(this.blockToClone);
        Vector2 currentPlacementPosition = this.blockToClone.transform.position;
        SpriteRenderer bridgeSprite = this.blockToClone.GetComponent<SpriteRenderer>();
        this.blockToClone.SetBridgeController(this);

        for (int x = 1; x < this.bridgeLength; x++)
        {
            currentPlacementPosition.x += bridgeSprite.size.x;//place the game object  adding the sprite width to the x position
            BridgeBlockController bridgeClone = Instantiate(this.blockToClone.gameObject, currentPlacementPosition, Quaternion.Euler(0, 0, 0))
                                                .GetComponent<BridgeBlockController>();
            bridgeClone.transform.parent = this.blockToClone.transform.parent;
            bridgeClone.name = this.blockToClone.name + " " + x;
            bridgeClone.SetBridgeController(this);
            this.bridgeBlocks.Add(bridgeClone);
            currentPlacementPosition = bridgeClone.transform.position;//Update the current position
        }

        this.blockToClone.name += " 0";
    }

    /// <summary>
    /// Checks if the players collider is within the activtable bridge bounds
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;

        if (player.GetSolidBoxController().ContactEventIsActive(this) == false && player.velocity.y <= 0 && player.GetGrounded())
        {
            if (player.transform.position.x <= solidBoxColliderBounds.max.x || player.transform.position.x >= solidBoxColliderBounds.min.x)
            {
                triggerAction = true;
            }
        }
        //If the player isnt within activatable range but the bridge block is depressed reset the bridge
        if (triggerAction == false && this.bridgeBlocks[this.bridgeLength / 2].GetTargetHeight() != 0)
        {
            this.ResetBridge();
        }

        return triggerAction;
    }

    /// <summary>
    /// Change the log height based on the players position
    /// <param name="player">The player object to apply the velocity to  </param>
    /// </summary>
    public override void HedgeOnCollisionStay(Player player)
    {
        base.HedgeOnCollisionStay(player);
        float playerXPosition = player.transform.position.x;
        this.distanceFromCenter = Mathf.Max(Mathf.Abs(Mathf.Abs(this.boxCollider2D.bounds.center.x - playerXPosition) - (this.bridgeLength / 2 * 16)), 0);//Distance from the center point of the bridge
        this.tensionPoint = Mathf.Floor((playerXPosition - this.boxCollider2D.bounds.min.x) / 16) + 1;
        this.tensionPoint = Mathf.Clamp(this.tensionPoint, 1, this.bridgeBlocks.Count);
        this.maxDepression = this.tensionPoint <= this.bridgeBlocks.Count / 2 ? this.maxDepression = this.tensionPoint * 2 : this.maxDepression = (this.bridgeBlocks.Count - this.tensionPoint + 1) * 2;

        for (int i = 0; i < this.bridgeBlocks.Count; i++)
        {
            // The difference in position of this log to current log stood on
            float difference = Mathf.Abs(i + 1 - this.tensionPoint);
            //The distance between the current log and the tension point from either side
            float logDistance = i < this.tensionPoint ? logDistance = 1 - (difference / this.tensionPoint) : logDistance = 1 - (difference / (this.bridgeBlocks.Count - this.tensionPoint + 1));
            this.bridgeBlocks[i].SetTargetHeight(-Mathf.Floor(this.maxDepression * Mathf.Sin(90 * Mathf.Deg2Rad) * logDistance));  //the final y position for the log
        }
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.OnBridge);
        player.transform.parent = this.bridgeBlocks[(int)this.tensionPoint - 1].transform;
    }

    /// <summary>
    /// Reset the players sink amound and reset the bridge
    /// </summary>
    public override void HedgeOnCollisionExit(Player player)
    {
        base.HedgeOnCollisionExit(player);
        player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        Sensors sensors = player.GetSensors();
        player.transform.parent = null;
        this.ResetBridge();
    }

    /// <summary>
    /// Update the smooth delta of the bridge
    /// </summary>
    private void UpdateBridgeSmoothDelta()
    {
        if (this.OnBridge())
        {
            if (this.currentBridgeSmoothDelta < 90)
            {
                this.currentBridgeSmoothDelta += this.bridgeSmoothIncrementa;
            }
        }
        else
        {
            if (this.currentBridgeSmoothDelta > 0)
            {
                this.currentBridgeSmoothDelta -= this.bridgeSmoothIncrementa;
            }
        }

        this.currentBridgeSmoothDelta = Mathf.Clamp(this.currentBridgeSmoothDelta, 0f, 90f);
    }

    /// <summary>
    /// Reset the bridges height to regular position
    /// </summary>
    private void ResetBridge()
    {
        this.distanceFromCenter = 0;
        this.tensionPoint = 0;

        for (int i = 0; i < this.bridgeBlocks.Count; i++)
        {
            this.bridgeBlocks[i].SetTargetHeight(1);
        }
    }

    /// <summary>
    /// Gets the depression speed of the bridge
    /// </summary>
    public float GetDepressionSpeed() => this.depressionSpeed;

    private void OnDrawGizmos()
    {
        if (this.blockToClone != null)
        {
            BoxCollider2D collider = this.GetComponent<BoxCollider2D>();
            SpriteRenderer blockSprite = this.blockToClone.GetComponent<SpriteRenderer>();
            collider.size = new Vector2(blockSprite.size.x * this.bridgeLength, blockSprite.size.y);
            collider.offset = new Vector2(16 + (blockSprite.size.x / 2 * (this.bridgeLength - 1)), -16f);
            GizmosExtra.DrawRect(this.transform, collider, this.debugColor);
            GizmosExtra.DrawWireRect(collider.bounds, Color.red);
            if (Application.isPlaying == false)
            {
                if (this.leftBridgeHandle != null)
                {
                    this.leftBridgeHandle.transform.position = this.blockToClone.transform.position + new Vector3(-16, 16);
                }

                if (this.rightBridheHandle != null)
                {
                    this.rightBridheHandle.transform.position = this.blockToClone.transform.position + new Vector3(16 * this.bridgeLength, 16);
                }
            }
        }
    }

}
