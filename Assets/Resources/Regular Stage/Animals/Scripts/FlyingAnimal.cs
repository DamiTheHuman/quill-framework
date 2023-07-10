using UnityEngine;

/// <summary>
/// Controller Class for Animals that fly after initial contact
/// </summary>
public class FlyingAnimal : Animal
{
    //Used for defaults
    public override void Reset()
    {
        base.Reset();
        this.acceleration = new Vector2(0.033f, 0.05f);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (this.animalState == AnimalState.Action)
        {
            this.ApplyFlightMovement();
        }
    }

    /// <summary>
    /// Perform flight movement by constantly apply accelration every step after initial ground contact 
    /// </summary>
    private void ApplyFlightMovement() => this.velocity += new Vector2(this.acceleration.x * this.directionFacing, this.acceleration.y);
    /// <summary>
    /// On Initial ground contact disable gravity application to the flying animal
    /// </summary>
    public override void OnInitialGroundContact() => this.applyGravity = false;
}
