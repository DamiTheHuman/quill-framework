using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// **IT IS RECOMMENDED TO USE THE SCRIPT AND CACHE IT FOR REUSE FOR BETTER PERFORMANCE AND TO SAVE CHANGES MADE AS COLLISION MAY NOT BE 100% ACCURETE**
/// A helper class which splits a sprite component into multiple pieces while also retaining its polygon based collisios
/// It does this via the pointer and the texuure in question and pointer object which moves vertically and horizontally through sprite
/// It moves in a manner of rows and columns if its done checking vertically it then moves horizontally one piece and the vertically again
/// IT IS IMPORTANT THE SPRITE MUST BE SINGLE AND READABLE for it to create the images clearly
/// Author - Headless Coder
/// </summary
//[ExecuteInEditMode]
[RequireComponent(typeof(CollapsingBlockController), typeof(PolygonCollider2D), typeof(SpriteRenderer))]
public class SpriteSplitter : MonoBehaviour
{
#if UNITY_EDITOR
    private CollapsingBlockController collapsingBlockController;
    private PolygonCollider2D polygonCollider;
    private GameObject blockSpritePiece;
    private SpriteRenderer spriteRenderer;
    [SerializeField, Button("SplitBlocks")]
    private bool splitBlocks;
    public void SplitBlocks()
    {
        this.activePlatformBlocks = new List<GameObject>();
        this.Activate();
    }

    [Tooltip("The path which the sprite pieces will be placed in"), SerializeField]
    private string path = "Assets/Resources/Sprite Split Pieces";
    [LayerList, Help("The layer that all the child objects will be created on"), SerializeField]
    private int collapsableBlockLayer = 13;
    [Tooltip("The collision mask for the current object"), SerializeField]
    private LayerMask collisionMask = new LayerMask();
    [Tooltip("A slight offset to be considered when raycasting for the end of the polygon"), SerializeField]
    private float raycastLeeway = 0.1f;

    [SerializeField]
    private float individualPieceSize = 16;
    private Transform currentParentTransform;
    public List<GameObject> activePlatformBlocks = new List<GameObject>();
    private Vector2 nextPolygonPosition;
    private Transform parentTransform;

    /// <summary>
    ///Activates the split operation when called
    /// </summary> 
    public void Activate()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.collapsingBlockController = this.GetComponent<CollapsingBlockController>();
        this.polygonCollider = this.GetComponent<PolygonCollider2D>();
        this.SplitSpriteToIndividualBlocks(this.spriteRenderer);

        if (this.collapsingBlockController != null)
        {
            //this.collapsingBlockController.platformPieces = this.activePlatformBlocks;
        }
    }

    /// <summary>
    ///Begins the task of splliting each sprite into idividual block pieces
    /// <param name="blockSprite">The sprite to be split into pieces based off the block size  </param>
    /// </summary> 
    private void SplitSpriteToIndividualBlocks(SpriteRenderer blockSprite)
    {
        int column = 1;
        this.nextPolygonPosition = Vector2.zero;
        Vector2 blockPosition = blockSprite.bounds.min - this.transform.position;
        Vector2 initialBlockPosition = blockPosition;
        Vector2 pointerPosition = new Vector2();
        this.CreateNewColumn(column, blockPosition);
        float iterativeUpdateValue = this.GetIterativeValue(blockSprite.sprite);
        bool lastColumn = false;// A flag to dermine the last vertical block within the column to be placed
        for (int x = 0; true; x++)
        {
            if (x != 0)
            {
                if (!lastColumn)//If the pointer is not at the top boundary and the flag for skipping was set up
                {
                    pointerPosition.y += this.individualPieceSize;
                }
                else//Move the pointer to the next row and start at the beginning of the column
                {
                    pointerPosition.y = 0;
                    pointerPosition.x += this.individualPieceSize;
                    blockPosition.y = initialBlockPosition.y;//Restart the y position pointer
                    blockPosition.x += iterativeUpdateValue;

                    if (pointerPosition.x >= blockSprite.sprite.texture.width)
                    {
                        break;
                    }

                    lastColumn = false;
                    column++;
                    this.CreateNewColumn(column, blockPosition);
                }
            }

            float yLimit = this.individualPieceSize;
            float xLimit = this.individualPieceSize;
            if ((pointerPosition.y + this.individualPieceSize) >= this.spriteRenderer.bounds.size.y)
            {
                yLimit = (this.spriteRenderer.bounds.size.y - (pointerPosition.y));
                lastColumn = true;
            }

            if ((pointerPosition.x + this.individualPieceSize) >= this.spriteRenderer.bounds.size.x)
            {
                xLimit = (this.spriteRenderer.bounds.size.x - (pointerPosition.x));
            }


            Rect textureRect = new Rect(pointerPosition.x, pointerPosition.y, xLimit, yLimit);//Formulate a rect of the texture based off the pointer and the blocksize
            Sprite spriteBlockFromTexture = Sprite.Create(this.GenerateTextureFromSprite(blockSprite.sprite), textureRect, new Vector2(0f, 0f), blockSprite.sprite.pixelsPerUnit);//Create a sprite from the texure

            if (pointerPosition.y + this.individualPieceSize <= blockSprite.sprite.texture.height && yLimit == this.individualPieceSize) //If the next sprite is fully transparent the current block is the last block
            {
                float xLimi2t = this.individualPieceSize;
                float yLimit2 = this.individualPieceSize;
                if ((pointerPosition.y + this.individualPieceSize) + this.individualPieceSize >= this.spriteRenderer.bounds.size.y)
                {
                    yLimit2 = (this.spriteRenderer.bounds.size.y - (pointerPosition.y + this.individualPieceSize));
                    lastColumn = true;
                    if (yLimit2 <= 0)
                    {
                        Debug.Log(yLimit2);
                        yLimit2 = 0;
                    }
                }

                Rect rect2 = new Rect(pointerPosition.x, pointerPosition.y + this.individualPieceSize, xLimit, yLimit2);
                Sprite spriteBlockFromTexture2 = Sprite.Create(this.GenerateTextureFromSprite(blockSprite.sprite), rect2, new Vector2(0f, 0f), blockSprite.sprite.pixelsPerUnit);
                //The next sprite is transparent thus this is the last sprite
                if (this.IsTransparent(spriteBlockFromTexture2.texture, pointerPosition.x / this.individualPieceSize, (pointerPosition.y + this.individualPieceSize) / this.individualPieceSize))
                {
                    lastColumn = true;
                }
            }

            if (spriteBlockFromTexture.bounds.size != Vector3.zero)
            {
                if (pointerPosition.y + this.individualPieceSize == blockSprite.sprite.texture.height && !lastColumn)
                {
                    lastColumn = true;
                }
                this.blockSpritePiece = this.CreateSpritePiece(blockPosition, spriteBlockFromTexture, blockSprite);//Create a new game object from the texture

                if (lastColumn)
                {
                    this.SplitPolygonColliderToBlockPiece(this.blockSpritePiece, blockSprite, blockPosition, initialBlockPosition.y, pointerPosition.x);
                }
                blockPosition.y += iterativeUpdateValue;
                this.activePlatformBlocks.Add(this.blockSpritePiece);
                this.blockSpritePiece.GetComponent<SpriteRenderer>().sprite = this.SaveSpriteToEditorPath(spriteBlockFromTexture, this.path, this.activePlatformBlocks.Count);
                this.blockSpritePiece.transform.position += new Vector3(this.individualPieceSize / 2, this.individualPieceSize / 2);
                this.blockSpritePiece.transform.parent = this.parentTransform;
            }

        }
        blockSprite.enabled = false;
    }

    /// <summary>
    /// Creates a column which all child blocks will be parented to 
    /// <param name="column">The id of the column</param>
    /// <param name="blockPosition">The curremt position of the block within the texture points </param>
    /// </summary> 
    private void CreateNewColumn(int column, Vector2 blockPosition)
    {
        //Create a parent transform for the first column 
        GameObject parentColumn = new GameObject
        {
            name = "Column " + column
        };
        parentColumn.transform.parent = this.transform;
        this.parentTransform = parentColumn.transform;
        this.parentTransform.transform.localPosition = blockPosition;
        parentColumn.layer = this.collapsableBlockLayer;
    }

    /// <summary>
    /// Creats an individual gameobject based on the sprite retrieved from the pointer and sets its defaults
    /// <param name="blockPosition">The curremt position of the block within the texture points </param>
    /// <param name="spriteBlockFromTexture">The current sprite attained from the split texture</param>
    /// <param name="blockSprite">The parent srite being split </param>
    /// </summary> 
    private GameObject CreateSpritePiece(Vector2 blockPosition, Sprite spriteBlockFromTexture, SpriteRenderer blockSprite)
    {
        GameObject blockSpritePiece = new GameObject(this.name + " Piece " + (this.activePlatformBlocks.Count + 1));
        blockSpritePiece.AddComponent<SpriteRenderer>().sprite = spriteBlockFromTexture;
        blockSpritePiece.GetComponent<SpriteRenderer>().sortingLayerID = blockSprite.sortingLayerID;
        blockSpritePiece.AddComponent<CollapsableBlockController>();

        blockSpritePiece.transform.parent = this.transform;
        blockSpritePiece.transform.localScale = new Vector2(1, 1);
        blockSpritePiece.transform.localPosition = blockPosition;
        blockSpritePiece.transform.tag = this.transform.tag;
        blockSpritePiece.layer = this.collapsableBlockLayer;

        return blockSpritePiece;
    }

    /// <summary>
    /// Splits the current polygon collider into smaller chunks for each individual split block
    /// Creats an individual gameobject based on the sprite retrieved from the pointer and sets its defaults
    /// <param name="blockSpritePiece">The individual sprite piece created by the texture</param>
    /// <param name="blockPosition">The curremt position of the block within the texture points </param>
    /// <param name="blockSprite">The parent srite being split </param>
    /// <param name="initialSpriteLowestBounds">The lowest boundaries of the sprite object in question</param>
    /// <param name="horizontalPointerPosition">The current position of the horizontal pointer relative to the texture </param>
    /// </summary> 
    private void SplitPolygonColliderToBlockPiece(GameObject blockSpritePiece, SpriteRenderer blockSprite, Vector2 blockPosition, float initialSpriteLowestBounds, float horizontalPointerPosition)
    {
        this.parentTransform.gameObject.AddComponent<PolygonCollider2D>();
        List<Vector2> blockPolygonPoints = new List<Vector2>();
        List<Vector2> orderedBlockPolygonPoints = new List<Vector2>();
        orderedBlockPolygonPoints.AddRange(this.polygonCollider.points);
        orderedBlockPolygonPoints = orderedBlockPolygonPoints.OrderBy(xV => xV.x).ToList();
        float horizontalLimit = blockPosition.x;

        if (this.nextPolygonPosition == Vector2.zero)
        {
            blockPolygonPoints.Add(new Vector2(horizontalLimit, initialSpriteLowestBounds));
        }
        else
        {
            blockPolygonPoints.Add(this.nextPolygonPosition);
        }
        for (int y = 0; y < orderedBlockPolygonPoints.Count; y++)//Loop through the ordered and construct its polygon
        {
            if (orderedBlockPolygonPoints[y].x >= horizontalLimit)
            {
                if (orderedBlockPolygonPoints[y].x < horizontalLimit + this.individualPieceSize)
                {
                    blockPolygonPoints.Add(orderedBlockPolygonPoints[y]);
                }
                else
                {
                    if (horizontalPointerPosition + this.individualPieceSize >= blockSprite.sprite.texture.width)//Last Iteration
                    {
                        blockPolygonPoints.Add(orderedBlockPolygonPoints[orderedBlockPolygonPoints.Count - 1]);
                        blockPolygonPoints.Add(new Vector2(orderedBlockPolygonPoints[orderedBlockPolygonPoints.Count - 1].x, initialSpriteLowestBounds));//Bottom Right
                        if (this.nextPolygonPosition != Vector2.zero)
                        {
                            blockPolygonPoints.Add(new Vector2(horizontalLimit, initialSpriteLowestBounds));//Bottom Left
                        }
                        break;
                    }

                    if (y - 1 < 0)
                    {
                        y = 1;
                    }

                    float distanceBetweenLimitAndLastPoint = -(horizontalLimit + this.individualPieceSize - orderedBlockPolygonPoints[y - 1].x);
                    Vector2 v2 = orderedBlockPolygonPoints[y] - orderedBlockPolygonPoints[y - 1];
                    float angleBetweenBothPoints = Mathf.Atan2(v2.y, v2.x);
                    Vector2 newPolygonPosition;

                    newPolygonPosition.x = horizontalLimit + this.individualPieceSize;
                    newPolygonPosition.y = orderedBlockPolygonPoints[y - 1].y - (distanceBetweenLimitAndLastPoint * Mathf.Sin(angleBetweenBothPoints));

                    Vector2 startPosition = (Vector2)this.transform.position + new Vector2(horizontalLimit + this.individualPieceSize + this.raycastLeeway, 0);
                    Vector2 endPosition = startPosition + new Vector2(0, -10000);

                    RaycastHit2D raycastHit2D = Physics2D.Linecast(startPosition, endPosition, this.collisionMask);
                    if (raycastHit2D)
                    {
                        newPolygonPosition = raycastHit2D.point;
                        newPolygonPosition -= (Vector2)this.transform.position;
                        blockPolygonPoints.Add(newPolygonPosition);
                    }
                    else
                    {
                        blockPolygonPoints.Add(newPolygonPosition);
                    }
                    blockPolygonPoints.Add(new Vector2(newPolygonPosition.x, initialSpriteLowestBounds));//Bottom Right

                    if (this.nextPolygonPosition != Vector2.zero)
                    {
                        blockPolygonPoints.Add(new Vector2(horizontalLimit, initialSpriteLowestBounds));//Bottom Left
                    }
                    this.nextPolygonPosition = newPolygonPosition;
                    break;//Segment is completed so end the loop
                }
            }
        }

        this.parentTransform.GetComponent<PolygonCollider2D>().points = blockPolygonPoints.ToArray();
        this.parentTransform.GetComponent<PolygonCollider2D>().offset = this.parentTransform.localPosition * -1;
        blockSpritePiece.GetComponent<CollapsableBlockController>().blockType = CollapsableBlockType.RemoveCollisionWhenActivated;
    }

    /// <summary>
    /// Calculate the iterative value for each sprite based on the block size and the sprite in question
    /// <param name="sprite">The sprite in question</param>
    /// </summary> 
    private float GetIterativeValue(Sprite sprite) => this.individualPieceSize / sprite.pixelsPerUnit;
    /// <summary>
    /// Generates a texture object from a sprite
    /// <param name="sprite">The sprite in question</param>
    /// </summary>  
    public Texture2D GenerateTextureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);

            newText.SetPixels(newColors);
            newText.Apply();

            return newText;
        }
        else
        {
            return sprite.texture;
        }
    }

    /// <summary>
    /// Checks if a texture component if fully transparent by verifying if all its pixels are ALPHA
    /// <param name="texture">The sprite texture to check for transparency within</param>
    /// <param name="multiplierX">The horizontal multipier used to calculate the texture offset </param>
    /// <param name="multiplierY">The vertical multipier used to calculate the texture offset</param>
    /// </summary>  
    private bool IsTransparent(Texture2D texture, float multiplierX, float multiplierY)
    {
        Vector2 textureGridOffset = new Vector2(this.individualPieceSize * multiplierX, this.individualPieceSize * multiplierY);
        bool hasColor = false;

        for (int x = 0; x < this.individualPieceSize; x++)
        {
            for (int y = 0; y < this.individualPieceSize; y++)
            {
                if (texture.GetPixel((int)(x + textureGridOffset.x), (int)(y + textureGridOffset.y)).a != 0)
                {
                    hasColor = true;

                    continue;
                }
            }
        }

        return !hasColor;
    }

    /// <summary>
    /// Saves and returns a reference of the sprite object created from splitting the texture
    /// <param name="sprite">The sprite in question</param>
    /// <param name="path">The path to save for the file</param>
    /// <param name="id">The id of the sprite piece for naming conventions and easy referencing</param>
    /// </summary> 
    private Sprite SaveSpriteToEditorPath(Sprite sprite, string path, int id)
    {

        if (!Application.isEditor || Application.isPlaying)
        {
            return sprite;
        }

        return sprite;

        try
        {
            string dirPath = Application.dataPath + "/" + path;//This path also includes the drive
            Debug.Log(dirPath);
            Texture2D spriteTexture = this.GenerateTextureFromSprite(sprite);
            byte[] textureBytes = spriteTexture.EncodeToPNG();

            if (!Directory.Exists(dirPath))//Creates a new directory if required
            {
                Directory.CreateDirectory(dirPath);
            }
            string fileName = this.spriteRenderer.sprite.name + "_Split_" + id;
            string dirFullPathToFile = dirPath + "/" + fileName;
            string dirAssetPathToFile = "Assets/" + path + "/" + fileName + ".png";
            File.WriteAllBytes(dirFullPathToFile + ".png", textureBytes);
            AssetDatabase.Refresh();
            AssetDatabase.AddObjectToAsset(sprite, dirPath);
            AssetDatabase.SaveAssets();
            TextureImporter ti = AssetImporter.GetAtPath(dirAssetPathToFile) as TextureImporter;

            ti.spritePixelsPerUnit = sprite.pixelsPerUnit;
            ti.mipmapEnabled = false;
            ti.filterMode = FilterMode.Point;
            General.SetDirty(ti);
            ti.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath(dirAssetPathToFile, typeof(Sprite)) as Sprite;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return sprite;
    }

    private void OnDrawGizmos()
    {
        //Debug.Log(AssetImporter.GetAtPath(debugPath));
        SpriteRenderer blockSprite = this.GetComponent<SpriteRenderer>();
        float iterator = this.GetIterativeValue(blockSprite.sprite);
        //Start from the left side
        Vector2 startPosition = new Vector2(blockSprite.bounds.min.x, blockSprite.bounds.min.y);
        Vector2 lowerLeftBounds = startPosition;
        Vector2 lowerRightBounds = new Vector2(startPosition.x + iterator, startPosition.y);
        Vector2 upperLeftBounds = new Vector2(startPosition.x, startPosition.y + iterator);
        Vector2 upperRightBounds = new Vector2(startPosition.x + iterator, startPosition.y + iterator);

        Debug.DrawLine(lowerLeftBounds, lowerRightBounds, Color.red);
        Debug.DrawLine(lowerLeftBounds, upperLeftBounds, Color.red);
        Debug.DrawLine(upperLeftBounds, upperRightBounds, Color.red);
        Debug.DrawLine(upperRightBounds, lowerRightBounds, Color.red);
    }

#endif
}
