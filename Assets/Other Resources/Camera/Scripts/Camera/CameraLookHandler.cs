using System.Collections;
using UnityEngine;

public class CameraLookHandler : MonoBehaviour
{
    [SerializeField]
    [Tooltip("How Long to wait for before performing either of the look action")]
    private float scrollActionWaitTime = 120f;
    [Tooltip("How Long the camera waits for after performing a spindash like move"), SerializeField]
    private float spinDashLagTime = 4f;
    [Tooltip("How fast the camera scrolls while looking"), SerializeField]
    private float scrollActionLookSpeed = 2f;
    [Tooltip("How fast the camera moves when chasing after the player"), SerializeField]
    private float maxCameraFollowAmount = 8;
    [Tooltip("A flag to check whether the target is currently looking up"), SerializeField]
    private bool lookingUp = false;
    [Tooltip("How far vertically the camera can move while looking up"), SerializeField]
    private float lookUpLimit = 76f;
    [Tooltip("A flag to check whether the target is currently looking down"), SerializeField]
    private bool lookingDown = false;
    [Tooltip("How far vertically the camera can move while looking down"), SerializeField]
    private float lookDownLimit = -88f;
    [LastFoldoutItem(), Tooltip("A flag to determine when the waittime has run out and scrolling can begin"), SerializeField]
    private bool beginScrollAction = false;

    private IEnumerator lookCoroutine;

    /// <summary>
    /// Get the max camera follow amount value
    /// </summary>
    public float GetMaxCameraFollowAmount() => this.maxCameraFollowAmount;

    /// <summary>
    /// Get the scroll action look speed
    /// </summary>
    public float GetScrollActionLookSpeed() => this.scrollActionLookSpeed;

    /// <summary>
    /// Get the spin dash lag time
    /// </summary>
    public float GetSpinDashLagTime() => this.spinDashLagTime;

    /// <summary>
    /// Actions that take place when switched to end zone
    /// This assums the current camera target is set to the end stage object
    /// </summary>
    public void OnEndZoneCameraMode() => this.StopCoroutine(this.ApplyDashLag());
    /// <summary>
    /// Set the Look up Camera Value
    /// </summary>
    public void SetLookUp(bool lookUpValue)
    {
        this.lookingUp = lookUpValue;
        this.beginScrollAction = false;

        if (this.lookCoroutine != null)
        {
            this.StopCoroutine(this.lookCoroutine);
        }

        if (lookUpValue)
        {
            this.lookCoroutine = this.CountdownScrollWaitTime();
            this.StartCoroutine(this.lookCoroutine);
        }
    }

    /// <summary>
    /// Moves the camera offset 
    /// </summary>
    private IEnumerator CountdownScrollWaitTime()
    {
        this.beginScrollAction = false;

        yield return new WaitForSeconds(General.StepsToSeconds(this.scrollActionWaitTime));
        this.beginScrollAction = this.lookingUp || this.lookingDown; //If either of these return true at the end we can begin scrolling
    }

    /// <summary>
    /// Set the look down camera value
    /// </summary>
    public void SetLookDown(bool lookDownValue)
    {
        this.lookingDown = lookDownValue;
        this.beginScrollAction = false;

        if (this.lookCoroutine != null)
        {
            this.StopCoroutine(this.lookCoroutine);
        }

        if (lookDownValue)
        {
            this.lookCoroutine = this.CountdownScrollWaitTime();
            this.StartCoroutine(this.lookCoroutine);
        }
    }

    /// <summary>
    /// Handles the way the camera moves while performing a look action
    /// </summary>
    public Vector2 HandleLookAction(Vector2 cameraOffset)
    {
        float scrollDeltaSpeed = this.scrollActionLookSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;

        if (this.beginScrollAction)
        {
            if (this.lookingUp)
            {
                cameraOffset.y = Mathf.MoveTowards(cameraOffset.y, this.lookUpLimit, scrollDeltaSpeed);
            }
            else if (this.lookingDown)
            {
                cameraOffset.y = Mathf.MoveTowards(cameraOffset.y, this.lookDownLimit, scrollDeltaSpeed);
            }
        }
        if (!this.lookingUp && !this.lookingDown)
        {
            cameraOffset.y = Mathf.MoveTowards(cameraOffset.y, 0, this.scrollActionLookSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
        }
        cameraOffset.x = Mathf.MoveTowards(cameraOffset.x, 0, this.scrollActionLookSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

        return cameraOffset;
    }

    /// <summary>
    /// Makes the camera wait behind for the set amount of seconds before catching up later
    /// </summary>
    public void BeginDashLag()
    {
        if (HedgehogCamera.Instance().GetCameraMode() == CameraMode.EndLevel)
        {
            return;
        }

        if (HedgehogCamera.Instance().GetCameraType() == CameraType.Retro && HedgehogCamera.Instance().GetCameraMode() == CameraMode.FollowTarget)
        {
            this.StopCoroutine(this.ApplyDashLag());
            this.StartCoroutine(this.ApplyDashLag());
        }
    }

    /// <summary>
    /// Apply the dash lag by delaying the camera movement for a bit
    /// </summary>
    public IEnumerator ApplyDashLag()
    {
        HedgehogCamera.Instance().SetCameraMode(CameraMode.Freeze);

        yield return new WaitForSeconds(General.StepsToSeconds(this.GetSpinDashLagTime()));
        HedgehogCamera.Instance().SetCameraMode(CameraMode.FollowTarget);
    }

}
