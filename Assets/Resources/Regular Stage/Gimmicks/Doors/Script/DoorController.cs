using UnityEngine;
/// <summary>
/// A controller object that moves a set door object upwards or downwards based on it state
/// </summary> 
public class DoorController : MonoBehaviour
{
    [SerializeField]
    private bool flipOpenDirection = false;
    [Help("The object set as the target is the actual object to have its position manipulated")]
    [SerializeField, Tooltip("This actual door object being moved")]
    private Transform solidDoor = null;
    [Tooltip("The current state of the door"), SerializeField]
    private DoorState doorState = DoorState.Idle;
    [SerializeField, Tooltip("This maximum height the door can reach"), FirstFoldOutItem("Door Movement Info")]
    private float doorMaxHeight = 64f;
    [SerializeField, Tooltip("This current height of the door")]
    private float currentDoorHeight = 0;
    [SerializeField, Tooltip("The position the door is moved by every step when opening or closing")]
    private float dropSpeed = 16;

    [SerializeField, Tooltip("Dust spawned when the door is successfully closed"), FirstFoldOutItem("Door Dust Info"), LastFoldoutItem()]
    public Transform doorSlamDustSpawnPosition;

    // Start is called before the first frame update
    private void Start()
    {
        this.currentDoorHeight = 0;
        this.solidDoor.transform.position += new Vector3(0, this.currentDoorHeight);//Start at the dropped position
    }

    private void FixedUpdate()
    {
        Vector2 position = this.transform.position;

        if (this.doorState == DoorState.Closing)
        {
            this.CloseDoor();
        }
        else if (this.doorState == DoorState.Opening)
        {
            this.OpenDoor();
        }

        this.currentDoorHeight = Mathf.Clamp(this.currentDoorHeight, 0, this.doorMaxHeight);
        float doorAngleInRadians = this.transform.eulerAngles.z * Mathf.Deg2Rad;

        if (this.flipOpenDirection)
        {
            doorAngleInRadians *= -1;
        }

        this.solidDoor.transform.position = position + new Vector2(Mathf.Sin(doorAngleInRadians) * -this.currentDoorHeight, Mathf.Cos(doorAngleInRadians) * this.currentDoorHeight);
    }

    /// <summary>
    /// Sets the state of te door
    /// <param name="doorState">THe new door state  </param>
    /// </summary> 
    public void SetDoorState(DoorState doorState)
    {
        if (this.doorState == DoorState.Idle)
        {
            if (doorState == DoorState.Closing && this.currentDoorHeight == 0)
            {
                return;
            }
            else if (doorState == DoorState.Opening && this.currentDoorHeight == this.doorMaxHeight)
            {
                return;
            }
        }

        this.doorState = doorState;
    }

    /// <summary>
    /// Get the door state
    /// </summary> 
    internal DoorState GetDoorState() => this.doorState;

    /// <summary>
    /// Open the door by incrementing the  vertical limit
    /// </summary> 
    private void OpenDoor()
    {
        this.currentDoorHeight += this.dropSpeed;

        if (this.currentDoorHeight >= this.doorMaxHeight)
        {
            this.SetDoorState(DoorState.Idle);
        }
    }

    /// <summary>
    /// Closes the door by decrementing the vertical limit
    /// </summary> 
    private void CloseDoor()
    {
        this.currentDoorHeight -= this.dropSpeed;

        if (this.currentDoorHeight <= 0)
        {
            this.SlamDoor();
            this.SetDoorState(DoorState.Idle);
        }
    }

    /// <summary>
    /// When the door is fully closed after being initially open
    /// </summary> 
    private void SlamDoor()
    {
        if (this.doorSlamDustSpawnPosition != null)
        {
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.MotobugExhaust, this.doorSlamDustSpawnPosition.position);

        }
    }

    /// <summary>
    /// Gets the flip open direction flag
    /// </summary>
    public bool GetFlipOpenDirection() => this.flipOpenDirection;
}
