using UnityEngine;

///<summary>
/// The movement of the individual swing links
/// Author - LARK SS orignal by DW & Damizean [Sonic Worlds]  
/// Changes - Converted the code to work for spinning spike
///</summary>

public class Pseudo3DBallLinkController : MonoBehaviour
{
    [FirstFoldOutItem("Dependencies"), SerializeField]
    private SpriteRenderer spriteRenderer;
    [LastFoldoutItem, SerializeField]
    private Pseudo3DBallController pseudo3DBallController;
    [Tooltip("The current link ID of the swing which affects its distance and is based on its distance from the swing controller")]
    public int linkID = 0;
    [Tooltip("The distance of the current swing link from the parent")]
    public float distance = 0;
    // Start is called before the first frame update
    private void Reset()
    {
        this.pseudo3DBallController = this.GetComponentInParent<Pseudo3DBallController>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    public void Start()
    {
        if (this.spriteRenderer == null)
        {
            this.Reset();
        }

        this.distance = this.linkID * this.pseudo3DBallController.linkItemHeight;
    }

    private void FixedUpdate()
    {
        this.UpdateMovement();
    }

    /// <summary>
    /// Update the position of the links
    /// </summary>
    public void UpdateMovement()
    {
        this.UpdateScale();
        this.LinkMovement();
        this.ToggleSortingOrder();
    }

    /// <summary>
    /// Update the scale of the link relative to the angle of degree
    /// </summary>
    private void UpdateScale()
    {
        float degrees = Mathf.Sin(this.pseudo3DBallController.delta * Mathf.Deg2Rad);
        this.transform.localScale = new Vector3(1 + (degrees / 4), 1 + (degrees / 4), 1);
        this.spriteRenderer.color = new Color(1 + (degrees / 2), 1 + (degrees / 2), 1 + (degrees / 2));
    }

    /// <summary>
    /// Move the links based on the distance to the top point
    /// </summary>
    private void LinkMovement()
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
    /// Toggle the sorting order
    /// </summary>
    private void ToggleSortingOrder()
    {
        if (this.pseudo3DBallController.ballDepth == BackgroundDepth.Background && this.spriteRenderer.sortingOrder != -1)
        {
            this.spriteRenderer.sortingOrder = -1;
            this.spriteRenderer.sortingLayerName = "Background Layer 1";
        }
        else if (this.pseudo3DBallController.ballDepth == BackgroundDepth.Foreground && this.spriteRenderer.sortingOrder != 2)
        {
            this.spriteRenderer.sortingOrder = 2;
            this.spriteRenderer.sortingLayerName = "Foreground Layer 1";
        }
    }

}
