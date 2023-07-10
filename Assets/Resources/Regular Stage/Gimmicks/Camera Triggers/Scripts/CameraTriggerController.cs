using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Place this at any point to register a trigger point, when the camera exceeds any of the trigger its details will be updated
/// </summary>
public class CameraTriggerController : MonoBehaviour
{
    [SerializeField, Button(nameof(FindNextAndPreviousCameraTriggers))]
    private bool findNextAndPreviousCameraTriggers;
    [SerializeField]
    private CameraTriggerType triggerType = CameraTriggerType.Regular;

    [Help("This value should be inverted so the higher the number the lower the Y bounds and vice versa"), SerializeField]
    private float targetCameraBottomBorder = 0;

    [SerializeField, Tooltip("The time line trigger ahead of  this")]
    private CameraTriggerController previousCameraTrigger;
    [SerializeField, Tooltip("The time line trigger before this")]
    private CameraTriggerController nextCameraTrigger;

    [SerializeField]
    private Color debugColor = Color.yellow;

    private void Start()
    {
        this.FindNextAndPreviousCameraTriggers();
        HedgehogCamera.Instance().GetCameraTriggersHandler().RegisterCameraTrigger(this);
    }

    /// <summary>
    /// Find the next and previous triggers based on the closest <see cref="CameraTriggerController"/> ahead or before this one
    /// </summary>
    private void FindNextAndPreviousCameraTriggers()
    {
        List<CameraTriggerController> allCameraTriggers = FindObjectsOfType<CameraTriggerController>().ToList();
        CameraTriggerController minCameraTrigger = null;
        CameraTriggerController maxCameraTrigger = null;

        for (int x = 0; x < allCameraTriggers.Count; x++)
        {
            if (allCameraTriggers[x] == this)
            {
                continue;
            }

            if (allCameraTriggers[x].transform.position.x < this.transform.position.x)
            {
                if (minCameraTrigger == null || allCameraTriggers[x].transform.position.x > minCameraTrigger.transform.position.x)
                {
                    minCameraTrigger = allCameraTriggers[x];
                }
            }

            if (allCameraTriggers[x].transform.position.x > this.transform.position.x)
            {
                if (maxCameraTrigger == null || allCameraTriggers[x].transform.position.x < maxCameraTrigger.transform.position.x)
                {
                    maxCameraTrigger = allCameraTriggers[x];
                }
            }
        }

        this.previousCameraTrigger = minCameraTrigger;
        this.nextCameraTrigger = maxCameraTrigger;

        if (Application.isPlaying == false)
        {
            General.SetDirty(this);
        }
    }

    /// <summary>
    /// Get the zone bottom bounds that the stage will be set to 
    /// </summary>
    public float GetTargetCameraBottomBorder() => this.targetCameraBottomBorder;

    /// <summary>
    /// Get the trigger type
    /// </summary>
    public CameraTriggerType GetTriggerType() => this.triggerType;

    /// <summary>
    /// Get the next camera trigger ahead of this one
    /// </summary>
    public CameraTriggerController GetNextCameraTrigger() => this.nextCameraTrigger;

    /// <summary>
    /// Get the previous camera trigger behind this one
    /// </summary>
    public CameraTriggerController GetPreviousCameraTrigger() => this.previousCameraTrigger;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        SceneView sceneView = SceneView.currentDrawingSceneView;

        if (sceneView != null)
        {
            Vector3 position = Camera.current.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
            position.y = sceneView.pivot.y;
            this.transform.position = new Vector3(this.transform.position.x, position.y);//Ensures we can always move the handle at the center of the scene view
        }

        Gizmos.color = this.debugColor;
        HedgehogCamera hedgehogCamera = HedgehogCamera.Instance();

        if (hedgehogCamera == null)
        {
            return;
        }

        Vector3 cameraTriggerPosition = new Vector2(this.transform.position.x, -this.targetCameraBottomBorder);

        GizmosExtra.DrawPolyLine(4, new Vector2(this.transform.position.x, hedgehogCamera.GetCameraBoundsHandler().GetActBounds().GetTopBorderPosition()), new Vector2(this.transform.position.x, hedgehogCamera.GetCameraBoundsHandler().GetActBounds().GetBottomBorderPosition()), this.debugColor);
        GizmosExtra.Draw2DArrow(cameraTriggerPosition, 90);
        GizmosExtra.Draw2DArrow(cameraTriggerPosition, 270);
        Gizmos.DrawLine(cameraTriggerPosition - new Vector3(64, 0), cameraTriggerPosition + new Vector3(64, 0));
#endif
    }
}

