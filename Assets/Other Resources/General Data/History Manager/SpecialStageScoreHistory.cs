using UnityEngine;

/// <summary>
/// A class that stores data relating to the special stage that was just completed
/// </summary>
[System.Serializable]
public class SpecialStageScoreHistory
{
    [SerializeField, Tooltip("The special stages score data")]
    private ScoreData scoreData = new ScoreData();
    [SerializeField, Tooltip("The state of the special stage on end")]
    private SpecialStageState specialStageState = SpecialStageState.Failed;

    /// <summary>
    /// Get the score data
    /// </summary>
    public ScoreData GetScoreData() => this.scoreData;

    /// <summary>
    /// Set the score data
    /// </summary>
    public void SetScoreData(ScoreData scoreData) => this.scoreData = scoreData;

    /// <summary>
    /// Get the special stage state
    /// </summary>
    public SpecialStageState GetSpecialStageState() => this.specialStageState;

    /// <summary>
    /// Set the specials tage state
    /// </summary>
    public void SetSpecialStageState(SpecialStageState specialStageState) => this.specialStageState = specialStageState;
}
