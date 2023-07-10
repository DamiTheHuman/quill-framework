using UnityEngine;

public class SuperSparkController : MonoBehaviour
{
    [SerializeField]
    private Player player;
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Player not moving anymore so deactivate
        if (this.player.GetHorizontalVelocity() == 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Set the player for the super spark
    /// <param name="player">The player object triggering the spark</param>
    /// </summary>
    public void SetPlayer(Player player) => this.player = player;
}
