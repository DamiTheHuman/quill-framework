using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple point to point movemeent script for the fish
/// </summary
public class FishController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Animator animator;
    [SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;

    [SerializeField, IsDisabled]
    private Vector2 startPosition;
    private IEnumerator fishWaitCoroutine;
    [Tooltip("The active state of the moto bug"), FirstFoldOutItem("Fish Info"), SerializeField]
    private FishMode currentFishMode = FishMode.Swimming;
    [SerializeField, Tooltip("The current direction of the fish")]
    private int currentDirection = 1;
    [LastFoldoutItem(), Tooltip("Width of the fish")]
    private float currentBodyWidthRadius;

    [Tooltip("How much the fish waits before turning around"), FirstFoldOutItem("Movement Info"), SerializeField]
    private float waitTimeInSteps = 60;
    [Tooltip("The swim speed of the fish"), LastFoldoutItem(), SerializeField]
    private float swimSpeed = 1f;

    [FirstFoldOutItem("Movement Parameters"), Tooltip("A list of points for the controller to travel through"), SerializeField]
    private List<Vector2> relativeTargetPoints = new List<Vector2>();
    [Tooltip("The current targetPoint being navigated"), LastFoldoutItem(), SerializeField]
    private int currentPointTarget = 0;

    [SerializeField, Tooltip("The debug color")]
    private Color debugColor = Color.red;

    public void Reset()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    public void Start()
    {
        if (this.animator == null)
        {
            this.Reset();
        }

        this.startPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (this.currentFishMode == FishMode.Swimming)
        {
            this.PointToPointPlatformMovement(this.transform.position);
        }

        this.animator.SetInteger("State", (int)this.currentFishMode);
    }

    /// <summary>
    /// Update the direction of the fish
    /// </summary>
    private void UpdateDirection()
    {
        this.currentDirection = General.ObjectDirectionHorizontally(this.transform.position, this.startPosition + this.relativeTargetPoints[this.currentPointTarget]);
        this.transform.localScale = new Vector3(this.currentDirection, 1, 1);
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
                this.relativeTargetPoints.Reverse();//Reverse the order of the targets
                this.currentPointTarget = 0;
            }
            else
            {
                this.currentPointTarget++;// Update the target within the array list
            }

            //If the target point is in a different direction from current position, switch direction
            if (General.ObjectDirectionHorizontally(position, this.startPosition + this.relativeTargetPoints[this.currentPointTarget]) != this.currentDirection)
            {
                this.Flip();
            }
        }

        position = Vector2.MoveTowards(position, this.startPosition + this.relativeTargetPoints[this.currentPointTarget], this.swimSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

        this.transform.position = position;
    }
    /// <summary>
    /// Turn the fish around after the set time
    /// </summary>
    private void Flip()
    {
        if (this.fishWaitCoroutine != null)
        {
            this.StopCoroutine(this.fishWaitCoroutine);
            this.fishWaitCoroutine = null;
        }

        this.fishWaitCoroutine = this.FishWait();
        this.StartCoroutine(this.fishWaitCoroutine);

        this.SetFishMode(FishMode.Turning);
    }

    /// <summary>
    /// Sets the new state for the fish
    /// <param name="fishMode">The new state of the fish</param>
    /// </summary>
    public void SetFishMode(FishMode fishMode) => this.currentFishMode = fishMode;

    /// <summary>
    /// Make the fish wait for the specified amount of time before turning and moving towards its new direction
    /// </summary>
    private IEnumerator FishWait()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.waitTimeInSteps));

        this.SetFishMode(FishMode.Swimming);
        this.UpdateDirection();

        yield return null;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        this.currentDirection = (int)Mathf.Sign(this.transform.localScale.x);
    }

    private void OnDrawGizmos()
    {
        Vector2 position = Application.isPlaying ? this.startPosition : this.transform.position;
        Gizmos.color = this.debugColor;

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
    }
}
