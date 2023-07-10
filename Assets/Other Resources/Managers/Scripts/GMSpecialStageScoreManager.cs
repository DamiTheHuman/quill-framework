using UnityEngine;
/// <summary>
/// A manager that handles the players score through out a special stage
/// </summary>
public class GMSpecialStageScoreManager : ScoreManager
{
    [Help("This represents how much time a player has to reach the goal")]
    [SerializeField, Tooltip("The max amount of time taken to get through a special stage in seconds")]
    private float startTimeValueInSeconds = 60;

    /// <summary>
    /// The single instance of the special stage score manager
    /// </summary>
    private static GMSpecialStageScoreManager instance;

    private void Start()
    {
        instance = this;

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.SpecialStage)
        {
            this.gameObject.SetActive(false);

            return;
        }

        this.SetRingCount(0);
        this.SetTimerCount(this.startTimeValueInSeconds);
    }

    /// <summary>
    /// Get a reference to the static instance of the specal stagescore manager
    /// </summary>
    public static GMSpecialStageScoreManager Instance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<GMSpecialStageScoreManager>();
        }

        return instance;
    }

    protected override void Update() => base.Update();

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override void UpdateTimer()
    {
        if (GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Running)
        {
            this.scoreData.timerCount -= Time.deltaTime;

            if (this.TimeOver())
            {
                GMSpecialStageManager.Instance().SpecialStageFailed(SpecialStageFailureReason.TimeOver);
            }
        }
    }

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override void ClampScoreInfo()
    {
        this.SetTimerCount(Mathf.Clamp(this.scoreData.timerCount, 0, this.startTimeValueInSeconds));//599.999f Translates to 9:59:99
        this.SetRingCount(Mathf.Clamp(this.scoreData.ringCount, 0, 999));
        this.ringCountUI.text = this.scoreData.ringCount.ToString("000");
        this.timerCountUI.text = this.ConvertTimerToString(this.scoreData.timerCount);
    }

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    public override bool TimeOver() => this.scoreData.timerCount <= 0 && GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Running;

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override bool ShouldFlashRingUI() => this.scoreData.ringCount == 0;

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override bool ShouldFlashTimerUI() => this.scoreData.timerCount < 10;
}
