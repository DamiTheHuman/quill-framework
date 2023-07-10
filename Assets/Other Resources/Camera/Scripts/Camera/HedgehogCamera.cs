using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// The main camera object for the game
/// </summary>
public class HedgehogCamera : MonoBehaviour
{
    [SerializeField]
    private PixelPerfectCamera pixelPerfectCamera;
    [Tooltip("The z position used on the camera at all times"), IsDisabled, SerializeField]
    private float cameraZPosition = -100f;
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField]
    private new Camera camera;
    [SerializeField, Tooltip("Handles the look and spindash lag action")]
    private CameraLookHandler cameraLookHandler;
    [SerializeField, Tooltip("Handles the camera interactions with bounds")]
    private CameraBoundsHandler cameraBoundsHandler;
    [SerializeField, Tooltip("Handles the camera interactions with triggers")]
    private CameraTriggersHandler cameraTriggersHandler;
    [SerializeField, Tooltip("Handles the camera parallax handler")]
    private CameraParallaxHandler cameraParallaxHandler;
    [SerializeField, LastFoldoutItem(), Tooltip("Handles iamge displayed on the camera when rendering is to be stopped")]
    private CameraRenderFreezeHandler cameraRenderFreezeHandler;

    [SerializeField]
    private CameraType cameraType = CameraType.Retro;
    [SerializeField]
    private CameraMode cameraMode = CameraMode.FollowTarget;

    [Tooltip("The target for the camera to follow"), SerializeField]
    private GameObject cameraTarget;
    [SerializeField]
    [Tooltip("The current camera offset applied to the camera")]
    private Vector2 cameraOffset = Vector2.zero;

    [FirstFoldOutItem("Landing Readjustment info"), SerializeField]
    [Tooltip("The current ground offset")]
    private float currentGroundOffset;
    [Tooltip("The speed to follow the camera"), LastFoldoutItem, SerializeField]
    private float groundReadjustmentSpeed = 4f;

    [FirstFoldOutItem("Camera Info"), SerializeField]
    [Tooltip("The current camera position")]
    private Vector2 cameraPosition;
    [SerializeField]
    [Tooltip("The position of the target")]
    private Vector2 targetPosition;
    [SerializeField, IsDisabled, Tooltip("Start position of the camera")]
    private Vector2 startPosition;
    [LayerList, Tooltip("The editor only"), SerializeField]
    private int editorOnlyLayer = 31;
    [SerializeField]
    [Tooltip("A flag to inform the camera that there may be an offset when the player regrounds and to take this into account")]
    private bool prepareGroundAdjustement;
    [SerializeField]
    [Tooltip("The distance between the player and the camera"), LastFoldoutItem()]
    private Vector2 cameraToPlayerDistance;

    


    /// <summary>
    /// The single instance of the Camera
    /// </summary>
    private static HedgehogCamera instance;

    private void Reset()
    {
        this.camera = this.GetComponent<Camera>();
        this.cameraRenderFreezeHandler = this.GetComponent<CameraRenderFreezeHandler>();
        this.cameraLookHandler = this.GetComponent<CameraLookHandler>();
        this.cameraBoundsHandler = this.GetComponent<CameraBoundsHandler>();
        this.cameraTriggersHandler = this.GetComponent<CameraTriggersHandler>();
        this.cameraParallaxHandler = this.GetComponent<CameraParallaxHandler>();
    }

    private void Start()
    {
        instance = this;

        if (this.camera == null)
        {
            this.camera = this.GetComponent<Camera>();
        }

        if (this.cameraTarget != null && this.cameraTarget.CompareTag("Player"))
        {
            this.player = this.cameraTarget.GetComponent<Player>();
        }

        if (this.cameraTarget == null && this.cameraType != CameraType.Static)
        {
            Debug.Log("No Target found switching to Static Camera");
            this.cameraType = CameraType.Static;
        }

        this.SetCameraPosition(this.player != null && this.cameraType != CameraType.Static ? (Vector2)this.player.transform.position : this.cameraPosition);

        if (this.player == null && this.cameraTarget == null)
        {
            this.cameraType = CameraType.Static;
        }

        this.UpdateCameraDetails();
        this.FixedUpdate();
        this.startPosition = this.cameraPosition;
    }

    /// <summary>
    /// Get a reference to the static instance of the camera
    /// </summary>
    public static HedgehogCamera Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<HedgehogCamera>();
        }

        return instance;
    }

    private void LateUpdate()
    {
        PixelPerfectMoveCamera();
    }

    private void FixedUpdate()
    {
        if (this.cameraMode == CameraMode.None)
        {
            return;
        }

        this.UpdateCameraDetails();
        this.CalculateCameraMovement();
        this.cameraPosition = this.cameraBoundsHandler.ClampPositionToZoneBounds(this.cameraPosition);
        this.cameraBoundsHandler.UpdateCurrentZoneBottomBorder();
        this.MoveCamera();

        if (this.cameraMode != CameraMode.SpecialStage)
        {
            this.cameraTriggersHandler.CheckCameraTriggersUpdated(this.targetPosition);
        }

#if UNITY_EDITOR
        this.DebugEditorOnlyObects();
#endif
    }

    /// <summary>
    /// Updates the core details that handles camera movement
    /// </summary>y
    private void UpdateCameraDetails()
    {
        this.targetPosition = this.cameraTarget != null ? this.cameraTarget.transform.position : this.cameraPosition;
        this.cameraToPlayerDistance = new Vector2(this.targetPosition.x - this.cameraPosition.x, this.targetPosition.y - this.cameraPosition.y);
    }

    /// <summary>
    /// Calculates the position of the camera relative to the cameras mode and the onscreen target
    /// </summary>y
    private void CalculateCameraMovement()
    {
        switch (this.cameraMode)
        {
            case CameraMode.EndLevel:
                this.EndOfActCameraMovement();

                break;
            case CameraMode.BossMode:
                this.BossFightCameraMovement();

                break;
            default:
                if (this.cameraType == CameraType.Retro)
                {
                    this.RetroCameraMovement();
                }
                else if (this.cameraType == CameraType.ConstantFollow)
                {
                    this.ConstantFollowMovement();
                }

                if (this.player != null)
                {
                    this.cameraOffset = this.cameraLookHandler.HandleLookAction(this.cameraOffset);
                }

                this.cameraPosition += this.cameraOffset;

                break;
        }
    }

    /// <summary>
    /// Camera movement when following retro like movement
    /// </summary>
    private void RetroCameraMovement()
    {
        if (this.cameraMode is CameraMode.FollowTarget or CameraMode.Freeze)
        {
            if (this.player.GetGrounded() || this.cameraOffset.y != 0 || this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.OnHandle)
            {
                this.HandleGroundedVerticalCameraMovement();
            }
        }

        if (this.cameraMode == CameraMode.FollowTarget)
        {
            this.HandleHorizontalCameraMovement();

            if (this.player.GetGrounded() == false && this.cameraOffset.y == 0)
            {
                this.HandleAirVerticalCameraMovement();
            }
        }
    }

    /// <summary>
    /// Camera movement when in the constant follow mode
    /// </summary>
    private void ConstantFollowMovement() => this.cameraPosition = this.targetPosition;

    /// <summary>
    /// Scrolls the camera towards to the end level position on end
    /// </summary>
    private void EndOfActCameraMovement() => this.cameraPosition = Vector2.MoveTowards(this.cameraPosition, GMStageManager.Instance().GetActClearGimmick().GetStartPosition(), this.GetCameraLookHandler().GetScrollActionLookSpeed() * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

    /// <summary>
    /// Movement of the camera in a boss fight
    /// </summary>
    private void BossFightCameraMovement() => this.cameraPosition = Vector2.MoveTowards(this.cameraPosition, GMStageManager.Instance().GetBoss().GetBossTrigger().GetBossFightBounds().GetCenterPosition(), this.GetCameraLookHandler().GetScrollActionLookSpeed() * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

    /// <summary>
    /// Updates the position of the gameobject in respect to the positions calculated
    /// </summary>
    private void MoveCamera()
    {
        this.SetCameraPosition(this.cameraPosition);
        this.cameraParallaxHandler.UpdateParallaxBackgroundPositions();
    }

    /// <summary>
    /// Updates the position of the camera in relation to pixel perfect to reduce jitter
    /// </summary>
    private void PixelPerfectMoveCamera()
    {
        if (this.pixelPerfectCamera == null || this.pixelPerfectCamera.enabled == false)
        {
            return;
        }

        this.transform.position = pixelPerfectCamera.RoundToPixel(this.transform.position);
    }

    /// <summary>
    /// Handle the way the camera horizontally at all times
    /// </summary>
    private void HandleHorizontalCameraMovement()
    {
        float shiftAmount = 0;

        if (this.targetPosition.x < this.cameraBoundsHandler.GetPlayerBounds().GetLeftBorderPosition())
        {
            shiftAmount = this.cameraToPlayerDistance.x + this.cameraBoundsHandler.GetPlayerBounds().leftBounds;
        }
        else if (this.targetPosition.x > this.cameraBoundsHandler.GetPlayerBounds().GetRightBorderPosition())
        {
            shiftAmount = this.cameraToPlayerDistance.x - this.cameraBoundsHandler.GetPlayerBounds().rightBounds;
        }
        if (this.cameraType == CameraType.Retro)
        {
            shiftAmount = Mathf.Clamp(shiftAmount, -this.cameraLookHandler.GetMaxCameraFollowAmount(), this.cameraLookHandler.GetMaxCameraFollowAmount());
        }

        this.cameraPosition.x += shiftAmount;
    }

    /// <summary>
    /// Handles the way the camera moves while the player moves on the ground
    /// </summary>
    private void HandleGroundedVerticalCameraMovement()
    {
        if (this.player.GetGrounded() == false && this.cameraOffset.y == 0)
        {
            return;
        }

        if (this.prepareGroundAdjustement && (this.player.GetGrounded() || this.player.GetGimmickManager().GetActiveGimmickMode() == GimmickMode.OnHandle) && this.cameraOffset.y == 0)
        {
            this.currentGroundOffset = this.transform.position.y - this.player.transform.position.y;//Calculate how far the camera is from the center point of the player
            this.prepareGroundAdjustement = false;
        }

        if (this.currentGroundOffset != 0)
        {
            this.currentGroundOffset = Mathf.MoveTowards(this.currentGroundOffset, 0, this.groundReadjustmentSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime); //Update the ground offset towards 0
            this.cameraPosition.y = this.targetPosition.y + this.currentGroundOffset;//Apply the ground offset to the camera position to bring it back to the player as it reaches 0
        }
        else
        {
            this.cameraPosition.y += this.cameraToPlayerDistance.y;//No ground offset so follow as usually
        }
    }

    /// <summary>
    /// Handles the way the camera moves while the player is in the air
    /// </summary>y
    private void HandleAirVerticalCameraMovement()
    {
        if (this.player.GetGrounded())
        {
            return;
        }

        this.prepareGroundAdjustement = true;//Set a flag to prepare for potential offset to the ground as long as the player isnt hanging
        float shiftAmount = 0;

        if (this.targetPosition.y < this.cameraBoundsHandler.GetPlayerBounds().GetBottomBorderPosition())
        {
            shiftAmount = this.cameraToPlayerDistance.y + this.cameraBoundsHandler.GetPlayerBounds().bottomBounds;
        }
        else if (this.targetPosition.y > this.cameraBoundsHandler.GetPlayerBounds().GetTopBorderPosition())
        {
            shiftAmount = this.cameraToPlayerDistance.y - this.cameraBoundsHandler.GetPlayerBounds().topBounds;
        }

        if (this.cameraType == CameraType.Retro)
        {
            shiftAmount = Mathf.Clamp(shiftAmount, -this.cameraLookHandler.GetMaxCameraFollowAmount(), this.cameraLookHandler.GetMaxCameraFollowAmount());
        }

        this.cameraPosition.y += shiftAmount;
    }

    /// <summary>
    /// Sets the position of the camera to specificed position while maintaing the z axis
    /// <param name="position">The new position of the camera </param>
    /// </summary>
    public void SetCameraPosition(Vector2 position)
    {
        this.cameraPosition = this.cameraBoundsHandler.ClampPositionToZoneBounds(position);

        if (this.cameraMode == CameraMode.SpecialStage)
        {
            Vector3 cameraPosition = this.cameraTarget.transform.position;
            cameraPosition += (Vector3)this.cameraOffset;
            cameraPosition.z += this.cameraZPosition;
            this.transform.position = cameraPosition;

            return;
        }

        this.transform.position = new Vector3(this.cameraPosition.x, this.cameraPosition.y, this.cameraZPosition);
    }

    /// <summary>
    /// Gets the current position of the camera
    /// </summary>
    public Vector2 GetCameraPosition() => this.cameraPosition;

    /// <summary>
    /// Changes the active mode of the camera
    /// <param name="cameraMode">The new mode of the camera </param>
    /// </summary>
    public void SetCameraMode(CameraMode cameraMode)
    {
        if (this.cameraMode == CameraMode.EndLevel && cameraMode != CameraMode.EndLevel)
        {
            this.cameraBoundsHandler.SetActBounds(this.cameraBoundsHandler.GetPreActEndBounds());
        }

        if (cameraMode == CameraMode.EndLevel)
        {
            this.OnEndZoneCameraMode();
        }

        if (this.cameraMode == CameraMode.BossMode && cameraMode != CameraMode.BossMode)
        {
            this.cameraPosition = this.transform.position;
        }

        this.cameraMode = cameraMode;
    }

    /// <summary>
    /// Overload of <see cref="SetCameraMode(CameraMode)"/> that allows a trigger object to be passed for more flexibility
    /// <param name="cameraMode">The new mode of the camera </param>
    /// </summary>
    public void SetCameraMode<T>(CameraMode cameraMode, T trigger = default) => this.SetCameraMode(cameraMode);

    /// <summary>
    /// Getht the camera mode
    /// </summary>
    public CameraMode GetCameraMode() => this.cameraMode;

    /// <summary>
    /// Changes the active camera type
    /// <param name="cameraType">The new type of the camera </param>
    /// </summary>
    public void SetCameraType(CameraType cameraType) => this.cameraType = cameraType;

    /// <summary>
    /// Get the current camera type
    /// </summary>
    public CameraType GetCameraType() => this.cameraType;

    /// <summary>
    /// Set the camera target
    /// </summary>
    public void SetCameraTarget(GameObject cameraTarget) => this.cameraTarget = cameraTarget;

    /// <summary>
    /// Get the camera target
    /// </summary>
    public GameObject GetCameraTarget() => this.cameraTarget;

    /// <summary>
    /// Actions that take place when switched to end zone
    /// This assums the current camera target is set to the end stage object
    /// </summary>
    private void OnEndZoneCameraMode()
    {
        this.targetPosition = this.cameraTarget.transform.position;
        float halfCameraHeight = 2f * this.camera.orthographicSize / 2;
        float halfCameraWidth = halfCameraHeight * this.camera.aspect;
        this.cameraBoundsHandler.OnEndZoneCameraMode();
        this.cameraLookHandler.OnEndZoneCameraMode();
    }

    /// <summary>
    /// Check is the target objects position is below the camera view
    /// <param name="position"/> The position to check against</param>
    /// </summary>
    public bool PositionIsBelowCameraView(Vector2 position) => position.y < this.GetBounds().min.y;

    /// <summary>
    /// Check is the target objects position is past the left bounds of the camera view
    /// <param name="position"/> The position to check against</param>
    /// </summary>
    public bool PositionIsLeftOfCmaeraView(Vector2 position) => position.x < this.GetBounds().min.x;

    /// <summary>
    /// Check is the target object is past the right bounds of the camera view
    /// <param name="position"/> The position to check against</param>
    /// </summary>
    public bool PositionIsRightOfCameraView(Vector2 position) => position.x > this.GetBounds().max.x;

    /// <summary>
    /// Check is the target objects position is above the camera view
    /// <param name="positiontEx"/> The position to check against</param>
    /// </summary>
    public bool PositionIsAboveCameraView(Vector2 positiont) => positiont.y > this.GetBounds().max.y;

    /// <summary>
    /// Calculates the boundaries of the camera view
    /// </summary>
    public Bounds GetBounds()
    {
        if (this.camera == null)
        {
            this.camera = this.GetComponent<Camera>();
        }

        Vector2 cameraExtents = new Vector2(this.camera.orthographicSize * (Screen.width * this.camera.rect.width) / (Screen.height * this.camera.rect.height), this.camera.orthographicSize);
        Bounds cameraBounds = new Bounds
        {
            min = (Vector2)this.camera.transform.position - cameraExtents,
            max = (Vector2)this.camera.transform.position + cameraExtents
        };

        if (this.camera.orthographic)
        {
            return cameraBounds;
        }
        else
        {
            Debug.LogError("Camera is not orthographic!", this.camera);

            return new Bounds();
        }
    }

    /// <summary>
    /// Checks if a sprite render is within the visibility of the camera view
    /// <param name="spriteRenderer"/> The sprite renderer of the object being checked for </param>
    /// </summary/>
    public bool IsSpriteWithinCameraView(SpriteRenderer spriteRenderer)
    {
        Bounds bounds = spriteRenderer.bounds;

        if (spriteRenderer.isVisible)
        {
            return this.AreBoundsWithinCameraView(bounds);
        }

        return false;
    }

    /// <summary>
    /// Checks if the bounds are within the cameras view
    /// <param name="bounds"/> The bounds being checked against </param>
    /// </summary/>
    public bool AreBoundsWithinCameraView(Bounds bounds)
    {
        bool isWithinHorizontalBounds = bounds.max.x > this.GetBounds().min.x && bounds.min.x < this.GetBounds().max.x;
        bool isWithinVerticalBounds = bounds.max.y > this.GetBounds().min.y && bounds.min.y < this.GetBounds().max.y;

        return isWithinHorizontalBounds && isWithinVerticalBounds;
    }

    /// <summary>
    /// Gets the start position of the camera
    /// </summary>
    public Vector3 GetStartPosition() => this.startPosition;

    /// <summary>
    /// Sets the camera
    /// </summary/>
    public void SetCamera(Camera camera) => this.camera = camera;
    /// <summary>
    /// Gets the camera 
    /// </summary/>
    public Camera GetCamera() => this.camera;

    /// <summary>
    /// Gets the camera bounds handler
    /// </summary/>
    public CameraBoundsHandler GetCameraBoundsHandler() => this.cameraBoundsHandler;

    /// <summary>
    /// Gets the camera triggers handler
    /// </summary/>
    public CameraTriggersHandler GetCameraTriggersHandler() => this.cameraTriggersHandler;

    /// <summary>
    /// Gets the camera pause handler
    /// </summary/>
    public CameraRenderFreezeHandler GetCameraRenderFreezeHandler() => this.cameraRenderFreezeHandler;

    /// <summary>
    /// Gets the camera look handler
    /// </summary/>
    public CameraLookHandler GetCameraLookHandler() => this.cameraLookHandler;


    /// <summary>
    /// Gets the camera parllax handler
    /// </summary/>
    public CameraParallaxHandler GetCameraParallaxHandler() => this.cameraParallaxHandler;

#if UNITY_EDITOR
    /// <summary>
    /// Displays editor only game objects only when the gizmos flag is set
    /// </summary>
    private void DebugEditorOnlyObects()
    {
        if (GMSceneManager.Instance().GetGizmosEnabled() && General.ContainsLayer(this.camera.cullingMask, 31) == false)
        {
            this.RemoveLayerFromCullingMask(this.editorOnlyLayer);
        }
        else if (GMSceneManager.Instance().GetGizmosEnabled() == false && General.ContainsLayer(this.camera.cullingMask, 31))
        {
            this.AddLayerToCullingMask(this.editorOnlyLayer);
        }
    }
#endif
    /// <summary>
    /// Adds a Layer to the cameras culling mask
    /// <param name="layer"> The layer to addk</param>
    /// </summary>
    public void AddLayerToCullingMask(int layer) => this.camera.cullingMask |= 1 << layer;

    /// <summary>
    /// Removes a layer from the cameras culling mask
    /// <param name="layer"> The layer to remove </param>
    /// </summary>
    public void RemoveLayerFromCullingMask(int layer) => this.camera.cullingMask &= ~(1 << layer);
}

