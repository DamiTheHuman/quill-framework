using UnityEngine;

///<summary>
/// The movement of the individual swing links
/// Author - LARK SS orignal by DW & Damizean [Sonic Worlds]  
/// Changes - Converted to UNITY and made it more approachable
///</summary>

public class SwingLinkController : MonoBehaviour
{
    private SwingController swingController;
    [Tooltip("The current link ID of the swing which affects its distance and is based on its distance from the swing controller")]
    public int linkID = 0;
    [Tooltip("The distance of the current swing link from the parent")]
    public float distance = 0;
    // Start is called before the first frame update
    public void Start()
    {
        this.swingController = this.GetComponentInParent<SwingController>();
        this.distance = this.linkID * this.swingController.linkItemHeight;
    }

    // Update is called once per frame
    private void FixedUpdate() => this.LinkMovement();

    /// <summary>
    /// Move the links based on the distance to the top point
    /// </summary>
    private void LinkMovement()
    {
        Vector2 pos = new Vector2
        {
            x = this.swingController.transform.position.x + (this.distance * Mathf.Cos(this.swingController.swingAngle * Mathf.Deg2Rad)),
            y = this.swingController.transform.position.y + (this.distance * Mathf.Sin(this.swingController.swingAngle * Mathf.Deg2Rad))
        };

        this.transform.position = pos;
    }

}
