using System.Collections;
using UnityEngine;
/// <summary>
/// Contact event parent for all gimmick types except special stage gimmicks
/// </summary>
public class ContactEvent : MonoBehaviour
{
    [IsDisabled, Tooltip("The current collision state of the object"), SerializeField]
    private CollisionState collisionState = CollisionState.Inactive;
    [SerializeField, IsDisabled]
    protected Vector2 startPosition;

    private IEnumerator delegateCollisionStateSwitchCoroutine;
    protected virtual void Start() => this.startPosition = this.transform.position;

    public virtual void Reset()
    {
        if (!(this is TriggerContactGimmick) && this.gameObject.GetComponent<Rigidbody2D>() == null)
        {
            this.gameObject.AddComponent<Rigidbody2D>();
            this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    /// <summary>
    /// Resets the delegate coroutine to null if its currently running
    /// </summary>
    private void ResetDelegateCollisionStateSwitchCoroutine()
    {
        if (this.delegateCollisionStateSwitchCoroutine != null)
        {
            GMStageManager.Instance().StopCoroutine(this.delegateCollisionStateSwitchCoroutine);
        }

        this.delegateCollisionStateSwitchCoroutine = null;
    }

    /// <summary>
    /// Checks whether to activate the collider based on the conditions set in here defaulting to true
    /// <param name="player">The player obect  </param>
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public virtual bool HedgeIsCollisionValid(Player player, Bounds targetColliderBounds) => true;

    /// <summary>
    /// The action to be performed when a collision occurs
    /// <param name="player">The player obect </param>
    /// </summary>
    public virtual void HedgeOnCollisionEnter(Player player) => this.SetCollisionState(CollisionState.OnCollisionEnter);

    /// <summary>
    /// The action to be performed while the player is actively in contact with the object
    /// <param name="player">The player obect </param>
    /// </summary>
    public virtual void HedgeOnCollisionStay(Player player) { }

    /// <summary>
    /// The action to be performed when contact with the gimmick ends
    /// <param name="player">The player obect </param>
    /// </summary>
    public virtual void HedgeOnCollisionExit(Player player) => this.SetCollisionState(CollisionState.OnCollisionExit);

    /// <summary>
    /// Checks whether the target is infront of the gameobject from either sides via its furthest ahead boundsaries
    /// This based on how far left or right the player is from the objects center bounds
    /// <param name="gameObjectBounds">The bounds of the current game object </param>
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public bool TargetBoundsAreWithHorizontalBounds(Bounds gameObjectBounds, Bounds targetColliderBounds)
    {
        float solidBoxBoundsMaxX = General.RoundToDecimalPlaces(targetColliderBounds.max.x);
        float solidBoxBoundsMinX = General.RoundToDecimalPlaces(targetColliderBounds.min.x);
        float gameObjectBoundsMinX = General.RoundToDecimalPlaces(gameObjectBounds.min.x);
        float gameObjectBoundsMaxX = General.RoundToDecimalPlaces(gameObjectBounds.max.x);

        return solidBoxBoundsMaxX > gameObjectBoundsMinX && solidBoxBoundsMinX < gameObjectBoundsMaxX;
    }

    /// <summary>
    /// Checks whether the target is vertically infront of the gameobject from either sides 
    /// This is based on how far upwards or downards the player is from the objects vertical center point bounds
    /// <param name="gameObjectBounds">The bounds of the current game object </param>
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public bool TargetBoundsAreWithinVerticalBounds(Bounds gameObjectBounds, Bounds targetColliderBounds)
    {
        float solidBoxBoundsMaxY = General.RoundToDecimalPlaces(targetColliderBounds.max.y);
        float solidBoxBoundsMinY = General.RoundToDecimalPlaces(targetColliderBounds.min.y);
        float gameObjectBoundsMinY = General.RoundToDecimalPlaces(gameObjectBounds.min.y, 1);
        float gameObjectBoundsMaxY = General.RoundToDecimalPlaces(gameObjectBounds.center.y, 1);

        return solidBoxBoundsMaxY > gameObjectBoundsMinY && solidBoxBoundsMinY < gameObjectBoundsMaxY;
    }

    /// <summary>
    /// Checks whether the target is to the to of the object
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public bool TargetIsToTheTop(Bounds targetColliderBounds) => targetColliderBounds.min.y > this.transform.position.y;

    /// <summary>
    /// Checks whether the target is to the right of the object
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public bool TargetIsToTheRight(Bounds targetColliderBounds) => targetColliderBounds.min.x > this.transform.position.x;

    /// <summary>
    /// Checks whether the target is to the left of the object
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public bool TargetIsToTheLeft(Bounds targetColliderBounds) => targetColliderBounds.max.x < this.transform.position.x;

    /// <summary>
    /// Checks whether the target is at the bottom of the object
    /// <param name="targetColliderBounds"/> The target colliders bounds</param>
    /// </summary>
    public bool TargetIsToTheBottom(Bounds targetColliderBounds) => targetColliderBounds.max.y < this.transform.position.y;

    /// <summary>
    /// Sets the collision state of the object and updates it appropriately for events that last a frame
    /// <param name="collisionState"/> The collision state to set to </param>
    /// </summary>
    public void SetCollisionState(CollisionState collisionState)
    {
        this.ResetDelegateCollisionStateSwitchCoroutine();

        switch (collisionState)
        {
            case CollisionState.OnCollisionEnter:
                if (this.gameObject.activeSelf)
                {
                    this.delegateCollisionStateSwitchCoroutine = GMStageManager.Instance().DelegateTillNextFixedUpdateCycles(() => this.SetCollisionState(CollisionState.OnCollisionStay));
                }

                this.collisionState = collisionState;

                break;
            case CollisionState.OnCollisionExit:
                if (this.gameObject.activeSelf)
                {
                    this.delegateCollisionStateSwitchCoroutine = GMStageManager.Instance().DelegateTillNextFixedUpdateCycles(() => this.SetCollisionState(CollisionState.Inactive));
                }

                this.collisionState = collisionState;

                break;
            case CollisionState.Inactive:
            case CollisionState.OnCollisionStay:
            default:
                this.collisionState = collisionState;

                break;
        }
    }

    /// <summary>
    /// Gets the current collision state
    /// </summary>
    public CollisionState GetCollisionState() => this.collisionState;

    /// <summary>
    /// Gets the start position of the contact event
    /// </summary>
    public Vector2 GetStartPosition() => this.startPosition;
}
