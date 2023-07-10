using UnityEngine;
/// <summary>
/// Data pertaining to the special stage object in the scene
/// </summary>
[System.Serializable]
public class SpecialStageData
{
    [Tooltip("The scene id of the special stage"), SerializeField, SceneList]
    private int sceneId;
    [Tooltip("The position of the special stage object"), SerializeField]
    private Vector2Serializer position;

    public SpecialStageData(int sceneId, Vector2 position)
    {
        this.sceneId = sceneId;
        this.position.Fill(position);
    }

    /// <summary>
    /// Get the scene id of the special stage object
    /// </summary>
    public int GetSceneId() => this.sceneId;

    /// <summary>
    /// Get the position of the special stage object
    /// </summary>
    public Vector2 GetPosition() => this.position.Vector2;
}
