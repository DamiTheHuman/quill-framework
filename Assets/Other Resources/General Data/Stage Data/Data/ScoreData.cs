using UnityEngine;
/// <summary>
/// Data pertaining to the score the player has on a stage
/// </summary>
[System.Serializable]
public class ScoreData
{
    [FirstFoldOutItem("Scores"), Tooltip("The amount of rings the player currently has"), SerializeField]
    public int ringCount = 0;
    [Tooltip("The current score attained by the player"), SerializeField]
    public int scoreCount = 0;
    [Tooltip("The current player life count"), SerializeField]
    public int lifeCount = 3;
    [Tooltip("The current timer count"), SerializeField]
    public float timerCount = 0;
}
