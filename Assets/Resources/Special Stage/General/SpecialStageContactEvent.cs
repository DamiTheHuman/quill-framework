using UnityEngine;
/// <summary>
/// Duplicate of <see cref="ContactEvent"/> but for special stages
/// </summary>

public class SpecialStageContactEvent : MonoBehaviour
{
    [SerializeField, Button("InvertAngle")]
    private bool invertAngle;
    public void InvertAngle()
    {
        this.angle = 360 - this.angle;
        this.CalculateObjectPosition();
        General.SetDirty(this);
    }

    [Range(0, 360), SerializeField, Tooltip("The angle of the object")]
    protected int angle = 0;
    [SerializeField, Tooltip("The shadow of the object")]
    protected SpecialStageShadowController shadowController;
    public virtual void Reset()
    {
    }

    /// <summary>
    /// Checks whether to activate the collider based on the conditions set in here defaulting to true
    /// <param name="player">The player obect  </param>
    /// <param name="solidBoxColliderBounds"/> The boundsaries of the players solid box</param>
    /// </summary>
    public virtual bool HedgeIsCollisionValid(SpecialStagePlayer player, Bounds solidBoxColliderBounds) => true;

    /// <summary>
    /// The action to be performed when a collision occurs with a special stage object
    /// <param name="player">The player obect  </param>
    /// </summary>
    public virtual void HedgeOnCollisionEnter(SpecialStagePlayer player) { }
    /// <summary>
    /// The action to be performed while the player is actively in contact with the special stageobject
    /// <param name="player">The player obect  </param>
    /// </summary>
    public virtual void HedgeOnCollisionStay(SpecialStagePlayer player) { }
    /// <summary>
    /// The action to be performed when contact with the special stagegimmick ends
    /// <param name="player">The player obect  </param>
    /// </summary>
    public virtual void HedgeOnCollisionExit(SpecialStagePlayer player) { }

    /// <summary>
    /// Calculates the player position relative to the pipe
    /// </summary>
    public void CalculateObjectPosition()
    {
        if (GMSpecialStageManager.Instance().GetSpecialStageSlider() == null)
        {
            return;
        }

        float y = this.transform.position.y;
        float angleInDegrees = this.angle > 270 ? this.angle - 360 : this.angle;
        Vector3 position = GMSpecialStageManager.Instance().GetSpecialStageSlider().GetPositionAtAngle(angleInDegrees);
        position.y = y;
        this.transform.position = position;

        if (this.shadowController != null)
        {
            this.shadowController.CalculateShadowPosition(position, angleInDegrees);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            this.CalculateObjectPosition();
        }
    }
}
