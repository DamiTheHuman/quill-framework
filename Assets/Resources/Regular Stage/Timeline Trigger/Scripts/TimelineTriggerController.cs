using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Get a reference to the timeline trigger
/// </summary>
public class TimelineTriggerController : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector timelinePlayableDirector;

    [SerializeField, Range(-1, 1)]
    private int cutSceneDirection = 1;

    [SerializeField]
    private TimelineTriggerType triggerType = TimelineTriggerType.MoveToPositionAndPlay;
    [SerializeField]
    private TimelineTriggerEndCondition triggerEndCondition = TimelineTriggerEndCondition.EndImmediately;

    [SerializeField, Tooltip("The type of audio the timeline trigger will use when played")]
    private CutsceneAudioType cutsceneAudioType = CutsceneAudioType.None;
    [SerializeField, EnumConditionalEnable(nameof(cutsceneAudioType), (int)CutsceneAudioType.UseBGM)]
    private BGMToPlay cutsceneBGM;
    [SerializeField, EnumConditionalEnable(nameof(cutsceneAudioType), (int)CutsceneAudioType.UseCutsceneAudio)]
    private AudioClip cutsceneAudioClip;

    [SerializeField, FirstFoldOutItem("Debug")]
    private Transform debugTransform;
    [SerializeField, LastFoldoutItem()]
    private Color debugColor = Color.green;

    private void Start() => this.Bind(this.timelinePlayableDirector, "Player", GMStageManager.Instance().GetPlayer().GetAnimatorManager().GetAnimator());

    /// <summary>
    /// Get the current trigger type
    /// </summary>
    public TimelineTriggerType GetTriggerType() => this.triggerType;

    /// <summary>
    /// Get the current End Trigger Type
    /// </summary>
    public TimelineTriggerEndCondition GetEndTriggerType() => this.triggerEndCondition;

    /// <summary>
    /// Get the current playable director
    /// </summary>
    public PlayableDirector GetTimelinePlayableDirector() => this.timelinePlayableDirector;

    /// <summary>
    /// Get the current direction
    /// </summary>
    public int GetCurrentDirection() => this.cutSceneDirection;


    /// <summary>
    /// Binds the instantiated to player and camera to the timeline
    /// </summary>
    public void Bind(PlayableDirector director, string trackName, Animator animator)
    {
        TimelineAsset timeline = director.playableAsset as TimelineAsset;

        foreach (TrackAsset track in timeline.GetOutputTracks())
        {
            if (track.name.Contains(trackName))
            {
                director.SetGenericBinding(track, animator);
            }
            else if (track.name.Contains("Camera"))
            {
                director.SetGenericBinding(track, HedgehogCamera.Instance().GetComponent<Animator>());
            }
        }
    }

    /// <summary>
    /// Get the the cutscene audio type
    /// </summary>
    public CutsceneAudioType GetCutsceneAudioType() => this.cutsceneAudioType;

    /// <summary>
    /// Get the the cutscene audio when the timeline is running
    /// </summary>
    public AudioClip GetCutsceneAudioClip() => this.cutsceneAudioClip;

    /// <summary>
    /// Get the the cutscene bgm to play when the timeline is running
    /// </summary>
    public BGMToPlay GetCutsceneBGM() => this.cutsceneBGM;

}
