using UnityEngine;

public class HUDSpecialStageMessageController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private int messageHash;
    private int displayMessageHash;

    private void Reset() => this.animator = this.GetComponent<Animator>();

    private void Awake() => GMSpecialStageManager.Instance().SetSpecialStageMessageController(this);

    private void Start()
    {
        if (this.animator == null)
        {
            this.Reset();
        }

        this.DisplayMessage(0);
        this.messageHash = Animator.StringToHash("Message");
        this.displayMessageHash = Animator.StringToHash("DisplayMessage");
    }

    /// <summary>
    /// Begin updating the player score and allow movement
    /// </summary>
    public void BeginScoreUpdate() => GMSpecialStageManager.Instance().SetSpecialStageState(SpecialStageState.Running);

    /// <summary>
    /// Deactivates the title card
    /// </summary>
    public void DeactivateMessage() => this.gameObject.SetActive(false);

    /// <summary>
    /// Displays a message based on the type
    /// <param name="messageId">The message id to display</param>
    /// </summary>
    public void DisplayMessage(SpecialStageMessage specialStageMessage)
    {
        this.animator.SetInteger(this.messageHash, (int)specialStageMessage);
        this.animator.SetTrigger(this.displayMessageHash);
    }
}
