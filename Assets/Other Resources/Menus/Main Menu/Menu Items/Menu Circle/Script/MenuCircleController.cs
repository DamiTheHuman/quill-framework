using UnityEngine;

/// <summary>
/// Controls the rotating circles in the menu screen
/// </summary>
public class MenuCircleController : MonoBehaviour
{
    [Tooltip("The current delta of the circle"), SerializeField]
    private float delta = 0;
    [Tooltip("The speed in which the circlee grows and rotates"), SerializeField]
    private float speed = 1;

    [Tooltip("The range in with the circle is allowed to grow"), SerializeField]
    private float range = 60;
    [Tooltip("Extra padding added to ensure the circles only drop to a fixed size"), SerializeField]
    private float padding = 0.75f;
    [Tooltip("The scale of the menu circle when initialized"), SerializeField]
    public Vector3 startScale;

    private void Start() => this.startScale = this.transform.localScale;

    private void Update()
    {
        this.UpdateScale(this.transform.localScale.x);
        this.UpdateRotation();
        this.UpdateDelta();
    }

    /// <summary>
    /// Updates the local scale of the menu circle
    /// <param name="scale">The current scale of the menu circle</param>
    /// </summary>
    private void UpdateScale(float scale)
    {
        scale = this.startScale.x + (Mathf.Cos(this.delta * Mathf.Deg2Rad) * this.range);
        scale += this.padding;
        this.transform.localScale = new Vector3(scale, scale, this.startScale.z);
    }

    /// <summary>
    /// Updates the rotation of the menu circle
    /// </summary>
    private void UpdateRotation() => this.transform.localEulerAngles = new Vector3(0, 0, this.delta);
    /// <summary>
    /// Update the delta of the menu circle
    /// </summary>
    private void UpdateDelta()
    {
        this.delta += this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.delta = General.ClampDegreeAngles(this.delta);
    }
}
