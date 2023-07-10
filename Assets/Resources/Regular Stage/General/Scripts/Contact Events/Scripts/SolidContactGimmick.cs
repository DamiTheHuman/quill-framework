using UnityEngine;
/// <summary>
/// An extension class used to signify objects that perform action based on collisions by the player solid box and sensors
/// </summary>
public class SolidContactGimmick : ContactEvent
{
    [Tooltip("The solid gimmick type")]
    public SolidContactGimmickType solidGimmickType = SolidContactGimmickType.Normal;

    public override void Reset()
    {
        base.Reset();

        if (this.gameObject.layer == 0)
        {
            this.gameObject.layer = 13;
        }
    }
}
