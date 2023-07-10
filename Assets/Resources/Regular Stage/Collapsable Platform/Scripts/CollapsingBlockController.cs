using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
/// <summary>
/// A gimmick which drops sprite pieces in succession after coming in contact with it 
///</summary>

public class CollapsingBlockController : TriggerContactGimmick
{
    [SerializeField]
    private Texture2D baseTexture;

    [Button(nameof(SpritePositionsHelper))]
    public bool spritePositionHelper;

    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Collider2D currentCollider2D;
    [SerializeField]
    private SpriteSplitter spriteSplitter;
    [SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;
    [Tooltip("The direction which the platform falls in ")]
    public CollapseDirection collapsDirection = CollapseDirection.BottomLeftToTopRight;
    [Tooltip("A reference to each individual platform piece")]
    public List<CollapsableBlockController> platformPieces = new List<CollapsableBlockController>();
    [Tooltip("The vertical force that each platform piece falls with")]
    public float fallGravity = 0.5f;
    [Tooltip("The interval before dropping begins, in steps")]
    public int initialDropWaitTime = 4;
    [Tooltip("The interval between each drop in steps")]
    public int dropIntervalInSteps = 2;
    [Tooltip("The audio played when the blocks begin to collapse")]
    public AudioClip blocksCollapsingSound;

    public override void Reset()
    {
        base.Reset();
        this.currentCollider2D = this.GetComponent<Collider2D>();
        this.spriteSplitter = this.GetComponent<SpriteSplitter>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.currentCollider2D == null)
        {
            this.Reset();
        }

        if (this.spriteRenderer != null)
        {
            this.spriteRenderer.enabled = false;
        }

        if (this.collapsDirection == CollapseDirection.BottomLeftToTopRight)
        {
            this.platformPieces = this.platformPieces.OrderBy(v => v.transform.position.x).ToList();
        }
        else if (this.collapsDirection == CollapseDirection.BottomRightToTopLeft)
        {
            this.platformPieces = this.platformPieces.OrderBy(v => v.transform.position.x).Reverse().ToList();
        }
    }

    /// <summary>
    /// Checks if the player comes in contact with the collapsing block by standing on it
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerAction = false;
        triggerAction = player.GetGrounded() && player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform.parent == this.transform;

        return triggerAction;
    }

    /// <summary>
    /// Begins dropping the individual block pieces at a per interval setp
    /// <param name="player">The player object to come in contact with the block </param>
    /// </summary
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.currentCollider2D.enabled = false;
        this.StartCoroutine(this.DropCollapsableBlock(this.dropIntervalInSteps, player));
    }

    /// <summary>
    /// Drops each block object contained in the activePlatformBlock array at a set speed per second
    /// <param name="timeBetweenDrops">The time between each drop </param>
    /// <param name="player">The player object in question  </param>
    /// </summary>  
    private IEnumerator DropCollapsableBlock(int timeBetweenDrops, Player player)
    {
        if (this.initialDropWaitTime > 0)
        {
            for (int x = 0; x < this.initialDropWaitTime; x++)
            {
                yield return new WaitForFixedUpdate();
            }

            GMAudioManager.Instance().PlayOneShot(this.blocksCollapsingSound);
        }

        foreach (CollapsableBlockController platformPiece in this.platformPieces)
        {
            platformPiece.BeginCollapse(this.fallGravity);

            //Player was standing on a piece thus fall
            if (player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetHit().transform == platformPiece.transform)
            {
                platformPiece.GetParentCollider().enabled = false;
                player.SetGrounded(false);
            }

            for (int x = 0; x < timeBetweenDrops; x++)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        yield return null;
    }

    /// <summary>
    /// Helps update the sprite position adequately and the posistion of the platofmr pieces
    /// </summary>  
    public void SpritePositionsHelper()
    {
#if UNITY_EDITOR

        List<Sprite> sprites = new List<Sprite>();

        string spriteSheet = AssetDatabase.GetAssetPath(this.baseTexture);
        sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet)
            .OfType<Sprite>().ToArray().ToList();
        sprites = sprites.OrderBy(v => v.textureRect.x).ThenBy(v => v.textureRect.y).ToArray().ToList();
        List<SpriteRenderer> spriteRenderers = this.GetComponentsInChildren<SpriteRenderer>().OrderBy(v => v.transform.position.x).ThenBy(v => v.transform.position.y).ToList();
        this.platformPieces.Clear();

        for (int x = 0; x < spriteRenderers.Count; x++)
        {
            this.platformPieces.Add(spriteRenderers[x].GetComponent<CollapsableBlockController>());
        }

        Vector2 previousPosition = new Vector2();

        for (int x = 0; x < this.platformPieces.Count; x++)
        {
            this.platformPieces[x].GetComponent<SpriteRenderer>().sprite = sprites[x];
            this.platformPieces[x].transform.localPosition = new Vector3(8, sprites[x].textureRect.y + 40);
            this.platformPieces[x].transform.name = this.baseTexture.name + "Piece " + x;

            if (previousPosition.x != this.platformPieces[x].transform.position.x && x > 0)//When the previous position moved
            {
                this.platformPieces[x - 1].blockType = CollapsableBlockType.RemoveCollisionWhenActivated;
            }

            previousPosition = this.platformPieces[x].transform.position;
        }

        this.platformPieces[this.platformPieces.Count - 1].blockType = CollapsableBlockType.RemoveCollisionWhenActivated;
        General.SetDirty(this);
#endif

    }
}
