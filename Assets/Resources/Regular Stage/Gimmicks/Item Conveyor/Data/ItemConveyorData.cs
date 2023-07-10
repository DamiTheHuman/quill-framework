using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This class contains information on the conveyor path to be followed at runtime
///</summary>
[System.Serializable]
public class ItemConveyorData
{
    [SerializeField, Tooltip("The type of route the conveyor type can use"),]
    private RouteMovementType routeMovementType = RouteMovementType.Circular;
    [SerializeField, Tooltip("The speed of each item on the route")]
    private float speed = 1;
    [Header("Bezier Route Settings"), SerializeField, Tooltip("The Transforms within the beziers whichs position effects its shape"),]
    private List<Transform> controlPoints = new List<Transform>();
    [SerializeField, Tooltip("The path points that each child item will follow")]
    private List<Vector2> pathPoints;
    [SerializeField, Tooltip("Each X individual point that an item will be placed on the bezier")]
    private int increment = 2;
    [SerializeField, Tooltip("The smoothness of the path generated which also effects the amount of points generated"), LastFoldoutItem()]
    [Range(2, 60)]
    private float curveSmoothness = 20;
    [Header("Circluar Route Settings"), SerializeField, Tooltip("The size of each individual item placed on the circle")]
    private float itemWidth = 32;
    [SerializeField, Tooltip("The radius of the path that each circle follows")]
    private float radius = 60;
    [SerializeField, Tooltip("A list referencing the items created by the conveyor"),]
    private List<GameObject> platformItems;

    /// <summary>
    /// Returns all the path points created by the bezier curve
    /// </summary>
    public List<Vector2> GetBezierPathPoints() => this.pathPoints;

    /// <summary>
    /// Get the route movement type
    /// </summary>
    public RouteMovementType GetRouteMovementType() => this.routeMovementType;

    /// <summary>
    /// Get the speed of the item
    /// </summary>
    public float GetSpeed() => this.speed;

    /// <summary>
    /// Get the control points
    /// </summary>
    public List<Transform> GetControlPoints() => this.controlPoints;

    /// <summary>
    /// Get the path points
    /// </summary>
    public List<Vector2> GetPathPoints() => this.pathPoints;

    /// <summary>
    /// Get the increment set for the conveyor
    /// </summary>
    public int GetIncrement() => this.increment;

    /// <summary>
    /// Get the smoothness for the conveyor
    /// </summary>
    public float GetCurveSmoothness() => this.curveSmoothness;

    /// <summary>
    /// Get the item width
    /// </summary>
    public float GetItemWidth() => this.itemWidth;

    /// <summary>
    /// Get the radius
    /// </summary>
    public float GetRadius() => this.radius;

    /// <summary>
    /// Get the platform items
    /// </summary>
    public List<GameObject> GetPlatformItems() => this.platformItems;
}

