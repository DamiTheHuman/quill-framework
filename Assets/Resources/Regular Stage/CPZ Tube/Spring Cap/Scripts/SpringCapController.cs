using UnityEngine;
/// <summary>
/// The functionality of the spring cap controller which has the base functionality of a regular spring
/// </summary>
public class SpringCapController : SpringController
{

    [FirstFoldOutItem("Spring Cap Properties"), LastFoldoutItem(), SerializeField]
    private bool open;

    /// <summary>
    /// Sets the state of the cap
    /// <param name="value">The open state of the spring cap </param>
    /// </summary>
    public void SetCapOpenState(bool value)
    {
        if (this.open == value)
        {
            return;
        }

        this.open = value;
        this.animator.SetBool("Open", this.open);
        this.boxCollider2D.enabled = this.open == false;//Disable the collider to allow the player go through it
    }

}
