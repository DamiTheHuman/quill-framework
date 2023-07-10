using UnityEngine;

[System.Serializable]
public class SpriteEffectsData
{
    [HideInInspector, SerializeField]
    protected string name;
    [SerializeField]
    protected Animator animator;
    [SerializeField]
    protected SpriteEffectToggle tag;
    [SerializeField]
    protected GameObject prefab;
    [SerializeField]
    protected Vector2 relativeSpawnPosition;

    /// <summary>
    /// Set the name of the pool data
    /// </summary>
    public string SetName(string name) => this.name = name;

    /// <summary>
    /// Get the tag of the pool data items
    /// </summary>
    public SpriteEffectToggle GetTag() => this.tag;

    /// <summary>
    /// Set the tag of the pool data items
    /// </summary>
    public void SetTag(SpriteEffectToggle tag) => this.tag = tag;

    /// <summary>
    /// Get the prefab to be cloned of the prefab
    /// </summary>
    public GameObject GetPrefab() => this.prefab;

    /// <summary>
    /// Set the prefab
    /// </summary>
    public void SetPrefab(GameObject prefab) => this.prefab = prefab;

    /// <summary>
    /// Get the relative spawn Position
    /// </summary>
    public Vector2 GetRelativeSpawnPosition() => this.relativeSpawnPosition;

    /// <summary>
    /// Set the relative spawn Position
    /// </summary>
    public void SetRelativeSpawnPosition(Vector2 relativeSpawnPosition) => this.relativeSpawnPosition = relativeSpawnPosition;

    /// <summary>
    /// Get the sprite effects Animator
    /// </summary>
    public Animator GetSpriteEffectAnimator() => this.animator;

    /// <summary>
    /// Set the sprite effects Animator
    /// </summary>
    public Animator SetAnimator(Animator animator) => this.animator = animator;
}
