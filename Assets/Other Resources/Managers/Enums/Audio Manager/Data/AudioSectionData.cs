using UnityEngine;

[System.Serializable]
public class AudioSectionData
{
    [Help("X = Minutes | Y = Seconds | Z = MilliSeconds")]
    [Tooltip("The time in which this clip will end in MM:ss:ms"), SerializeField]
    private Vector3 startTime;
    [Tooltip("The time in which this clip will end in MM:ss:ms"), SerializeField]
    private Vector3 endTime;
    [Tooltip("The audio clip for the specified sound"), SerializeField]
    private AudioClip audioClip;
    [Tooltip("The audio source for the specified sound"), SerializeField]
    private AudioSource audioSource;

    /// <summary>
    /// Sets the audioclip for the BGM Data
    /// <param name="audioClip">The audioclip to set</param>
    /// </summary>
    public void SetAudioClip(AudioClip audioClip)
    {
        this.audioClip = audioClip;
        this.audioSource.clip = audioClip;
    }

    /// <summary>
    /// Copy the audio section data from one object to the other
    /// </summary>
    public void CopyAudioSectionData(AudioSectionData audioSectionData)
    {
        this.startTime = audioSectionData.startTime;
        this.endTime = audioSectionData.endTime;
        this.audioClip = audioSectionData.audioClip;
        this.audioSource = audioSectionData.audioSource;
    }

    /// <summary>
    /// Get the audio clip
    /// </summary>
    public AudioClip GetAudioClip() => this.audioClip;

    /// <summary>
    /// Get the audio soruce
    /// </summary>
    public AudioSource GetAudioSource() => this.audioSource;

    /// <summary>
    /// Set the audio soruce
    /// </summary>
    public void SetAudioSource(AudioSource audioSource) => this.audioSource = audioSource;

    /// <summary>
    /// Get the start time
    /// </summary>
    public Vector3 GetStartTime() => this.startTime;

    /// <summary>
    /// Get the end time
    /// </summary>
    public Vector3 GetEndTime() => this.endTime;
}
