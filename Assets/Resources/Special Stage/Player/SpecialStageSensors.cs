using UnityEngine;
/// <summary>
/// Duplicate of <see cref="Sensors"/> but for special stages
/// </summary>
public class SpecialStageSensors : MonoBehaviour
{
    [SerializeField]
    private SpecialStagePlayer player;
    [SerializeField]
    private SpecialStageSolidBoxController solidBoxController;

    private void Reset()
    {
        this.player = this.GetComponent<SpecialStagePlayer>();
        this.solidBoxController = this.GetComponent<SpecialStageSolidBoxController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Move the player gameobject and also perform collision detection while in the special stage
    /// <param name="velocity">The players current velocity</param>
    /// </summary>
    public void MoveAndCollide(Vector2 velocity)
    {
        float delta = Time.deltaTime;
        this.transform.localPosition += (Vector3)(velocity * GMStageManager.Instance().GetPhysicsMultiplier() * delta);//Move the player by the current velocity
        this.solidBoxController.CheckCollisions();
    }
}
