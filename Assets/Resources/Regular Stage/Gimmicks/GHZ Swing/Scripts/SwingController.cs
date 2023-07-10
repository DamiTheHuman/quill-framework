using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The main swing controller that all the other swing related items get their information from
/// Author - LARK SS orignal by DW & Damizean [Sonic Worlds]  
/// Changes - Converted to UNITY and made it more approachable
///</summary>

public class SwingController : MonoBehaviour
{
    [FirstFoldOutItem("Swing Information")]
    [Tooltip("The type of swing to be spawned based on type")]
    public SwingType swingType = SwingType.Platform;
    [Tooltip("The primary swing link game object to be cloned")]
    public SwingLinkController swingLinkToClone;
    [Tooltip("The swing platform types available")]
    public SwingPlatformController[] swingPlatformTypes;
    [SerializeField]
    [Tooltip("The active swing game object"), LastFoldoutItem()]
    private SwingPlatformController activeSwingObject = null;

    [FirstFoldOutItem("Swing Link and Platform Movement")]
    [Tooltip("The height of each link related item")]
    public float linkItemHeight = 16;
    [Tooltip("Controls the length of the swing and how many links are constructed")]
    public int swingLength = 8;
    [SerializeField]
    [Tooltip("The angle the swing starts on at spawn with 180 being at the left most and 360 the right most")]
    private float intitialAngle = 180;
    [Tooltip("The gravity which affects the swings sway and restricts its curve")]
    public float swingGravity = 1.75f;
    [Tooltip("The current speed of the swing which is affected directly by gravity ")]
    public float currentSwingSpeed;
    [LastFoldoutItem()]
    [Tooltip("The current angle of the swing")]
    public float swingAngle;

    [Tooltip("The color of the debug lines drawn")]
    public Color debugColor = Color.white;

    private void Awake()
    {
        this.SetSwing();
        this.SpawnLinks();
    }
    private void Start()
    {
        //On water levels the swing gravity is halved if the swing is placed below the water level height
        if (GMWaterLevelManager.Instance().GetCurrentActIsWaterLevel() && this.transform.position.y <= GMWaterLevelManager.Instance().GetWaterLevelHeight())
        {
            this.swingGravity /= 2;
        }

        this.swingAngle = this.intitialAngle;
    }
    private void OnValidate() => this.SetSwing();
    private void FixedUpdate() => this.UpdateSwingAngleAndSpeed();

    /// <summary>
    /// Update the angle and speed of the swing
    /// </summary>
    private void UpdateSwingAngleAndSpeed()
    {
        this.currentSwingSpeed -= this.swingGravity * Mathf.Cos(this.swingAngle * Mathf.Deg2Rad);
        this.swingAngle += this.currentSwingSpeed * Time.deltaTime;

        if (this.swingAngle > 360)
        {
            this.swingAngle -= 360;
        }
    }

    /// <summary>
    /// Create links the size of the length specified
    /// </summary>
    private void SpawnLinks()
    {
        this.swingLinkToClone.linkID = 1;

        for (int x = 1; x < this.swingLength; x++)//Start at 1 as the cloned link contains the original link ID of 0
        {
            SwingLinkController clonedSwingLink = Instantiate(this.swingLinkToClone.gameObject).GetComponent<SwingLinkController>();
            clonedSwingLink.transform.parent = this.swingLinkToClone.transform.parent;
            clonedSwingLink.transform.position = this.swingLinkToClone.transform.position - new Vector3(0, x * this.linkItemHeight);
            clonedSwingLink.linkID = x + 1;
            clonedSwingLink.name = this.swingLinkToClone.name + " " + x;
        }

        this.activeSwingObject.transform.position = this.swingLinkToClone.transform.position - new Vector3(0, this.swingLength); //Place the active swing at the end of the link 
    }

    /// <summary>
    /// Set the swing based on the type chosen
    /// </summary>
    private void SetSwing()
    {
        if (this.swingType == SwingType.Platform)
        {
            this.swingPlatformTypes[1].gameObject.SetActive(false);
            this.swingPlatformTypes[0].gameObject.SetActive(true);
            this.activeSwingObject = this.swingPlatformTypes[0];
        }
        else if (this.swingType == SwingType.Spike)
        {
            this.swingPlatformTypes[0].gameObject.SetActive(false);
            this.swingPlatformTypes[1].gameObject.SetActive(true);
            this.activeSwingObject = this.swingPlatformTypes[1];

        }
    }

    private void OnDrawGizmos()
    {
        Vector2 startPosition = this.transform.position;
        float distance = (this.swingLength + 1) * this.linkItemHeight;//emulate the distance for the swing platform
        float debugSwingAngle = 180;
        float debugSwingSpeed = 0;
        List<Vector2> debugPositions = new List<Vector2>();

        for (int x = 0; x < 90; x++)//Visualize the movement
        {
            if (Mathf.RoundToInt(debugSwingAngle) == 360)
            { break; }
            startPosition.x = this.transform.position.x + (distance * Mathf.Cos(debugSwingAngle * Mathf.Deg2Rad));
            startPosition.y = this.transform.position.y + (distance * Mathf.Sin(debugSwingAngle * Mathf.Deg2Rad));
            debugSwingSpeed -= Mathf.RoundToInt(this.swingGravity * Mathf.Cos(debugSwingAngle * Mathf.Deg2Rad));
            debugSwingAngle += Mathf.RoundToInt(debugSwingSpeed / 16);
            debugPositions.Add(startPosition);
            if (x > 0)
            {
                Debug.DrawLine(debugPositions[x - 1], debugPositions[x], this.debugColor);
            }
        }
    }
}
