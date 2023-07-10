using UnityEngine;

public class SmartRotatingRaycasts : MonoBehaviour
{
    [SerializeField]
    private float rayDistance = 45;
    [SerializeField]
    private float rayAngle = 35;
    [SerializeField]
    private LayerMask collisionMask;
    [SerializeField]
    private Vector2 leftRaycastOffset = new Vector2(-8, 0);
    [SerializeField]
    private Vector2 rightRaycastOffset = new Vector2(8, 0);

    /// <summary>
    /// Casts a raycast from the specified origin at the set angle
    /// <param name="castPosition">The origin point of the raycast </param>
    /// <param name="offsetFromOrigin">The offset of the object from the origin point</param>
    /// <param name="distance">How far the raycast should travel</param>
    /// <param name="angleInDegrees">The angle to cast towards in degrees </param>
    /// <param name="debugColor">The color of the ray</param>
    /// <param name="collisionMask">The things to collide with </param>
    /// </summary>
    private RaycastHit2D CastRaycastAtAngle(Vector2 castPosition, Vector2 offsetFromOrigin, float distance, float angleInDegrees, Color debugColor, LayerMask collisionMask = new LayerMask())
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float castAngleInDegrees = angleInDegrees - 90;// -90 so that it casts downwards when set to 0
        float castAngleInRadians = castAngleInDegrees * Mathf.Deg2Rad;

        float racyastPositionX = castPosition.x + (Mathf.Cos(angleInRadians) * offsetFromOrigin.x) + (Mathf.Sin(angleInRadians) * -offsetFromOrigin.y);
        float raycastPositionY = castPosition.y + (Mathf.Cos(angleInRadians) * offsetFromOrigin.y) + (Mathf.Sin(angleInRadians) * offsetFromOrigin.x);
        Vector2 raycastPosition = new Vector2(racyastPositionX, raycastPositionY);

        Vector2 castDirection = new Vector2(Mathf.Cos(castAngleInRadians), Mathf.Sin(castAngleInRadians));

        RaycastHit2D raycast2D = Physics2D.Raycast(raycastPosition, castDirection, distance, collisionMask);
        Debug.DrawLine(raycastPosition, raycastPosition + (castDirection * distance), debugColor);

        return raycast2D;
    }

    private void FixedUpdate()
    {
        RaycastHit2D leftRay = this.CastRaycastAtAngle(this.transform.position, this.leftRaycastOffset, this.rayDistance, this.rayAngle, Color.magenta, this.collisionMask);
        RaycastHit2D rightRay = this.CastRaycastAtAngle(this.transform.position, this.rightRaycastOffset, this.rayDistance, this.rayAngle, Color.yellow, this.collisionMask);

        if (leftRay || rightRay)
        {
            Debug.Log("Do Stuff");
        }
        else
        {
            Debug.Log("Do not Stuff");
        }
    }
}
