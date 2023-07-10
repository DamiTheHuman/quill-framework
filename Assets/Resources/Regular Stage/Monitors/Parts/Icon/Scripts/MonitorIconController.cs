using UnityEngine;

/// <summary>
/// Controls the way the monitor Icon is displayed when it floats before it fades away
///  Original Author - Damizean [Sonic Worlds] 
///  Added functionality - Converted to unity and made it count backwards 
/// </summary>
public class MonitorIconController : MonoBehaviour
{
    public Transform initialParent;
    private Animator animator;
    public Transform monitorIcon;
    public float monitorAge = 0.4833333333f;
    public float monitorSpeed = 22.5f;

    // Start is called before the first frame update
    private void Start() => this.animator = this.GetComponent<Animator>();

    private void Update() => this.UpdateTimer();
    private void FixedUpdate()
    {
        if (this.monitorAge > 0)
        {
            this.UpdateMonitorPosition();
        }
        else
        {
            this.BeginMonitorDeactivation();
        }
    }

    /// <summary>
    /// Updates the monitor age towards zero
    /// </summary>
    private void UpdateTimer() => this.monitorAge -= Time.deltaTime;

    /// <summary>
    /// Continously moves the monitor upwards until the monitors age is fully exhausted
    /// </summary>
    private void UpdateMonitorPosition()
    {
        Vector2 pos = this.transform.position;
        float deltaMonitorSpeed = GMStageManager.Instance().ConvertToDeltaValue(this.monitorSpeed);
        pos.y += deltaMonitorSpeed * Time.fixedDeltaTime;
        this.transform.position = pos;
    }

    /// <summary>
    /// Begin to fadeway the monitor icon till it is eventually destroyed
    /// </summary>
    private void BeginMonitorDeactivation() => this.animator.SetTrigger("Fadeaway");
    /// <summary>
    /// Deactives the main child icon displayed as a starter to the monitors end
    /// </summary>
    public void DeactivateChildIcon() => this.monitorIcon.gameObject.SetActive(false);

    /// <summary>
    /// Set the monitor Icon back to his parent
    /// </summary>
    public void Reparent()
    {
        if (this.initialParent != null)
        {
            this.transform.parent = this.initialParent;
        }
    }
}
