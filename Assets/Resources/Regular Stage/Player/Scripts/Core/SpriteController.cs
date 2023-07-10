using UnityEngine;
/// <summary>
/// Handles the effects placed on the plaeyrs sprite such as flipping
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField]
    private FXPaletteController fXPaletteController;
    [SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("The parent object for all add ons")]
    private Transform addOnHolder;
    [SerializeField, Tooltip("The current hedgeAddOnController attached to the object")]
    private HedgeSpriteAddOnController spriteAddOnController;
    [Tooltip("The current material of the sprite"), SerializeField]
    private Material spriteMaterial;
    [Tooltip("The current angle of the Sprite object"), SerializeField]
    private float spriteAngle = 0;
    [Tooltip("The speed in which the player rotates back towards 0"), SerializeField]
    private float revertRotationSpeed = 168.75f;

    private void Reset()
    {
        this.player = this.GetComponentInParent<Player>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.fXPaletteController = this.GetComponent<FXPaletteController>();
    }

    private void Start()
    {
        if (this.player == null || this.spriteRenderer == null)
        {
            this.Reset();
        }

        this.CheckForAddOns();
        this.SetSpriteMaterial(this.spriteRenderer.material);
    }

    /// <summary>
    /// Update the angle of the sprite based on a set of conditions
    /// <param name="angleInDegrees">The current angle In Degrees</param>
    /// </summary>
    public void UpdatePlayerSpriteAngle(float angleInDegrees)
    {
        if (this.player.GetGimmickManager().GetActiveGimmickMode() is GimmickMode.OnCorkscrew or GimmickMode.Sliding)
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, this.spriteAngle);//Update the sprite rotation
            this.player.GetHedgePowerUpManager().effectPivotPoint.rotation = Quaternion.Euler(0f, 0f, 0f);
            return;
        }

        ActionManager actionManager = this.player.GetActionManager();
        bool playerIsRolling = actionManager.CheckActionIsBeingPerformed<Roll>() || actionManager.CheckActionIsBeingPerformed<Jump>();

        if (playerIsRolling)
        {
            this.spriteAngle = 0;
        }

        if (this.player.GetGrounded())
        {
            float smoothAngleCondition1 = Mathf.Round(Mathf.Abs(((angleInDegrees - this.spriteAngle + 540) % 360) - 180));
            float smoothAngleCondition2 = Mathf.Round(Mathf.Abs(((0 - angleInDegrees + 540) % 360) - 180));

            if (smoothAngleCondition1 < 60 && smoothAngleCondition2 >= 45)
            {
                this.spriteAngle = (this.spriteAngle % 360) + ((((angleInDegrees - this.spriteAngle + 540) % 360) - 180) * Mathf.Max(0.165f, Mathf.Abs(this.player.velocity.x) / this.player.GetPlayerPhysicsInfo().recommendedVelocityLimit * 0.8f));
            }
            else if (smoothAngleCondition1 < 60 && smoothAngleCondition2 < 45)
            {
                this.spriteAngle = (this.spriteAngle % 360) + ((((0 - this.spriteAngle + 540) % 360) - 180) * Mathf.Max(0.165f, Mathf.Abs(this.player.velocity.x) / this.player.GetPlayerPhysicsInfo().recommendedVelocityLimit * 0.8f));
            }
            else if (smoothAngleCondition1 >= 60)
            {
                this.spriteAngle = angleInDegrees % 360;
            }

            //Stop the angle from being negative
            if (this.spriteAngle < 0)
            {
                this.spriteAngle += 360;
            }
        }
        else
        {
            if (this.spriteAngle < 180)
            {
                this.spriteAngle = Mathf.MoveTowards(this.spriteAngle, 0, this.revertRotationSpeed * Time.deltaTime);//If player is rotated rotate the player back to zero
            }
            else
            {
                this.spriteAngle = Mathf.MoveTowards(this.spriteAngle, 360, this.revertRotationSpeed * Time.deltaTime);//If player is rotated rotate the player back to zero
            }
        }

        this.transform.rotation = Quaternion.Euler(0f, 0f, this.spriteAngle);//Update the sprite rotation
        this.spriteRenderer.color = this.player.GetSpriteController().spriteRenderer.color;
        this.player.GetHedgePowerUpManager().effectPivotPoint.rotation = Quaternion.Euler(0f, 0f, 0f);
        this.CallSpriteAddOnLifeCycle();
    }

    /// <summary>
    /// Get the active player sprite
    /// </summary>
    public SpriteRenderer GetSpriteRenderer() => this.spriteRenderer;
    /// <summary>
    /// Flip the players sprite based on the direction input with left = -1 and right = 1
    /// <param name="direction">The new direction of the player</param>
    /// </summary>
    public void SetSpriteDirection(int direction)
    {
        if (direction == 0)
        {
            direction = 1;
        }

        this.transform.localScale = new Vector2(1 * direction, 1);
    }

    /// <summary>
    /// Get the current direction the sprite is facing assuming the sprite is not rotated
    /// </summary>
    public int GetSpriteDirection() => (int)Mathf.Sign(this.transform.localScale.x);
    /// <summary>
    /// Gets the current sprite angle
    /// </summary
    public float GetSpriteAngle() => this.spriteAngle;
    /// <summary>
    /// Sets the sprite angle
    /// </summary
    public void SetSpriteAngle(float spriteAngle) => this.spriteAngle = spriteAngle;
    /// <summary>
    /// Sets the sprite transparency
    /// </summary
    public float GetTransparency() => this.spriteRenderer.color.a;
    /// <summary>
    /// Sets the sprite transparency
    /// </summary
    public void SetTransparency(int transparency) => this.spriteRenderer.color = new Color(this.spriteRenderer.color.r, this.spriteRenderer.color.g, this.spriteRenderer.color.b, transparency);
    /// <summary>
    /// Gets the current sprite material
    /// </summary
    public Material GetSpriteMaterial() => this.spriteMaterial;
    /// <summary>
    /// Sets the current sprite material
    /// </summary
    public void SetSpriteMaterial(Material spriteMaterial)
    {
        this.spriteMaterial = spriteMaterial;
        this.fXPaletteController.SetSpriteMaterial(spriteMaterial);

        if (this.GetSpriteAddOnController() != null)
        {
            this.GetSpriteAddOnController().SetSpriteMaterial(spriteMaterial);
        }
    }

    /// <summary>
    /// Swaps the players sprite controller data with that of the new object
    /// <param name="targetSpriteController"> The sprite controller data to swap to </param>
    /// </summary>
    public void SwapSpriteControllerData(SpriteController targetSpriteController)
    {
        this.SetSpriteMaterial(targetSpriteController.GetSpriteRenderer().material);
        this.GetSpriteRenderer().material = this.spriteMaterial;//Swap the material
    }

    /// <summary>
    /// Sets the current sprite add on controller of the sprite
    /// <param name="spriteAddOnController"> The sprite add on controller to set </param>
    /// </summary>
    public void SetSpriteAddOnController(HedgeSpriteAddOnController spriteAddOnController) => this.spriteAddOnController = spriteAddOnController;
    /// <summary>
    /// GEts the current add on controller attached to the player
    /// </summary
    public HedgeSpriteAddOnController GetSpriteAddOnController() => this.spriteAddOnController;
    /// <summary>
    /// Checks if any add ons need to applied to the player like Tails'Tails
    /// </summary
    public void CheckForAddOns()
    {
        if (this.player.GetPlayerPhysicsInfo().character == PlayableCharacter.Tails)
        {
            if (this.GetSpriteAddOnController() == null)
            {
                this.addOnHolder.gameObject.SetActive(true);
                SAOTailsController saoTailsController = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.TailsTails, this.transform.position).GetComponent<SAOTailsController>();
                saoTailsController.transform.parent = this.addOnHolder;
                saoTailsController.transform.localPosition = Vector3.zero;
                saoTailsController.SetPlayer(this.player);
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
    /// Runs the update cycle of add-ons where available
    /// </summary
    public void CallSpriteAddOnLifeCycle()
    {
        if (this.spriteAddOnController != null)
        {
            this.spriteAddOnController.UpdateAddOnInfo();
        }
    }
}
