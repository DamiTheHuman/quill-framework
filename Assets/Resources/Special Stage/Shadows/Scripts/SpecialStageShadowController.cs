using UnityEngine;
/// <summary>
/// Handles the shadows underneath objects in the special stage
/// </summary>
public class SpecialStageShadowController : MonoBehaviour
{

    [SerializeField]
    private Vector3 scale = new Vector3(1f, 0.5f, 1f);
    [SerializeField]
    private float verticallOffset = 16;

    /// <summary>
    /// Calculates the position of the shadow relative to the angle of the parent
    /// <param name="parentPosition">The position of the parent of the shadow</param>
    /// <param name="parentAngleInDegrees">The angle of the parent object</param>

    /// </summary>
    public void CalculateShadowPosition(Vector3 parentPosition, float parentAngleInDegrees)
    {

        float angleInDegrees = parentAngleInDegrees;

        Vector3 position = parentPosition;

        this.transform.localScale = this.scale;

        if (angleInDegrees is > 90 and <= 270)
        {
            angleInDegrees = (angleInDegrees is < (-90) and >= (-270)) ? -180 - angleInDegrees : 180 - angleInDegrees;
            position = GMSpecialStageManager.Instance().GetSpecialStageSlider().GetPositionAtAngle(angleInDegrees);
            position.y = parentPosition.y;
            float distance = Vector3.Distance(position, parentPosition) / GMSpecialStageManager.Instance().GetSpecialStageSlider().GetRange();

            if (distance > 1)
            {
                this.transform.localScale = this.scale * distance;
            }
        }

        this.SetShadowAngle(angleInDegrees, position);
    }

    /// <summary>
    /// Casts the shadow at the specified position
    /// <param name="angleInDegrees">The angle to cast the shadow in</param>
    /// </summary>
    private void SetShadowAngle(float angleInDegrees, Vector3 parentPosition)
    {
        if (this.transform.parent == null)
        {
            Debug.Log("Shadow needs to be parented");
            return;
        }

        Vector3 position = new Vector3
        {
            x = angleInDegrees / 15 * 4,
            y = -this.verticallOffset,
            z = 0
        };
        Vector3 rotation = new Vector3(0, 0, angleInDegrees);
        this.transform.localPosition = position + parentPosition - parentPosition;
        parentPosition.z += this.verticallOffset;
        parentPosition.x += position.x;
        this.transform.position = parentPosition;
        this.transform.localEulerAngles = rotation;
    }
}

