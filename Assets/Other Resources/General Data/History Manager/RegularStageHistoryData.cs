using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A class that stores data relating to the regular stage
/// Its main purpose is to be used when switching from regular stage to special stage then back but it can be applied to other use cases
/// </summary>
[System.Serializable]
public class RegularStageHistoryData
{
    [Tooltip("The active respawn points of the player the most recent respawn point is always used"), SerializeField]
    private List<SpawnPointData> activeSpawnPointDataList = new List<SpawnPointData>();
    [Tooltip("The list of broken life monitors in this zone"), SerializeField]
    private List<BrokenLifeMonitorData> brokenLifeMonitorDataList = new List<BrokenLifeMonitorData>();
    [SerializeField, Tooltip("The current score data based on the history")]
    private ScoreData scoreData = new ScoreData();

    /// <summary>
    /// Saves the current stages history to be used
    /// </summary>
    public void SaveStageHistory()
    {
        this.scoreData = GMRegularStageScoreManager.Instance().GetScoreData();
        GMHistoryManager.Instance().SetHasScoreHistory(true);
    }

    /// <summary>
    /// Gets a list of active spawn point data
    /// </summary>
    public List<SpawnPointData> GetActiveSpawnPointDataList() => this.activeSpawnPointDataList;

    /// <summary>
    /// Gets a list of brokene life monitor data
    /// </summary>
    public List<BrokenLifeMonitorData> GetBrokenLifeMonitorDataList() => this.brokenLifeMonitorDataList;

    /// <summary>
    /// Get the score data
    /// </summary>
    public ScoreData GetScoreData() => this.scoreData;

    /// <summary>
    /// Adds an active spawn point, the most recent spawn point will be used to determine the players position on reload
    /// <param name="spawnPointType">The type of spawn point to add </param>
    /// <param name="spawnPoint">The spawn point to add</param>
    /// </summary>
    public void AddSpawnPoint(SpawnPointType spawnPointType, Vector2 spawnPoint)
    {
        List<Vector2> positions = this.GetSpawnPointListByType(spawnPointType);

        if (positions.Contains(spawnPoint) == false)
        {
            this.activeSpawnPointDataList.Add(new SpawnPointData(spawnPointType, spawnPoint, this.activeSpawnPointDataList.Count));
        }
    }

    /// <summary>
    /// Adds a broken life monitor to the history
    /// <param name="monitor">The monitor to add </param>
    /// <param name="spawnPoint">The position of the monitor</param>
    /// </summary>
    public void AddBrokenLifeMonitor(MonitorController monitor, Vector2 spawnPoint)
    {
        if (monitor.GetPowerUpToGrant() != PowerUp.ExtraLife)
        {
            return;
        }

        List<Vector2> positions = this.GetBrokenLifeMonitorPositions();

        if (positions.Contains(spawnPoint) == false)
        {
            this.brokenLifeMonitorDataList.Add(new BrokenLifeMonitorData(monitor, spawnPoint, this.brokenLifeMonitorDataList.Count));
        }
    }

    /// <summary>
    /// Adds the position of a life icon monitor to to the list
    /// <param name="spawnPointType">The type of spawn point to add </param>
    /// <param name="spawnPoint">The spawn point to add</param>
    /// </summary>
    public void AddMonitorLifeIcon(SpawnPointType spawnPointType, Vector2 spawnPoint)
    {
        List<Vector2> positions = this.GetSpawnPointListByType(spawnPointType);

        if (positions.Contains(spawnPoint) == false)
        {
            this.activeSpawnPointDataList.Add(new SpawnPointData(spawnPointType, spawnPoint, this.activeSpawnPointDataList.Count));
        }
    }

    /// <summary>
    /// Removes the spawn point at the specified positon
    /// <param name="spawnPointType">The type of spawn point to remove </param>
    /// <param name="spawnPoint">The spawn point to remove</param>
    /// </summary>
    public void RemoveSpawnPoint(SpawnPointType spawnPointType, Vector2 spawnPoint) => this.activeSpawnPointDataList.RemoveAll(x => x.GetSpawnPointType() == spawnPointType && x.GetPosition() == spawnPoint);

    /// <summary>
    /// Checks if the spawn point has already been registered
    /// <param name="spawnPointType">The type of spawn point to check for </param>
    /// <param name="spawnPoint">The spawn point position to check for</param>
    /// </summary>
    public bool CheckIfSpawnPointExists(SpawnPointType spawnPointType, Vector2 spawnPoint)
    {
        List<Vector2> positions = this.GetSpawnPointListByType(spawnPointType);

        if (this.activeSpawnPointDataList.Count == 0)
        {
            return false;
        }

        return positions.Contains(spawnPoint);
    }

    /// <summary>
    /// Checks if there is a broken life monitor at the specified position
    /// <param name="position">The sposition of the broken monitor</param>
    /// </summary>
    public bool CheckIfBrokenLifeMonitorAtPositionExists(Vector2 position)
    {
        List<Vector2> positions = this.GetBrokenLifeMonitorPositions();

        if (this.activeSpawnPointDataList.Count == 0)
        {
            return false;
        }

        return positions.Contains(position);
    }

    /// <summary>
    /// Gets the most recent spawn point of the stage
    /// <param name="spawnPointType">The type of spawn point to get</param>
    /// </summary>
    public Vector2 GetRecentSpawnPosition(SpawnPointType spawnPointType)
    {
        if (this.HasSpawnPointsOfType(spawnPointType) == false)
        {
            Debug.LogError("The Spawn Point You are checking for doesnt Exist!");
            Debug.Break();

            return new Vector2();
        }

        List<Vector2> positions = this.GetSpawnPointListByType(spawnPointType);

        return positions[positions.Count - 1];
    }

    /// <summary>
    /// Determines whether we the history object has any spawn points
    /// </summary>
    public bool HasSpawnPoints() => this.activeSpawnPointDataList.Count > 0;

    /// <summary>
    /// Determines whether we the history object has any broken monitors
    /// </summary>
    public bool HasBrokenMonitors() => this.brokenLifeMonitorDataList.Count > 0;

    /// <summary>
    /// Determines whether we the history object has a spawn point of the specified type
    /// <param name="spawnPointType">The type of spawn point to check for</param>
    /// </summary>
    public bool HasSpawnPointsOfType(SpawnPointType spawnPointType) => this.GetSpawnPointListByType(spawnPointType).Count > 0;

    /// <summary>
    /// Gets a list of spawn points in the specified type
    /// <param name="spawnPointType">The type of spawn point to get</param>
    /// </summary>
    public List<Vector2> GetSpawnPointListByType(SpawnPointType spawnPointType) => this.activeSpawnPointDataList.Where(x => x.GetSpawnPointType() == spawnPointType).Select(p => p.GetPosition()).ToList();

    /// <summary>
    /// Gets a list of positions of broken life monitors
    /// </summary>
    public List<Vector2> GetBrokenLifeMonitorPositions() => this.brokenLifeMonitorDataList.Select(p => p.GetPosition()).ToList();
}
