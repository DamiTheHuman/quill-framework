using UnityEngine;
/// <summary>
/// A hidden monitor which is typically unseen by the user unless active
/// This controller class is aimed to make monitors visible when hit by a sign post
/// </summary>
public class HiddenMonitorController : MonoBehaviour
{
    [SerializeField, Tooltip("The monitor to be instantiated and hidden")]
    private GameObject monitorToHide = null;
    [SerializeField, Tooltip("Strictly for debugging and visualisation purposes")]
    private SpriteRenderer debugIcon = null;
    private GameObject activeMonitor;
    private MonitorController activeMonitorController;

    private void Reset() => this.debugIcon = this.GetComponent<SpriteRenderer>();

    // Start is called before the first frame update
    private void Start()
    {
        if (this.debugIcon == null)
        {
            this.Reset();
        }

        this.InstantiateHiddenMonitor();
    }

    /// <summary>
    /// Creates a the monitor gameobject specified by the user and updates the active monitor details
    /// </summary>
    private void InstantiateHiddenMonitor()
    {
        GameObject instantiatedMonitor = Instantiate(this.monitorToHide);
        instantiatedMonitor.transform.parent = this.transform;
        instantiatedMonitor.transform.localPosition = Vector3.zero;
        this.activeMonitorController = instantiatedMonitor.GetComponent<MonitorController>();
        this.activeMonitorController.SetIsHiddenMonitor(true);
#if UNITY_EDITOR

        this.debugIcon.sprite = this.activeMonitorController.GetMonitorIconController().monitorIcon.GetComponent<SpriteRenderer>().sprite;
#endif
        this.activeMonitor = instantiatedMonitor;

        this.activeMonitor.SetActive(false);
    }

    private void Update()
    {
        if (this.activeMonitor == null)
        {
            this.Start();
        }
    }

    /// <summary>
    /// Unhides the monitor instantiated and deactivates the current game object
    /// <param name="spawnPosition">The position to place the spawned monitor  </param>
    /// </summary>
    public void ShowMonitor(Vector2 spawnPosition)
    {
        this.activeMonitor.SetActive(true);
        this.activeMonitor.transform.position = spawnPosition;
        this.activeMonitorController.SetBumped(true);
        this.activeMonitorController.SetGrounded(false);
        this.activeMonitor.transform.parent = null;
        this.gameObject.SetActive(false);
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        SpriteRenderer spriteIcon = this.monitorToHide.GetComponent<MonitorController>().GetMonitorIconController().monitorIcon.GetComponent<SpriteRenderer>();

        if (spriteIcon != null)
        {
            this.debugIcon.sprite = this.monitorToHide.GetComponent<MonitorController>().GetMonitorIconController().monitorIcon.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void OnDrawGizmos()
    {
        SpriteRenderer spriteIcon = this.monitorToHide.GetComponent<MonitorController>().GetMonitorIconController().monitorIcon.GetComponent<SpriteRenderer>();

        if (spriteIcon != null)
        {
            this.debugIcon.sprite = this.monitorToHide.GetComponent<MonitorController>().GetMonitorIconController().monitorIcon.GetComponent<SpriteRenderer>().sprite;
        }
    }
#endif
}
