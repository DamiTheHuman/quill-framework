using UnityEngine;
/// <summary>
/// Data pertaining to the spawn point object in the scene
/// </summary>
[System.Serializable]
public class SpawnPointData
{
    [HideInInspector, SerializeField]
    private string name;
    [Tooltip("The type of spawn point"), SerializeField]
    private SpawnPointType spawnPointType;
    [Tooltip("The position of the spawn point"), SerializeField]
    private Vector2 position;

    public SpawnPointData(SpawnPointType spawnPointType, Vector2 position, int index = 0)
    {
        this.spawnPointType = spawnPointType;
        this.position = position;
        this.name = General.TransformSpacesToUpperCaseCharacters(this.spawnPointType.ToString() + " " + (index + 1));
    }

    /// <summary>
    /// Set the name of the spawn point type
    /// </summary>

    public string SetName(string name) => this.name = name;

    /// <summary>
    /// Get the spawn point type
    /// </summary>
    public SpawnPointType GetSpawnPointType() => this.spawnPointType;

    /// <summary>
    /// Get the position of the spawn point
    /// </summary>
    public Vector2 GetPosition() => this.position;
}
