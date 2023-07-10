using System.Collections;
using UnityEngine;
/// <summary>
/// Controls the on global blit effect primarly fading and fading out
/// </summary>
public class FXBlitController : MonoBehaviour
{
    [SerializeField]
    private Material material;
    [Tooltip("The current state of the fade in/out action"), SerializeField]
    private BlitTransitionState currentStateTransition = BlitTransitionState.None;
    [Tooltip("The time taken to fade in the screen elements"), SerializeField]
    private float fadeInTime = 0.5f;
    [Tooltip("The time taken to fade out the screen elements"), SerializeField]
    private float fadeOutTime = 0.5f;

    private int _redID;
    private int _greenID;
    private int _blueID;

    private void Awake()
    {
        if (this._redID == 0)
        {
            this._redID = Shader.PropertyToID("_Red");
            this._greenID = Shader.PropertyToID("_Green");
            this._blueID = Shader.PropertyToID("_Blue");
        }

        this.Reset();
    }

    private void OnDisable() => this.Reset();

    private void Reset()
    {
        this.material.SetFloat(this._redID, 0f);
        this.material.SetFloat(this._greenID, 0f);
        this.material.SetFloat(this._blueID, 0f);
    }

    /// <summary>
    /// Fades out the screen elements
    /// </summary>
    public void BeginFadeOut()
    {
        this.StartCoroutine(this.LerpColorsOverTime(0, -1, this.fadeOutTime));
        this.SetTransitionState(BlitTransitionState.FadeOut);
    }

    /// <summary>
    /// Fades in the screen elements
    /// </summary>
    public void BeginFadeIn()
    {
        this.StartCoroutine(this.LerpColorsOverTime(-1, 0, this.fadeInTime));
        this.SetTransitionState(BlitTransitionState.FadeIn);
    }

    /// <summary>
    /// Get the fade in time
    /// </summary>
    public float GetFadeInTime() => this.fadeInTime;

    /// <summary>
    /// Get the fade out time
    /// </summary>
    public float GetFadeOutTime() => this.fadeOutTime;

    /// <summary>
    /// Sets the current transition state of the scene
    /// </summary>
    public void SetTransitionState(BlitTransitionState transitionState) => this.currentStateTransition = transitionState;

    /// <summary>
    /// Get the current transition state of the fade out 
    /// </summary>
    public BlitTransitionState GetTransitionState() => this.currentStateTransition;

    /// <summary>
    /// Moves the color to the specified colour within the specified time frame 
    /// <param name="startingColor">The color at the beginning of the time frame </param>
    /// <param name="endingColor">The color displayed at the end of the time frame </param>
    /// <param name="time">The time frame to complete this change </param>
    /// </summary>
    private IEnumerator LerpColorsOverTime(float startingColor, float endingColor, float time)
    {
        this.material.SetFloat(this._redID, startingColor);
        this.material.SetFloat(this._greenID, startingColor);
        this.material.SetFloat(this._blueID, startingColor);

        yield return new WaitForEndOfFrame();

        float inversedTime = 1 / time;

        for (float step = 0.0f; step < 1.0f; step += Mathf.Min(Time.unscaledDeltaTime, 0.05f) * inversedTime)
        {
            float t = this.currentStateTransition == BlitTransitionState.FadeIn ? (1 - (step / inversedTime / time)) * 3f : step / inversedTime / time * 3f;
            this.material.SetFloat(this._redID, -Mathf.Clamp01(t));
            this.material.SetFloat(this._greenID, -Mathf.Clamp01(t - 1f));
            this.material.SetFloat(this._blueID, -Mathf.Clamp01(t - 2f));

            yield return null;
        }

        this.material.SetFloat(this._redID, endingColor);
        this.material.SetFloat(this._greenID, endingColor);
        this.material.SetFloat(this._blueID, endingColor);
        this.SetTransitionState(BlitTransitionState.None);

        yield return null;
    }
}
