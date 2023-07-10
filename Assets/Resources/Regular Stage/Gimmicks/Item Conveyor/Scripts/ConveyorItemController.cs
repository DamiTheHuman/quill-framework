using UnityEngine;

///<summary>
/// An individual item which follows the path of its parent item conveyor
///</summary>
public class ConveyorItemController : MonoBehaviour
{
    [SerializeField]
    private ItemConveyorController itemConveyorController;
    [SerializeField, Tooltip("The unique identifier for the object")]
    public int id = 0;
    [SerializeField, Tooltip("The current target position within thhe curve")]
    public int currentPointTarget = 0;
    [SerializeField, Tooltip("The current delta of the object within the circular movement")]
    private float delta = 0;
    private Vector2 startPosition;
    private void Start() => this.startPosition = this.itemConveyorController.transform.position;
    private void FixedUpdate()
    {
        if (this.itemConveyorController != null)
        {
            if (this.itemConveyorController.itemConveyorInfo.GetRouteMovementType() == RouteMovementType.Bezier)
            {
                this.PointToPointItemMovement(this.transform.position);
            }
            else if (this.itemConveyorController.itemConveyorInfo.GetRouteMovementType() == RouteMovementType.Circular)
            {
                this.CircularItemMovement(this.transform.position);
            }
        }
    }
    /// <summary>
    /// Sets the item to follow the path of the parents bezier curve path within the appropriate delta offset
    /// <param name="itemConveyorController">The parent conveyor which holds the necessary details for the item</param>
    /// <param name="id">The unique id of the item which determines its placement within the path</param>
    /// </summary>
    public void SetBezierConveyor(ItemConveyorController itemConveyorController, int id)
    {
        this.itemConveyorController = itemConveyorController;
        this.id = id;
        this.startPosition = this.transform.position;
        this.currentPointTarget = itemConveyorController.itemConveyorInfo.GetIncrement() * this.id;
        this.transform.position = itemConveyorController.itemConveyorInfo.GetBezierPathPoints()[this.currentPointTarget];//Start at the spawn position based on id
    }


    /// <summary>
    /// Sets the item to follow the path of the parents circular path within the appropriate delta offset
    /// <param name="itemConveyorController">The parent conveyor which holds the necessary details for the item</param>
    /// <param name="id">The unique id of the item which determines its placement within the path</param>
    /// </summary>
    public void SetCircularConveyor(ItemConveyorController itemConveyorController, int id)
    {
        this.itemConveyorController = itemConveyorController;
        this.id = id;
        this.startPosition = this.transform.position;
        this.delta = id * itemConveyorController.itemConveyorInfo.GetItemWidth();
        Vector2 position;
        position.x = this.startPosition.x + (Mathf.Cos(this.delta * Mathf.Deg2Rad) * itemConveyorController.itemConveyorInfo.GetRadius());
        position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * itemConveyorController.itemConveyorInfo.GetRadius());
        this.transform.position = position;
    }
    /// <summary>
    /// Moves the item continously from one path to another
    /// <param name="position">The current item position</param>
    /// </summary>
    private void PointToPointItemMovement(Vector2 position)
    {

        if (position - (this.startPosition + this.itemConveyorController.itemConveyorInfo.GetBezierPathPoints()[this.currentPointTarget]) == Vector2.zero)
        {
            if (this.currentPointTarget == this.itemConveyorController.itemConveyorInfo.GetBezierPathPoints().Count - 1)
            {
                this.currentPointTarget = 0;

            }
            else
            {
                this.currentPointTarget++; // Update the target within the array list
            }
        }
        position = Vector2.MoveTowards(position, this.startPosition + this.itemConveyorController.itemConveyorInfo.GetBezierPathPoints()[this.currentPointTarget], this.itemConveyorController.itemConveyorInfo.GetSpeed() * Time.deltaTime);

        this.transform.position = position;

    }
    /// <summary>
    /// Moves the item in a circular motion
    /// <param name="position">The current item position</param>
    /// </summary>
    private void CircularItemMovement(Vector2 position)
    {
        position.x = this.startPosition.x + (Mathf.Cos(this.delta * Mathf.Deg2Rad) * this.itemConveyorController.itemConveyorInfo.GetRadius());
        position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.itemConveyorController.itemConveyorInfo.GetRadius());

        this.delta += this.itemConveyorController.itemConveyorInfo.GetSpeed() * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;

        if (this.delta > 360)
        {
            this.delta -= 360;
        }

        this.transform.position = position;
    }
}
