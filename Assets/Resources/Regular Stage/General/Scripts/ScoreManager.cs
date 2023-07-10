using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    protected ScoreData scoreData = new ScoreData();

    [Tooltip("The UI element containing the score value"), SerializeField, FirstFoldOutItem("UI Elements")]
    protected Text scoreCountUI = null;
    [Tooltip("The UI element containing the timer value"), SerializeField]
    protected Text timerCountUI = null;
    [Tooltip("The UI element containing the ring count value"), SerializeField]
    protected Text ringCountUI = null;
    [Tooltip("The UI element containing the life count value"), SerializeField, LastFoldoutItem()]
    protected Text lifeCountUI = null;

    [FirstFoldOutItem("UI Flash Elements"), Tooltip("The UI element for the timer flash object"), SerializeField]
    protected GameObject timerFlashUI;
    [Tooltip("The UI  element for the ring  flashobject"), SerializeField]
    protected GameObject ringFlashUI;
    [LastFoldoutItem(), Tooltip("The frequency of the transparance for the Time and Ring UI elements when flashing"), SerializeField]
    protected float flashFrequency = 0.1333f;

    protected IEnumerator flashUIElementsCoroutine;

    public virtual void Reset() => this.scoreData = new ScoreData();

    protected virtual void Update()
    {
        if (GMPauseMenuManager.Instance().GameIsPaused())
        {
            return;
        }

        this.UpdateUIElements();
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    public void UpdateUIElements()
    {
        this.UpdateTimer();
        this.ClampScoreInfo();
        this.CheckToFlashUIElements();
    }

    /// <summary>
    /// Actions taken to update the timer
    /// </summary>
    protected virtual void UpdateTimer() { }

    /// <summary>
    /// Clamps the score info to their set boundaries
    /// </summary>
    protected virtual void ClampScoreInfo() { }

    /// <summary>
    /// Checks to flash UI elements
    /// </summary
    public virtual void CheckToFlashUIElements()
    {
        if (this.ShouldFlashRingUI() || this.ShouldFlashTimerUI())
        {
            if (this.flashUIElementsCoroutine == null)
            {
                this.flashUIElementsCoroutine = this.FlashHUDTextCoroutine();
                this.StartCoroutine(this.flashUIElementsCoroutine);
            }
        }
    }


    /// <summary>
    /// Convert the player Timer to a string format in "MM:SS:MS" format
    /// <param name="time">The current time </param>
    /// </summary>
    public virtual string ConvertTimerToString(float time)
    {
        int minutes = this.GetTimeCountIn(TimeFormat.Minutes, time);
        int seconds = this.GetTimeCountIn(TimeFormat.Seconds, time);
        int milliSeconds = this.GetTimeCountIn(TimeFormat.MilliSeconds, time);

        return string.Format("{0:00}'{1:00}\"{2:00}", minutes, seconds, milliSeconds);
    }

    /// <summary>
    /// Gets the amount time that has currently passed in the specified format
    /// <param name="timeFormat">The format to retrieve the time value in</param>
    /// </summary
    public virtual int GetTimeCountIn(TimeFormat timeFormat, float timeCount)
    {
        switch (timeFormat)
        {
            case TimeFormat.Minutes:
                return (int)timeCount / 60;
            case TimeFormat.Seconds:
                return (int)timeCount % 60;
            case TimeFormat.MilliSeconds:
                return (int)(timeCount * 100f) % 100;
            default:
                break;
        }

        return 0;
    }


    /// <summary>
    /// Gets the amount of rings the player has
    /// </summary>
    public int GetRingCount() => this.scoreData.ringCount;

    /// <summary>
    /// Sets the player ring count to the specified value
    /// <param name="ringCount">The value to set the ting count to </param>
    /// </summary
    public void SetRingCount(int ringCount)
    {
        this.scoreData.ringCount = ringCount;

        if (this.flashUIElementsCoroutine == null)
        {
            this.ringFlashUI.SetActive(ringCount == 0);//If the value is zero immedietly start flashing
        }
    }

    /// <summary>
    /// Increases the ring count by the specified value
    /// <param name="value">The value to increment the ring by </param>
    /// </summary>
    public virtual void IncrementRingCount(int value) => this.scoreData.ringCount += value;

    /// <summary>
    /// Gets the amount lives the player has
    /// </summary>
    public int GetLifeCount() => this.scoreData.lifeCount;
    /// <summary>
    /// Sets the player life count to the specified value
    /// <param name="lifeCount">The value to set the life count to </param>
    /// </summary>
    public void SetLifeCount(int lifeCount) => this.scoreData.lifeCount = lifeCount;

    /// <summary>
    /// Increases the life count by the specified value
    /// <param name="value">The value to increment the life count by </param>
    /// </summary>
    public void IncrementLifeCount(int value)
    {
        this.scoreData.lifeCount += value;
        GMAudioManager.Instance().PlayAndQueueBGM(BGMToPlay.ExtraLifeJingle);
    }

    /// <summary>
    /// Reduces the player life count by 1
    /// </summary>
    public void DecrementLifeCount() => this.scoreData.lifeCount--;

    /// <summary>
    /// Set the timer count of the regular stage
    /// <param name="timerCount">The value to set the timer count to </param>
    /// </summary>
    public void SetTimerCount(float timerCount) => this.scoreData.timerCount = timerCount;

    /// <summary>
    /// Gets the amount time that has currently passed
    /// </summary>
    public float GetTimerCount() => this.scoreData.timerCount;

    /// <summary>
    /// Whether the time is over for the player
    /// </summary
    public virtual bool TimeOver() => false;

    /// <summary>
    /// Gets the score count of the player
    /// </summary>
    public int GetScoreCount() => this.scoreData.scoreCount;

    /// <summary>
    /// Increases the score count by the specified value
    /// <param name="value">The value to increment the life count by </param>
    /// </summary>
    public virtual void IncrementScoreCount(int value) => this.scoreData.scoreCount += value;

    /// <summary>
    /// Sets the player score to the specified value
    /// <param name="value">The value to set the score to </param>
    /// </summary>
    public void SetScoreCount(int value) => this.scoreData.scoreCount = value;

    /// <summary>
    /// Gets the value of the current score data
    /// </summary
    public ScoreData GetScoreData() => this.scoreData;

    /// <summary>
    /// When this returns true the ring UI will begin flashing
    /// </summary
    protected virtual bool ShouldFlashRingUI() => false;

    /// <summary>
    /// When this returns true the timer UI will begin flashing
    /// </summary
    protected virtual bool ShouldFlashTimerUI() => false;


    /// <summary>
    /// Turns on the flash UI text to cover base text when neccessary
    /// </summary>
    private IEnumerator FlashHUDTextCoroutine()
    {
        bool ringFlashing = false;
        bool timerFlashing = false;

        while (true)
        {
            if (this.ShouldFlashRingUI() || this.ShouldFlashTimerUI())
            {
                if (this.ShouldFlashRingUI())
                {
                    if (timerFlashing && ringFlashing == false)
                    {
                        this.ringFlashUI.SetActive(this.timerFlashUI.activeSelf);
                        ringFlashing = true;

                        continue;
                    }

                    ringFlashing = true;
                    this.ringFlashUI.SetActive(!this.ringFlashUI.activeSelf);//Flash by switching on and off its current state
                }
                else
                {
                    ringFlashing = false;
                    this.ringFlashUI.SetActive(ringFlashing);
                }

                if (this.ShouldFlashTimerUI())
                {
                    if (ringFlashing && timerFlashing == false)
                    {
                        this.timerFlashUI.SetActive(this.ringFlashUI.activeSelf);
                        timerFlashing = true;

                        continue;
                    }

                    this.timerFlashUI.SetActive(!this.timerFlashUI.activeSelf);//Flash by switching on and off its current state
                    timerFlashing = true;
                }
                else
                {
                    timerFlashing = false;
                    this.timerFlashUI.SetActive(timerFlashing);
                }
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(this.flashFrequency);
        }

        timerFlashing = false;
        ringFlashing = false;
        this.ringFlashUI.SetActive(ringFlashing);
        this.timerFlashUI.SetActive(timerFlashing);
        this.flashUIElementsCoroutine = null;

        yield return null;
    }
}
