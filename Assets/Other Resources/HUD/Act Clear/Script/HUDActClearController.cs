using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The actions that take place when the stage has been successfully cleared
/// </summary>
public class HUDActClearController : ActClearBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.scoreAddAudioSource.Stop();
    }

    // Update is called once per frame
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
        if (this.inputMaster == null)
        {
            this.inputMaster = new InputMaster();
        }

        base.OnEnable();
        this.CalculatePlayerScores();
    }

    protected override void OnDisable() => base.OnDisable();

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
        this.endTotalCountUI.text = this.totalCountValue.ToString();
        this.coolBonusUI.text = this.coolBonusValue.ToString();
    }

    /// <summary>
    /// Calculates the value of the ring total value
    /// </summary>
    private int CalculateRingValue() => GMRegularStageScoreManager.Instance().GetRingCount() * 100;
    /// <summary>
    /// Sums up and calculates the value of the timer total
    /// </summary>
    private int CalculateTimerValue()
    {
        int minutes = GMRegularStageScoreManager.Instance().GetTimeCountIn(TimeFormat.Minutes);
        int seconds = GMRegularStageScoreManager.Instance().GetTimeCountIn(TimeFormat.Seconds);
        switch (minutes)
        {
            case 0:
                if (seconds is >= 0 and < 30)
                {
                    return 50000;
                }
                else if (seconds is > 29 and < 45)
                {
                    return 10000;
                }
                else if (seconds is > 44 and < 59)
                {
                    return 5000;
                }
                break;
            case 1:
                if (seconds is > 0 and < 30)
                {
                    return 4000;
                }
                else if (seconds is > 29 and < 59)
                {
                    return 3000;
                }
                break;
            case 2:
                return 2000;
            case 3:
                return 1000;
            case 4:
                return 500;
            case 5:
                return 0;
            case 9:
                if (seconds == 59)
                {
                    return 100000;
                }
                break;
            default:
                return 0;
        }

        return 0;
    }

    /// <summary>
    /// Begins the calculation of the player score
    /// </summary>
    private void BeginCalculating()
    {
        this.scoreAddAudioSource.Play();
        this.actClearHUDMode = ActClearHUDState.Calculating;

        if (GMStageManager.Instance().GetActClearGimmick() is SignPostController)
        {
            SignPostController signPostController = GMStageManager.Instance().GetActClearGimmick() as SignPostController;

            if (signPostController.GetSignFaceController() != null)
            {
                signPostController.GetSignFaceController().PlaySignFaceAnimation();
            }
        }
    }

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
            GMRegularStageScoreManager.Instance().IncrementScoreCount(this.totalCountValue);
            GMAudioManager.Instance().PlayOneShot(this.scoreTotalSound);
            this.scoreAddAudioSource.Stop();
            this.actClearHUDMode = ActClearHUDState.PostCalculation;
            this.Invoke(nameof(this.EndActClear), General.StepsToSeconds(this.timeTillEnd));//Waits the set amount of time till act clear
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
    /// When the entire act clear operation has ended
    /// </summary>
    private void ActClearEnd()
    {
        GMStageManager.Instance().OnActClear();

        //If we have time line triggers play them first and when they are over commence the act clear
        if (GMCutsceneManager.Instance().HasEndActTimeLineTriggers())
        {
            GMCutsceneManager.Instance().PlayActClearTimeLineTriggers(GMSceneManager.Instance().LoadNextScene);
        }
        else
        {
            GMSceneManager.Instance().LoadNextScene();
        }

        this.gameObject.SetActive(false);
    }
}
