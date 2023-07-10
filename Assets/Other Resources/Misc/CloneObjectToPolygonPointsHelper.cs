#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Clones a game object to the points of a polygon
/// </summary>
public class CloneObjectToPolygonPointsHelper : MonoBehaviour
{
    [SerializeField, Tooltip("The game object to clone")]
    private GameObject objectToClone;
    [SerializeField, Tooltip("The polygon collider attached")]
    private PolygonCollider2D polygonCollider2D;
    [Button(nameof(PlaceChildrenAtPolygonPoints))]
    public bool placeChildrenAtPolygonPoints = false;

    private void Reset() => this.polygonCollider2D = this.GetComponent<PolygonCollider2D>();
    private void OnValidate()
    {
        if (this.objectToClone == null)
        {
            return;
        }

        bool isPrefab = PrefabUtility.GetCorrespondingObjectFromSource(this.objectToClone) == null && PrefabUtility.GetPrefabInstanceHandle(this.objectToClone) != null;

        if (isPrefab == false)
        {
            Debug.LogError("Please use a prefab!");
            this.objectToClone = null;
        }

    }
    /// <summary>
    /// Clones gameobjects as children at the points of the polygon
    /// </summary>
    public void PlaceChildrenAtPolygonPoints()
    {
        if (this.polygonCollider2D == null)
        {
            this.Reset();
        }

        int count = 0;

        foreach (Vector2 point in this.polygonCollider2D.points)
        {
            GameObject childObject = PrefabUtility.InstantiatePrefab(this.objectToClone) as GameObject;

            childObject.transform.name = this.objectToClone.name.Split(" (")[0].Trim() + "(" + count + ")";
            childObject.transform.parent = this.transform;
            childObject.transform.localPosition = point;
            count++;
        }
    }
}
#endif

