using UnityEngine;
/// <summary>
/// Controller Class for Animals that hop on each ground contact
/// </summary>
public class HoppingAnimal : Animal
{
    //Used for defaults
    public override void Reset()
    {
        base.Reset();
        this.acceleration = new Vector2(2, 3);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (this.animalState == AnimalState.Action)
        {
            this.UpdateHopAnimation();
        }
    }

    /// <summary>
    /// Update the hop animation based on the vertical movement of the animal
    /// </summary>
    private void UpdateHopAnimation()
    {
        int animationState = this.velocity.y > 0 ? 1 : 2;
        this.UpdateAnimationState(animationState);
    }

    /// <summary>
    /// ON contact with the ground apply acceleration to give a hopping effet
    /// </summary>
    public override void TriggerGroundAction()
    {
        base.TriggerGroundAction();
        this.velocity = this.acceleration;
        this.velocity.x *= this.directionFacing;
    }
}