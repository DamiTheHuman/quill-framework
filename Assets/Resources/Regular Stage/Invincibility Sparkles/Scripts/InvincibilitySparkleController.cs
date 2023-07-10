using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Controls the circular movement of every individual sparkle
///  Original Author - Nihil  [Core Framework] 
///  Added functionality - Made it as close to mania's from what can be seen visualy
/// </summary>
public class InvincibilitySparkleController : MonoBehaviour
{
    [FirstFoldOutItem("Dependencies")]
    public SpriteRenderer spriteRenderer;
    private Color startColor;
    public InvincibilitySparkType invincibilitySparkType;
    [SerializeField, LastFoldoutItem()]
    private Animator animator;
    [Tooltip("The id of the sparkle"), SerializeField]
    private int sparkleID = 0;
    [Tooltip("The radius of the sparkle"), SerializeField]
    private float sparkleRadius = 18;
    [Tooltip("The delta (rotation in regards to pivot) of the sparkle"), SerializeField]
    private float delta = 0;

    private void Reset()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.invincibilitySparkType = this.GetComponentInParent<InvincibilitySparkType>();
        this.animator = this.GetComponent<Animator>();
    }

    private void Awake()
    {
        if (this.animator == null)
        {
            this.Reset();
        }

        this.startColor = this.spriteRenderer.color;
    }

    private void OnEnable()
    {
        this.spriteRenderer.color = this.startColor;
        int animationOffset = 8;
        int animationLength = 12;

        switch (this.invincibilitySparkType.sparkleType)
        {
            case SparkleType.Type1:
                this.delta = 0 + (180 * this.sparkleID);
                animationLength = 12;
                animationOffset = 0;

                break;
            case SparkleType.Type2:
                this.delta = 45 + (180 * this.sparkleID);
                animationLength = 20;
                animationOffset = this.sparkleID == 1 ? 20 : 29;

                break;
            case SparkleType.Type3:
                this.delta = 90 + (180 * this.sparkleID);
                animationLength = 24;
                animationOffset = this.sparkleID == 1 ? 14 : 3;

                break;
            case SparkleType.Type4:
                this.delta = 135 + (180 * this.sparkleID);
                animationLength = 24;
                animationOffset = this.sparkleID == 1 ? 9 : 19;

                break;
            default:
                break;
        }

        this.StartIdleAnimationAtIndex(animationOffset, animationLength);
        this.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Set the <see cref="sparkleID"/>
    /// </summary>
    /// <param name="sparkleId"></param>
    public void SetSparkleId(int sparkleId) => this.sparkleID = sparkleId;

    private void FixedUpdate() => this.UpdateSparklePosition(this.transform.position);
    /// <summary>
    /// Starts the idle animation at the specified index
    /// <param name="offset">The offset to play the animation clip</param>
    /// <param name="limit">The length of the animation clip </param>
    /// </summary>
    public void StartIdleAnimationAtIndex(float offset, float limit)
    {
        float animationFrameIndex = offset;
        animationFrameIndex /= limit;
        this.animator.Play("Idle", 0, animationFrameIndex);
    }

    /// <summary>
    /// Update the individual sparkle position rotating it arounds the parent trail renderer
    /// </summary>
    private void UpdateSparklePosition(Vector2 position)
    {
        position.x = this.transform.parent.position.x + (Mathf.Cos(this.delta * Mathf.Deg2Rad) * this.sparkleRadius);
        position.y = this.transform.parent.position.y + (Mathf.Sin(this.delta * Mathf.Deg2Rad) * this.sparkleRadius);
        this.delta += this.invincibilitySparkType.GetDeltaIncrement();
        this.delta = General.ClampDegreeAngles(this.delta);

        this.transform.position = new Vector2(GMStageManager.Instance().ConvertToDeltaValue(position.x), GMStageManager.Instance().ConvertToDeltaValue(position.y));
    }

    /// <summary>
    /// A message called by the broadcast to inform all sparkles to fade out
    /// </summary>
    public void FadeOut() => this.StartCoroutine(this.LerpColorsOverTime(this.startColor, new Color(1, 1, 1, 0), General.StepsToSeconds(60)));
    /// <summary>
    /// A message called to reset the invinciblity coroutine traits if it is already fading out
    /// </summary>
    public void ResetFadeOut()
    {
        if (this.spriteRenderer.color != this.startColor)
        {
            this.StopAllCoroutines();
            this.spriteRenderer.color = this.startColor;
        }
    }

    /// <summary>
    /// Moves the color to the specified colour within the specified time frame 
    /// <param name="startingColor">The color at the beginning of the time frame </param>
    /// <param name="endingColor">The color displayed at the end of the time frame </param>
    /// <param name="time">The time frame to complete this change </param>
    /// </summary>
    private IEnumerator LerpColorsOverTime(Color startingColor, Color endingColor, float time)
    {
        float inversedTime = 1 / time;

        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            this.spriteRenderer.color = Color.Lerp(startingColor, endingColor, step);

            yield return null;
        }
    }

}
