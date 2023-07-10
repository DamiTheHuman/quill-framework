using System.Collections;
using UnityEngine;
/// <summary>
/// Base class for all the collision info used by the player primarily, ground, ceiling and wall
/// </summary>
[System.Serializable]
public class PlayerCollisionInfo
{
    [Tooltip("The current player object"), SerializeField]
    protected Player player;
    [IsDisabled, Tooltip("The current collision state of the object"), SerializeField]
    private CollisionState collisionState = CollisionState.Inactive;
    [SerializeField]
    private CollisionInfoData currentCollisionInfo = new CollisionInfoData();
    [SerializeField]
    private CollisionInfoData previousCollisionInfo = new CollisionInfoData();

    [Tooltip("What the player ground sensors can collide with"), SerializeField]
    protected LayerMask collisionMask;

    [SerializeField, FirstFoldOutItem("Debug")]
    private CollisionInfoData debugPreviousCollisionInfo = new CollisionInfoData();
    [SerializeField]
    protected Color leftSensorColor = new Color(0, 0.9411765f, 0, 1);
    [SerializeField, LastFoldoutItem()]
    protected Color rightSensorColor = new Color(0.2196078f, 1f, 0.6352941f, 1);

    private IEnumerator collisionStateSwitchCoroutine = null;

    public PlayerCollisionInfo(Player player)
    {
        this.player = player;
        this.Reset();
    }

    /// <summary>
    /// Resets collisionInfo to defaults
    /// </summary>
    public void Reset() => this.SetCurrentCollisionInfo(new CollisionInfoData());

    /// <summary>
    /// Resets the delegate coroutine to null if its currently running
    /// </summary>
    private void ResetDelegateCollisionStateSwitchCoroutine()
    {
        if (this.collisionStateSwitchCoroutine != null)
        {
            GMStageManager.Instance().StopCoroutine(this.collisionStateSwitchCoroutine);
        }

        this.collisionStateSwitchCoroutine = null;
    }


    /// <summary>
    /// On the update of the collision info
    /// </summary>
    public virtual void Update(ref Vector2 velocity)
    {
        if (this.currentCollisionInfo.GetHit())
        {
            this.OnCollisionStay();
        }
    }

    /// <summary>
    /// Sets the current player
    /// </summary>
    public void SetPlayer(Player player) => this.player = player;

    /// <summary>
    /// Sets the value for the current collision info and keeps a reference in the previous collisionInfo
    /// </summary>
    public void SetCurrentCollisionInfo(CollisionInfoData collisionInfo)
    {
        if (collisionInfo.GetHit() && !this.currentCollisionInfo.GetHit())
        {
            this.SetCollisionState(CollisionState.OnCollisionEnter);
        }
        else if (collisionInfo.GetHit() == false)
        {
            if (this.currentCollisionInfo.GetHit() && (this.GetCollisionState() != CollisionState.OnCollisionExit || this.GetCollisionState() != CollisionState.Inactive))
            {
                this.SetCollisionState(CollisionState.OnCollisionExit);
            }
        }

        this.previousCollisionInfo.SetCollisionInfo(this.currentCollisionInfo);
        this.currentCollisionInfo = collisionInfo;

        if (this.previousCollisionInfo.GetHit())
        {
            this.debugPreviousCollisionInfo.SetCollisionInfo(this.previousCollisionInfo);
        }
    }

    /// <summary>
    /// Gets the current collision info
    /// </summary>
    public CollisionInfoData GetCurrentCollisionInfo() => this.currentCollisionInfo;

    /// <summary>
    /// Gets the previous collision info before it was updated
    /// </summary>
    public CollisionInfoData GetPreviousCollisionInfo() => this.previousCollisionInfo;

    /// <summary>
    /// Determines whether a collision should happen
    /// </summary>
    public virtual void CheckForCollision(Vector2 position, ref Vector2 velocity) { }

    /// <summary>
    /// Validates the collision info based on settings set
    /// </summary>
    public virtual CollisionInfoData ValidateCollisionContact(CollisionInfoData collisionInfo, Vector2 position, ref Vector2 velocity, float extensionCheck = 0) => collisionInfo;

    /// <summary>
    /// Calculate repositioning post contact
    /// </summary>
    public virtual Vector2 CalculateCollisionRepositioning(Vector2 position, CollisionInfoData groundInfo) => position;

    /// <summary>
    /// In the event an initial collision occurs
    /// </summary>
    public virtual void OnCollisionEnter() { }

    /// <summary>
    /// While colliding with something think of stick to ground
    /// </summary>
    public virtual void OnCollisionStay() { }

    /// <summary>
    /// When the collision is exit
    /// </summary>
    public virtual void OnCollisionExit() { }

    /// <summary>
    /// Sets the collision state of the object and updates it appropriately for events that last a frame
    /// <param name="collisionState"/> The collision state to set to </param>
    /// </summary>
    public void SetCollisionState(CollisionState collisionState)
    {
        if (this.collisionState == collisionState)
        {
            return;
        }

        this.ResetDelegateCollisionStateSwitchCoroutine();

        switch (collisionState)
        {
            case CollisionState.OnCollisionEnter:
                this.collisionStateSwitchCoroutine = GMStageManager.Instance().DelegateTillNextFixedUpdateCycles(() => this.SetCollisionState(CollisionState.OnCollisionStay));
                this.collisionState = collisionState;
                this.OnCollisionEnter();

                break;
            case CollisionState.OnCollisionExit:
                this.collisionStateSwitchCoroutine = GMStageManager.Instance().DelegateTillNextFixedUpdateCycles(() => this.SetCollisionState(CollisionState.Inactive));
                this.collisionState = collisionState;
                this.OnCollisionExit();

                break;
            case CollisionState.Inactive:
            case CollisionState.OnCollisionStay:
            default:
                this.collisionState = collisionState;

                break;
        }
    }

    /// <summary>
    /// Converts the current velocity to ground velocity based on the grounds angle
    /// </summary
    public virtual Vector2 ConvertVelocityOnContactToGroundVelocity(Vector2 velocity, CollisionInfoData collisionInfo) => velocity;

    /// <summary>
    /// Adds a Layer to the ground layer mask
    /// <param name="layer"> The layer to addk</param>
    /// </summary>
    public void AddToCollisionMask(int layer) => this.collisionMask |= 1 << layer;

    /// <summary>
    /// Removes a layer from the ground Layer
    /// <param name="layer"> The layer to remove </param>
    /// </summary>
    public void RemoveFromCollisionMask(int layer) => this.collisionMask &= ~(1 << layer);

    /// <summary>
    /// Get the current collision mask
    /// </summary>
    public LayerMask GetCollisionMask() => this.collisionMask;

    /// <summary>
    /// Checks if a layer is in the current collision mask
    /// <param name="layer">The layer to check for</param>
    /// </summary>
    public bool IsLayerIsInCollisionMask(int layer) => this.collisionMask == (this.collisionMask | (1 << layer));

    /// <summary>
    /// A Check to see if the player is about to interact with a solid gimmick that is breakable on interaction
    /// A solid gimmick being breakable or hazardous means on interaction the players velocity and position should not be manipulated unless specified
    /// <param name="collisionInfo"> The current collision info being checked</param>
    /// </summary>
    protected SolidContactGimmickType CheckNewCollisionWithSolidGimmick(CollisionInfoData collisionInfo)
    {
        SolidContactGimmick currentSolidContactGimmick = null;
        bool gimmickCollisionIsValid = this.player.GetSolidBoxController().ValidateContactEventCollisionByRacastHit(collisionInfo.GetHit(), ref currentSolidContactGimmick);

        if (currentSolidContactGimmick != null && gimmickCollisionIsValid)
        {
            return currentSolidContactGimmick.solidGimmickType;
        }

        return SolidContactGimmickType.NoCollision;
    }

    /// <summary>
    /// Updates the collision info angle data using the atan angle from the left sensor hit point and right sensor hit point as opposed to the normal
    /// <param name="collisionInfo"> The current ground info identified</param>
    /// <param name="leftSensorHit"> The left sensor </param>
    /// <param name="rightSensorHit"> The right sensor</param>
    /// </summary>
    protected CollisionInfoData CalculateCollisionInfoWithAtan(CollisionInfoData collisionInfo, RaycastHit2D leftSensorHit, RaycastHit2D rightSensorHit)
    {
        float atanAngleInRadians = Mathf.Atan2(rightSensorHit.point.y - leftSensorHit.point.y, rightSensorHit.point.x - leftSensorHit.point.x);
        float atanAngleInDegrees = Mathf.Rad2Deg * atanAngleInRadians;
        atanAngleInDegrees = atanAngleInDegrees < 0 ? atanAngleInDegrees + 360 : atanAngleInDegrees;
        atanAngleInDegrees = atanAngleInDegrees >= 360 ? atanAngleInDegrees - 360 : atanAngleInDegrees;
        collisionInfo.SetAngleInDegrees(General.RoundToDecimalPlaces(atanAngleInDegrees));
        collisionInfo.ClampAngleData();

        return collisionInfo;
    }

    /// <summary>
    /// Gets the current collision state
    /// </summary>
    public CollisionState GetCollisionState() => this.collisionState;

    /// <summary>
    /// Switches to the next collision state after one fixed update frame useful of states like on entry or on exit
    /// <param name="startState"/> The state to start at</param>
    /// <param name="endState"/> The state to end at after a frame</param>
    /// </summary>
    private IEnumerator SwitchToCollisionStateAfterFrame(CollisionState startState, CollisionState endState)
    {
        this.collisionState = startState;

        yield return new WaitForFixedUpdate();

        this.collisionState = endState;

        yield return null;
    }

    public virtual void OnDrawGimzos() { }
}
