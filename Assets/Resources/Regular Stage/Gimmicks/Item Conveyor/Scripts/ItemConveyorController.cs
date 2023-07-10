using UnityEngine;

///<summary>
/// Holds the informations of te conveyor info
///</summary>
public class ItemConveyorController : MonoBehaviour
{
    public ItemConveyorData itemConveyorInfo;
    private void OnDrawGizmos()
    {
        if (this.itemConveyorInfo.GetRouteMovementType() == RouteMovementType.Bezier)
        {
            for (int x = 0; x < this.itemConveyorInfo.GetPathPoints().Count; x++)
            {
                if (x > 0)
                {
                    Gizmos.DrawLine(this.itemConveyorInfo.GetPathPoints()[x - 1] + (Vector2)this.transform.position, this.itemConveyorInfo.GetPathPoints()[x] + (Vector2)this.transform.position);
                }
            }
            Gizmos.DrawLine(this.itemConveyorInfo.GetPathPoints()[this.itemConveyorInfo.GetPathPoints().Count - 1] + (Vector2)this.transform.position, this.itemConveyorInfo.GetPathPoints()[0] + (Vector2)this.transform.position);
        }
        else
        {
            GizmosExtra.Draw2DCircle(this.transform.position, this.itemConveyorInfo.GetRadius(), Color.white);
        }
    }
}
