using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stores information persistently on a per stage level  i.e checkpoints, special stages etc
/// </summary>
public class GMHistoryManager : MonoBehaviour
{
    [SerializeField, Tooltip("Determines whether we have score data that is going to be used on the next reload")]
    private bool hasScoreHistory = false;
    [Tooltip("The current shield the player has on prior to entering the stage")]
    public ShieldType activeShieldType = ShieldType.None;
    [SerializeField]
    private RegularStageHistoryData regularStageScoreHistoryData = new RegularStageHistoryData();
    [SerializeField]
    private SpecialStageScoreHistory specialStageScoreHistoryData = new SpecialStageScoreHistory();

    public static GMHistoryManager instance;
    private void Awake()
    {
        this.transform.parent = null;
        DontDestroyOnLoad(this);

        if (Instance() != null && Instance() != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);//Destroy duplicate spawn managers

            return;
        }
    }

    /// <summary>
    /// Everytime a new scene is loaded this is called, This will server as the Start function
    /// </summary>
    private void OnSceneLoaded() => instance = this;

    /// <summary>
    /// The actions performed on every scene reload
    /// </summary>
    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode) => this.OnSceneLoaded();

    /// <summary>
    /// Get the current special stage score history data
    /// </summary>
    public RegularStageHistoryData GetRegularStageScoreHistory() => this.regularStageScoreHistoryData;

    /// <summary>
    /// Get the current special stage score history data
    /// </summary>
    public SpecialStageScoreHistory GetSpecialStageScoreHistory() => this.specialStageScoreHistoryData;

    /// <summary>
    /// Get the currently active shield before entering a special stage
    /// </summary>
    /// 
    public ShieldType GetActiveShieldType() => this.activeShieldType;

    /// <summary>
    /// Sets the active shield type to keep track of when entering a special stage
    /// <param name="activeShieldType">The new active shield value</param>
    /// </summary>
    public void SetActiveShieldType(ShieldType activeShieldType) => this.activeShieldType = activeShieldType;

    /// <summary>
    /// Adds the start position as a spawn point
    /// </summary>
    public void CheckAddStartPointAsSpawnPoint()
    {
        //If there are currently no spawn points in our scene set the start point as the initial spawn point
        if (GMSceneManager.Instance().GetCurrentStartPoint() != null && this.regularStageScoreHistoryData.HasSpawnPointsOfType(SpawnPointType.CheckPoint) == false)
        {
            this.regularStageScoreHistoryData.AddSpawnPoint(SpawnPointType.CheckPoint, GMSceneManager.Instance().GetCurrentStartPoint().transform.position);
        }
    }

    /// <summary>
    /// Get a reference to the static instance of the history manager
    /// </summary>
    public static GMHistoryManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMHistoryManager>();
        }

        return instance;
    }

    /// <summary>
    /// Clears all the spawn point for a stage
    /// </summary>
    public void ClearHistoryInformation()
    {
        this.regularStageScoreHistoryData.GetActiveSpawnPointDataList().Clear();//This will cause the player to start at the initial spawn point
        this.regularStageScoreHistoryData.GetBrokenLifeMonitorDataList().Clear();
        this.activeShieldType = ShieldType.None;
        this.ClearHistory();
    }

    /// <summary>
    /// Clear the stage history
    /// </summary>
    public void ClearHistory()
    {
        this.regularStageScoreHistoryData = new RegularStageHistoryData();
        this.specialStageScoreHistoryData = new SpecialStageScoreHistory();
    }

    /// <summary>
    /// Saves the special stage history
    /// </summary>
    public void SaveSpecialStageHistory()
    {
        this.specialStageScoreHistoryData.SetSpecialStageState(GMSpecialStageManager.Instance().GetSpecialStageState());
        this.specialStageScoreHistoryData.SetScoreData(GMSpecialStageScoreManager.Instance().GetScoreData());
    }

    /// <summary>
    /// Set the vaue for has score history
    /// <param name="hasScoreHistory">The new value of has score historyt</param>
    /// </summary>
    public void SetHasScoreHistory(bool hasScoreHistory) => this.hasScoreHistory = hasScoreHistory;

    /// <summary>
    /// Check whether we have score history
    /// </summary>
    public bool HasScoreHistory() => this.hasScoreHistory;

    private void OnValidate()
    {
        for (int x = 0; x < this.regularStageScoreHistoryData.GetActiveSpawnPointDataList().Count; x++)
        {
            this.regularStageScoreHistoryData.GetActiveSpawnPointDataList()[x].SetName(General.TransformSpacesToUpperCaseCharacters(this.regularStageScoreHistoryData.GetActiveSpawnPointDataList()[x].GetSpawnPointType().ToString() + " " + (x + 1)));
        }
    }
}
