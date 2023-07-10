using UnityEngine;
///<summary>
/// The movement of the individual end block
/// Author - LARK SS orignal by DW & Damizean [Sonic Worlds]  
/// Changes - Converted the code to work for spinning spike
///</summary>
public class Pseudo3DBallSpikeController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Pseudo3DBallController pseudo3DBallController;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField, LastFoldoutItem()]
    private BoxCollider2D boxCollider2D;

    [Tooltip("The current link ID of the swing which affects its distance and is based on its distance from the swing controller")]
    public int linkID = 0;
    [Tooltip("The distance of the current swing link from the parent")]
    public float distance = 0;

    public Color debugColor = Color.red;

    private void Reset()
    {
        this.pseudo3DBallController = this.GetComponentInParent<Pseudo3DBallController>();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    public void Start()
    {
        if (this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.distance = (this.pseudo3DBallController.length + 1) * this.pseudo3DBallController.linkItemHeight;
    }

    private void FixedUpdate() => this.UpdateMovement();

    /// <summary>
    /// Update the position of the ball spike
    /// </summary>
    public void UpdateMovement()
    {
        this.UpdateScale();
        this.SwingMovement();
        this.ToggleCollider();
    }

    /// <summary>
    /// Update the scale of the gameobject relative to the angle
    /// </summary>
    private void UpdateScale()
    {
        float degrees = Mathf.Sin(this.pseudo3DBallController.delta * Mathf.Deg2Rad);
        this.transform.localScale = new Vector3(1 + (degrees / 4), 1 + (degrees / 4), 1);
        this.spriteRenderer.color = new Color(1 + (degrees / 2), 1 + (degrees / 2), 1 + (degrees / 2));
    }

    /// <summary>
    /// Move the spike ball at the end
    /// </summary>
    private void SwingMovement()
    {
        Vector2 pos = this.transform.position;

        if (this.pseudo3DBallController.spinDirection == Pseudo3DBallSpinDirection.Horizontal)
        {
            pos.x = this.pseudo3DBallController.transform.position.x + (this.distance * Mathf.Cos(this.pseudo3DBallController.delta * Mathf.Deg2Rad));
            pos.y = this.pseudo3DBallController.transform.position.y;
        }
        else if (this.pseudo3DBallController.spinDirection == Pseudo3DBallSpinDirection.Vertical)
        {
            pos.x = this.pseudo3DBallController.transform.position.x;
            pos.y = this.pseudo3DBallController.transform.position.y - (this.distance * Mathf.Cos(this.pseudo3DBallController.delta * Mathf.Deg2Rad));
        }

        this.transform.position = pos;
    }

    /// <summary>
    /// Toggle the activeness of the box collider
    /// </summary>
    private void ToggleCollider()
    {
        if (this.pseudo3DBallController.ballDepth == BackgroundDepth.Background && this.spriteRenderer.sortingOrder != -1)
        {
            this.spriteRenderer.sortingOrder = -1;
            this.spriteRenderer.sortingLayerName = "Background Layer 1";
            this.boxCollider2D.enabled = false;
        }
        else if (this.pseudo3DBallController.ballDepth == BackgroundDepth.Foreground && this.spriteRenderer.sortingOrder != 2)
        {
            this.spriteRenderer.sortingOrder = 2;
            this.spriteRenderer.sortingLayerName = "Foreground Layer 1";
            this.boxCollider2D.enabled = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (this.pseudo3DBallController == null || this.boxCollider2D == null)
        {
            this.Reset();
        }
        if (this.pseudo3DBallController.ballDepth == BackgroundDepth.Foreground)
        {
            GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor, true);
        }
    }
}
