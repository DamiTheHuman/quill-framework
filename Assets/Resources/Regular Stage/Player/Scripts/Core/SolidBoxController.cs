using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// The controller handles the collisions that occur against objects on the Object layer 
/// This helps with interactions against springs and monitors alike while being in sync with the physics update
/// The collision is based on AABB collision where if a collision is found it notifies the object to verify the collision
/// </summary>
public class SolidBoxController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField, LastFoldoutItem()]
    private BoxCollider2D boxCollider2D;
    [Tooltip("The layers the solid box collider can interact with")]
    public LayerMask collisionMask;
    [Tooltip("The layers of solid contact gimmicks objects"), SerializeField]
    private LayerMask solidContactEventCollisionMask;
    [Tooltip("The layers of trigger contact gimmicks objects"), SerializeField]
    private LayerMask triggerContactEventCollisionMask;

    [Tooltip("The size of the solid box collider when regular")]
    public Rect normalBounds;
    [Tooltip("The size of the solid box collider when shrunk")]
    public Rect shrinkBounds;
    [Tooltip("The size of the solid box collider when gliding")]
    public Rect glideBounds;
    [Tooltip("The debug colour of the collider bounds")]
    public Color solidBoxDebugColor = General.RGBToColour(251, 242, 54, 170);
    [SerializeField, Tooltip("Extra offset applied to the colliders")]
    private Vector2 extraSolidBoxOffset = new Vector2(0, 0);

    [SerializeField, Tooltip("List of Contact Events gimmicks that are solid")]
    private List<ContactEvent> activeSolidContactEvents;
    [SerializeField, Tooltip("List of Contact Events gimmicks that are triggers")]
    private List<ContactEvent> activeTriggerContactEvents;

    [SerializeField, Tooltip("List of Contact Events Entered this frame")]
    private List<ContactEvent> onEnteredContactEvent;
    [SerializeField, Tooltip("List of Contact Events Exited this frame")]
    private List<ContactEvent> onExitedContactEvents;

    private void Reset()
    {
        this.player = this.GetComponentInParent<Player>();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        this.normalBounds.x = this.boxCollider2D.offset.x;
        this.normalBounds.y = this.boxCollider2D.offset.y;
        this.normalBounds.width = this.boxCollider2D.size.x;
        this.normalBounds.height = this.boxCollider2D.size.y;
    }

    /// <summary>
    ///  Check for collisions with solid objects
    /// </summary>
    public bool CheckSolidContactEventCollisions()
    {
        this.activeSolidContactEvents = this.ValidateContactEventCollisionByBoxCast(this.activeSolidContactEvents, this.solidContactEventCollisionMask);
        return this.activeSolidContactEvents != null;
    }

    /// <summary>
    /// Check for collisions with trigggers
    /// </summary>
    public bool CheckCollisionWithContactEvent<T>()
    {
        if (typeof(T) == typeof(TriggerContactGimmick))
        {
            this.activeSolidContactEvents = this.ValidateContactEventCollisionByBoxCast(this.activeSolidContactEvents, this.solidContactEventCollisionMask);

            return this.activeSolidContactEvents != null;
        }
        else if (typeof(T) == typeof(SolidContactGimmick))
        {
            this.activeTriggerContactEvents = this.ValidateContactEventCollisionByBoxCast(this.activeTriggerContactEvents, this.triggerContactEventCollisionMask);

            return this.activeTriggerContactEvents != null;
        }

        Debug.LogError("Invalid Contact Event Specificed Must be {Trigger} or {Solid}" + typeof(T));

        return false;
    }

    /// <summary>
    /// Check Contact event Collisions by casting a box
    /// <param name="currentGimmick">The current list of activ contact events</param>
    /// <param name="layerMask">The layer mask to check</param>
    /// </summary>
    private List<ContactEvent> ValidateContactEventCollisionByBoxCast(List<ContactEvent> currentGimmick, LayerMask layerMask)
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Die>())
        {
            return null;
        }

        this.UpdateSolidBoxAngle(this.player.GetSpriteController().GetSpriteAngle());
        Bounds bounds = this.CalculateBoxColliderBounds();

        Collider2D[] solidBoxCollisions = Physics2D.OverlapBoxAll(bounds.center, new Vector2(bounds.size.x, bounds.size.y), 0, layerMask);//Perform AABB collision
        List<ContactEvent> validContactEvents = new List<ContactEvent>();

        foreach (Collider2D solidBoxCollision in solidBoxCollisions)
        {
            if (solidBoxCollision)
            {
                ContactEvent hedgeSolidGimmick = solidBoxCollision.transform.GetComponent<ContactEvent>();

                if (hedgeSolidGimmick == null)
                {
                    continue;
                }

                validContactEvents.Add(hedgeSolidGimmick);
            }
        }

        this.UpdateCurrentContactEvents(validContactEvents, ref currentGimmick);

        return currentGimmick;
    }

    /// <summary>
    /// Checks for an object interaction and performs the specified action if the its true based on the sensor data
    /// Raycasts are restricted to <see cref="SolidContactGimmick"/> events
    /// <param name="sensorHit">The sensor to check if it hit a solid gimmick</param>
    /// </summary>
    public bool ValidateContactEventCollisionByRacastHit(RaycastHit2D sensorHit, ref SolidContactGimmick hedgeSolidGimmick)
    {
        if (sensorHit && General.ContainsLayer(this.solidContactEventCollisionMask, sensorHit.transform.gameObject.layer))
        {
            hedgeSolidGimmick = sensorHit.transform.GetComponent<SolidContactGimmick>();
        }

        if (hedgeSolidGimmick == null)
        {
            return false;
        }

        this.UpdateCurrentContactEvents(new List<ContactEvent>() { hedgeSolidGimmick }, ref this.activeSolidContactEvents);

        return hedgeSolidGimmick != null && this.activeSolidContactEvents.Contains(hedgeSolidGimmick);
    }

    /// <summary>
    /// Update the list of current contants based on a set of valid contact events, removing contact events do not appear in that list
    /// <param name="validContactEvents">A list of valid contact events</param>
    /// <param name="currentContactEvents">A list of current contact events</param>
    /// </summary>
    private void UpdateCurrentContactEvents(List<ContactEvent> validContactEvents, ref List<ContactEvent> currentContactEvents)
    {
        this.onEnteredContactEvent.Clear();
        this.onExitedContactEvents.Clear();

        //TODO: Figure if we can optimise this 
        for (int x = 0; x < currentContactEvents.Count; x++)
        {
            ContactEvent currentContactEvent = currentContactEvents[x];

            if (validContactEvents.Contains(currentContactEvent) == false)
            {
                this.onExitedContactEvents.Add(currentContactEvent);

                continue;
            }

            currentContactEvent.HedgeOnCollisionStay(this.player);
        }

        foreach (ContactEvent validContactEvent in validContactEvents)
        {
            if (currentContactEvents.Contains(validContactEvent) == false)
            {
                if (validContactEvent.HedgeIsCollisionValid(this.player, this.CalculateBoxColliderBounds()))
                {
                    this.onEnteredContactEvent.Add(validContactEvent);
                }
            }
        }

        foreach (ContactEvent contactEvent in this.onEnteredContactEvent)
        {
            //If we are homing at the current object trigger an event
            if (this.player.GetActionManager().CheckActionIsBeingPerformed<HomingAttack>())
            {
                HomingAttack homingAttack = this.player.GetActionManager().GetAction<HomingAttack>() as HomingAttack;
                if (homingAttack.GetHomingAttackMode() == HomingAttackMode.Homing && homingAttack.GetCurrentTarget() == contactEvent.gameObject)
                {
                    homingAttack.OnHitTargetObject(contactEvent);
                }
            }

            currentContactEvents.Add(contactEvent);
            contactEvent.HedgeOnCollisionEnter(this.player);
        }

        foreach (ContactEvent contactEvent in this.onExitedContactEvents)
        {
            currentContactEvents.Remove(contactEvent);
            contactEvent.HedgeOnCollisionExit(this.player);
        }
    }

    /// <summary>
    /// Check if the recently Entered contact event has an object of the passed in type
    /// </summary>
    public bool NextContactEventOfType<T>() => this.onEnteredContactEvent.OfType<T>().Count() > 0;

    /// <summary>
    /// Check if the recently exited contact event has an object of the passed in type
    /// </summary>
    public bool PreviousContactEventOfType<T>() => this.onExitedContactEvents.OfType<T>().Count() > 0;

    /// <summary>
    /// Check if the current contact event is within the active event
    /// </summary>
    public bool ContactEventIsActive(ContactEvent contactEvent) => this.activeSolidContactEvents.Contains(contactEvent) || this.activeTriggerContactEvents.Contains(contactEvent);

    /// <summary>
    /// Gets a list of active solid contact events
    /// </summary>
    public List<ContactEvent> GetActiveSolidContactEvents() => this.activeSolidContactEvents;

    /// <summary>
    /// Gets a list of active trigger contact events
    /// </summary>
    public List<ContactEvent> GetActiveTriggerContactEvents() => this.activeTriggerContactEvents;

    /// <summary>
    /// Check whether we have any gimmicks in the specified state
    /// </summary>
    public bool HasGimmickInState(CollisionState collisionState) => this.activeTriggerContactEvents.FirstOrDefault(x => x.GetCollisionState() == collisionState) != null || this.activeSolidContactEvents.FirstOrDefault(x => x.GetCollisionState() == collisionState) != null;

    /// <summary>
    /// Check if the player is about to enter a gimmick of the specified type
    /// </summary>
    public bool IsEnteringEventOfType<T>() => this.onEnteredContactEvent.OfType<T>().Count() > 0;

    /// <summary>
    /// Check if the player is touching a contact event of the passed type
    /// </summary>
    public bool IsTouchingEventOfType<T>() => this.activeSolidContactEvents.OfType<T>().Count() > 0 || this.activeTriggerContactEvents.OfType<T>().Count() > 0;

    /// <summary>
    /// Check if the solid box is interacting with any contact event
    /// </summary>
    public bool HasActiveContactEvents() => this.activeSolidContactEvents.Count > 0 || this.activeTriggerContactEvents.Count > 0;

    /// <summary>
    /// Swaps the solid box controller data with the new target solid box controller
    /// <param name="targetSolidBoxController">The solid box controller to swap to</param>
    /// </summary>
    public void SwapSolidBoxController(SolidBoxController targetSolidBoxController)
    {
        this.normalBounds = targetSolidBoxController.normalBounds;
        this.shrinkBounds = targetSolidBoxController.shrinkBounds;

        if (this.player.GetActionManager().currentPrimaryAction != null)
        {
            this.UpdateSolidBoxBounds(this.player.GetActionManager().currentPrimaryAction.sizeMode);
        }
        else
        {
            this.UpdateSolidBoxBounds(SizeMode.Regular);
        }
    }

    /// <summary>
    /// Updates the angle of the solid box
    /// <param name="angle">The new angle of the solid box </param>
    /// </summary>
    public void UpdateSolidBoxAngle(float angle) => this.transform.rotation = Quaternion.Euler(0f, 0f, angle);

    /// <summary>
    /// Updates the current colliders bounds based on the size mode
    /// <param name="sizeMode">The new size mode of the solid box</param>
    /// </summary>
    public void UpdateSolidBoxBounds(SizeMode sizeMode)
    {
        switch (sizeMode)
        {
            case SizeMode.Regular:
                this.boxCollider2D.offset = new Vector2(this.normalBounds.x, this.normalBounds.y);
                this.boxCollider2D.size = new Vector2(this.normalBounds.width, this.normalBounds.height);

                break;
            case SizeMode.Shrunk:
                this.boxCollider2D.offset = new Vector2(this.shrinkBounds.x, this.shrinkBounds.y);
                this.boxCollider2D.size = new Vector2(this.shrinkBounds.width, this.shrinkBounds.height);

                break;
            case SizeMode.Gliding:
                this.boxCollider2D.offset = new Vector2(this.glideBounds.x, this.glideBounds.y);
                this.boxCollider2D.size = new Vector2(this.glideBounds.width, this.glideBounds.height);

                break;
            default:
                break;
        }

        this.boxCollider2D.offset += this.extraSolidBoxOffset;
    }

    /// <summary>
    /// Get the box collider of the solid box
    /// </summary>
    public BoxCollider2D GetBoxCollider2D() => this.boxCollider2D;

    /// <summary>
    /// Set the extra solid box offset
    /// <param name="extraOffset">The extra offset applied to the solid box</param>
    /// </summary>
    public void SetExtraSolidBoxOffset(Vector2 extraOffset)
    {
        this.extraSolidBoxOffset = extraOffset;
        this.UpdateSolidBoxBounds(this.player.GetSensors().GetSizeMode());
    }

    /// <summary>
    /// Updates the box collider state
    /// <param name="state">The new state of the collider</param>
    /// </summary>
    public void SetSolidBoxColliderEnabled(bool state) => this.boxCollider2D.enabled = state;

    /// <summary>
    /// Checks if the player is interacting with an object of a specific tytpe
    /// </summary>
    public bool InteractingWithContactEventOfType<ObjectType>(GameObject gameObject)
    {
        if (this.activeSolidContactEvents.Count == 0)
        {
            return false;
        }

        foreach (SolidContactGimmick action in this.activeSolidContactEvents)
        {
            if (action is ObjectType && gameObject != action.gameObject)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes a contact event from the list of active contact events
    /// <param name="contactEvent">The contact event to remove</param>
    /// </summary>
    public void RemoveActiveContactEvent(ContactEvent contactEvent)
    {
        this.activeSolidContactEvents.Remove(contactEvent);
        this.activeTriggerContactEvents.Remove(contactEvent);
    }

    /// <summary>
    /// Calculate the current bounds of the box collider, this is needed because sometimes using <see cref="boxCollider2D.bounds"/> is out of sync
    /// </summary>
    public Bounds CalculateBoxColliderBounds()
    {
        Bounds bounds = this.boxCollider2D.bounds;
        bounds.center = (Vector2)this.transform.position + this.boxCollider2D.offset;

        return bounds;
    }
    private void OnDrawGizmos()
    {
        BoxCollider2D boxCollider2D = this.GetComponent<BoxCollider2D>();
        GizmosExtra.DrawRect(this.transform, boxCollider2D, this.solidBoxDebugColor, true);

        if (Application.isPlaying == false)
        {
            this.normalBounds.x = boxCollider2D.offset.x;
            this.normalBounds.y = boxCollider2D.offset.y;
            this.normalBounds.width = boxCollider2D.size.x;
            this.normalBounds.height = boxCollider2D.size.y;
            this.normalBounds.x += this.extraSolidBoxOffset.x;
            this.normalBounds.y += this.extraSolidBoxOffset.y;
        }
    }
}
