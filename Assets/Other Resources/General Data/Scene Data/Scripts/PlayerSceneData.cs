using UnityEngine;

[System.Serializable]
public class PlayerSceneData
{
    [HideInInspector]
    public string name;

    [SerializeField]
    private SceneType sceneType = SceneType.Transition;
    [EnumConditionalEnable(nameof(sceneType), 2)]
    private ActNumber actNumber = ActNumber.NotAvailable;
    [SceneList, SerializeField()]
    private int sceneId;
    [SceneList, SerializeField()]
    private int nextSceneId;

    public PlayerSceneData() { }

    public PlayerSceneData(SceneType sceneType, ActNumber actNumber, int sceneId, int nextSceneId, string name = "")
    {
        this.sceneType = sceneType;
        this.actNumber = actNumber;
        this.sceneId = sceneId;
        this.nextSceneId = nextSceneId;
        this.name = name;
    }

    /// <summary>
    /// Set the scene type
    /// <paramref name="sceneType"/>
    /// </summary
    public void SetSceneType(SceneType sceneType) => this.sceneType = sceneType;

    /// <summary>
    /// Get the scene type
    /// </summary
    public SceneType GetSceneType() => this.sceneType;

    /// <summary>
    /// Set the act number
    /// <paramref name="actNumber"/>
    /// </summary
    public void SetActNumber(ActNumber actNumber) => this.actNumber = actNumber;

    /// <summary>
    /// Get the act number
    /// </summary
    public ActNumber GetActNumber() => this.actNumber;

    /// <summary>
    /// Set the scen Id
    /// <paramref name="sceneId"/>
    /// </summary
    public void SetSceneId(int sceneId) => this.sceneId = sceneId;

    /// <summary>
    /// Get the scene Id
    /// </summary
    public int GetSceneId() => this.sceneId;

    /// <summary>
    /// Set the next scene Id
    /// <paramref name="nextSceneId"/>
    /// </summary
    public void SetNextSceneId(int nextSceneId) => this.nextSceneId = nextSceneId;

    /// <summary>
    /// Get the next Scene Id
    /// </summary
    public int GetNextSceneId() => this.nextSceneId;
}
