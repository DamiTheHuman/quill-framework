using UnityEngine;
using UnityEngine.Video;
/// <summary>
/// Simple helper for changing scenes once a video is done playing
/// </summary>
public class VideoTransitionHelper : MonoBehaviour
{
    [SerializeField, Tooltip("The video playing")]
    private VideoPlayer videoPlayer;

    private void Reset() => this.videoPlayer = this.GetComponent<VideoPlayer>();

    private void Start()
    {
        if (this.videoPlayer == null)
        {
            this.Reset();
        }

        this.videoPlayer.loopPointReached += this.OnVideoOver;
    }

    /// <summary>
    /// Switch to the next scene once the video is over
    /// </summary>
    /// <param name="videoPlayer"></param>
    private void OnVideoOver(VideoPlayer videoPlayer) => GMSceneManager.Instance().LoadNextScene();
}
