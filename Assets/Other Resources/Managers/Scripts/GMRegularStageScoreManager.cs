using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A manager that handles the players score through out a regular stage
/// </summary>
public class GMRegularStageScoreManager : ScoreManager
{
    [FirstFoldOutItem("Score Combo"), Tooltip("The current combo counter"), SerializeField]
    private int currentCombo = 0;
    [LastFoldoutItem(), Tooltip("The Score sprites for each score"), SerializeField]
    private List<Sprite> scoreSprites = new List<Sprite>();

    [FirstFoldOutItem("Extra Life Gain"), Tooltip("The amount of score needed to attain an extra life"), SerializeField]
    private float scoreToNextLife = 50000;
    [LastFoldoutItem(), Tooltip("The amount of rings needed to attain an extra life"), SerializeField]
    private float ringExtraLifeLevel = 100;

    [FirstFoldOutItem("Super Form Ring Loss"), Tooltip("The ring amount lost when the player is transformed"), SerializeField]
    public int superRingLossAmount = 1;
    [LastFoldoutItem, Tooltip("The frequency in which ring(s) will be depleted "), Min(0)]
    public float superRingLossFrequency = 1f;
    private IEnumerator transformDepletionFactor;

    /// <summary>
    /// The single instance of the regular stage score manager
    /// </summary>
    private static GMRegularStageScoreManager instance;

    private void Start()
    {
        instance = this;

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.RegularStage)
        {
            this.gameObject.SetActive(false);

            return;
        }

        //If we are in a regular stage and the hsitory object is not null set our data appropriately
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage && GMHistoryManager.Instance().HasScoreHistory())
        {
            ScoreData regularStageScoreData = GMHistoryManager.Instance().GetRegularStageScoreHistory().GetScoreData();
            this.SetRingCount(regularStageScoreData.ringCount);
            this.SetScoreCount(regularStageScoreData.scoreCount);
            this.SetTimerCount(regularStageScoreData.timerCount);
            this.SetLifeCount(regularStageScoreData.lifeCount);

            GMHistoryManager.Instance().SetHasScoreHistory(false);//This is to prevent this information being fetched repeatedly on scene loads

            return;
        }

        this.SetLifeCount(GMSaveSystem.Instance().GetCurrentPlayerData().GetLives());
        this.SetScoreCount(GMSaveSystem.Instance().GetCurrentPlayerData().GetScore());
    }

    /// <summary>
    /// Get a reference to the static instance of the score manager
    /// </summary>
    public static GMRegularStageScoreManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMRegularStageScoreManager>();
        }

        return instance;
    }

    protected override void Update()
    {
        base.Update();

        if (this.TimeOver())
        {
            GMStageManager.Instance().GetPlayer().GetHealthManager().KillPlayer();
        }

        if (this.currentCombo != 0)
        {
            this.CheckToEndComboStreak();
        }


        this.ManageSuperRingLoss();
    }

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override void UpdateTimer()
    {
        if (GMStageManager.Instance().GetStageState() is RegularStageState.Running or RegularStageState.FightingBoss)//As long a sthe stage is not cleared update the timer
        {
            this.scoreData.timerCount += Time.deltaTime;
        }
    }

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override void ClampScoreInfo()
    {
        this.SetTimerCount(Mathf.Clamp(this.scoreData.timerCount, 0, 599.999f));//599.999f Translates to 9:59:99
        this.SetRingCount(Mathf.Clamp(this.scoreData.ringCount, 0, 999));
        this.SetScoreCount(Mathf.Clamp(this.scoreData.scoreCount, 0, 999999999));
        this.SetLifeCount(Mathf.Clamp(this.scoreData.lifeCount, 0, 99));
        this.scoreCountUI.text = this.scoreData.scoreCount.ToString();
        this.lifeCountUI.text = this.scoreData.lifeCount.ToString();
        this.ringCountUI.text = this.scoreData.ringCount.ToString();
        this.timerCountUI.text = this.ConvertTimerToString(this.scoreData.timerCount);
    }

    /// <summary>
    /// Watches the current combo streak to end it under the correct conditions
    /// </summary>
    private void CheckToEndComboStreak()
    {
        Player player = GMStageManager.Instance().GetPlayer();

        if (player.GetAttackingState() == false && player.GetHealthManager().GetHealthStatus() != HealthStatus.Invincible)
        {
            this.currentCombo = 0;
        }
    }

    /// <inheritdoc>
    /// Adds life incremenets
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    public override void IncrementRingCount(int value)
    {
        base.IncrementRingCount(value);

        if (this.scoreData.ringCount >= this.ringExtraLifeLevel)
        {
            this.IncrementLifeCount(1);
            this.ringExtraLifeLevel += 100;
        }
    }

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override bool ShouldFlashRingUI() => this.scoreData.ringCount == 0;


    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    protected override bool ShouldFlashTimerUI() => this.scoreData.timerCount > 540;

    /// <summary>
    /// Wacthes and updates the player ring count when they are super
    /// </summary>
    private void ManageSuperRingLoss()
    {
        Player player = GMStageManager.Instance().GetPlayer();

        if (player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            if (this.transformDepletionFactor == null)
            {
                this.transformDepletionFactor = this.DepleteRingCount(player, this.superRingLossAmount, this.superRingLossFrequency);
                this.StartCoroutine(this.transformDepletionFactor);
            }
        }
        else
        {
            if (this.transformDepletionFactor != null)
            {
                this.StopCoroutine(this.transformDepletionFactor);
                this.transformDepletionFactor = null;
            }
        }
    }

    /// <summary>
    /// Depletes the players ring count when they are super
    /// <param name="player">The active playeyr in content </param>
    /// <param name="lossAmount">The amount of rings lost every tick </param>
    /// <param name="frequency">The frequency in which the player loses rings</param>
    /// </summary>
    private IEnumerator DepleteRingCount(Player player, int lossAmount, float frequency = 1)
    {

        if (player == null)
        {
            Debug.LogWarning("No Player found!");

            yield return null;
        }

        while (true)
        {
            yield return new WaitForSeconds(frequency);

            if (player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm && player.GetPlayerState() == PlayerState.Awake)
            {
                this.IncrementRingCount(lossAmount * -1);

                if (this.scoreData.ringCount <= 0)
                {
                    yield return null;
                }
            }
        }
    }

    /// <inheritdoc>
    /// <see cref="ScoreManager"/>
    /// </inheritdoc>
    public override bool TimeOver() => this.scoreData.timerCount >= 599.999f && GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetDebugMode() == false;

    /// <summary>
    /// Gets the amount time that has currently passed in the specified format
    /// <param name="timeFormat">The format to retrieve the time value in</param>
    /// </summary
    public int GetTimeCountIn(TimeFormat timeFormat)
    {
        switch (timeFormat)
        {
            case TimeFormat.Minutes:
                return (int)this.GetTimerCount() / 60;
            case TimeFormat.Seconds:
                return (int)this.GetTimerCount() % 60;
            case TimeFormat.MilliSeconds:
                return (int)(this.GetTimerCount() * 100f) % 100;
            default:
                break;
        }

        return 0;
    }

    public override void IncrementScoreCount(int value)
    {
        base.IncrementScoreCount(value);

        if (this.scoreData.scoreCount >= this.scoreToNextLife && GMStageManager.Instance().GetStageState() == RegularStageState.Running)
        {
            this.IncrementLifeCount(1);
            this.scoreToNextLife += 50000;
        }
    }

    /// <summary>
    /// Grants the player a score based on the index passed in
    /// <param name="position">The position to spawn the score</param>
    /// <param name="index">The sprite value to be displayed referencing the <see cref="scoreSprites"/> array </param>
    /// </summary>
    public void DisplayScore(Vector2 position, int index = 0)
    {
        GameObject scoreObject = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.ScoreIcon, position);
        scoreObject.GetComponent<SpriteRenderer>().sprite = this.scoreSprites[index];
    }

    /// <summary>
    /// Increments the players combo
    /// </summary>
    public void IncrementCombo(Transform transform)
    {
        this.currentCombo++;
        this.currentCombo = Mathf.Clamp(this.currentCombo, 0, 17);

        if (transform != null)
        {
            this.DisplayScore(transform.position, this.UpdateComboCounter());
        }
    }

    /// <summary>
    /// Updates the combo counter and gets the current score combo value as an array index to know what icon to display
    /// </summary>
    public int UpdateComboCounter()
    {
        int result = 0;

        if (this.currentCombo == 0)
        {
            result = 0;
            this.IncrementScoreCount(10);
        }
        else if (this.currentCombo == 1)
        {
            result = 1;
            this.IncrementScoreCount(100);
        }
        else if (this.currentCombo == 2)
        {
            result = 2;
            this.IncrementScoreCount(200);
        }
        else if (this.currentCombo == 3)
        {
            result = 3;
            this.IncrementScoreCount(500);
        }
        else if (this.currentCombo is >= 4 and < 16)
        {
            result = 4;
            this.IncrementScoreCount(1000);
        }
        else if (this.currentCombo > 16)
        {
            result = 5;
            this.IncrementScoreCount(1000);
        }

        return result;
    }

    /// <summary>
    /// Calculate and display the players score
    /// </summary>
    public void CalculateActClearScore() => GMStageHUDManager.Instance().SetActClearUIActive(true);
}
