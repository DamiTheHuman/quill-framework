using UnityEngine;
/// <summary>
/// Parallax's a background in relation to the movement of the camera
/// Author - Referenced from Sonic Worlds thanks to the Sonic Worlds Team
/// Changes - Added some more responsiveness regarding to pivot points of the parallax
/// </summary>
[ExecuteInEditMode]
public class ParallaxController : MonoBehaviour
{
    [Tooltip("A check to see if the size of the background should be auto calculated"), Button(nameof(CalculateParallaxSize)), SerializeField]
    private bool autoCalculateSize = true;
    [Button(nameof(CreateBackgroundClone)), SerializeField]
    private bool createBackroundClone;

    [SerializeField, Tooltip("Don't take the child objects into account")]
    private bool ignoreChildrenInWidthCalculation;

    [SerializeField]
    private TileDirection tileDirection = TileDirection.Horizontal;
    [Tooltip("The left most point of the zone at the start")]
    private float currentLeftZoneBorder;
    [Tooltip("The bottom most point of the zone at the start")]
    private float currentBottomZoneBorder;

    [Tooltip("The current background or set of backgrounds to be parallaxed")]
    public Transform background;
    [SerializeField, Tooltip("The clone of the parallax used to allow seemless scrolling typically only 1 clone is needed")]
    private Transform backgroundClone;
    [Tooltip("The size of the background which is automatically calculated when appropriate")]
    public Vector2 parallaxSize = new Vector2(688, 249);
    [Help("Set the parallax Factor value to 1 on either axis for a constant follow on the specfied axis")]
    public Vector2 parallaxFactor = new Vector2(0.95f, 0.8f);

    // Start is called before the first frame update
    private void Start()
    {
        if (HedgehogCamera.Instance() == null)
        {
            Debug.Log("Need HedghogCamera Instance for parallax controller");
            this.enabled = false;

            return;
        }

        //We need a clone for the background so when the player moves between backgrounds it loops seemlessly
        if (this.background != null && this.backgroundClone == null)
        {
            this.CreateBackgroundClone();
            this.SetBackgroundToStartPosition(this.background);
        }

        this.currentLeftZoneBorder = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().leftBounds; //This allows the parallax to work even if the player doesnt start at 00
        this.currentBottomZoneBorder = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().bottomBounds;
        HedgehogCamera.Instance().GetCameraParallaxHandler().RegisterBackGround(this);
    }

    public void UpdateParallaxPosition() => this.CalculateParallaxPosition();

    /// <summary>
    /// Creates a clone of the current background which is used for semless tiling
    /// </summary>
    private void CreateBackgroundClone()
    {
        GameObject backgroundClone = Instantiate(this.background.gameObject);

        if (this.tileDirection == TileDirection.Horizontal)
        {
            backgroundClone.transform.position = this.background.transform.position + new Vector3(this.parallaxSize.x, 0);
        }
        else
        {
            backgroundClone.transform.position = this.background.transform.position + new Vector3(0, this.parallaxSize.y);
        }

        backgroundClone.transform.parent = this.transform;
        this.backgroundClone = backgroundClone.transform;
    }

    /// <summary>
    /// Sets the start position of the background
    /// </summary>
    public void SetBackgroundToStartPosition(Transform background)
    {
        //Place the background object at the left most point of the camera
        SpriteRenderer firstSprite = background.transform.GetComponentInChildren<SpriteRenderer>();
        Vector2 amountToMove;
        amountToMove = new Vector2(HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().GetLeftBorderPosition() - firstSprite.bounds.center.x, 0);
        amountToMove.x += firstSprite.size.x * firstSprite.transform.localScale.x / 2;
    }

    /// <summary>
    /// Calculate the position of the parallax in relation to the postiion of the camera
    /// </summary
    private void CalculateParallaxPosition()
    {
        HedgehogCamera hedgehogCamera = Application.isPlaying ? HedgehogCamera.Instance() : Camera.main.GetComponent<HedgehogCamera>();
        Vector2 distance = hedgehogCamera.transform.position - hedgehogCamera.GetStartPosition();
        Vector2 position = this.transform.position;

        if (this.tileDirection == TileDirection.Horizontal)
        {
            float leftCameraBorder = hedgehogCamera.GetBounds().min.x - 0.1825f;
            position.x = (leftCameraBorder + this.currentLeftZoneBorder) * this.parallaxFactor.x;
            position.x = leftCameraBorder + ((Mathf.RoundToInt(position.x - leftCameraBorder) - this.currentLeftZoneBorder) % this.parallaxSize.x);
            position.x += this.parallaxSize.x / 2;
            position.y = HedgehogCamera.Instance().GetBounds().max.y * this.parallaxFactor.y;
        }
        else
        {
            float bottomCameraBounds = hedgehogCamera.GetBounds().min.y - 0.1825f;
            position.y = (bottomCameraBounds + this.currentBottomZoneBorder) * this.parallaxFactor.y;
            position.y = bottomCameraBounds + ((Mathf.RoundToInt(position.y - bottomCameraBounds) - this.currentBottomZoneBorder) % this.parallaxSize.y);
            position.y += this.parallaxSize.y / 2;
            position.x = Mathf.RoundToInt(HedgehogCamera.Instance().GetBounds().max.x * this.parallaxFactor.x);
        }


        this.transform.position = position;
    }

    /// <summary>
    /// Calculate the size of the parallax in reference to the background
    /// </summary>
    private void CalculateParallaxSize()
    {
        if (this.background.transform.childCount == 0 || this.ignoreChildrenInWidthCalculation)
        {
            SpriteRenderer spriteRenderer = this.background.GetComponent<SpriteRenderer>();
            this.parallaxSize = (Vector2)(spriteRenderer.sprite.bounds.max - spriteRenderer.sprite.bounds.min);
        }
        else
        {
            this.parallaxSize = Vector2.zero;
            foreach (SpriteRenderer spriteRenderer in this.background.GetComponentsInChildren<SpriteRenderer>())
            {
                this.parallaxSize += (Vector2)(spriteRenderer.sprite.bounds.max - spriteRenderer.sprite.bounds.min);
            }
        }

        this.background.transform.localPosition = new Vector3(0, this.background.transform.localPosition.y, this.background.transform.localPosition.z);
        this.backgroundClone.transform.localPosition = new Vector3(this.parallaxSize.x, this.backgroundClone.transform.localPosition.y, this.backgroundClone.transform.localPosition.z);
    }
}
