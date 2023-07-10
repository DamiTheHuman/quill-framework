using System.Collections;
using UnityEngine;
/// <summary>
/// A controller relating to the sparks that fly based on the electric shields direction
/// </summary
public class ElectricSparkController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [Tooltip("The direction the spark should move in")]
    public Vector2 cardinalDirection = new Vector2(1, 1);
    [Tooltip("The speed of the spark")]
    public float speed = 2;
    [Tooltip("The total lifetime of the spark")]
    public float sparkAge = 0.4833333f;
    [Tooltip("The current age of te spark")]
    public float currentSparkAge = 0;
    [Tooltip("The number of flashes performed during the sparks lifetime")]
    public int flashIterations = 3;

    private void Reset() => this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    private void Awake()
    {
        if (this.spriteRenderer == null)
        {
            this.Reset();
        }
    }

    private void OnEnable()
    {
        this.currentSparkAge = this.sparkAge;//Reset timer
        this.transform.localPosition = Vector2.zero;
        this.spriteRenderer.color = new Color(1, 1, 1, 1);
        this.StartCoroutine(this.LerpColorsOverTime(this.spriteRenderer.color, new Color(1, 1, 1, 0), this.sparkAge / this.flashIterations));
    }

    // Update is called once per frame
    private void Update() => this.UpdateSparkAge();

    private void FixedUpdate() => this.MoveSpark();
    /// <summary>
    /// Decrements the spark over time towards zero
    /// </summary>
    private void UpdateSparkAge()
    {
        this.currentSparkAge -= Time.deltaTime;

        if (this.currentSparkAge <= 0)
        {
            this.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Updates the position of the spark object based on the velocity
    /// </summary>
    private void MoveSpark()
    {
        Vector2 velocity = new Vector2(GMStageManager.Instance().ConvertToDeltaValue(this.cardinalDirection.x), GMStageManager.Instance().ConvertToDeltaValue(this.cardinalDirection.y));
        velocity *= this.speed;
        this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the player by the current velocity
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

        for (int x = 0; x < this.flashIterations; x++)
        {
            for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
            {
                this.spriteRenderer.color = Color.Lerp(startingColor, endingColor, step);
                yield return null;
            }
            yield return null;
        }
    }
}
