using UnityEngine;

/// <summary>
/// A class that contains general information about the bubble being spawned
/// </summary>
[System.Serializable]
public class BubbleData
{
    [SerializeField, Tooltip("The type of bubble being spawned")]
    private BubbleType bubbleType = BubbleType.Small;
    [SerializeField, Tooltip("The time frame where the bubble possibility is checked and updated")]
    private float spawnInterval = 1;
    [SerializeField, Tooltip("The active probably of a bubble spawn based of randomization")]
    private int activeProbability = 0;
    [Help("ProbabilityLimit and CurrentProbability are only used by large bubbles")]
    [EnumConditionalEnable("bubbleType", (int)BubbleType.Large)]
    [SerializeField, Tooltip("LARGE BUBBLE ONLY - The maximum amount of times before a bubble of this type is forcibly spawned")]
    private int probabilityLimit = 3;
    [EnumConditionalEnable("bubbleType", (int)BubbleType.Large)]
    [SerializeField, Tooltip("LARGE BUBBLE ONLY - The current maximum amount of tries before a bubble is forcibly spawned")]
    private int currentProbability = 0;

    /// <summary>
    /// Get the bubble type
    /// </summary>
    public BubbleType GetBubbleType() => this.bubbleType;

    /// <summary>
    /// Get the spawn interval
    /// </summary>
    public float GetSpawnInterval() => this.spawnInterval;

    /// <summary>
    /// Get the active probability
    /// </summary>
    public int GetActiveProbability() => this.activeProbability;

    /// <summary>
    /// Set the active probability
    /// </summary>
    public void SetActiveProbability(int activeProbability) => this.activeProbability = activeProbability;

    /// <summary>
    /// Get the probability limit
    /// </summary>
    public int GetProbabilityLimit() => this.probabilityLimit;

    /// <summary>
    /// Get the current probability
    /// </summary>
    public int GetCurrentProbability() => this.currentProbability;

    /// <summary>
    /// Set the current probability
    /// </summary>
    public void SetCurrentProbability(int currentProbability) => this.currentProbability = currentProbability;
}
