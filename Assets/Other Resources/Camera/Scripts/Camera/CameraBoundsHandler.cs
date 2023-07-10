using UnityEditor;
using UnityEngine;

public class CameraBoundsHandler : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The boundaries of the entire zone")]
    private CameraBounds currentActBounds = new CameraBounds();
    [SerializeField, Tooltip("Act bounds before the act was cleared")]
    private CameraBounds preActEndBounds;
    [Tooltip("The boundaries which the player must exceed to move the camera"), SerializeField]
    private CameraBounds playerBounds = new CameraBounds();
    [Tooltip("The bounds the current act bounds will move towards"), SerializeField]
    private CameraBounds targetActBounds;
    [Tooltip("Death plane slider for debug purposes")]
    [SerializeField]
    private Transform deathPlaneSlider;

    [Tooltip("The bottom border for stage set on start"), SerializeField]
    private float initialBottomActBounds;

    private void Start()
    {
        if (this.playerBounds.transform == null)
        {
            this.playerBounds.transform = this.transform;
        }

        if (HedgehogCamera.Instance().GetCameraMode() != CameraMode.SpecialStage)
        {
            if (this.currentActBounds.transform == null)
            {
                this.currentActBounds.transform = this.transform;
                this.SetInitialBottomActBounds(this.currentActBounds.bottomBounds);
            }

            this.GetTargetActBounds().CopyCameraBoundData(this.currentActBounds);
        }
    }

    /// <summary>
    /// Set the initial bottom bounds of the act
    /// <param name="initialBottomBorder"/> The initial bottom bounds of the act </param>
    /// </summary>
    public void SetInitialBottomActBounds(float initialBottomBorder) => this.initialBottomActBounds = initialBottomBorder;

    /// <summary>
    /// Get the target zone bounds
    /// </summary>
    public CameraBounds GetTargetActBounds() => this.targetActBounds;

    /// <summary>
    /// Set the target zone bounds
    /// <param name="targetZoneBounds"/> The new bounds for the zone</param>
    /// </summary>
    public void SetTargetActBounds(CameraBounds targetZoneBounds) => this.targetActBounds = targetZoneBounds;


    /// <summary>
    /// Restricts the cameras position from exceeding the zones bounds
    /// </summary>
    public Vector2 ClampPositionToZoneBounds(Vector2 position)
    {
        if (HedgehogCamera.Instance().GetCamera() == null)
        {
            HedgehogCamera.Instance().SetCamera(Camera.main);
            Debug.Log("No Camera set, using main camera!");
        }

        Camera camera = HedgehogCamera.Instance().GetCamera();

        float halfCameraHeight = 2f * camera.orthographicSize / 2;
        float halfCameraWidth = halfCameraHeight * camera.aspect;
        Vector2 currentPosition = position;

        if (HedgehogCamera.Instance().GetCameraMode() != CameraMode.EndLevel)
        {
            //Clamp to the left bounds
            if (position.x < this.currentActBounds.GetLeftBorderPosition() + halfCameraWidth)
            {
                position.x = this.currentActBounds.GetLeftBorderPosition() + halfCameraWidth;
            }

            //Clam to the right bounds
            if (position.x > this.currentActBounds.GetRightBorderPosition() - halfCameraWidth)
            {
                position.x = this.currentActBounds.GetRightBorderPosition() - halfCameraWidth;
            }
        }

        //Clamp to the bottom bounds
        if (position.y < this.currentActBounds.GetBottomBorderPosition() + halfCameraHeight)
        {
            position.y = this.currentActBounds.GetBottomBorderPosition() + halfCameraHeight;
        }

        //Clamp to the top Bounds
        if (position.y > this.currentActBounds.GetTopBorderPosition() - halfCameraHeight)
        {
            position.y = this.currentActBounds.GetTopBorderPosition() - halfCameraHeight;
        }

        if (currentPosition != position)
        {
            return position;
        }

        return currentPosition;
    }

    /// <summary>
    /// Move the zone bounds bottom border to the target zones border
    /// </summary>
    public void UpdateCurrentZoneBottomBorder()
    {
        if (HedgehogCamera.Instance().GetCameraMode() == CameraMode.SpecialStage)
        {
            return;
        }

        if (this.currentActBounds.bottomBounds != this.GetTargetActBounds().bottomBounds)
        {
            this.currentActBounds.bottomBounds = Mathf.MoveTowards(this.currentActBounds.bottomBounds, this.GetTargetActBounds().bottomBounds, HedgehogCamera.Instance().GetCameraLookHandler().GetScrollActionLookSpeed() * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

            //When out of the view considerably just snap to the target bottom border
            if (this.currentActBounds.GetBottomBorderPosition() <= HedgehogCamera.Instance().GetBounds().min.y - HedgehogCamera.Instance().GetBounds().size.y)
            {
                this.currentActBounds.bottomBounds = this.GetTargetActBounds().bottomBounds;
            }
        }
    }

    /// <summary>
    /// Gets the boundaries of the stage based on the current camera mode
    /// </summary>
    public CameraBounds GetActiveStageBounds()
    {
        if (HedgehogCamera.Instance().GetCameraMode() == CameraMode.BossMode)
        {
            return GMStageManager.Instance().GetBoss().GetBossTrigger().GetBossFightBounds();
        }

        return this.currentActBounds;
    }

    /// <summary>
    /// Actions that take place when switched to end zone
    /// This assums the current camera target is set to the end stage object
    /// </summary>
    public void OnEndZoneCameraMode()
    {
        Vector2 targetPosition = HedgehogCamera.Instance().GetCameraTarget().transform.position;
        this.preActEndBounds.CopyCameraBoundData(this.currentActBounds);
        float halfCameraHeight = 2f * HedgehogCamera.Instance().GetCamera().orthographicSize / 2;
        float halfCameraWidth = halfCameraHeight * HedgehogCamera.Instance().GetCamera().aspect;
        this.currentActBounds.leftBounds = halfCameraWidth - targetPosition.x;
        this.currentActBounds.rightBounds = halfCameraWidth + targetPosition.x;
        this.currentActBounds.topBounds = (halfCameraHeight * 2) + targetPosition.y;
    }

    /// <summary>
    /// Gets the current death plane of the stage
    /// </summary/>
    public float GetDeathPlane() => this.currentActBounds.GetBottomBorderPosition() == this.GetTargetActBounds().GetBottomBorderPosition() ? this.currentActBounds.GetBottomBorderPosition() : this.GetTargetActBounds().GetBottomBorderPosition();

    /// <summary>
    /// Updates the current acts bottom border when the trigger changes
    /// <param name="activeCameraTrigger"/> The active camera trigger</param>
    /// <param name="previousCameraTrigger"/> The previous camera trigger</param>
    /// </summary/>
    public void OnCameraTriggerChanged(CameraTriggerController activeCameraTrigger, CameraTriggerController previousCameraTrigger)
    {
        if (this.ShouldSmoothlyFollowBottomBounds(activeCameraTrigger, previousCameraTrigger) && Time.frameCount > 0)
        {
            this.targetActBounds.bottomBounds = activeCameraTrigger.GetTargetCameraBottomBorder();
        }
        else
        {
            this.targetActBounds.bottomBounds = activeCameraTrigger.GetTargetCameraBottomBorder();
            this.currentActBounds.bottomBounds = this.targetActBounds.bottomBounds;
        }
    }


    /// <summary>
    /// Whether the bounds should perform a smooth update 
    /// <param name="activeCameraTrigger"/> The active camera trigger</param>
    /// </summary/>
    private bool ShouldSmoothlyFollowBottomBounds(CameraTriggerController activeCameraTrigger, CameraTriggerController previousCameraTrigger)
    {
        bool isGoingUp = -activeCameraTrigger.GetTargetCameraBottomBorder() >= -previousCameraTrigger.GetTargetCameraBottomBorder();
        HedgehogCamera hedgehogCamera = HedgehogCamera.Instance();
        CameraBounds targetBounds = new CameraBounds();
        targetBounds.CopyCameraBoundData(this.currentActBounds);
        targetBounds.bottomBounds = activeCameraTrigger.GetTargetCameraBottomBorder();
        float distanceBetweenTargetAndCurrent = targetBounds.GetBottomBorderPosition() - (-previousCameraTrigger.GetTargetCameraBottomBorder());

        if (isGoingUp)
        {
            if (hedgehogCamera.GetBounds().min.y <= targetBounds.GetBottomBorderPosition())
            {
                if (distanceBetweenTargetAndCurrent >= hedgehogCamera.GetBounds().size.y)
                {
                    this.currentActBounds.bottomBounds = -hedgehogCamera.GetBounds().min.y;
                }

                return true;
            }
        }
        else
        {
            if (hedgehogCamera.GetBounds().min.y >= -previousCameraTrigger.GetTargetCameraBottomBorder() || hedgehogCamera.GetBounds().min.y - targetBounds.GetBottomBorderPosition() <= hedgehogCamera.GetBounds().size.y)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get the current act bounds
    /// </summary/>
    public CameraBounds GetActBounds() => this.currentActBounds;

    /// <summary>
    /// Set the current act bounds
    /// <param name="currentActBounds"/> The new act bounds value</param>
    /// </summary/>
    public void SetActBounds(CameraBounds currentActBounds) => this.currentActBounds = currentActBounds;

    /// <summary>
    /// Get the current player bounds bounds
    /// </summary/>
    public CameraBounds GetPlayerBounds() => this.playerBounds;

    /// <summary>
    /// Set the current player bounds
    /// <param name="playerBounds"/> The new player bounds value</param>
    /// </summary/>
    public void SetPlayerBounds(CameraBounds playerBounds) => this.currentActBounds = playerBounds;

    /// <summary>
    /// Get the camera bounds before the act ended.
    /// </summary/>
    public CameraBounds GetPreActEndBounds() => this.preActEndBounds;

    private void OnDrawGizmos()
    {
        if (this.playerBounds.transform == null)
        {
            this.playerBounds.transform = this.transform;
        }

#if UNITY_EDITOR
        if (this.deathPlaneSlider != null && this.currentActBounds.HasInfinity() == false)
        {
            SceneView sceneView = SceneView.currentDrawingSceneView;
            Vector3 position = this.deathPlaneSlider.transform.position;
            if (sceneView != null)
            {
                position.x = sceneView.pivot.x;
            }
            position.y = this.currentActBounds.GetBottomLeftBorderPosition().y - 20;
            this.deathPlaneSlider.transform.position = position;//Ensures we can always move the handle at the center of the scene view
        }
#endif

        if (HedgehogCamera.Instance().GetCameraTarget() != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(this.playerBounds.GetTopLeftBorderPosition(), this.playerBounds.GetTopRightBorderPosition());//Top Horizontal Bounds
            Gizmos.DrawLine(this.playerBounds.GetBottomLeftBorderPosition(), this.playerBounds.GetBottomRightBorderPosition());//Bottom Horizontal Bounds
            Gizmos.DrawLine(this.playerBounds.GetTopLeftBorderPosition(), this.playerBounds.GetBottomLeftBorderPosition());//Left Vertical Bounds
            Gizmos.DrawLine(this.playerBounds.GetTopRightBorderPosition(), this.playerBounds.GetBottomRightBorderPosition());//Right Vertical Bounds

        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.currentActBounds.GetTopLeftBorderPosition(), this.currentActBounds.GetTopRightBorderPosition());//Top Horizontal Bounds
        Gizmos.DrawLine(this.currentActBounds.GetBottomLeftBorderPosition(), this.currentActBounds.GetBottomRightBorderPosition());//Bottom Horizontal Bounds
        Gizmos.DrawLine(this.currentActBounds.GetTopLeftBorderPosition(), this.currentActBounds.GetBottomLeftBorderPosition());//Left Vertical Bounds
        Gizmos.DrawLine(this.currentActBounds.GetTopRightBorderPosition(), this.currentActBounds.GetBottomRightBorderPosition());//Right Vertical Bounds
    }
}
