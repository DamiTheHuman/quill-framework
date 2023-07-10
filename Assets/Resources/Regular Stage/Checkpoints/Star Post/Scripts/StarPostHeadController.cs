using UnityEngine;
/// <summary>
/// The starpost head of the checkpoint which rotates around its axis
/// </summary>
public class StarPostHeadController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private Vector2 startPosition;
    public bool finished = false;
    [Tooltip("The active state of the checkpointl")]
    public bool activated;
    [Tooltip("The limit of amount of spins to be performed")]
    public float spinLimit = 2;
    [Tooltip("The radius of the star post")]
    public float spinRadius = 10;
    [Tooltip("The current delta of the star post head")]
    public float delta = 0;
    [Tooltip("The Y offset of the star post head to reveolve around its base")]
    public float baseOffsetY = 10;
    [Tooltip("How much delta is incremented by every step")]
    public float deltaIncrement = 12;
    public Color debugColor = Color.white;

    private void Reset() => this.animator = this.GetComponent<Animator>();
    private void Start()
    {
        //In the case the star post is already an active checkpoint
        if (this.finished)
        {
            return;
        }

        this.startPosition = this.transform.position;
        this.startPosition.y -= this.baseOffsetY;

        if (this.animator == null)
        {
            this.Reset();
        }

        this.animator.SetBool("Flash", false);
    }

    private void FixedUpdate()
    {
        if (this.activated && this.finished == false)
        {
            this.RotateStarPostHead();
        }
    }

    /// <summary>
    /// Set the activation state of the star head
    /// <param name="activated">the new state of the checkpoint</param>
    /// </summary>
    public void SetActivated(bool activated) => this.activated = activated;
    /// <summary>
    /// Gets the activation state of the star post head
    /// </summary>
    public bool GetActivated() => this.activated;
    /// <summary>
    /// Sets the sarpost head to the already active state
    /// </summary>
    public void AlreadyActive()
    {
        if (this.animator == null)
        {
            this.animator = this.GetComponent<Animator>();
        }

        this.animator.SetBool("Flash", true);
        this.finished = true;
        this.activated = true;
    }

    /// <summary>
    /// Rotates the checkpoint sphere head
    /// </summary>
    private void RotateStarPostHead()
    {
        if (this.delta < this.spinLimit * 360 || this.delta == 0)
        {
            this.delta += this.deltaIncrement;
            Vector2 pos = this.transform.position;
            //Move the checkpoint sphere
            pos.x = General.RoundToDecimalPlaces(this.startPosition.x + (this.spinRadius * Mathf.Sin(this.delta * Mathf.Deg2Rad)));
            pos.y = General.RoundToDecimalPlaces(this.startPosition.y + (this.spinRadius * Mathf.Cos(this.delta * Mathf.Deg2Rad)));
            this.transform.position = pos;
        }
        else
        {
            this.finished = true;
            this.animator.SetBool("Flash", true);
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 pos = Application.isPlaying ? this.startPosition : (Vector2)this.transform.position;
        pos.y -= Application.isPlaying ? 0 : this.baseOffsetY;
        GizmosExtra.Draw2DCircle(pos, this.spinRadius, this.debugColor);
    }
}
