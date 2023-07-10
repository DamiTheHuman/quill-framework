using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Handles the sounds and BGMS for the current active scene
/// </summary>
public class GMAudioManager : MonoBehaviour
{
    [Tooltip("The audio source for sounds that played via the oneshot in both ears"), SerializeField, FirstFoldOutItem("One Shot Audio Sources")]
    private AudioSource oneShotBothEars;
    [Tooltip("The audio source for onshots played in the left ear only"), SerializeField]
    private AudioSource oneShotLeftEarOnly;
    [Tooltip("The audio source for onshots played in the right ear only"), SerializeField, LastFoldoutItem()]
    private AudioSource oneShotRightEarOnly;
    [SerializeField]
    private BGMToPlay preQueueBGMSound;
    [SerializeField]
    private List<SoundData> BGMQueue = new List<SoundData>();
    [Tooltip("A list of sounds for the current act"), SerializeField]
    private ActBGMScritpableObject actSounds;

    [IsDisabled, SerializeField]
    private List<SoundData> currentBGMSounds = new List<SoundData>();

    [Tooltip("The currently playing audio track"), SerializeField]
    private SoundData currentAudioTrack;
    private static GMAudioManager instance;
    private IEnumerator cycleThroughQueueCoroutine;

    private void Awake()
    {
        AudioListener.pause = false;
        this.currentBGMSounds = new List<SoundData>();

        foreach (SoundData soundData in this.actSounds.BGMSounds)
        {
            SoundData copiedSoundData = new SoundData();
            copiedSoundData.CopySoundData(soundData);
            this.currentBGMSounds.Add(copiedSoundData);
        }

        if (this.oneShotBothEars == null)
        {
            this.oneShotBothEars = this.GetComponentInChildren<AudioSource>();
        }

        this.PopulateSoundPool();

        if (GMCutsceneManager.Instance().HasStartActTimeLineTriggers() == false)
        {
            this.ResumeAudio();
        }

        this.currentAudioTrack.CopySoundData(this.GetSound(BGMToPlay.MainBGM));
    }

    private void Start()
    {
        instance = this;

        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() is not SceneType.RegularStage and not SceneType.SpecialStage)
        {
            this.PlayBGM(BGMToPlay.MainBGM);
        }
    }

    /// <summary>
    /// Get a reference to the static instance of the audio manager
    /// </summary>
    public static GMAudioManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMAudioManager>();
        }

        return instance;
    }

    /// <summary>
    /// Populates the pool of main BGM sounds for the scene
    /// </summary>
    private void PopulateSoundPool()
    {
        GameObject bgmParent = new GameObject();
        bgmParent.transform.parent = this.transform;
        bgmParent.name = "BGM'S";

        //Create a set of gameobjects at the beginning which contains a reference to their own audio source
        foreach (SoundData sound in this.currentBGMSounds)
        {
            GameObject newBGMSound = new GameObject();
            newBGMSound.transform.parent = bgmParent.transform;
            newBGMSound.name = sound.soundTag.ToString();

            if (sound.loopType == AudioLoopType.SplitIntroAndLoop)
            {
                this.SetUpRunTimeBGM(sound, newBGMSound);
                //Build our intro track
                if (sound.BGMAudioData.GetIntroSection().GetAudioClip() == null)
                {
                    AudioClip introAudioClip = this.MakeSubclip(sound.clip, General.ConvertVector3TimeToSeconds(sound.BGMAudioData.GetIntroSection().GetStartTime()), General.ConvertVector3TimeToSeconds(sound.BGMAudioData.GetIntroSection().GetEndTime()), "Intro Point - ");
                    sound.BGMAudioData.GetIntroSection().SetAudioClip(introAudioClip);
                }

                if (sound.BGMAudioData.GetLoopSection().GetAudioClip() == null)
                {
                    AudioClip loopAudioClip = this.MakeSubclip(sound.clip, General.ConvertVector3TimeToSeconds(sound.BGMAudioData.GetLoopSection().GetStartTime()), General.ConvertVector3TimeToSeconds(sound.BGMAudioData.GetLoopSection().GetEndTime()), "Loop Point - ");
                    sound.BGMAudioData.GetLoopSection().SetAudioClip(loopAudioClip);
                }
            }
            else
            {
                sound.audioSource = newBGMSound.AddComponent<AudioSource>();
                sound.audioSource.clip = sound.clip;
                sound.audioSource.volume = sound.volume;
                sound.audioSource.pitch = sound.pitch;
                sound.audioSource.playOnAwake = false;

                if (sound.loopType != AudioLoopType.None)
                {
                    sound.audioSource.loop = true;
                }
            }
        }
    }

    /// <summary>
    /// Sets up the audio clip data and information relating to a BGM that uses a custom loop i.e 2 tracks
    /// <param name="sound">The sound the intro and loop section belong to </param>
    /// <param name="newBGMSound">The BGM sound this is parented to</param>
    /// </summary>
    private void SetUpRunTimeBGM(SoundData sound, GameObject newBGMSound)
    {
        //Create the intro audio source 
        GameObject BGMSoundIntroSection = new GameObject();
        BGMSoundIntroSection.transform.parent = newBGMSound.transform;
        BGMSoundIntroSection.name = "Intro Section - " + newBGMSound.name;
        sound.BGMAudioData.GetIntroSection().SetAudioSource(BGMSoundIntroSection.AddComponent<AudioSource>().GetComponent<AudioSource>());
        sound.BGMAudioData.GetIntroSection().GetAudioSource().volume = sound.volume;
        sound.BGMAudioData.GetIntroSection().GetAudioSource().pitch = sound.pitch;
        sound.BGMAudioData.GetIntroSection().GetAudioSource().playOnAwake = false;

        //Create the loop section audio source
        GameObject BGMSoundLoopSection = new GameObject
        {
            name = "Loop Section - " + newBGMSound.name
        };

        BGMSoundLoopSection.transform.parent = newBGMSound.transform;
        sound.BGMAudioData.GetLoopSection().SetAudioSource(BGMSoundLoopSection.AddComponent<AudioSource>());
        sound.BGMAudioData.GetLoopSection().GetAudioSource().volume = sound.volume;
        sound.BGMAudioData.GetLoopSection().GetAudioSource().pitch = sound.pitch;
        sound.BGMAudioData.GetLoopSection().GetAudioSource().playOnAwake = false;
        sound.BGMAudioData.GetLoopSection().GetAudioSource().loop = true;
    }

    /// <summary>
    /// Get a sound in <see cref="BGMSounds"/> by its tag
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public SoundData GetSound(BGMToPlay soundTag) => this.currentBGMSounds.FirstOrDefault(s => s.soundTag == soundTag);

    /// <summary>
    /// Plays a one shot audio clip
    /// <param name="clip">The clip to one shot</param>
    /// </summary>
    public void PlayOneShot(AudioClip clip, OneShotSoundDirection oneShotAudioType = OneShotSoundDirection.BothEars)
    {
        if (clip == null)
        {
            Debug.LogWarning("Cannot oneshot a null clip");

            return;
        }

        switch (oneShotAudioType)
        {
            case OneShotSoundDirection.BothEars:
                this.oneShotBothEars.PlayOneShot(clip);

                break;
            case OneShotSoundDirection.LeftEarOnly:
                this.oneShotLeftEarOnly.PlayOneShot(clip);

                break;
            case OneShotSoundDirection.RightEarOnly:
                this.oneShotRightEarOnly.PlayOneShot(clip);

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Plays the specified main BGM sound requested
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public SoundData PlayBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound == null)
        {
            Debug.Log("No Sounds Available");

            return null;
        }

        if (soundTag == BGMToPlay.ActClearJingle)//When the act clear jingle is playing stop all other BGM
        {
            this.StopBGM(this.currentAudioTrack.soundTag);//Stop the current audio

            foreach (SoundData BGMSound in this.currentBGMSounds)
            {
                if (BGMSound.soundTag != BGMToPlay.ActClearJingle)
                {
                    this.StopBGM(BGMSound.soundTag);
                }
            }

            this.ClearQueue();
            this.cycleThroughQueueCoroutine = null;
        }

        //We are already playing that BGM
        if (this.currentAudioTrack.soundTag == soundTag && this.BGMIsPlaying(soundTag))
        {
            return this.currentAudioTrack;
        }

        if (this.currentAudioTrack.clip != null)
        {
            if (this.BGMQueue.Count == 0)
            {
                this.PauseBGM(this.currentAudioTrack.soundTag);//Pause the current audio track
            }
            else if (this.currentAudioTrack.soundTag != soundTag)
            {
                this.MuteBGM(this.currentAudioTrack.soundTag);
            }
        }

        this.currentAudioTrack.CopySoundData(sound);//Update the current track

        if (this.currentAudioTrack.loopType == AudioLoopType.SplitIntroAndLoop)//Plays the intro section and prepares to play the loop section
        {
            if (this.currentAudioTrack.BGMAudioData.GetIntroSection().GetAudioClip() == null)
            {
                AudioClip introAudioClip = this.MakeSubclip(this.currentAudioTrack.clip, General.ConvertVector3TimeToSeconds(this.currentAudioTrack.BGMAudioData.GetIntroSection().GetStartTime()), General.ConvertVector3TimeToSeconds(this.currentAudioTrack.BGMAudioData.GetIntroSection().GetEndTime()), "Intro Point - ");
                sound.BGMAudioData.GetIntroSection().SetAudioClip(introAudioClip);
                this.currentAudioTrack.BGMAudioData.GetIntroSection().SetAudioClip(introAudioClip);
            }

            if (this.currentAudioTrack.BGMAudioData.GetLoopSection().GetAudioClip() == null)
            {
                AudioClip loopAudioClip = this.MakeSubclip(this.currentAudioTrack.clip, General.ConvertVector3TimeToSeconds(this.currentAudioTrack.BGMAudioData.GetLoopSection().GetStartTime()), General.ConvertVector3TimeToSeconds(this.currentAudioTrack.BGMAudioData.GetLoopSection().GetEndTime()), "Loop Point - ");
                sound.BGMAudioData.GetLoopSection().SetAudioClip(loopAudioClip);
                this.currentAudioTrack.BGMAudioData.GetLoopSection().SetAudioClip(loopAudioClip);
            }

            this.currentAudioTrack.BGMAudioData.GetIntroSection().GetAudioSource().Play();
            this.currentAudioTrack.BGMAudioData.GetLoopSection().GetAudioSource().Stop();
            this.currentAudioTrack.BGMAudioData.GetLoopSection().GetAudioSource().PlayDelayed(this.currentAudioTrack.BGMAudioData.GetIntroSection().GetAudioClip().length);
        }
        else
        {
            this.currentAudioTrack.audioSource.Stop();
            this.currentAudioTrack.audioSource.Play();
        }

        return this.currentAudioTrack;
    }

    /// <summary>
    /// Pause the BGM Requested
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public void PauseBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound.loopType == AudioLoopType.SplitIntroAndLoop)
        {
            sound.BGMAudioData.GetIntroSection().GetAudioSource().Pause();
            sound.BGMAudioData.GetLoopSection().GetAudioSource().Pause();
        }
        else
        {
            sound.audioSource.Pause();
        }
    }

    /// <summary>
    /// Unpause the BGM Requested
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public void UnpauseBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound.loopType == AudioLoopType.SplitIntroAndLoop)
        {
            sound.BGMAudioData.GetIntroSection().GetAudioSource().UnPause();
            sound.BGMAudioData.GetLoopSection().GetAudioSource().UnPause();
        }
        else
        {
            sound.audioSource.UnPause();
        }
    }

    /// <summary>
    /// Mute the BGM requested by setting its volume to 0
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public void MuteBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound.loopType == AudioLoopType.SplitIntroAndLoop)
        {
            sound.BGMAudioData.GetIntroSection().GetAudioSource().mute = true;
            sound.BGMAudioData.GetLoopSection().GetAudioSource().mute = true;
        }
        else
        {
            sound.audioSource.mute = true;
        }
    }

    /// <summary>
    /// UnMute the BGM requested by setting its volume to its default
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public SoundData UnMuteBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound.loopType == AudioLoopType.SplitIntroAndLoop)
        {
            sound.BGMAudioData.GetIntroSection().GetAudioSource().mute = false;
            sound.BGMAudioData.GetLoopSection().GetAudioSource().mute = false;
        }
        else
        {
            sound.audioSource.mute = false;//Back to defualt volume
        }

        return sound;
    }

    /// <summary>
    /// Checks if the BGM in question is currently playing
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public bool BGMIsPlaying(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound == null)
        {
            return false;
        }

        return sound.IsPlaying();
    }

    /// <summary>
    /// Stops the specified BGM
    /// </summary>
    public SoundData StopBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (sound.loopType == AudioLoopType.SplitIntroAndLoop)
        {
            sound.BGMAudioData.GetIntroSection().GetAudioSource().Stop();
            sound.BGMAudioData.GetLoopSection().GetAudioSource().Stop();
        }
        else
        {
            sound.audioSource.Stop();
        }

        if (this.BGMQueue.Count > 0)
        {
            this.BGMQueue.RemoveAll(s => s.soundTag == sound.soundTag);//Remove the sound from the queue
        }

        return sound;
    }

    /// <summary>
    /// Pause the audio for the entire scene
    /// </summary>
    public void PauseAudio()
    {
        //Stop the queue so the coroutine doesnt think the BGM has ended
        if (this.BGMQueue.Count > 0)
        {
            this.StopCoroutine(this.cycleThroughQueueCoroutine);
        }

        AudioListener.pause = true;
    }

    /// <summary>
    /// Resume the active audio track
    /// </summary>
    public void ResumeAudio()
    {
        AudioListener.pause = false;

        if (this.BGMQueue.Count > 0)
        {
            //Restart the Queue from the current list
            this.cycleThroughQueueCoroutine = this.CycleThroughQueue();
            this.StartCoroutine(this.cycleThroughQueueCoroutine);
        }
    }

    /// <summary>
    /// Clears the current BGM Queue
    /// </summary>
    public void ClearQueue() => this.BGMQueue.Clear();

    /// <summary>
    /// Play a BGM and queue the next one to be played
    /// <param name="soundTag">The tag of the BGM </param>
    /// </summary>
    public void PlayAndQueueBGM(BGMToPlay soundTag)
    {
        SoundData sound = this.GetSound(soundTag);

        if (this.BGMQueue.Count == 0)
        {
            this.preQueueBGMSound = this.currentAudioTrack.soundTag;
        }

        if (this.BGMQueue.Contains(sound) == false)
        {
            this.BGMQueue.Add(sound);
        }
        else if (this.currentAudioTrack == sound && sound.loopType != AudioLoopType.None)//If the track is to be looped and is already playing and in queue ignore it so it loops seemlessly
        {
            return;
        }

        if (sound.priority >= this.currentAudioTrack.priority)  //if the sound to be played is of higher priority play the new sound and put the current track to the background;
        {
            if (this.currentAudioTrack.soundTag != sound.soundTag)
            {
                this.MuteBGM(this.currentAudioTrack.soundTag);//Mute the current BGM
            }

            this.UnMuteBGM(soundTag);
            this.PlayBGM(soundTag);//Play the BGM of the sound tag
            this.UnpauseBGM(this.preQueueBGMSound);//Unpause the prequeue BGM allowing it to play in the background while muted
        }
        else
        {
            sound.audioSource.mute = true;
            sound.audioSource.Play();
        }

        if (this.cycleThroughQueueCoroutine == null)
        {
            this.cycleThroughQueueCoroutine = this.CycleThroughQueue();
            this.StartCoroutine(this.cycleThroughQueueCoroutine);
        }
    }

    /// <summary>
    /// A coroutine which cycles through a list of queued songs and plays them appropriately
    /// When there are no more BGMS in the Queue the default Main BGM is then restored
    /// </summary>
    private IEnumerator CycleThroughQueue()
    {
        while (true)
        {
            yield return new WaitWhile(() => this.BGMIsPlaying(this.currentAudioTrack.soundTag) && this.BGMQueue.Count != 0);//wait for the current track to play out

            if (this.BGMQueue.Count == 0)
            {
                break;
            }

            if (this.currentAudioTrack != null)
            {
                this.BGMQueue.RemoveAll(x => x.soundTag == this.currentAudioTrack.soundTag);
                this.currentAudioTrack.audioSource.Stop();

                if (this.BGMQueue.Count == 0)
                {
                    break;
                }

                //Reorder queue by priority
                this.BGMQueue = this.BGMQueue.OrderBy(s => s.priority).ToList();// reorder the list by priority
                this.currentAudioTrack.CopySoundData(this.BGMQueue[this.BGMQueue.Count - 1]);
                this.UnMuteBGM(this.currentAudioTrack.soundTag);//Unmute the next track
            }
        }

        this.currentAudioTrack.CopySoundData(this.UnMuteBGM(this.preQueueBGMSound));//When the queue is finished restart the previous low bgm

        if (this.BGMIsPlaying(this.currentAudioTrack.soundTag))
        {
            this.UnMuteBGM(this.currentAudioTrack.soundTag);
        }

        this.cycleThroughQueueCoroutine = null;

        yield return null;
    }

    /// <summary>
    /// Creates a sub clip from an audio clip based off of the start time
    /// and the stop time. The new clip will have the same frequency as the original.
    /// <param name="clip">The clip to make a sub clip from</param>
    /// <param name="start">The start point of the clip in seconds</param>
    /// <param name="stop">The end point of the clip in seconds</param>
    /// <param name="prefix">The prefix name for the clip for debugging purposes</param>
    /// </summary>
    private AudioClip MakeSubclip(AudioClip clip, float start, float stop, string prefix = "")
    {
        if (clip == null)
        {
            Debug.Log("No Clip Provided");

            return null;
        }

        stop *= 2;
        stop -= start;

        // Create a new audio clip
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(prefix + clip.name, samplesLength, 1, frequency * 2, false);
        float[] data = new float[samplesLength];// Create a temporary buffer for the samples 
        clip.GetData(data, (int)(frequency * start));// Get the data from the original clip 
        newClip.SetData(data, 0); // Transfer the data to the new clip

        return newClip;
    }
}

