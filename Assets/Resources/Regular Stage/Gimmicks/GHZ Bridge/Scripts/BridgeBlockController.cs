using System.Collections;
using UnityEngine;

/// <summary>
/// Movement of a singular bridge block relating to a bridge
/// </summary>
public class BridgeBlockController : MonoBehaviour
{
    [SerializeField]
    private BridgeController bridgeController;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [Tooltip("The height the bridg block should move towards"), SerializeField]
    private float targetHeight;
    private IEnumerator BeginBridgeBlockMovementCoroutine;

    private void Start() => this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    private void FixedUpdate()
    {
        if (this.bridgeController.OnBridge() || (this.transform.localPosition.y != 0))
        {
            this.BlockMovement();
            if (this.transform.childCount > 0 && HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer)) //Syncs up child objects to the platforms movement
            {
                Physics2D.SyncTransforms();
            }
        }
    }

    /// <summary>
    /// Sets the parent bridge conroller
    /// </summary>
    public void SetBridgeController(BridgeController bridgeController) => this.bridgeController = bridgeController;
    /// <summary>
    /// Set the new target height of the bridge block
    ///<param name="targetHeight">The target height to be set</param>
    /// </summary>
    public void SetTargetHeight(float targetHeight)
    {
        this.targetHeight = targetHeight;

        if (this.BeginBridgeBlockMovementCoroutine != null)
        {
            this.StopCoroutine(this.BeginBridgeBlockMovementCoroutine);
        }

        this.BeginBridgeBlockMovementCoroutine = this.BeginBridgeBlockMovement();
        this.StartCoroutine(this.BeginBridgeBlockMovementCoroutine);
    }

    /// <summary>
    /// Retrieves the set target height of the bridge block
    /// </summary>
    public float GetTargetHeight() => this.targetHeight;
    /// <summary>
    /// Moves the bridge block every frame
    /// </summary>
    private IEnumerator BeginBridgeBlockMovement()
    {
        yield return new WaitForEndOfFrame();

        yield return null;
    }

    /// <summary>
    /// Move the bridge block towards the sought out height
    /// </summary>
    public void BlockMovement()
    {
        Vector2 localPosition = this.transform.localPosition;
        localPosition.y = Mathf.MoveTowards(localPosition.y, this.targetHeight, this.bridgeController.GetDepressionSpeed() * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);//Makes it the position change smooth
        localPosition.y *= Mathf.Sin(this.bridgeController.GetBridgeSmoothDelta() * Mathf.Deg2Rad);
        this.transform.localPosition = localPosition;
    }
}
