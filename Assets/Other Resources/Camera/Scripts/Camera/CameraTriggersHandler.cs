using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraTriggersHandler : MonoBehaviour
{
    [Tooltip("A list of camera triggers set within the stage"), SerializeField]
    private List<CameraTriggerController> cameraTriggers = new List<CameraTriggerController>();
    [Tooltip("Gets the active camera trigger"), SerializeField]
    private CameraTriggerController activeCameraTrigger;
    [Tooltip("Gets the active camera trigger"), SerializeField]
    private CameraTriggerController previousCameraTrigger;
    [Tooltip("The index of the currently active trigger"), SerializeField]
    private int activeCameraTriggerIndex;

    /// <summary>
    /// Registers a camera trigger which determines the min y bounds of the camera when crossed
    /// <param name="cameraTrigger"/> The background to be registered </param>
    /// </summary>
    public void RegisterCameraTrigger(CameraTriggerController cameraTrigger)
    {
        this.cameraTriggers.Add(cameraTrigger);
        this.cameraTriggers = this.cameraTriggers.OrderBy(x => x.transform.position.x).ToList();

        if (this.activeCameraTrigger == null)
        {
            this.activeCameraTrigger = cameraTrigger;
        }

        this.previousCameraTrigger = null;
        this.activeCameraTrigger = this.FindActiveCameraTrigger(GMStageManager.Instance().GetPlayerSpawnPoint());

        if (this.activeCameraTrigger != null)
        {
            HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().bottomBounds = this.activeCameraTrigger.GetTargetCameraBottomBorder();
        }
    }

    /// <summary>
    /// Checks whether the target has exceeded the camery trigger
    /// <param name="cameraTargetPosition"/> The camera's target position</param>
    /// </summary/>
    public void CheckCameraTriggersUpdated(Vector2 cameraTargetPosition)
    {
        if (this.cameraTriggers.Count == 0)
        {
            return;
        }

        CameraTriggerController previousCameraTriggerController = this.activeCameraTrigger;
        this.activeCameraTrigger = this.FindActiveCameraTrigger(cameraTargetPosition);

        if (previousCameraTriggerController != this.activeCameraTrigger || this.previousCameraTrigger == null)
        {
            this.previousCameraTrigger = previousCameraTriggerController;

            if (this.activeCameraTrigger.GetTriggerType() == CameraTriggerType.StageEnding)
            {
                HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().leftBounds = -this.activeCameraTrigger.transform.position.x;
            }


            HedgehogCamera.Instance().GetCameraBoundsHandler().OnCameraTriggerChanged(this.activeCameraTrigger, this.previousCameraTrigger);
        }
    }

    /// <summary>
    /// Finds the active camera trigger based on the <see cref="target"/> position
    /// <param name="cameraTargetPosition"/> The camera's target position</param>
    /// </summary/>
    private CameraTriggerController FindActiveCameraTrigger(Vector2 targetCameraPosition)
    {
        if (this.cameraTriggers.Count == 0)
        {
            return this.activeCameraTrigger;
        }

        if (this.activeCameraTrigger.GetNextCameraTrigger() != null)
        {
            if (this.activeCameraTrigger.GetNextCameraTrigger().GetTriggerType() == CameraTriggerType.StageEnding)
            {
                if (HedgehogCamera.Instance().GetBounds().min.x >= this.activeCameraTrigger.GetNextCameraTrigger().transform.position.x)
                {
                    this.previousCameraTrigger = this.activeCameraTrigger;
                    this.activeCameraTrigger = this.activeCameraTrigger.GetNextCameraTrigger();
                }
            }
            else if (targetCameraPosition.x >= this.activeCameraTrigger.GetNextCameraTrigger().transform.position.x)
            {
                this.previousCameraTrigger = this.activeCameraTrigger;
                this.activeCameraTrigger = this.activeCameraTrigger.GetNextCameraTrigger();
            }
        }


        if (this.activeCameraTrigger.GetPreviousCameraTrigger() != null)
        {
            if (targetCameraPosition.x < this.activeCameraTrigger.transform.position.x)
            {
                this.previousCameraTrigger = this.activeCameraTrigger;
                this.activeCameraTrigger = this.activeCameraTrigger.GetPreviousCameraTrigger();
            }
        }

        return this.activeCameraTrigger;
    }
}
