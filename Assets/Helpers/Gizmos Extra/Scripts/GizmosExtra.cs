using UnityEditor;
using UnityEngine;
/// <summary>
/// An extra class for drawing custom gizmo objects
/// </summary>
public class GizmosExtra : MonoBehaviour
{
    /// <summary>
    /// Draws a line which rotates based on the angle while simultaneously maintiaing its integrity
    /// <param name="position">The position to begin the line  </param>
    /// <param name="angleInDegrees">The angle of the line in degrees </param>
    /// <param name="length">The length of the line </param>
    /// </summary>
    public static void DrawResponsiveLine(Vector2 position, float angleInDegrees, float offset, float length)
    {
        float angleInRadians = (angleInDegrees + 90) * Mathf.Deg2Rad;
        float linePositionX = position.x + (Mathf.Cos(angleInRadians) * 0) + (Mathf.Sin(angleInRadians) * -offset);
        float linePositionY = position.y + (Mathf.Cos(angleInRadians) * offset) + (Mathf.Sin(angleInRadians) * (-0));
        Vector2 linePosition = new Vector2(linePositionX, linePositionY); //Setup sensor position
        Vector2 leftArrowDirection = General.AngleToVector(angleInDegrees * Mathf.Deg2Rad);
        Gizmos.DrawRay(linePosition, leftArrowDirection * length);
    }

    /// <summary>
    /// Draws a two dimensional arrow headarrow head at the specified
    /// <param name="position">The position to begin the line  </param>
    /// <param name="angleInDegrees">The angle of the line in degrees </param>
    /// <param name="offset">The offset of the arrow if applicable </param>
    /// <param name="length">The length of the line </param>
    /// </summary>
    public static void Draw2DArrow(Vector2 position, float angleInDegrees, float offset = 0, float length = 16f)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float arrowPositionX = position.x + (Mathf.Cos(angleInRadians) * 0) + (Mathf.Sin(angleInRadians) * -offset);
        float arrowPositionY = position.y + (Mathf.Cos(angleInRadians) * offset) + (Mathf.Sin(angleInRadians) * (-0));
        Vector2 arrowPosition = new Vector2(arrowPositionX, arrowPositionY); //Setup sensor position
        //Left side of Arrow
        Vector2 leftArrowDirection = General.AngleToVector((angleInDegrees - 115) * Mathf.Deg2Rad);
        Gizmos.DrawRay(arrowPosition, leftArrowDirection * length);
        //Right side of Arrow
        Vector2 rightArrowDirection = General.AngleToVector((angleInDegrees + 295) * Mathf.Deg2Rad);
        Gizmos.DrawRay(arrowPosition, rightArrowDirection * length);
    }

    /// <summary>
    /// Draw a rectangle based on the bounds providing filled in with an optional grey outline
    /// <param name="bounds">The bounds of the object </param>
    /// <param name="color">The color of the gizmo </param>
    /// <param name="drawOutline">Whether to draw an outline on the box </param>
    /// </summary>
    public static void DrawBounds(Bounds bounds, Color color, bool drawOutline = true)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(new Vector3(bounds.center.x, bounds.center.y, 0.01f), new Vector3(bounds.size.x, bounds.size.y, 0.01f));

        if (drawOutline)
        {; DrawWireRect(bounds, new Color(color.r, color.g, color.b, 1)); }
    }

    /// <summary>
    /// Draws a rectangble based on the box collider
    /// <param name="transform">The transform of the object being referenced </param>
    /// <param name="bounds">The box collider data to be drawn </param>
    /// <param name="color">The color of the gizmo </param>
    /// </summary>
    public static void DrawRect(Transform transform, BoxCollider2D boxCollider2D, Color color, bool drawOutline = false)
    {
        Matrix4x4 gizmosMatrix = Gizmos.matrix;
        Rect bounds = new Rect
        {
            x = boxCollider2D.offset.x,
            y = boxCollider2D.offset.y,
            width = boxCollider2D.size.x,
            height = boxCollider2D.size.y
        };
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = color;
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(new Vector2(bounds.x, bounds.y), new Vector2(bounds.width, bounds.height));
        Gizmos.matrix = gizmosMatrix;

        if (drawOutline)
        {
            DrawBoxCollider2D(transform, boxCollider2D, new Color(color.r, color.g, color.b, 1));
        }
    }

    /// <summary>
    /// Draws a simple 2D circle
    /// <param name="position">The position to begin the line  </param>
    /// <param name="radius">The radius of the object </param>
    /// <param name="color">The color of the gizmo  </param>
    /// </summary>
    public static void Draw2DCircle(Vector3 position, float radius, Color color)
    {
#if UNITY_EDITOR
        Handles.color = color;
        Handles.DrawWireDisc(position, Vector3.forward, radius);
#endif
    }

    /// <summary>
    /// Draws a simple 3D circle
    /// <param name="position">The position to begin the line  </param>
    /// <param name="radius">The radius of the object </param>
    /// <param name="color">The color of the gizmo  </param>
    /// </summary>
    public static void Draw3DCircle(Vector3 position, Vector3 rotation, float radius, Color color)
    {
#if UNITY_EDITOR
        Handles.color = color;
        Handles.DrawWireDisc(position, rotation, radius);
#endif
    }

    /// <summary>
    /// Draws a wired rectangle based on the boundaries passed
    /// <param name="bounds">The bounds of the object </param>
    /// <param name="color">The color of the gizmo </param>
    /// </summary>
    public static void DrawWireRect(Bounds bounds, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(new Vector3(bounds.center.x, bounds.center.y, 0.01f), new Vector3(bounds.size.x, bounds.size.y, 0.01f));
    }

    /// <summary>
    /// Draws data of the box collider 2d allowing a custom colour
    /// <param name="transform">The transform of the object being referenced </param>
    /// <param name="box">The box collider data to be drawn </param>
    /// <param name="color">The color of the gizmo </param>
    /// </summary>
    public static void DrawBoxCollider2D(Transform transform, BoxCollider2D box, Color color)
    {
        Vector2 offset = box.offset;
        Vector2 extents = box.size * 0.5f;
        Vector2[] verts = new Vector2[] {
            transform.TransformPoint (new Vector2 (-extents.x, -extents.y) + offset),
            transform.TransformPoint (new Vector2 (extents.x, -extents.y) + offset),
            transform.TransformPoint (new Vector2 (extents.x, extents.y) + offset),
            transform.TransformPoint (new Vector2 (-extents.x, extents.y) + offset) };

        Gizmos.color = color;
        Gizmos.DrawLine(verts[0], verts[1]);
        Gizmos.DrawLine(verts[1], verts[2]);
        Gizmos.DrawLine(verts[2], verts[3]);
        Gizmos.DrawLine(verts[3], verts[0]);
    }

    /// <summary>
    /// Draws a specific side of the box collider
    /// <param name="transform">The transform of the object being referenced </param>
    /// <param name="box">The box collider data to be drawn </param>
    /// <param name="color">The color of the gizmo </param>
    /// <param name="boxColliderDrawType">The collider draw type </param>
    /// </summary>
    public static void DrawBoxTopBounds(Transform transform, BoxCollider2D box, Color color, BoxColliderDrawType boxColliderDrawType = BoxColliderDrawType.Top)
    {
        Vector2 offset = box.offset;
        Vector2 extents = box.size * 0.5f;
        Vector2[] verts = new Vector2[] {
            transform.TransformPoint (new Vector2 (-extents.x, -extents.y) + offset),
            transform.TransformPoint (new Vector2 (extents.x, -extents.y) + offset),
            transform.TransformPoint (new Vector2 (extents.x, extents.y) + offset),
            transform.TransformPoint (new Vector2 (-extents.x, extents.y) + offset) };

        Gizmos.color = color;
        switch (boxColliderDrawType)
        {
            case BoxColliderDrawType.Top:
                Gizmos.DrawLine(verts[2], verts[3]);

                break;
            case BoxColliderDrawType.Right:
                Gizmos.DrawLine(verts[1], verts[2]);

                break;
            case BoxColliderDrawType.Left:
                Gizmos.DrawLine(verts[3], verts[0]);

                break;
            case BoxColliderDrawType.Bottom:
                Gizmos.DrawLine(verts[0], verts[1]);

                break;
            default:
                break;

        }
    }

    /// <summary>
    /// Draws a AA poly line with the set thickness
    /// <param name="width">The width of the line drawn </param>
    /// <param name="pointA">The start draw Point </param>
    /// <param name="pointA">The end draw Point </param>
    /// <param name="color">The color of the gizmo </param>
    /// </summary>
    public static void DrawAAPolyLine(float width, Vector2 pointA, Vector2 pointB, Color color)
    {
#if UNITY_EDITOR
        Handles.color = color;
        Vector3[] lines = new Vector3[2];
        lines[0] = pointA;
        lines[1] = pointB;
        Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, width, lines);
#endif
    }

    /// <summary>
    /// Draws a simple line
    /// <param name="width">The width of the line drawn </param>
    /// <param name="pointA">The start draw Point </param>
    /// <param name="pointA">The end draw Point </param>
    /// <param name="color">The color of the gizmo </param>
    /// </summary>
    public static void DrawPolyLine(float width, Vector2 pointA, Vector2 pointB, Color color)
    {
#if UNITY_EDITOR
        Handles.color = color;
        Vector3[] lines = new Vector3[2];
        lines[0] = pointA;
        lines[1] = pointB;
        Handles.DrawPolyLine(lines);
#endif
    }
}
