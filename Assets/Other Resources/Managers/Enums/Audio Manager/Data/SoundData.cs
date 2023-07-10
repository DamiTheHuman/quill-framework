using System;
using UnityEngine;

[System.Serializable]
/// <summary>
/// A class which contains information for a specified main BGM sound
/// </summary>
public class SoundData
{
    [HideInInspector]
    public string name;
    [Tooltip("The tag of the sound to be played which is used to reference it as well")]
    public BGMToPlay soundTag;
    [Tooltip("The priority of the audio which determines what audio is prioritized when queuing")]
    public AudioPriority priority = AudioPriority.Low;
    [Tooltip("The clip attached to the sound")]
    public AudioClip clip;
    [Range(0f, 1f), Tooltip("The volume of the sound")]
    public float volume = 1;
    [Range(0.1f, 3f), Tooltip("The pitch of the sound")]
    public float pitch = 1;
    [Tooltip("Whether the audio source should loop on end"), Help("To Ensure Quality with Run Time made audio please use '.wav' files with a sample rate of 48000 Hz!")]
    public AudioLoopType loopType = AudioLoopType.None;
    [Tooltip("The audio source for the specified sound")]
    public AudioSource audioSource;
    [Help("If the Loop Type is set to SPLITINTROANDLOOP the values below must be set for both sections")]
    public AudioData BGMAudioData = new AudioData();

    /// <summary>
    /// Copy sound data from one object to another
    /// </summary>
    public void CopySoundData(SoundData sound)
    {
        this.name = sound.name;
        this.soundTag = sound.soundTag;
        this.priority = sound.priority;
        this.clip = sound.clip;
        this.volume = sound.volume;
        this.pitch = sound.pitch;
        this.loopType = sound.loopType;
        this.audioSource = sound.audioSource;
        if (sound.BGMAudioData != null)
        {
            this.BGMAudioData.CopyAudioData(sound.BGMAudioData);
        }
    }

    /// <summary>
    /// Checks whether the current auido clip is playing
    /// </summary>
    public bool IsPlaying()
    {
        switch (this.loopType)
        {
            case AudioLoopType.SplitIntroAndLoop:
                return this.BGMAudioData.GetIntroSection().GetAudioSource().isPlaying || this.BGMAudioData.GetLoopSection().GetAudioSource().isPlaying;
            case AudioLoopType.Regular:
                return this.audioSource.isPlaying;
            case AudioLoopType.None:
                return this.audioSource.isPlaying;
            default:
                return false;
        }
    }
}
