using UnityEngine;
/// <summary>
/// Holds general information about the after image for easier access
/// </summary>
public class AfterImageData : MonoBehaviour
{
    [SerializeField]
    private Vector2 position;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private SpriteRenderer addOnSpriteRenderer;
    [SerializeField]
    private float positionSetFrameCount = 0;

    // Start is called before the first frame update
    private void Awake()
    {
        if (this.spriteRenderer == null)
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        if (this.addOnSpriteRenderer == null)
        {
            this.addOnSpriteRenderer = this.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void FixedUpdate() => this.transform.position = this.position;

    /// <summary>
    /// Gets the current sprite renderer object
    /// </summary>
    public SpriteRenderer GetSpriteRenderer()
    {
        if (this.spriteRenderer == null)
        {
            Debug.LogError("Sprite Render not set! for " + this.gameObject.name);
            return null;
        }

        return this.spriteRenderer;
    }

    /// <summary>
    /// Set the sprte for the add on sprite renderer
    /// </summary>
    public SpriteRenderer GetAddOnSpriteRendererSprite()
    {
        if (this.addOnSpriteRenderer == null)
        {
            Debug.LogError("Add On Sprite Render not set! for " + this.gameObject.name);
            return null;
        }

        return this.addOnSpriteRenderer;
    }

    /// <summary>
    /// Set the current position of the after image sprite
    /// </summary>
    public void SetPosition(Vector2 position)
    {
        this.transform.position = position;
        this.position = position;
        this.positionSetFrameCount = Time.frameCount;
    }

    /// <summary>
    /// Get the current true position of the after image sprite
    /// </summary>
    public Vector2 GetPosition() => this.position;


    /// <summary>
    /// Get the frame in which it was set
    /// </summary>
    public float GetPositionSetFrameCount() => this.positionSetFrameCount;

}
