using System.Collections;
using UnityEngine;
/// <summary>
/// Controls the movement of the sign face once activated
/// This works by rotating the signs y axis in contrast to animating and allows a more fluid method
/// </summary>
public class SignFaceController : MonoBehaviour
{
    private Player player;
    [SerializeField]
    private Animator animator;
    [FirstFoldOutItem("Sign Face Components"), SerializeField]
    private GameObject characterFace;
    [SerializeField]
    private GameObject leftSignEdge;
    [LastFoldoutItem(), SerializeField]
    private GameObject rightSignEdge;
    [Tooltip("The current state of the sign face")]
    [FirstFoldOutItem("Sign Face State")]
    public SignFaceState signFaceState = SignFaceState.None;
    [Tooltip("Whether the sign has been flipped which also aids to know which side is being show"), LastFoldoutItem(), SerializeField]
    private bool flipped = false;

    [FirstFoldOutItem("Sign Face Movement")]
    [Help("For the speed please use a value directly divisible by 360 with no remainder"), SerializeField]
    private float startSpeed = 24f;
    [Tooltip("The current rotation speed which will also increment delta"), SerializeField]
    private float currentSpeed = 0f;
    [Tooltip("The current angle of the sign face"), SerializeField]
    private float delta = 0;
    [Tooltip("The time taken in steps for the spin face to spin")]
    [Help("Please input the timer in steps - (timeInSeconds * 59.9999995313f)"), LastFoldoutItem(), SerializeField]
    private float spinDepletionTimeInSteps = 121;

    private IEnumerator depleteSignRotationCoroutine;

    private void Reset() => this.animator = this.GetComponent<Animator>();

    // Start is called before the first frame update
    private void Start()
    {
        this.delta = 0;
        this.currentSpeed = this.startSpeed;
        this.signFaceState = SignFaceState.None;

        if (this.animator == null)
        {
            this.Reset();
        }
    }

    private void FixedUpdate()
    {
        if (this.signFaceState is SignFaceState.Active or SignFaceState.Depleting)
        {
            this.SpinSignPostHead();
            this.UpdateSignEdgeVisibility();
            this.UpdateSignFaceVisibility();
        }
    }

    /// <summary>
    /// Spin the sign post
    /// </summary>
    private void SpinSignPostHead()
    {
        this.delta += this.currentSpeed;
        this.transform.eulerAngles = new Vector3(0, this.delta, 0);
        this.delta = General.ClampDegreeAngles(this.delta);
    }

    /// <summary>
    /// Updates the visible edges for the sign face
    /// </summary>
    private void UpdateSignEdgeVisibility()
    {
        if (this.delta is > 180 and < 360)
        {
            this.leftSignEdge.SetActive(false);
            this.rightSignEdge.SetActive(true);
        }
        else
        {
            this.rightSignEdge.SetActive(false);
            this.leftSignEdge.SetActive(true);
        }
    }

    /// <summary>
    /// Updates the layerd face of the sign
    /// </summary>
    private void UpdateSignFaceVisibility()
    {
        if (this.IsUnflipped(this.delta))
        {
            if (this.flipped)
            {
                this.characterFace.SetActive(false);
            }
            this.flipped = false;
        }
        else
        {
            if (!this.flipped)
            {
                this.characterFace.SetActive(true);
            }

            if (this.delta is 0 or 360)
            {
                if (this.signFaceState == SignFaceState.Depleting && this.depleteSignRotationCoroutine == null)
                {
                    this.BeginFadeSpinCoroutine();
                }
            }

            this.flipped = true;
        }
    }

    /// <summary>
    /// Determines which side of the sign is unflipped
    /// <param name="angleInDegrees"> The current angle of the sign</param>
    /// </summary>
    private bool IsUnflipped(float angleInDegrees) => angleInDegrees is not (<= 90 or (>= 270 and <= 360));
    /// <summary>
    /// Sets the state of the sign face
    /// <param name="signFaceState"> The new state of the sign face</param>
    /// </summary>
    public void SetSignFaceState(SignFaceState signFaceState) => this.signFaceState = signFaceState;

    /// <summary>
    /// Starts a coroutine that deplets the speed of the sign face towards 0 within the set amount of steps
    /// </summary>
    private void BeginFadeSpinCoroutine()
    {
        //2.016665 seconds 
        //18/24 or 121 steps
        //4 spins with 3 spins after showing the character face once
        this.depleteSignRotationCoroutine = this.DepleteSignRotationd(General.StepsToSeconds(121));
        this.StartCoroutine(this.depleteSignRotationCoroutine);
    }

    /// <summary>
    /// A coroutine which deplets the player speed towards zero
    /// <param name="spinTime"> The amount of time before the speed is zeroed out</param>
    /// </summary>
    private IEnumerator DepleteSignRotationd(float spinTime)
    {
        float inversedTime = 1 / spinTime;

        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            this.currentSpeed = Mathf.Lerp(this.startSpeed, 0f, step);

            yield return new WaitForFixedUpdate();
        }

        this.EndSignFaceAction();

        yield return null;
    }

    /// <summary>
    /// The end of the spin operation when the speed reaches 0 and the player score is calculated
    /// </summary>
    private void EndSignFaceAction()
    {
        this.characterFace.SetActive(true);
        this.currentSpeed = 0;
        this.signFaceState = SignFaceState.End;
        this.depleteSignRotationCoroutine = null;
        GMRegularStageScoreManager.Instance().CalculateActClearScore();
        this.player.SetBeginVictoryActionOnGroundContact(true);
    }

    /// <summary>
    /// Sets a reference to the player object that crossed the sign
    /// </summary>
    public void SetPlayer(Player player) => this.player = player;
    /// <summary>
    /// A function used to play the end sign face animation
    /// </summary>
    public void PlaySignFaceAnimation() => this.animator.SetBool("Activate", true);

    /// <summary>
    /// Get the spin depletion
    /// </summary>
    public float GetSpinDepletionTimeInSteps() => this.spinDepletionTimeInSteps;
}
