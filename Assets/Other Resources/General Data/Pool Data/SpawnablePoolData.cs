using UnityEngine;
[System.Serializable]
/// <summary>
/// Contains a data about our pool
/// </summary>
public class SpawnablePoolData
{
    [HideInInspector, SerializeField]
    private string name;
    [SerializeField, Tooltip("The tag of the pool data")]
    private ObjectToSpawn tag;
    [SerializeField, Tooltip("The prefab to instantiate and map to the tag")]
    private GameObject prefab;
    [SerializeField, Tooltip("How many prefabs to make")]
    private int size = 50;

    /// <summary>
    /// Set the name of the pool data
    /// </summary>
    public string SetName(string name) => this.name = name;

    /// <summary>
    /// Get the tag of the pool data items
    /// </summary>
    public ObjectToSpawn GetTag() => this.tag;

    /// <summary>
    /// Get the prefab to be cloned of the prefab
    /// </summary>
    public GameObject GetPrefab() => this.prefab;

    /// <summary>
    /// Get how many prefabs need to be made
    /// </summary>
    public int GetSize() => this.size;
}
