using UnityEditor;
using UnityEngine;

/// <summary>
///Handles the general water level for the entire stage
/// </summary>
public class GMWaterLevelManager : MonoBehaviour
{
    [LayerList, SerializeField]
    private int waterLevelLayer = 28;
    [SerializeField]
    [Tooltip("Whether the current stage has a water level")]
    private bool currentActIsWaterLevel = false;

    [SerializeField, FirstFoldOutItem("Water Movement")]
    [Tooltip("The target water level which moves the current water level at run time on its change")]
    private float targetWaterLevelHeight;
    [SerializeField, LastFoldoutItem()]
    [Tooltip("How fast the water level moves when following its target")]
    private float waterMovementSpeed = 1;

    [SerializeField, FirstFoldOutItem("Water Level Info")]
    [Tooltip("A slider used to adjust the water level before play")]
    public Transform waterLevelSlider;
    [SerializeField, LastFoldoutItem]
    [Tooltip("The boundaries of the current water level based off the cameras limits")]
    public CameraBounds waterLevelBounds;

    [SerializeField, FirstFoldOutItem("Water Object/Prefab")]
    [Tooltip("The water object to place on the stage")]
    private GameObject waterPrefab;
    [SerializeField, LastFoldoutItem()]
    [Tooltip("The active water object instantiated from the water prefab")]
    private GameObject currentActWater;

    [Tooltip("The debug color for the waterLevel")]
    public Color debugColor = General.RGBToColour(0, 102, 255);

    /// <summary>
    /// The single instance of the Water level manager
    /// </summary>
    private static GMWaterLevelManager instance;
    private void Awake()
    {
        if (this.currentActIsWaterLevel == false)
        {
            return;
        }

        this.currentActWater = Instantiate(this.waterPrefab);
        this.currentActWater.SetActive(false);
        this.currentActWater.transform.parent = this.transform;
        this.targetWaterLevelHeight = this.waterLevelSlider.transform.position.y;//Default the target water level to the waterLevel Slider Position
        this.waterLevelBounds.topBounds = this.waterLevelSlider.transform.position.y;
        this.waterLevelBounds.bottomBounds = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().bottomBounds;
        this.waterLevelBounds.leftBounds = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().leftBounds;
        this.waterLevelBounds.rightBounds = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().rightBounds;
        this.UpdateWaterLevel();
        this.UpdateWaterBounds();
        this.currentActWater.SetActive(true);
        instance = this;
    }

    /// <summary>
    /// Get a reference to the static instance of the water level manager
    /// </summary>
    public static GMWaterLevelManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMWaterLevelManager>();
        }

        return instance;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (this.currentActIsWaterLevel == false)
        {
            return;
        }

        this.UpdateWaterLevel();
        this.UpdateWaterBounds();
    }

    /// <summary>
    /// Get the water level layer of the stage
    /// </summary>
    public int GetWaterLevelLayer() => this.waterLevelLayer;

    /// <summary>
    /// Update the water level of the sage if its not at the target
    /// </summary>
    private void UpdateWaterLevel()
    {
        if (this.waterLevelBounds.GetTopBorderPosition() != this.targetWaterLevelHeight)
        {
            float targetY = this.waterLevelBounds.GetTopBorderPosition();
            targetY = Mathf.MoveTowards(this.waterLevelBounds.GetTopBorderPosition(), this.targetWaterLevelHeight, this.waterMovementSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
            this.waterLevelBounds.topBounds = targetY;
        }
    }

    /// <summary>
    /// Update the water boundaries of the water level
    /// </summary>
    private void UpdateWaterBounds()
    {
        Vector2 position = new Vector2(this.waterLevelBounds.GetRightBorderPosition() + this.waterLevelBounds.GetLeftBorderPosition(), this.waterLevelBounds.GetTopBorderPosition() + this.waterLevelBounds.GetBottomBorderPosition());
        position *= 0.5f;
        GameObject waterPool = this.currentActWater.GetComponentInChildren<WaterController>().gameObject;
        waterPool.transform.localPosition = position;
        waterPool.transform.localScale = new Vector2(this.waterLevelBounds.GetRightBorderPosition() - this.waterLevelBounds.GetLeftBorderPosition(), this.waterLevelBounds.GetTopBorderPosition() - this.waterLevelBounds.GetBottomBorderPosition());
    }

    /// <summary>
    /// Updates the water target level
    /// <param name="currentTargetWaterLevelHeight">The new position of the  water level </param>
    /// </summary>
    public void SetTargetWaterLevelHeight(float currentTargetWaterLevelHeight)
    {
        if (currentTargetWaterLevelHeight >= this.waterLevelBounds.GetBottomBorderPosition())
        {
            this.targetWaterLevelHeight = currentTargetWaterLevelHeight;
        }
        else
        {
            this.targetWaterLevelHeight = this.waterLevelBounds.GetBottomBorderPosition();
        }
    }

    /// <summary>
    /// Get the height of the water level
    /// </summary>
    public float GetWaterLevelHeight() => this.waterLevelSlider.transform.position.y;

    /// <summary>
    /// Checks if the object in question is below the water level
    /// <param name="objectPosition">The objects position to comapre with</param>
    /// </summary>
    public bool UnderWaterLevel(Vector2 objectPosition) => objectPosition.y <= this.waterLevelSlider.transform.position.y;

    /// <summary>
    /// Checks if the object in question is above the water level
    /// <param name="objectPosition">The objects position to comapre with</param>
    /// </summary>
    public bool OverWaterLevel(Vector2 objectPosition) => objectPosition.y >= this.waterLevelSlider.transform.position.y;

    /// <summary>
    /// Spawn a splash at the point of the object
    /// <param name="object">The objects to spawn the splash at<param>
    /// </summary>
    public void SpawnPlayerSplash(Transform @object)
    {
        Vector2 splashPosition = new Vector2(@object.position.x, Instance().GetWaterLevelHeight());
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.WaterSplash, splashPosition);
    }

    /// <summary>
    /// Get whether the current act is a water level
    /// </summary>
    public bool GetCurrentActIsWaterLevel() => this.currentActIsWaterLevel;

    private void OnValidate() => this.waterLevelSlider.gameObject.SetActive(this.currentActIsWaterLevel);

    private void OnDrawGizmos()
    {
        if (this.currentActIsWaterLevel)
        {
#if UNITY_EDITOR
            SceneView sceneView = SceneView.currentDrawingSceneView;
            Vector3 position = this.waterLevelSlider.transform.position;
            if (sceneView != null)
            {
                position.x = sceneView.pivot.x;
            }
            position.y = Application.isPlaying ? this.waterLevelBounds.GetTopBorderPosition() : this.waterLevelSlider.transform.position.y;
            this.waterLevelSlider.transform.position = position;//Ensures we can always move the handle at the center of the scene view

            Gizmos.color = this.debugColor;
            HedgehogCamera hedgehogCamera = HedgehogCamera.Instance();

            if (Application.isPlaying == false)
            {
                this.waterLevelBounds.topBounds = this.waterLevelSlider.transform.position.y;
                this.waterLevelBounds.bottomBounds = hedgehogCamera.GetCameraBoundsHandler().GetActBounds().bottomBounds;
                this.waterLevelBounds.leftBounds = hedgehogCamera.GetCameraBoundsHandler().GetActBounds().leftBounds;
                this.waterLevelBounds.rightBounds = hedgehogCamera.GetCameraBoundsHandler().GetActBounds().rightBounds;
            }

            Gizmos.DrawLine(this.waterLevelBounds.GetTopLeftBorderPosition(), this.waterLevelBounds.GetTopRightBorderPosition());//Top Horizontal Bounds
            Gizmos.DrawLine(this.waterLevelBounds.GetBottomLeftBorderPosition(), this.waterLevelBounds.GetBottomRightBorderPosition());//Bottom Horizontal Bounds
            Gizmos.DrawLine(this.waterLevelBounds.GetTopLeftBorderPosition(), this.waterLevelBounds.GetBottomLeftBorderPosition());//Left Vertical Bounds
            Gizmos.DrawLine(this.waterLevelBounds.GetTopRightBorderPosition(), this.waterLevelBounds.GetBottomRightBorderPosition());//Right Vertical Bounds
            GizmosExtra.Draw2DArrow(this.waterLevelBounds.GetTopLeftBorderPosition(), 90, 0, 64);
            GizmosExtra.Draw2DArrow(this.waterLevelBounds.GetTopRightBorderPosition(), 270, 0, 64);
#endif

        }
    }
}
