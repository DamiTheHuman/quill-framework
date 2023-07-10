using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonCollisionHelper : MonoBehaviour
{
    [SerializeField]
    private PolygonCollider2D polygonCollider2D;
    [SerializeField, Button(nameof(RoundCollisionPoints))]
    private bool roundCollisionPoints;
    [SerializeField, Tooltip("The digit to round the values to"), Min(1)]
    private int roundToValue = 4;

    [SerializeField, Button(nameof(AddOffsetToCollisionPoints))]
    private bool addOffsetToCollisionPoints;
    [SerializeField, Tooltip("The offset to add to the polygon points")]
    private Vector2 offsetToAddToCollisionPoints;

    [SerializeField, Button(nameof(MultiplyCollisionPoints))]
    private bool multiplyCollisionPoints;
    [SerializeField, Tooltip("The values to multiply polygon points")]
    private Vector2 vectorToMultiplyCollisionPoints;

    [SerializeField, Button(nameof(RemoveDuplicateCollisionPoints))]
    private bool removeDuplicateCollisionPoints;
    private void GetPolygonCollider()
    {
        if (this.polygonCollider2D == null)
        {
            this.polygonCollider2D = this.GetComponent<PolygonCollider2D>();
        }
    }

    public void RoundCollisionPoints()
    {

        this.GetPolygonCollider();

        List<Vector2> roundedPoints = new List<Vector2>();
        roundedPoints.AddRange(this.polygonCollider2D.points);

        for (int x = 0; x < roundedPoints.Count; x++)
        {
            roundedPoints[x] = new Vector2(General.RoundToNearestDigit(roundedPoints[x].x, this.roundToValue), General.RoundToNearestDigit(roundedPoints[x].y, this.roundToValue));
        }

        this.polygonCollider2D.points = roundedPoints.ToArray();
    }

    public void AddOffsetToCollisionPoints()
    {
        this.GetPolygonCollider();

        List<Vector2> roundedPoints = new List<Vector2>();
        roundedPoints.AddRange(this.polygonCollider2D.points);

        for (int x = 0; x < roundedPoints.Count; x++)
        {
            roundedPoints[x] += this.offsetToAddToCollisionPoints;
        }

        this.polygonCollider2D.points = roundedPoints.ToArray();
    }

    public void MultiplyCollisionPoints()
    {
        this.GetPolygonCollider();

        List<Vector2> roundedPoints = new List<Vector2>();
        roundedPoints.AddRange(this.polygonCollider2D.points);

        for (int x = 0; x < roundedPoints.Count; x++)
        {
            roundedPoints[x] *= this.vectorToMultiplyCollisionPoints;
        }

        this.polygonCollider2D.points = roundedPoints.ToArray();
    }


    public void RemoveDuplicateCollisionPoints()
    {
        this.GetPolygonCollider();
        List<Vector2> roundedPoints = new List<Vector2>();
        roundedPoints.AddRange(this.polygonCollider2D.points);

        this.polygonCollider2D.points = roundedPoints.Distinct().ToArray();
    }
}
