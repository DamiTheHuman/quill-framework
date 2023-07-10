using UnityEngine;

[System.Serializable]
public class InitializedObjectData
{
    [SerializeField]
    private GameObject gameObject;
    [SerializeField]
    private Vector2 startPosition;

    public InitializedObjectData(GameObject gameObject, Vector3 startPosition)
    {
        this.gameObject = gameObject;
        this.startPosition = startPosition;
    }

    /// <summary>
    /// Get the game object
    /// </summary>
    public GameObject GetGameObject() => this.gameObject;

    /// <summary>
    /// Get the start position
    /// </summary>
    public Vector2 GetStartPosition() => this.startPosition;
}
