using UnityEngine;
/// <summary>
/// Duplicate of <see cref="SolidBoxController"/> but for special stages
/// </summary>
public class SpecialStageSolidBoxController : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private SpecialStagePlayer player;
    [SerializeField, LastFoldoutItem()]
    private BoxCollider boxCollider;
    [Tooltip("The layers the soli box collider can interact with"), SerializeField]
    private LayerMask collisionMask;
    [Tooltip("The size of the solid box collider when regular"), SerializeField]
    private Rect normalBounds;
    [Tooltip("The debug color for the solid box"), SerializeField]
    private Color solidBoxDebugColor = General.RGBToColour(251, 242, 54, 170);
    [Tooltip("The debug colour of the collider bounds while attacking"), SerializeField]
    private Color attackingDebugColor = General.RGBToColour(255, 77, 84, 170);

    private void Reset()
    {
        this.player = this.GetComponentInParent<SpecialStagePlayer>();
        this.boxCollider = this.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
        //normalBounds.x = boxCollider.offset.x;
        // normalBounds.y = boxCollider.offset.y;
        this.normalBounds.width = this.boxCollider.size.x;
        this.normalBounds.height = this.boxCollider.size.y;
    }

    /// <summary>
    /// Checks for an object interaction and performs the specified action if the its true
    /// </summary>
    public void CheckCollisions()
    {
        if (GMSpecialStageManager.Instance().GetSpecialStageState() != SpecialStageState.Running)
        {
            return;
        }

        //AABB collisions
        Bounds bounds = this.boxCollider.bounds;
        Collider[] boxCollisions = Physics.OverlapBox(bounds.center, new Vector3(bounds.size.x, bounds.size.y, bounds.size.x), new Quaternion(), this.collisionMask);

        foreach (Collider boxCollision in boxCollisions)
        {
            SpecialStageContactEvent specialStageContactEvent = boxCollision.GetComponent<SpecialStageContactEvent>();
            if (specialStageContactEvent != null)
            {
                if (specialStageContactEvent.HedgeIsCollisionValid(this.player, bounds))
                {
                    specialStageContactEvent.HedgeOnCollisionEnter(this.player);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && GMSpecialStageManager.Instance().GetSpecialStageState() != SpecialStageState.Running)
        {
            return;
        }
        BoxCollider boxCollider = this.GetComponent<BoxCollider>();
        Gizmos.color = Application.isPlaying && this.player.GetAttackingState() ? this.attackingDebugColor : this.solidBoxDebugColor;
        Gizmos.DrawCube(boxCollider.bounds.center, boxCollider.bounds.size);

        if (Application.isPlaying == false)
        {
            this.normalBounds.width = boxCollider.size.x;
            this.normalBounds.height = boxCollider.size.y;
        }
    }
}
