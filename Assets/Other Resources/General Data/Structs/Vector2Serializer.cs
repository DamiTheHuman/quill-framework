using UnityEngine;

/// <summary>
/// Help serialize vector 2's so they cannot be serialized and saved natively
/// </summary>

[System.Serializable]
public struct Vector2Serializer
{
    /// <summary>
    /// The x Vector
    /// </summary>
    private float x;
    /// <summary>
    /// The y vector
    /// </summary>
    private float y;

    /// <summary>
    /// Store the vector2 info
    /// </summary>
    public void Fill(Vector2 vector2)
    {
        this.x = vector2.x;
        this.y = vector2.y;
    }

    /// <summary>
    /// Get the serialized info as a <see cref="Vector2"/>
    /// </summary>
    public Vector2 Vector2 => new Vector2(this.x, this.y);
}
