using UnityEngine;
/// <summary>
/// Handles interactions with player sprites in special stages
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class SpecialStageSpriteController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private SpecialStagePlayer player;
    [SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("The current hedgeAddOnController attached to the object")]
    private HedgeSpecialStageSpriteAddOnController spriteAddOnController;
    [SerializeField, Tooltip("The parent object for all add ons")]
    private Transform addOnHolder;

    private void Reset()
    {
        this.player = this.GetComponentInParent<SpecialStagePlayer>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (this.player == null || this.spriteRenderer == null)
        {
            this.Reset();
        }

        this.CheckForAddOns();
    }

    /// <summary>
    /// Actions the sprite controller performs every update
    /// </summary
    public void UpdatePlayerSpriteLifeCycle()
    {
        if (this.spriteAddOnController != null)
        {
            this.spriteAddOnController.UpdateAddOnInfo();
        }
    }

    /// <summary>
    /// Checks if any add ons need to applied to the player like Tails'Tails
    /// </summary
    public void CheckForAddOns()
    {
        if (GMCharacterManager.Instance().currentCharacter == PlayableCharacter.Tails)
        {
            if (this.GetSpriteAddOnController() == null)
            {
                this.addOnHolder.gameObject.SetActive(true);
                SAOSpecialStageTailsController saoTailsController = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.SpecialStageTailsTails, this.transform.position).GetComponent<SAOSpecialStageTailsController>();
                saoTailsController.transform.parent = this.addOnHolder;
                saoTailsController.transform.localPosition = Vector3.zero;
                saoTailsController.Start();
                saoTailsController.SetSpriteMaterial(this.GetSpriteRenderer().material);
                return;
            }
        }
        else
        {
            this.addOnHolder.gameObject.SetActive(false);//no add ons so deactivate it
            this.spriteAddOnController = null;
        }
    }

    /// <summary>
    /// Get the active player sprite
    /// </summary>
    public SpriteRenderer GetSpriteRenderer() => this.spriteRenderer;
    /// <summary>
    /// Sets the current sprite add on controller of the sprite
    /// <param name="spriteAddOnController"> The sprite add on controller to set </param>
    /// </summary>
    public void SetSpriteAddOnController(HedgeSpecialStageSpriteAddOnController spriteAddOnController) => this.spriteAddOnController = spriteAddOnController;
    /// <summary>
    /// GEts the current add on controller attached to the player
    /// </summary
    public HedgeSpecialStageSpriteAddOnController GetSpriteAddOnController() => this.spriteAddOnController;
    /// <summary>
    /// Sets the sprite transparency
    /// </summary
    public void SetTransparency(int transparency) => this.spriteRenderer.color = new Color(this.spriteRenderer.color.r, this.spriteRenderer.color.g, this.spriteRenderer.color.b, transparency);

    /// <summary>
    /// Sets the sprite transparency
    /// </summary
    public float GetTransparency() => this.spriteRenderer.color.a;
}
