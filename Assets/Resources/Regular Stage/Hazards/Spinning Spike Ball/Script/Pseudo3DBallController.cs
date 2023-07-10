using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The controller for the spinning Pseudo3DBallController
/// **This isnt 100% accurate but is just based off how I would make something similar** 
///</summary>
public class Pseudo3DBallController : MonoBehaviour
{
    [FirstFoldOutItem("Ball Information")]
    [Tooltip("The direction the spike ball spins in")]
    public Pseudo3DBallSpinDirection spinDirection = Pseudo3DBallSpinDirection.Horizontal;
    [Tooltip("The depth/layer of the spinning spike ball")]
    public BackgroundDepth ballDepth = BackgroundDepth.Background;
    [Tooltip("The primary spinning spike ball link game object to be cloned"), SerializeField]
    private Pseudo3DBallLinkController pseudo3DLinkToClone = null;
    [SerializeField]
    [Tooltip("The active spinning spike ball game object"), LastFoldoutItem()]
    private Pseudo3DBallSpikeController pseudo3DBallSpikeController = null;

    [FirstFoldOutItem("Link and Ball Spike Movement")]
    [Tooltip("The height of each link related item")]
    public float linkItemHeight = 16;
    [Tooltip("Controls the length of the spinning spike ball and how many links are constructed")]
    public int length = 4;
    [SerializeField]
    [Tooltip("The angle the spinning spike ball starts on at spawn with 180 being at the left most and 360 the right most")]
    private float intitialAngle = 180;
    [Tooltip("The current speed of the spinning spike ball ")]
    public float speed = 2;
    [LastFoldoutItem()]
    [Tooltip("The current angle of the swing")]
    public float delta;

    [SerializeField, IsDisabled]
    private List<Pseudo3DBallLinkController> pseudo3DBallLinks = new List<Pseudo3DBallLinkController>();


    [Tooltip("The color of the debug lines drawn")]
    public Color debugColor = Color.white;

    private void Awake() => this.SpawnLinks();
    private void Start() => this.delta = this.intitialAngle;
    private void FixedUpdate()
    {
        this.UpdateDelta();
        this.UpdateSwingLayer();
    }

    /// <summary>
    /// Update the delta of the current spike
    /// </summary>
    private void UpdateDelta()
    {
        this.delta += this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.delta = General.ClampDegreeAngles(this.delta);
    }

    /// <summary>
    /// Update the spinning spike ball layer whether its in the background layer or forgeround layer
    /// </summary>
    private void UpdateSwingLayer() => this.ballDepth = this.delta >= 180 ? BackgroundDepth.Background : BackgroundDepth.Foreground;

    /// <summary>
    /// Create links the size of the length specified
    /// </summary>
    private void SpawnLinks()
    {
        this.delta = this.intitialAngle;

        Pseudo3DBallLinkController pseudo3DLinkToClone = this.pseudo3DLinkToClone.GetComponent<Pseudo3DBallLinkController>();
        pseudo3DLinkToClone.GetComponent<Pseudo3DBallLinkController>().linkID = 1;
        pseudo3DLinkToClone.Start();
        pseudo3DLinkToClone.UpdateMovement();

        this.pseudo3DBallLinks.Add(this.pseudo3DLinkToClone);

        for (int x = 1; x < this.length; x++)//Start at 1 as the cloned link contains the original link ID of 0
        {
            Pseudo3DBallLinkController clonedSwingLink = Instantiate(this.pseudo3DLinkToClone.gameObject).GetComponent<Pseudo3DBallLinkController>();
            clonedSwingLink.transform.parent = this.pseudo3DLinkToClone.transform.parent;
            clonedSwingLink.transform.position = this.pseudo3DLinkToClone.transform.position - new Vector3(x * this.linkItemHeight, 0);
            clonedSwingLink.linkID = x + 1;
            clonedSwingLink.name = this.pseudo3DLinkToClone.name + " " + x;
            clonedSwingLink.Start();
            clonedSwingLink.UpdateMovement();
            this.pseudo3DBallLinks.Add(clonedSwingLink);
        }

        this.pseudo3DBallSpikeController.gameObject.transform.position = this.pseudo3DLinkToClone.transform.position - new Vector3(0, this.length); //Place the active spinning spike ball at the end of the link 
        this.pseudo3DBallSpikeController.Start();
        this.pseudo3DBallSpikeController.UpdateMovement();
    }

    /// <summary>
    /// Gets a list of the links attached to this object
    /// </summary>
    public List<Pseudo3DBallLinkController> GetPseudo3DBallLinks() => this.pseudo3DBallLinks;

    /// <summary>
    /// Gets the spike ball at the of this object
    /// </summary>
    public Pseudo3DBallSpikeController GetPseudo3DBallSpikeController() => this.pseudo3DBallSpikeController;

    private void OnDrawGizmos()
    {
        Vector2 starPosition = this.transform.position;
        float distance = (this.length + 1) * this.linkItemHeight;//emulate the distance for the spinning spike ball platform
        Gizmos.color = this.debugColor;

        if (this.spinDirection == Pseudo3DBallSpinDirection.Horizontal)
        {
            float leftXPosition = starPosition.x + (distance * Mathf.Cos(0 * Mathf.Deg2Rad));
            float rightXPosition = starPosition.x + (distance * Mathf.Cos(180 * Mathf.Deg2Rad));
            Gizmos.DrawLine(new Vector2(leftXPosition, starPosition.y), new Vector2(rightXPosition, starPosition.y));
            GizmosExtra.Draw2DArrow(new Vector2(leftXPosition, starPosition.y), 270);
            GizmosExtra.Draw2DArrow(new Vector2(rightXPosition, starPosition.y), 90);
        }
        else if (this.spinDirection == Pseudo3DBallSpinDirection.Vertical)
        {
            float topYPosition = starPosition.y + (distance * Mathf.Cos(0 * Mathf.Deg2Rad));
            float bottomYPosition = starPosition.y + (distance * Mathf.Cos(180 * Mathf.Deg2Rad));
            Gizmos.DrawLine(new Vector2(starPosition.x, topYPosition), new Vector2(starPosition.x, bottomYPosition));
            GizmosExtra.Draw2DArrow(new Vector2(starPosition.x, topYPosition), 0);
            GizmosExtra.Draw2DArrow(new Vector2(starPosition.x, bottomYPosition), 180);
        }
    }

}
