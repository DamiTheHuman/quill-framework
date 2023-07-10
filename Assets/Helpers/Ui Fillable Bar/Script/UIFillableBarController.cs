using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Base logic for  a reusable fillable bar that increases based on the value and the limits set
/// </summary>
public class UIFillableBarController : MonoBehaviour
{
    [Help("Set an animator if you want an animation value to playe when the bar is full")]
    [Tooltip("Optional animator which sets the animator value when a bar is full"), SerializeField]
    private Animator animator;
    [Tooltip("The animtor value set to true when the bar is maxed out"), SerializeField]
    private string animatorKey = "AtMax";
    [Tooltip("The image to update the bar with"), SerializeField]
    private Image image;
    [Tooltip("The value of the current bar"), SerializeField, Range(0, 100)]
    private float value = 50;
    [Tooltip("The offset of the bar from its start point"), SerializeField, Range(0, 100)]
    private float offset = 0f;
    [Tooltip("The relative max fill value that will serve as a multiplier for the bar"), SerializeField, Range(0, 100)]
    private float relativeMaxFillValue = 100;
    [Tooltip("The method used to fill the bar"), SerializeField]
    private FillMethod fillMethod = FillMethod.Lerp;
    [Tooltip("The speed the bar fills at"), SerializeField, EnumConditionalEnable("fillMethod", 0)]
    private float fillSpeed = 10f;
    private int animatorKeyHash;

    private void Reset() => this.image = this.GetComponentInChildren<Image>();

    // Start is called before the first frame update
    private void Start()
    {
        if (this.image == null)
        {
            this.Reset();
        }

        this.animatorKeyHash = Animator.StringToHash(this.animatorKey);
        this.image.fillAmount = 0;
    }

    private void FixedUpdate()
    {
        float multiplier = 100 / this.relativeMaxFillValue;
        float targetFillAmount = (this.value / multiplier / 100) + (this.offset / 100);
        this.image.fillAmount = this.fillMethod switch
        {
            FillMethod.Lerp => Mathf.Lerp(this.image.fillAmount, targetFillAmount, GMStageManager.Instance().ConvertToDeltaValue(this.fillSpeed / 100)),
            FillMethod.Instant => targetFillAmount,
            _ => targetFillAmount,
        };

        if (this.animator != null)
        {
            this.animator.SetBool(this.animatorKeyHash, this.value == this.relativeMaxFillValue);
        }
    }

    /// <summary>
    /// Sets the value for our fill
    /// <param name="value">The new value for the fill</param>
    /// </summary>
    public void SetValue(float value) => this.value = value;

    private void OnValidate()
    {
        float multiplier = 100 / this.relativeMaxFillValue;
        float targetFillAmount = (this.value / multiplier / 100) + (this.offset / 100);
        this.image.fillAmount = targetFillAmount;
    }
}
