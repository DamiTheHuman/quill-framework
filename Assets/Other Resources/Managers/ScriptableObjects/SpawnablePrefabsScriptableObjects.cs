using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles a list of pooled objects that can be used regularly throughout every scene
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SpawnablePrefabs", order = 5)]
public class SpawnablePrefabsScriptableObjects : ScriptableObject
{
    public List<SpawnablePoolData> pool;

    private void OnValidate()
    {
        foreach (SpawnablePoolData pool in this.pool)
        {
            pool.SetName(General.TransformSpacesToUpperCaseCharacters(pool.GetTag().ToString()));
        }
    }
}
