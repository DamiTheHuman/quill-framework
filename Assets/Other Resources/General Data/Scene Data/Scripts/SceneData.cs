using UnityEngine;
[System.Serializable]
public class SceneData
{
    [HideInInspector]
    public string name;
    [SerializeField]
    private SceneType sceneType = SceneType.Transition;
    [EnumConditionalEnable(nameof(sceneType), 2), SerializeField]
    private ActNumber actNumber = ActNumber.NotAvailable;
    [SceneList, SerializeField()]
    private int sceneId;
    [SceneList, SerializeField()]
    private int nextSceneId;
    [SerializeField]
    private Sprite thumbnail;
    [SerializeField(), EnumConditionalEnable(nameof(sceneType), (int)SceneType.RegularStage)]
    private string actAbbreviation = "GHZ";

    public SceneData() { }

    public SceneData(SceneType sceneType, ActNumber actNumber, int sceneId, int nextSceneId, Sprite thumbnail, string actAbbreviation = "GHZ", string name = "")
    {
        this.sceneType = sceneType;
        this.actNumber = actNumber;
        this.sceneId = sceneId;
        this.nextSceneId = nextSceneId;
        this.thumbnail = thumbnail;
        this.actAbbreviation = actAbbreviation;
        this.name = name;
    }

    public SceneData(SceneData sceneData)
    {
        this.sceneId = sceneData.sceneId;
        this.nextSceneId = sceneData.nextSceneId;
        this.name = sceneData.name;
        this.thumbnail = sceneData.thumbnail;
        this.sceneType = sceneData.sceneType;
        this.actNumber = sceneData.actNumber;
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
    /// Set the thumbnail
    /// <paramref name="thumbnail"/>
    /// </summary
    public void SetThumbnail(Sprite thumbnail) => this.thumbnail = thumbnail;

    /// <summary>
    /// Get thumbnail
    /// </summary
    public Sprite GetThumbnail() => this.thumbnail;

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

    /// <summary>
    /// Set the act abbreviation
    /// <paramref name="actAbbreviation"/>
    /// </summary
    public void SetActAbbreviation(string actAbbreviation) => this.actAbbreviation = actAbbreviation;

    /// <summary>
    /// Get the act abbreviation
    /// </summary
    public string GetActAbbreviation() => this.actAbbreviation;
}
