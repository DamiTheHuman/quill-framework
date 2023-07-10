using UnityEngine;
/// <summary>
/// A scriptable object which stores information about custom loop information of a BGM
/// </summary>
[System.Serializable]
public class AudioData
{
    [Tooltip("The BGM data of the intro section"), SerializeField]
    private AudioSectionData introSection = new AudioSectionData();
    [Tooltip("The BGM data of the loop section"), SerializeField]
    private AudioSectionData loopSection = new AudioSectionData();

    /// <summary>
    /// Copy Audio Data from one object to another
    /// </summary>
    public void CopyAudioData(AudioData bGMAudioData)
    {
        this.introSection.CopyAudioSectionData(bGMAudioData.introSection);
        this.loopSection.CopyAudioSectionData(bGMAudioData.loopSection);
    }

    /// <summary>
    /// Get the intro section
    /// </summary>
    public AudioSectionData GetIntroSection() => this.introSection;

    /// <summary>
    /// Get the loop section
    /// </summary>
    public AudioSectionData GetLoopSection() => this.loopSection;
}
