using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// The actions that take place when a special stage is cleared an the scene data is retrieved
/// Unlike the act clear the score data will be retrieved from the special stage manager
/// </summary>
public class HUDSpecialStageActClear : ActClearBase
{
    [SerializeField, Tooltip("Time waited before going back to the regular stage")]
    private float timeBeforeReturningToRegularStage = 3;
    [Tooltip("The audio clip for when an emerald is gotten"), SerializeField]
    private AudioClip emeraldGottenClip;
    [Tooltip("The audio clip for when the player can go super"), SerializeField]
    private AudioClip canNowGoSuperAudioClip;
    [SerializeField, Tooltip("Signifies whether the player can g super")]
    private bool playerCanNowGoSuper = false;

    private void Start()
    {
        this.animator = this.GetComponent<Animator>();

        if (this.scoreAddAudioSource != null)
        {
            this.scoreAddAudioSource.Stop();
        }

        bool hasAllChaosEmeralds = GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds() >= 7;
        SpecialStageState historySpecialStageState = GMHistoryManager.Instance().GetSpecialStageScoreHistory().GetSpecialStageState();
        SpecialStageActClearType specialStageActClearType = SpecialStageActClearType.Failed;

        if (historySpecialStageState == SpecialStageState.Clear)
        {
            if (hasAllChaosEmeralds)
            {
                specialStageActClearType = SpecialStageActClearType.GotAllEmerailds;
                this.playerCanNowGoSuper = true;
            }
            else
            {
                specialStageActClearType = SpecialStageActClearType.GotAnEmerald;
            }
        }

        this.animator.SetInteger("Type", (int)specialStageActClearType);
    }

    private void Update()
    {
        if (this.actClearHUDMode == ActClearHUDState.Calculating && GMPauseMenuManager.Instance().GameIsPaused() == false)
        {
            this.CountdownScores();
            this.UpdateUI();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.CalculatePlayerScores();
    }

    /// <summary>
    /// Tallies up the players score based on the information from the score manager
    /// </summary>
    private void CalculatePlayerScores()
    {
        this.timerBonusValue = this.CalculateTimerValue();
        this.ringBonusValue = this.CalculateRingValue();
        this.totalCountValue = 0;
        this.coolBonusValue = 0;
        this.machBonusValue = 0;
        this.UpdateUI();
    }

    /// <summary>
    /// Updates the UI text object relating to the Act Clear Function
    /// </summary>
    private void UpdateUI()
    {
        this.timerBonusUI.text = this.timerBonusValue.ToString();
        this.ringBonusUI.text = this.ringBonusValue.ToString();
        this.machBonusUI.text = this.machBonusValue.ToString();
    }

    /// <summary>
    /// Calculates the value of the ring total value
    /// </summary>
    private int CalculateRingValue() => GMHistoryManager.Instance().GetSpecialStageScoreHistory().GetScoreData().ringCount * 100;

    /// <summary>
    /// Sums up and calculates the value of the timer total
    /// </summary>
    private int CalculateTimerValue()
    {
        int seconds = GMRegularStageScoreManager.Instance().GetTimeCountIn(TimeFormat.Seconds);

        if (seconds is >= 0 and < 10)
        {
            return 500;
        }
        else if (seconds is >= 20 and < 30)
        {
            return 1000;
        }
        else if (seconds is >= 30 and <= 40)
        {
            return 1500;
        }
        else if (seconds is >= 30 and <= 40)
        {
            return 1500;
        }
        else if (seconds is >= 50 and <= 60)
        {
            return 10000;
        }
        else
        {
            return 20000;
        }
    }

    /// <summary>
    /// Begins the calculation of the player score
    /// </summary>
    private void BeginCalculating()
    {
        this.scoreAddAudioSource.Play();
        this.actClearHUDMode = ActClearHUDState.Calculating;

        if (GMHistoryManager.Instance().GetSpecialStageScoreHistory().GetSpecialStageState() == SpecialStageState.Clear)
        {
            GMAudioManager.Instance().PlayOneShot(this.emeraldGottenClip);
        }
    }

    /// <summary>
    /// Go Back to the regular stage in which you came
    /// </summary>
    private void BackToRegularStage() => GMSpecialStageManager.Instance().GoBackToRegularStage();
    /// <summary>
    /// Provides a delay before going back
    /// </summary>
    private void StartGoBackToRegularStage() => this.Invoke(nameof(BackToRegularStage), this.timeBeforeReturningToRegularStage);
    /// <summary>
    /// Updates the total score based on the score passed in
    /// <param name="scoreToUpdate">The variable to decrement from  </param>
    /// </summary>
    private int UpdateTotalScore(int scoreToUpdate)
    {
        int updateSpeed = scoreToUpdate - this.calculationSpeed < 0 ? scoreToUpdate : this.calculationSpeed; //Ensures the score is never negative
        scoreToUpdate -= updateSpeed;
        this.totalCountValue += updateSpeed;

        return scoreToUpdate;
    }

    /// <summary>
    /// Counts down the players variables
    /// </summary>
    private void CountdownScores()
    {
        if (this.coolBonusValue != 0)
        {
            this.coolBonusValue = this.UpdateTotalScore(this.coolBonusValue);
        }
        else if (this.ringBonusValue != 0)
        {
            this.ringBonusValue = this.UpdateTotalScore(this.ringBonusValue);
        }
        else if (this.timerBonusValue != 0)
        {
            this.timerBonusValue = this.UpdateTotalScore(this.timerBonusValue);
        }
        else
        {
            if (this.playerCanNowGoSuper == false)
            {
                this.StartGoBackToRegularStage();
            }
            else
            {
                this.animator.SetTrigger("CanGoSuper");
            }
            this.actClearHUDMode = ActClearHUDState.PostCalculation;
            GMAudioManager.Instance().PlayOneShot(this.scoreTotalSound);
            this.scoreAddAudioSource.Stop();
        }
    }

    /// <summary>
    /// When the player makes use of any of the mapped inputs
    /// </summary>
    protected override void OnInputPressed(InputActionPhase phase)
    {
        if (phase != InputActionPhase.Performed)
        {
            return;
        }

        if (this.actClearHUDMode == ActClearHUDState.Calculating && GMPauseMenuManager.Instance().GameIsPaused() == false)
        {
            this.SkipCountdown();
            this.UpdateUI();
        }
    }


    /// <summary>
    /// Quickly sums up the players total count
    /// </summary>
    private void SkipCountdown()
    {
        this.totalCountValue += this.coolBonusValue + this.ringBonusValue + this.timerBonusValue;
        this.coolBonusValue = 0;
        this.ringBonusValue = 0;
        this.timerBonusValue = 0;
        this.machBonusValue = 0;
    }

    /// <summary>
    /// Limits the counter variables to ensure they are no negatives
    /// </summary>
    private void ClampValues()
    {
        this.timerBonusValue = Mathf.Max(this.timerBonusValue, 0);
        this.coolBonusValue = Mathf.Max(this.coolBonusValue, 0);
        this.ringBonusValue = Mathf.Max(this.ringBonusValue, 0);
        this.totalCountValue = Mathf.Max(this.totalCountValue, 0);
    }

    /// <summary>
    /// Begins playing the act clear jingle
    /// </summary>
    public void PlayActClearJingle() => GMAudioManager.Instance().PlayBGM(BGMToPlay.ActClearJingle);

    /// <summary>
    /// Begin the performace of act clearing
    /// </summary>
    private void EndActClear() => this.animator.SetTrigger("EndAct");

    /// <summary>
    /// Audio played when the event that takes place signifying the player can go super
    /// </summary>
    private void PlayCanNowGoSuperJingle() => GMAudioManager.Instance().PlayOneShot(this.canNowGoSuperAudioClip);

    /// <summary>
    /// When the entire act clear operation has ended
    /// </summary>
    private void ActClearEnd()
    {
        GMStageManager.Instance().OnActClear();
        this.gameObject.SetActive(false);
    }
}
