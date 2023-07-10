/// <summary>
/// An extension class used to signify objects that perform action based on collisions by the player hitbox ONLY
/// </summary>
public class HitBoxContactEvent : ContactEvent
{
    public override void Reset()
    {
        if (this.gameObject.layer == 0)
        {
            this.gameObject.layer = 20;
        }
    }

    /// <summary>
    /// When an object has been touched by a secondary hitbox item that is not the player like Tails' Tails or Knuckles Gloves
    /// </summary>
    public virtual void SecondaryHitBoxObjectAction(SecondaryHitBoxController secondaryHitBoxController) { }
}
