using System.Collections.Generic;
using UnityEngine;
///<summary>
/// Creates a set of items with set offsets and cusomzation within its  conveyor info <see cref="itemConveyorInfo"/>
/// This constructor creats the path and items when te create conveyor boolean <see cref="createConveyor"/> is ticked some exmaples can be found within the Item Conveyor folder
///</summary>
public class ItemConveyorConstructor : MonoBehaviour
{
    [SerializeField, Button("CreateConveyor")]
    private bool createConveyor;
    [Tooltip("The Item to be cloned and placed within the route")]
    public GameObject itemToConvey;
    [Tooltip("An optional early end point")]
    public int earlyEndPoint = 2;
    [Tooltip("Contains information on the conveyor type being created")]
    public ItemConveyorData itemConveyorInfo;

    /// <summary>
    /// Creates a new conveyor alongside the items that follow along its path
    /// </summary>
    private void InstantiateNewConveyor()
    {
        GameObject conveyorObject = new GameObject
        {
            name = this.itemToConvey.name + " Conveyor"
        };
        ItemConveyorController itemConveyController = conveyorObject.AddComponent<ItemConveyorController>().GetComponent<ItemConveyorController>();
        itemConveyController.itemConveyorInfo = this.itemConveyorInfo;

        if (this.itemConveyorInfo.GetPlatformItems().Count == 0)
        {
            GameObject itemParent = new GameObject
            {
                name = "Conveyor Item"
            };
            itemParent.transform.parent = itemConveyController.transform;
            itemParent.transform.localPosition = new Vector3(1, 1, 1);
            if (this.itemConveyorInfo.GetRouteMovementType() == RouteMovementType.Bezier)
            {
                this.CreateBezierPathItems(itemParent.transform, itemConveyController);
            }
            else
            {
                this.CreateCirclePathItems(itemParent.transform, itemConveyController);
            }
        }
    }

    /// <summary>
    /// Creates the set conveyor item with the appropriate parameters that follow the bezier curve
    /// <param name="itemParent">The parent that the item created will be childed to </param>
    /// <param name="itemConveyorController">A reference to the item conveyor controller to be shared amongst all child items </param>
    /// </summary>
    private void CreateBezierPathItems(Transform itemParent, ItemConveyorController itemConveyorController)
    {
        for (int x = 0; x < (this.itemConveyorInfo.GetPathPoints().Count / this.itemConveyorInfo.GetIncrement()) - this.earlyEndPoint; x++)
        {
            GameObject createdItem = Instantiate(this.itemToConvey);
            createdItem.transform.parent = itemParent;
            createdItem.name = this.itemToConvey.name + " " + x;
            createdItem.AddComponent<ConveyorItemController>();
            createdItem.GetComponent<ConveyorItemController>().SetBezierConveyor(itemConveyorController, x);
            this.itemConveyorInfo.GetPlatformItems().Add(createdItem);
        }
    }

    /// <summary>
    /// Creates the set conveyor item with the appropriate parameters that follow the circle curve
    /// <param name="itemParent">The parent that the item created will be childed to </param>
    /// <param name="itemConveyorController">A reference to the item conveyor controller to be shared amongst all child items </param>
    /// </summary>
    private void CreateCirclePathItems(Transform itemParent, ItemConveyorController itemConveyorController)
    {
        for (int x = 1; x < (360 / this.itemConveyorInfo.GetItemWidth()) - this.earlyEndPoint; x++)
        {
            GameObject createdItem = Instantiate(this.itemToConvey);
            createdItem.transform.parent = itemParent;
            createdItem.name = this.itemToConvey.name + " " + x;
            createdItem.transform.localPosition = new Vector3(1, 1, 1);
            createdItem.AddComponent<ConveyorItemController>();
            createdItem.GetComponent<ConveyorItemController>().SetCircularConveyor(itemConveyorController, x);
            this.itemConveyorInfo.GetPlatformItems().Add(createdItem);

        }
    }

    /// <summary>
    /// Updates the details of the path created by the bezier curve
    /// </summary>
    private void UpdateBezierCurve()
    {
        this.itemConveyorInfo.GetPlatformItems().Clear();
        List<Vector2> pathControlPoints = new List<Vector2>();
        this.itemConveyorInfo.GetPathPoints().Clear();

        foreach (Transform controlPoint in this.itemConveyorInfo.GetControlPoints())
        {
            pathControlPoints.Add(controlPoint.position);
        }

        this.UpdatePathPoints(pathControlPoints);
        //Mirrored
        Vector2 lastPointOffset = pathControlPoints[0] + pathControlPoints[3];

        for (int x = 0; x < pathControlPoints.Count; x++)
        {
            pathControlPoints[x] *= new Vector2(-1, -1);
            ;
            pathControlPoints[x] += lastPointOffset;
        }

        this.UpdatePathPoints(pathControlPoints);
    }

    /// <summary>
    ///  Calculates a bezier curve based on the control points given
    /// <param name="gizmoControlPoints">The positions of the control points </param>
    /// </summary>
    private void UpdatePathPoints(List<Vector2> gizmoControlPoints)
    {
        Vector2 gizmosPosition = new Vector2();

        for (float t = 0; t <= 1; t += 1 / this.itemConveyorInfo.GetCurveSmoothness())
        {
            gizmosPosition = (Mathf.Pow(1 - t, 3) * gizmoControlPoints[0]) +
            (3 * Mathf.Pow(1 - t, 2) * t * gizmoControlPoints[1]) +
            (3 * (1 - t) * Mathf.Pow(t, 2) * gizmoControlPoints[2]) +
            (Mathf.Pow(t, 3) * gizmoControlPoints[3]);
            if (this.itemConveyorInfo.GetPathPoints().Contains(gizmosPosition) == false)
            {
                this.itemConveyorInfo.GetPathPoints().Add(gizmosPosition);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (this.itemConveyorInfo.GetRouteMovementType() == RouteMovementType.Bezier)
            {
                this.UpdateBezierCurve();
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(this.itemConveyorInfo.GetControlPoints()[0].transform.position.x, this.itemConveyorInfo.GetControlPoints()[0].transform.position.y),
                    new Vector2(this.itemConveyorInfo.GetControlPoints()[1].transform.position.x, this.itemConveyorInfo.GetControlPoints()[1].transform.position.y));

                Gizmos.DrawLine(new Vector3(this.itemConveyorInfo.GetControlPoints()[2].transform.position.x, this.itemConveyorInfo.GetControlPoints()[2].transform.position.y),
                   new Vector2(this.itemConveyorInfo.GetControlPoints()[3].transform.position.x, this.itemConveyorInfo.GetControlPoints()[3].transform.position.y));

                Gizmos.color = Color.white;

                if (this.itemConveyorInfo.GetRouteMovementType() == RouteMovementType.Bezier)
                {
                    for (int x = 0; x < this.itemConveyorInfo.GetPathPoints().Count; x++)
                    {
                        if (x > 0)
                        {
                            Gizmos.DrawLine(this.itemConveyorInfo.GetPathPoints()[x - 1], this.itemConveyorInfo.GetPathPoints()[x]);
                        }
                        if (this.itemConveyorInfo.GetIncrement() > 0 && x % this.itemConveyorInfo.GetIncrement() == 0 && x < this.itemConveyorInfo.GetPathPoints().Count - this.earlyEndPoint)
                        {
                            GizmosExtra.Draw2DCircle(this.itemConveyorInfo.GetPathPoints()[x], this.itemConveyorInfo.GetItemWidth() / 2, Color.green);
                        }
                    }
                    Gizmos.DrawLine(this.itemConveyorInfo.GetPathPoints()[this.itemConveyorInfo.GetPathPoints().Count - 1], this.itemConveyorInfo.GetPathPoints()[0]);
                }

            }
            else
            {
                GizmosExtra.Draw2DCircle(this.transform.position, this.itemConveyorInfo.GetRadius(), Color.white);
                for (int x = 1; x < (360 / this.itemConveyorInfo.GetItemWidth()) - this.earlyEndPoint; x++)
                {
                    Vector2 position;
                    float delta = x * this.itemConveyorInfo.GetItemWidth();
                    Vector2 startPosition = this.transform.position;
                    position.x = startPosition.x + (Mathf.Cos(delta * Mathf.Deg2Rad) * this.itemConveyorInfo.GetRadius());
                    position.y = startPosition.y + (Mathf.Sin(delta * Mathf.Deg2Rad) * this.itemConveyorInfo.GetRadius());

                    GizmosExtra.Draw2DCircle(position, this.itemConveyorInfo.GetItemWidth() / 2, Color.green);

                }
            }
        }
    }
}
