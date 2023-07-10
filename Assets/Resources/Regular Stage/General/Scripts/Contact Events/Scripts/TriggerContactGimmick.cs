/// <summary>
/// An extension class used to signify objects that perform action based solely collisions by the player solid box
/// </summary>
public class TriggerContactGimmick : ContactEvent
{
    public override void Reset()
    {
        base.Reset();

        if (this.gameObject.layer == 0)
        {
            this.gameObject.layer = 15;
        }
    }
}
