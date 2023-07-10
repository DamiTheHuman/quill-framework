using UnityEngine;
/// <summary>
/// Movement for broken shards spawned after a monitor has been destroyed
///  Original Author - Nihil [Core Framework]
///  Notice - This is a more pseudo version and thus incomplete
/// </summary>
public class BrokenMonitorShardController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private SpriteRenderer spriteRenderer;
    [SerializeField, LastFoldoutItem()]
    private Animator animator;
    [Tooltip("The current position offset of the shard when spawend"), SerializeField]
    private Vector2 positionOffset = new Vector2(16, -16);
    [Tooltip("The velocity of the shard"), SerializeField]
    private Vector2 velocity;
    [Tooltip("The force applied to move the shard downwards every step"), SerializeField]
    private float gravity = 0.4f;

    private void Reset()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        if (this.animator == null)
        {
            this.Reset();
        }
    }

    private void OnEnable()
    {
        this.animator.Play("Idle", 0, Random.Range(0.0f, 1.0f));//start the animation at a random index
        this.transform.localPosition = this.positionOffset;
        this.velocity.x = Random.Range(-30f, 30f) / 10.0f;
        this.velocity.y = Random.Range(80f, -20f) / 10.0f;
    }

    private void FixedUpdate()
    {
        this.MoveShard();
        this.ApplyGravity();
        this.CheckToDeactivate();
    }

    /// <summary>
    /// Move the shard pased on its velocity
    /// </summary>
    private void MoveShard() => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(this.velocity.x, this.velocity.y, 0f);
    /// <summary>
    /// Apply gravity to the shards y velocity
    /// </summary>
    private void ApplyGravity() => this.velocity.y -= GMStageManager.Instance().ConvertToDeltaValue(this.gravity);
    /// <summary>
    /// Simply deactivates the shard when it is below the camera view port
    /// </summary>
    private void CheckToDeactivate()
    {
        if (HedgehogCamera.Instance().PositionIsBelowCameraView(this.spriteRenderer.bounds.max))
        {
            this.gameObject.SetActive(false);
        }
    }
}
