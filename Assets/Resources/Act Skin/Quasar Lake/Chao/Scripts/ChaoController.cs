using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaoController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Animator animator;
    [SerializeField, LastFoldoutItem()]
    private SpriteRenderer spriteRenderer;
    [SerializeField, Tooltip("Affects the animation and movement of the child")]
    private ChaoType chaoType = ChaoType.Idle;
    [SerializeField, IsDisabled]
    private Vector2 startPosition;
    private IEnumerator chaoWaitCoroutine;
    [Tooltip("The active state of the moto bug"), FirstFoldOutItem("Chao Info"), SerializeField]
    private ChaoMode currentChaoMode = ChaoMode.Moving;
    [SerializeField, Tooltip("The current direction of the chao")]
    private int currentDirection = 1;
    [LastFoldoutItem(), Tooltip("Width of the chao")]
    private float currentBodyWidthRadius;

    [Tooltip("How much the chao waits before turning around"), FirstFoldOutItem("Movement Info"), SerializeField]
    private float waitTimeInSteps = 60;
    [Tooltip("The move speed of the chao"), LastFoldoutItem(), SerializeField]
    private float movementSpeed = 1f;

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
        if (this.currentChaoMode == ChaoMode.Moving && this.chaoType != ChaoType.Idle)
        {
            this.PointToPointPlatformMovement(this.transform.position);
        }

        this.animator.SetInteger("Type", (int)this.chaoType);
        this.animator.SetFloat("Mode", (int)this.currentChaoMode);
    }

    /// <summary>
    /// Update the direction of the chao
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
        if (this.relativeTargetPoints.Count <= 1)
        {
            return;
        }

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

        position = Vector2.MoveTowards(position, this.startPosition + this.relativeTargetPoints[this.currentPointTarget], this.movementSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);

        this.transform.position = position;
    }
    /// <summary>
    /// Turn the chao around after the set time
    /// </summary>
    private void Flip()
    {
        if (this.chaoWaitCoroutine != null)
        {
            this.StopCoroutine(this.chaoWaitCoroutine);
            this.chaoWaitCoroutine = null;
        }

        this.chaoWaitCoroutine = this.ChaoWait();
        this.StartCoroutine(this.chaoWaitCoroutine);

        this.SetChaoMode(ChaoMode.Turning);
    }

    /// <summary>
    /// Sets the new state for the chao
    /// <param name="chaoMode">The new state of the chao</param>
    /// </summary>
    public void SetChaoMode(ChaoMode chaoMode) => this.currentChaoMode = chaoMode;

    /// <summary>
    /// Make the chao wait for the specified amount of time before turning and moving towards its new direction
    /// </summary>
    private IEnumerator ChaoWait()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.waitTimeInSteps));

        this.SetChaoMode(ChaoMode.Moving);
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
