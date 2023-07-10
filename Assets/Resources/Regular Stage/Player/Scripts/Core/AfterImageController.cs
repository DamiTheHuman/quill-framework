using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the display of afterimages
/// </summary>
public class AfterImageController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [FirstFoldOutItem("After Image Data")]
    [Tooltip("The after image prefab to clone"), SerializeField]
    private GameObject afterImagePrefab = null;
    [Tooltip("A flag to determine the display images"), SerializeField]
    private Transform afterImageParent;
    [Tooltip("The number of after images to be made"), SerializeField]
    private float afterImageCount = 3;
    [Tooltip("The list of after images that belong to the player"), SerializeField, LastFoldoutItem()]
    private List<AfterImageData> afterImages = new List<AfterImageData>();

    [Tooltip("The x offset of each sparkle regarding the distance to the target"), SerializeField]
    private float offsetWidth = 2;
    [Tooltip("The current timer"), SerializeField]
    private float personalTimer = 0;
    private float globalTimer = 0;
    [Tooltip("The recent positions of the sprite"), SerializeField]
    private RecentObjectPositions[] recentPositions = new RecentObjectPositions[60];
    private void Reset()
    {
        this.player = this.GetComponentInParent<Player>();
        this.SetAfterImagesPositions(this.player.transform.position);
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        this.PopulateAfterImagePool();
        this.afterImageParent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Runs through the update cycle for after images
    /// </summary>
    public void UpdateAfterImages()
    {
        if (this.player.velocity == Vector2.zero || this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.InTeleporter || this.player.GetActionManager().CheckActionIsBeingPerformed<Pushing>() || this.player.GetActionManager().CheckActionIsBeingPerformed<Climb>())
        {
            this.SetAfterImageVisibility(false);

            return;
        }

        //If the player is super or has power sneakers display the after image
        bool displayAfterImages = this.player.GetHedgePowerUpManager().GetPowerSneakersPowerUp().PowerUpIsActive() || this.player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm;
        this.SetAfterImageVisibility(displayAfterImages);

        if (displayAfterImages)
        {
            this.UpdateAndDisplayAfterImages();
        }
    }

    /// <summary>
    /// Creates a set number of objects that can be used as after images for the player sprite
    /// </summary>
    private void PopulateAfterImagePool()
    {
        Transform afterImagePool = new GameObject().transform; //A parent object to keep the inspector tidy
        afterImagePool.transform.parent = this.transform;
        afterImagePool.name = "After Images";
        this.afterImageParent = afterImagePool;

        for (int x = 0; x < this.afterImageCount; x++)
        {
            AfterImageData afterImage = Instantiate(this.afterImagePrefab).AddComponent<AfterImageData>(); //Create an after image object and add it to the inspector
            Material mat = new Material(Shader.Find("Shader Graphs/Transparent_PaletteSwapShader"));
            afterImage.GetSpriteRenderer().material = mat;
            afterImage.transform.parent = afterImagePool;
            afterImage.name = "After Image " + x;
            this.afterImages.Add(afterImage);
        }
    }

    /// <summary>
    /// Gets the recent positions of the player
    /// </summary>
    public RecentObjectPositions[] GetRecentPositions() => this.recentPositions;

    /// <summary>
    /// Set the visibility of the after image
    /// <param name="visibility">The new visibility value of the after image controller </param>
    /// </summary>
    public void SetAfterImageVisibility(bool visibility)
    {
        if (visibility != this.afterImageParent.gameObject.activeSelf)
        {
            this.afterImageParent.gameObject.SetActive(visibility);
        }
    }

    /// <summary>
    /// Update the recent positions of the timer
    /// <param name="position">The position of the player  </param>
    /// <param name="angle">The angle to set the after image position at </param>
    /// </summary>
    public void UpdateRecentObjectPosition(Vector2 position, float angle)
    {
        this.personalTimer += Time.fixedDeltaTime;
        this.globalTimer = Mathf.Round(this.personalTimer * 60);
        int index = (int)this.globalTimer % 60;
        this.recentPositions[index].position = position;
        this.recentPositions[index].angle = angle;
        //TODO: Hacky fix to ensure no matter what the timer is we have aren't pointing to a recent value
        //THANKS FLOATING POINTS :(
        if (index > 0)
        {
            this.recentPositions[index - 1].position = position;
            this.recentPositions[index - 1].angle = angle;
        }
        else
        {
            this.recentPositions[this.recentPositions.Length - 1].position = position;
            this.recentPositions[this.recentPositions.Length - 1].angle = angle;
        }
        if (index < 59)
        {
            this.recentPositions[index + 1].position = position;
            this.recentPositions[index + 1].angle = angle;
        }
        else
        {
            this.recentPositions[0].position = position;
            this.recentPositions[0].angle = angle;
        }
    }

    /// <summary>
    /// Sets the position of all after image objects
    /// <param name="position">The position of the player  </param>
    /// <param name="angle">The angle to set the after image position at </param>
    /// </summary>
    public void SetAfterImagesPositions(Vector2 position, float angle = 0)
    {
        for (int x = 0; x < this.afterImages.Count; x++)
        {
            this.afterImages[x].SetPosition(position);
        }
        for (int x = 0; x < this.recentPositions.Length; x++)
        {
            this.recentPositions[x].position = position;
            this.recentPositions[x].angle = angle;
        }
    }

    /// <summary>
    /// Get a reference to the players personal global timer
    /// </summary>
    public float GetGlobalTimer() => this.globalTimer;

    /// <summary>
    /// Swaps the after image controller data with that of the active player
    /// <param name="targetAfterImageController">The after image controller to retrieve data from </param>
    /// </summary>
    public void SwapAfterImageControler(AfterImageController targetAfterImageController)
    {
        this.afterImagePrefab = targetAfterImageController.afterImagePrefab;
        this.UpdateAfterImages();
    }

    /// <summary>
    /// Update the position of the main trail renderer position
    /// </summary>
    private void UpdateAndDisplayAfterImages()
    {
        for (int x = 0; x < this.afterImages.Count; x++)
        {
            bool expirePosition = Time.frameCount - this.afterImages[x].GetPositionSetFrameCount() > 30;
            if ((int)((this.globalTimer - (2 * (x + 1))) % 60) >= 0 || expirePosition)
            {
                RecentObjectPositions recentSpritePosition = this.recentPositions[(int)(this.globalTimer - (this.offsetWidth * (x + 1))) % 60];
                Vector2 targetPosition = recentSpritePosition.position;

                if (this.afterImages[x].GetPosition() != targetPosition)
                {
                    this.afterImages[x].SetPosition(targetPosition);
                    this.afterImages[x].transform.eulerAngles = new Vector3(0, 0, recentSpritePosition.angle);
                    this.afterImages[x].GetSpriteRenderer().sprite = this.player.GetSpriteController().GetSpriteRenderer().sprite;

                    if (this.player.GetSpriteController().GetSpriteAddOnController() != null)
                    {
                        this.afterImages[x].GetAddOnSpriteRendererSprite().sprite = this.player.GetSpriteController().GetSpriteAddOnController().GetSpriteRenderer().sprite;
                        this.afterImages[x].GetAddOnSpriteRendererSprite().transform.localEulerAngles = this.player.GetSpriteController().GetSpriteAddOnController().GetSpriteRenderer().transform.localEulerAngles;
                    }
                    else
                    {
                        this.afterImages[x].GetAddOnSpriteRendererSprite().sprite = null;
                    }

                    this.afterImages[x].GetSpriteRenderer().material.CopyPropertiesFromMaterial(this.player.GetSpriteController().GetSpriteRenderer().sharedMaterial);
                }
            }
        }
    }
}
