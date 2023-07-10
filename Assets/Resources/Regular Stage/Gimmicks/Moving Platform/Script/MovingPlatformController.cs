using System;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(1000)]
/// <summary>
/// A moving platform object that is constantly moving which can be used to guide the player
/// </summary>
public class MovingPlatformController : PlatformController
{
    [SerializeField, Tooltip("The platform type of the object")]
    private PlatformType platformType = PlatformType.Circular;
    [Header("General Platform Parameters"), SerializeField, Tooltip("The speed that delta is incremented with")]
    private float speed = 1;
    [SerializeField, Tooltip("The current delta of the platform movement affecting its position within its lifetime")]
    private float delta = 0;
    [SerializeField, Tooltip("The radius of length of the platforms movement boundaries")]
    private float range = 60;
    [Header("Circle Parameters"), SerializeField, Tooltip("The type of linear movement type"), EnumConditionalEnable("platformType", (int)PlatformType.Circular)]
    private bool useParentAsStart;

    [Header("Linear Parameters"), SerializeField, Tooltip("The type of linear movement type"), EnumConditionalEnable("platformType", (int)PlatformType.Linear)]
    private LinearMovementType linearMovementType = LinearMovementType.Horizontal;
    [Range(0, 360)]
    [SerializeField, Tooltip("The angle of the diagonal movement type"), EnumConditionalEnable("linearMovementType", (int)LinearMovementType.Diagonal)]
    private float angleInDegrees = 45;

    [Header("Point to Point Parameters"), SerializeField, Tooltip("A list of points for the controller to travel through"), EnumConditionalEnable("platformType", (int)PlatformType.PointToPoint)]
    private List<Vector2> relativeTargetPoints = new List<Vector2>();
    [SerializeField, Tooltip("The current targetPoint being navigated"), EnumConditionalEnable("platformType", (int)PlatformType.PointToPoint)]
    private int currentPointTarget = 0;
    [SerializeField, Tooltip("A flag to determine whether to reverse the platform after traversing all the points"), EnumConditionalEnable("platformType", (int)PlatformType.PointToPoint)]
    private bool reverseAtEnd = false;

    [SerializeField, Tooltip("Debug color")]
    private Color debugColor = Color.white;

    /// <inheritdoc>
    /// <see cref="PlatformController"/>
    /// </inheritdoc>
    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    /// <inheritdoc>
    /// <see cref="PlatformController"/>
    /// </inheritdoc>
    protected override void Start()
    {
        base.Start();

        if (this.platformType == PlatformType.PointToPoint)
        {
            this.transform.position = this.relativeTargetPoints[this.currentPointTarget] + this.startPosition;
        }

        this.SetStartPositions();
    }

    private void FixedUpdate()
    {
        switch (this.platformType)
        {
            case PlatformType.Circular:
                this.CircularPlatformMovement(this.transform.position);

                break;
            case PlatformType.Linear:
                this.LinearPlatformMovement(this.transform.position);

                break;
            case PlatformType.PointToPoint:
                this.PointToPointPlatformMovement(this.transform.position);

                break;
            default:
                break;
        }

        this.delta = General.ClampDegreeAngles(this.delta);

        this.SyncChildPositions();
    }

    /// <summary>
    /// Sets the platform at the appropriat poisition when spawned
    /// </summary>
    private void SetStartPositions()
    {
        float previousDelta = this.delta;

        if (this.delta != 0)
        {
            switch (this.platformType)
            {
                case PlatformType.Circular:
                    this.CircularPlatformMovement(this.transform.position);
                    break;
                case PlatformType.Linear:
                    this.LinearPlatformMovement(this.transform.position);
                    break;
                case PlatformType.PointToPoint:
                    this.PointToPointPlatformMovement(this.transform.position);
                    break;
                default:
                    break;
            }

        }

        previousDelta = this.delta;
    }

    /// <summary>
    /// Moves the platform linearly which could either be vertically, horizontally or diagonally
    /// <param name="position">The current platform position</param>
    /// </summary>
    private void LinearPlatformMovement(Vector2 position)
    {
        switch (this.linearMovementType)
        {
            case LinearMovementType.Horizontal:
                position.x = this.startPosition.x + (Mathf.Cos(this.delta * Mathf.Deg2Rad) * this.range);

                break;
            case LinearMovementType.Vertical:
                position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.range);

                break;
            case LinearMovementType.Diagonal:
                position.x = this.startPosition.x + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.range * Mathf.Cos(this.angleInDegrees * Mathf.Deg2Rad));
                position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.range * Mathf.Sin(this.angleInDegrees * Mathf.Deg2Rad));

                break;
            default:
                break;
        }

        this.delta += this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.transform.position = position;
    }

    /// <summary>
    /// Moves the platform in a circular motion
    /// <param name="position">The current platform position</param>
    /// </summary>
    private void CircularPlatformMovement(Vector2 position)
    {
        if (this.useParentAsStart)
        {
            this.startPosition = this.transform.parent.position;
        }

        position.x = this.startPosition.x + (Mathf.Cos(this.delta * Mathf.Deg2Rad) * this.range);
        position.y = this.startPosition.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.range);

        this.delta += this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        this.transform.position = position;
    }

    /// <summary>
    /// Moves the platform continously from one path to another
    /// <param name="position">The current platform position</param>
    /// </summary>
    private void PointToPointPlatformMovement(Vector2 position)
    {
        if (position - (this.startPosition + this.relativeTargetPoints[this.currentPointTarget]) == Vector2.zero)
        {
            if (this.currentPointTarget == this.relativeTargetPoints.Count - 1)
            {
                if (this.reverseAtEnd)
                {
                    this.relativeTargetPoints.Reverse();//Reverse the order of the targets
                }
                this.currentPointTarget = 0;

            }
            else
            {
                // Update the target within the array list
                this.currentPointTarget++;
            }
        }
        position = Vector2.MoveTowards(position, this.startPosition + this.relativeTargetPoints[this.currentPointTarget], this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

        this.transform.position = position;
    }


    /// <summary>
    /// Get the platform range
    /// </summary>
    public float GetRange() => this.range;

    /// <summary>
    /// Set the platform range
    /// </summary>
    public void SetRange(float range) => this.range = range;

    private void OnDrawGizmos()
    {
        Vector2 position = Application.isPlaying ? this.startPosition : this.transform.position;
        float angleInRadians = this.angleInDegrees * Mathf.Deg2Rad;
        Gizmos.color = this.debugColor;

        switch (this.platformType)
        {
            case PlatformType.Circular:
                GizmosExtra.Draw2DCircle(position, this.range, this.debugColor);

                break;
            case PlatformType.Linear:
                switch (this.linearMovementType)
                {
                    case LinearMovementType.Horizontal:
                        Gizmos.DrawLine(position + (Vector2.left * this.range), position + (Vector2.right * this.range));
                        GizmosExtra.Draw2DArrow(position + (Vector2.left * this.range), 90);
                        GizmosExtra.Draw2DArrow(position + (Vector2.right * this.range), 270);
                        break;
                    case LinearMovementType.Vertical:
                        Gizmos.DrawLine(position + (Vector2.up * this.range), position + (Vector2.down * this.range));
                        GizmosExtra.Draw2DArrow(position + (Vector2.up * this.range), 0);
                        GizmosExtra.Draw2DArrow(position + (Vector2.down * this.range), 180);
                        break;
                    case LinearMovementType.Diagonal:
                        float leftXPivotPosition = position.x + (Mathf.Cos(angleInRadians) * this.range) + (Mathf.Sin(angleInRadians) * 0);
                        float leftYPivotPosition = position.y + (Mathf.Cos(angleInRadians) * 0) + (Mathf.Sin(angleInRadians) * this.range);
                        float rightXPosition = position.x + (Mathf.Cos(angleInRadians) * -this.range) + (Mathf.Sin(angleInRadians) * 0);
                        float rightYPosition = position.y + (Mathf.Cos(angleInRadians) * 0) + (Mathf.Sin(angleInRadians) * -this.range);

                        GizmosExtra.Draw2DArrow(new Vector2(leftXPivotPosition, leftYPivotPosition), this.angleInDegrees + 270);
                        GizmosExtra.Draw2DArrow(new Vector2(rightXPosition, rightYPosition), this.angleInDegrees + 90);
                        Gizmos.DrawLine(new Vector2(leftXPivotPosition, leftYPivotPosition), new Vector2(rightXPosition, rightYPosition));

                        break;
                    default:

                        break;
                }
                break;
            case PlatformType.PointToPoint:

                for (int x = 0; x < this.relativeTargetPoints.Count; x++)
                {
                    if (x > 0)
                    {
                        Gizmos.DrawLine(this.relativeTargetPoints[x - 1] + position, this.relativeTargetPoints[x] + position);
                        float differenceAngle = General.AngleBetweenVector2(this.relativeTargetPoints[x - 1] + position, this.relativeTargetPoints[x] + position);
                        float differenceLength = Vector2.Distance(this.relativeTargetPoints[x - 1] + position, this.relativeTargetPoints[x] + position);
                        GizmosExtra.Draw2DArrow(this.relativeTargetPoints[x - 1] + position, (differenceAngle * Mathf.Rad2Deg) - 90, (differenceLength / 2) + 8);
                    }
                }
                break;
            default:
                break;
        }
    }
}
