using UnityEngine;
/// <summary>
/// Provides basic information about a add on object like Tails'Tails
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class HedgeSpriteAddOnController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    protected Player player;
    [SerializeField]
    protected Animator animator;
    [SerializeField, LastFoldoutItem()]
    protected SpriteRenderer spriteRenderer;
    [Tooltip("The current angle of the Sprite object"), SerializeField]
    protected float spriteAngle = 0;
    private int substateHash;

    public virtual void Reset()
    {
        this.player = this.GetComponentInParent<Player>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        if (this.player != null)
        {
            this.player.GetSpriteController().SetSpriteAddOnController(this);
        }

        this.substateHash = Animator.StringToHash("Substate");
    }

    /// <summary>
    /// Updates the basic info of the add on
    /// </summary
    public virtual void UpdateAddOnInfo() => this.transform.rotation = Quaternion.Euler(0f, 0f, this.spriteAngle);//Update the tails sprite add on controller
    /// <summary>
    /// Sets the player object the add on is attached too
    /// </summary
    public void SetPlayer(Player player) => this.player = player;
    /// <summary>
    /// Gets a reference to the player object
    /// </summary>
    public Player GetPlayer() => this.player;
    /// <summary>
    /// Gets a reference to the animator
    /// </summary>
    public Animator GetAnimator() => this.animator;
    /// <summary>
    /// Updates the main state of the animator
    /// <param name="newState">The new substate to branc to </param>
    /// </summary>
    public void SwitchSubstate(int newState) => this.animator.SetInteger(this.substateHash, newState);
    /// <summary>
    /// Updates the visibility of the add on
    /// <param name="value">The new value of the add on </param>
    /// </summary>
    public void SetAddOnVisibility(bool value) => this.spriteRenderer.enabled = value;

    /// <summary>
    /// Gets the local current add-on sprite angle
    /// </summary
    public float GetAddOnSpriteAngle() => this.spriteAngle;
    /// <summary>
    /// Sets the local add-on sprite angle
    /// </summary
    public void SetAddOnSpriteAngle(float spriteAngle) => this.spriteAngle = spriteAngle;
    /// <summary>
    /// Gets the sprite renderer of the add on
    /// </summary
    public SpriteRenderer GetSpriteRenderer() => this.spriteRenderer;
    /// <summary>
    /// Sets the material for the SAO add on 
    /// <param name="material">the new material for the add on</param>
    /// </summary>
    public void SetSpriteMaterial(Material material) => this.GetSpriteRenderer().material = material;
}
