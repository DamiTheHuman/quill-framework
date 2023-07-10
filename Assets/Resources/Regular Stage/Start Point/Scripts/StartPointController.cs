using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Doesnt do much but lets the start point object easy identifiable
/// </summary>
[ExecuteInEditMode]
public class StartPointController : MonoBehaviour
{
    public StartPointType startPointType = StartPointType.Default;
    [System.Serializable]
    protected class StartPointCharacterIcon
    {
        [HideInInspector]
        public string name = "";
        public PlayableCharacter character;
        public GameObject characterIcon;
        public GameObject characterColorBlock;
    }

    [Tooltip("The cutscene to play when the player is positioned at the start point"), SerializeField]
    private CutsceneController actStartCutscene;

    [SerializeField]
    protected List<StartPointCharacterIcon> startPointCharacterIcon;
    [SerializeField]
    protected float playerWillSpawnAtTextOffsetY = 48;
    [SerializeField]
    protected GameObject playerWillSpawnAtHolder;
    [SerializeField]
    protected TextMesh playerStartPoint;

    [SerializeField, EnumConditionalEnable("startPointType", 1)]
    private List<PlayableCharacter> charactersToSpawnAtPoint = new List<PlayableCharacter>();

    /// <summary>
    /// Returns a list of characters set to spawn at a unique point
    /// </summary>
    public List<PlayableCharacter> GetCharactersToSpawnAtUniquePoint() => this.charactersToSpawnAtPoint;


    private void LateUpdate()
    {
        if (Application.IsPlaying(this))
        {
            return;
        }

        this.UpdateIcons();
    }
    protected void OnValidate()
    {
        foreach (StartPointCharacterIcon spawnCharacterIcon in startPointCharacterIcon)
        {
            spawnCharacterIcon.name = spawnCharacterIcon.character.ToString();
        }

        this.UpdateIcons();
    }

    /// <summary>
    /// Update the list of icons
    /// </summary>
    public virtual void UpdateIcons()
    {
        if (this.startPointType == StartPointType.CharacterSpecific)
        {
            this.CharacterSpecificStartPointUpdate();

            return;
        }

        this.DefaultStartPointUpdate();
    }

    /// <summary>
    /// Updates the start point icons for default start point controllers
    /// </summary>
    protected void DefaultStartPointUpdate()
    {
        this.charactersToSpawnAtPoint.Clear();

        foreach (StartPointCharacterIcon startPointCharacterIcon in this.startPointCharacterIcon)
        {
            startPointCharacterIcon.characterIcon.SetActive(true);
            startPointCharacterIcon.characterColorBlock.SetActive(true);
        }

        StartPointController[] characterSpecificStartPoints = FindObjectsOfType<StartPointController>().Where(x => x.startPointType == StartPointType.CharacterSpecific && x.gameObject.activeSelf).ToArray();

        if (characterSpecificStartPoints == null)
        {
            return;
        }

        foreach (StartPointController characterSpecificStartPoint in characterSpecificStartPoints)
        {
            List<StartPointCharacterIcon> startPointIconsToDisable = this.startPointCharacterIcon.Where(x => characterSpecificStartPoint.GetCharactersToSpawnAtUniquePoint().Contains(x.character)).ToList();

            foreach (StartPointCharacterIcon startPointIconToDisable in startPointIconsToDisable)
            {
                startPointIconToDisable.characterIcon.SetActive(false);
                startPointIconToDisable.characterColorBlock.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Updates the start point icons for character specific start point controllers
    /// </summary>
    protected void CharacterSpecificStartPointUpdate()
    {
        foreach (StartPointCharacterIcon startPointCharacterIcon in this.startPointCharacterIcon)
        {
            startPointCharacterIcon.characterIcon.SetActive(false);
            startPointCharacterIcon.characterColorBlock.SetActive(false);
        }

        List<StartPointCharacterIcon> startPointIconsToDisable = this.startPointCharacterIcon.Where(x => this.GetCharactersToSpawnAtUniquePoint().Contains(x.character)).ToList();

        foreach (StartPointCharacterIcon startPointIconToEnable in startPointIconsToDisable)
        {
            startPointIconToEnable.characterIcon.SetActive(true);
            startPointIconToEnable.characterColorBlock.SetActive(true);
        }

        StartPointController[] startPointControllers = FindObjectsOfType<StartPointController>();

        foreach (StartPointController startPointController in startPointControllers)
        {
            if (startPointController.startPointType == StartPointType.CharacterSpecific)
            {
                continue;
            }

            startPointController.UpdateIcons();
        }
    }

    /// <summary>
    /// Gets the act start cutscene controller
    /// </summary>
    public CutsceneController GetActStartCutscene() => this.actStartCutscene;

    private void OnDrawGizmos()
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage)
        {
            Player player = FindObjectOfType<Player>();

            if ((player != null && Application.isPlaying == false) || (player != null && Application.isPlaying && GMStageManager.Instance().GetStageState() == RegularStageState.Idle && GMHistoryManager.Instance().GetRegularStageScoreHistory().GetRecentSpawnPosition(SpawnPointType.CheckPoint) == (Vector2)this.transform.position))
            {
                this.playerWillSpawnAtHolder.SetActive(true);
                this.playerStartPoint.text = this.transform.position.ToString();
                this.playerWillSpawnAtHolder.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + this.playerWillSpawnAtTextOffsetY);
            }
            else
            {
                this.playerWillSpawnAtHolder.transform.position = new Vector2(this.transform.position.x, this.transform.position.y + this.playerWillSpawnAtTextOffsetY);
                this.playerWillSpawnAtHolder.SetActive(false);
            }
        }
    }
}
