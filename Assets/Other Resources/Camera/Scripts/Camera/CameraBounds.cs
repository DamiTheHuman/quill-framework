using UnityEngine;

[System.Serializable]
public class CameraBounds
{
    [SerializeField]
    private BoundsType boundsType = BoundsType.AbsoluteBounds;
    [Tooltip("The object the transform is attached to")]
    public Transform transform;

    [Tooltip("The left bounds of the camera")]
    public float leftBounds = 16;
    [Tooltip("The right bounds of the camera")]
    public float rightBounds = 0;
    [Tooltip("The top bounds of the camera")]
    public float topBounds = 32;
    [Tooltip("The bottom bounds of the camera")]
    public float bottomBounds = 32;
    /// <summary>
    /// Clones the details of another camerbounds
    /// </summary>
    public void CopyCameraBoundData(CameraBounds cameraBounds)
    {
        this.boundsType = cameraBounds.boundsType;
        this.transform = cameraBounds.transform;
        this.leftBounds = cameraBounds.leftBounds;
        this.rightBounds = cameraBounds.rightBounds;
        this.topBounds = cameraBounds.topBounds;
        this.bottomBounds = cameraBounds.bottomBounds;
    }

    /// <summary>
    /// Get the left horizontal border position
    /// </summary>
    public float GetLeftBorderPosition()
    {
        Vector2 setPosition = this.boundsType == BoundsType.RelativeBounds ? this.transform.position : new Vector2(0, 0);

        return setPosition.x - this.leftBounds;
    }

    /// <summary>
    /// Get the Right horizontal Border Position
    /// </summary>
    public float GetRightBorderPosition()
    {
        Vector2 setPosition = this.boundsType == BoundsType.RelativeBounds ? this.transform.position : new Vector2(0, 0);

        return setPosition.x + this.rightBounds;
    }

    /// <summary>
    /// Get the Top vertical Border Position
    /// </summary>
    public float GetTopBorderPosition()
    {
        Vector2 setPosition = this.boundsType == BoundsType.RelativeBounds ? this.transform.position : new Vector2(0, 0);

        return setPosition.y + this.topBounds;
    }

    /// <summary>
    /// Get the bottom vertical borders position
    /// </summary>
    public float GetBottomBorderPosition()
    {
        Vector2 setPosition = this.boundsType == BoundsType.RelativeBounds ? this.transform.position : new Vector2(0, 0);

        return setPosition.y - this.bottomBounds;
    }

    /// <summary>
    /// Get the bottom vertical borders position
    /// </summary>
    public Vector2 GetCenterPosition()
    {
        Vector2 setPosition = this.boundsType == BoundsType.RelativeBounds ? this.transform.position : new Vector2(0, 0);

        return setPosition - new Vector2((this.leftBounds - this.rightBounds) / 2, (this.bottomBounds - this.topBounds) / 2);
    }

    /// <summary>
    /// Get the top left boundaries of the border
    /// </summary>
    public Vector2 GetTopLeftBorderPosition() => new Vector2(this.GetLeftBorderPosition(), this.GetTopBorderPosition());

    /// <summary>
    /// Get the top right boundaries of the border
    /// </summary>
    public Vector2 GetTopRightBorderPosition() => new Vector2(this.GetRightBorderPosition(), this.GetTopBorderPosition());

    /// <summary>
    /// Get the bottom left boundaries of the border
    /// </summary>
    public Vector2 GetBottomLeftBorderPosition() => new Vector2(this.GetLeftBorderPosition(), this.GetBottomBorderPosition());

    /// <summary>
    /// Get the bottom right boundaries of the border
    /// </summary>
    public Vector2 GetBottomRightBorderPosition() => new Vector2(this.GetRightBorderPosition(), this.GetBottomBorderPosition());

    /// <summary>
    /// Whether the camera bounds has a value of infinity
    /// </summary>
    public bool HasInfinity() => this.leftBounds == Mathf.Infinity || this.rightBounds == Mathf.Infinity || this.bottomBounds == Mathf.Infinity || this.topBounds == Mathf.Infinity;

}

