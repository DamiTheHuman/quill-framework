using System;
using UnityEngine;
/// <summary>
/// Contains information on the type of spark being spawned
/// This class also is the main object moved to follow the target rather than the sparks themselves
///  Original Author - Nihil  [Core Framework] 
///  Added functionality - Made it as close to mania's from what can be seen visualy 
///  - also allowed the trail to based on a targets position rather than a fixed object
/// </summary>
public class InvincibilitySparkType : MonoBehaviour
{
    [SerializeField]
    private InvincibilityTrailRenderer invincibilityTrailRenderer;
    [Tooltip("The category of sparkle to be spawned")]
    public SparkleType sparkleType = SparkleType.Type1;
    [Tooltip("The increment of delta which also affects the speed"), SerializeField]
    private float deltaIncrement = -12;
    [Tooltip("The x offset of each sparkle regarding the distance to the target"), SerializeField]
    private float offsetWidth = 2;
    [Tooltip("The current timer"), SerializeField]
    private float personalTimer = 0;
    [Tooltip("The global timer"), SerializeField]
    private float globalTimer = 0;
    [Tooltip("The recent positions of the target"), SerializeField]
    private RecentObjectPositions[] recentObjectPositions = new RecentObjectPositions[60];

    private void Reset() => this.invincibilityTrailRenderer = this.GetComponentInParent<InvincibilityTrailRenderer>();

    private void Awake()
    {
        if (this.invincibilityTrailRenderer == null)
        {
            this.Reset();
        }
        InvincibilitySparkleController[] invincibilitySparkleControllers = this.GetComponentsInChildren<InvincibilitySparkleController>();

        for (int x = 0; x < invincibilitySparkleControllers.Length; x++)
        {
            invincibilitySparkleControllers[x].SetSparkleId(x);
        }
    }

    private void OnEnable()
    {
        for (int x = 0; x < this.recentObjectPositions.Length; x++)
        {
            this.recentObjectPositions[x].position = this.invincibilityTrailRenderer.GetTarget().position;
        }
    }

    private void Update() => this.UpdateSparkTrailPosition(this.transform.position);

    private void FixedUpdate()
    {
        this.UpdateSparkleTimer();

        if (this.invincibilityTrailRenderer.player != null)
        {
            this.recentObjectPositions = this.invincibilityTrailRenderer.player.GetAfterImageController().GetRecentPositions();
        }
        else
        {
            this.UpdateRecentObjectPositions();
        }
    }

    /// <summary>
    /// Updates the personal timer of the sparkle type
    /// </summary>
    private void UpdateSparkleTimer()
    {
        this.personalTimer += Time.deltaTime;
        this.globalTimer = this.personalTimer * 60;
    }

    /// <summary>
    /// Update the recent positions of the timer
    /// </summary>
    private void UpdateRecentObjectPositions() => this.recentObjectPositions[(int)this.globalTimer % 60].position = this.invincibilityTrailRenderer.GetTarget().position;

    /// <summary>
    /// Update the position of the main trail renderer position
    /// </summary>
    private void UpdateSparkTrailPosition(Vector2 position)
    {
        float globalTimer = this.invincibilityTrailRenderer.player != null ? this.invincibilityTrailRenderer.player.GetAfterImageController().GetGlobalTimer() : this.globalTimer;

        switch (this.sparkleType)
        {
            case SparkleType.Type1:
                position = this.invincibilityTrailRenderer.player.transform.position;

                break;
            case SparkleType.Type2:
            case SparkleType.Type3:
            case SparkleType.Type4:
                if ((int)((globalTimer - (2 * (int)this.sparkleType)) % 60) >= 0)
                {
                    Vector2 targetPosition = this.recentObjectPositions[(int)(globalTimer - (this.offsetWidth * (int)this.sparkleType)) % 60].position;
                    position = targetPosition;
                }

                break;
            default:
                position = this.invincibilityTrailRenderer.GetTarget().position;

                break;
        }

        this.transform.position = position;
    }

    /// <summary>
    /// Get the <see cref="deltaIncrement"/>
    /// </summary>
    /// <returns></returns>
    public float GetDeltaIncrement() => this.deltaIncrement;

    private void OnDrawGizmos() => GizmosExtra.Draw2DCircle(this.transform.position, 16, Color.red);
}
