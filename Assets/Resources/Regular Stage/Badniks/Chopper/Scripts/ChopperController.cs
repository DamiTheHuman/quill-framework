using UnityEngine;
/// <summary>
/// The chopper aenemy which moves upwards and downards
/// Author - Damizean ported from Sonic Worlds
/// <summary>
public class ChopperController : BadnikController
{
    [Tooltip("The range of the chopper"), FirstFoldOutItem("Chopper Info")]
    public float range = 120;
    [Tooltip("How fast the chopper moves vertically")]
    public float speed = 1f;
    public Color debugColor = Color.red;
    protected override void Start()
    {
        base.Start();
        this.RandomizeStartPosition();
    }

    private void FixedUpdate() => this.MoveChopper();

    /// <summary>
    /// Randomizes the choppers start position on start
    /// </summary
    private void RandomizeStartPosition()
    {
        this.velocity.y = (1 + Random.Range(0, 2)) * 180;
        Vector2 position = this.transform.position;
        position.y = (this.range * Mathf.Sin(this.velocity.y * Mathf.Deg2Rad)) + this.startPosition.y;
        this.transform.position = position;
    }

    /// <summary>
    /// Moves the Chopper Enemy
    /// </summary>
    private void MoveChopper()
    {
        this.velocity.y += this.speed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime; //Increment the movement

        //If at the peak point restart back to 0
        if (this.velocity.y >= 360)
        {
            this.velocity.y = 0;
        }

        Vector2 position = this.transform.position;
        position.y = (this.range * Mathf.Sin(this.velocity.y * Mathf.Deg2Rad)) + this.startPosition.y;
        this.transform.position = position; //Move the object
    }

    private void OnDrawGizmos()
    {
        Vector2 debugPosition = Application.isPlaying ? (Vector3)this.startPosition : this.transform.position;
        debugPosition.x = this.transform.position.x;
        Vector2 pos1 = debugPosition;
        Vector2 pos2 = debugPosition;
        Gizmos.color = this.debugColor;

        pos1.y = (this.range * Mathf.Sin(90 * Mathf.Deg2Rad)) + debugPosition.y;
        pos2.y = (this.range * Mathf.Sin(270 * Mathf.Deg2Rad)) + debugPosition.y;
        Gizmos.DrawLine(pos1, pos2);
        GizmosExtra.Draw2DArrow(pos1, 0);
        GizmosExtra.Draw2DArrow(pos2, 180);
    }

}
