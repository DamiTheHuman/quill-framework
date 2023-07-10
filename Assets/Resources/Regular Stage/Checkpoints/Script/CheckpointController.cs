/// <summary>
/// The checkpoint controller which registers its spawn position on contact
/// </summary>
public class CheckpointController : TriggerContactGimmick
{
    /// <summary>
    /// Checks if the current checkpoint is already active
    /// </summary>
    public virtual bool CheckPointIsActive() => GMHistoryManager.Instance().GetRegularStageScoreHistory().CheckIfSpawnPointExists(SpawnPointType.CheckPoint, this.transform.position);

    /// <summary>
    /// Registers a checkpoint position to the stage manager
    /// </summary>
    public virtual void RegistorCheckPointPosition() => GMHistoryManager.Instance().GetRegularStageScoreHistory().AddSpawnPoint(SpawnPointType.CheckPoint, this.transform.position);
}
