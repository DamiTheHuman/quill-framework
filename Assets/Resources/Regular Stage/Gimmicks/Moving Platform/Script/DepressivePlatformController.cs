using UnityEngine;
[DefaultExecutionOrder(1001)]
/// <summary>
/// Attach this script to a platform that depresses when the player lands on it
/// </summary>
public class DepressivePlatformController : PlatformController
{
    [SerializeField, Tooltip("How deep the platform can be depressed when the player is standing on it"), FirstFoldOutItem("Depression Settings")]
    private float currentDepressionAmount = 0;
    [SerializeField, Tooltip("The speed in which the platform depresses")]
    private float depressionSpeed = 60;
    [SerializeField, Tooltip("The limit in which the platform can be depressed"), LastFoldoutItem()]
    private float depressionRange = 4;

    public override void Reset()
    {
        base.Reset();
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        this.currentDepressionAmount = 0;
    }

    private void FixedUpdate()
    {
        if (this.GetCollisionState() is CollisionState.OnCollisionEnter or CollisionState.OnCollisionStay)
        {
            if (this.currentDepressionAmount != this.depressionRange)
            {
                this.DepressPlatform();
            }
        }
        else
        {
            if (this.currentDepressionAmount != 0)
            {
                this.RepressPlatform();
            }
        }
    }

    /// <summary>
    /// Depress platform when the player steps on
    /// </summary/>
    private void DepressPlatform()
    {
        Vector2 position = this.startPosition;
        this.currentDepressionAmount = Mathf.MoveTowards(this.currentDepressionAmount, this.depressionRange, this.depressionSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
        position.y -= this.currentDepressionAmount;
        this.transform.position = new Vector3(this.transform.position.x, position.y);

        this.SyncChildPositions();
    }
    /// <summary>
    /// Repress the platform when the player steps off
    /// </summary/>
    private void RepressPlatform()
    {
        Vector2 position = this.startPosition;
        this.currentDepressionAmount = Mathf.MoveTowards(this.currentDepressionAmount, 0, this.depressionSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
        position.y -= this.currentDepressionAmount;
        this.transform.position = new Vector3(this.transform.position.x, position.y);

        this.SyncChildPositions();
    }
}
