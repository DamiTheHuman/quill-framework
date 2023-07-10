using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// A class to manage a list of a time line triggers that make up a cutscene
/// </summary>
public class CutsceneController : MonoBehaviour
{
    [SerializeField, Tooltip("The list of time line triggers")]
    private List<TimelineTriggerController> timelineTriggers = new List<TimelineTriggerController>();

    [SerializeField, Tooltip("The audio source for the cutscene"), IsDisabled]
    private AudioSource audioSource;

    private void Awake() => this.audioSource = this.gameObject.AddComponent<AudioSource>();

    /// <summary>
    /// Get the time line triggers set in <see cref="timelineTriggers"/>
    /// </summary>
    public List<TimelineTriggerController> GetTimelineTriggers() => this.timelineTriggers;


    /// <summary>
    /// Get the audio source for the cutscene
    /// </summary>
    public AudioSource GetAudioSource() => this.audioSource;


    private void Reset()
    {
        if (this.timelineTriggers.Count == 0)
        {
            this.timelineTriggers = this.GetComponentsInChildren<TimelineTriggerController>().ToList();
        }
    }
}
