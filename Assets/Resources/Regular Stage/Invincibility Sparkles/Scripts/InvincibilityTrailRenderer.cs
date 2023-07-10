using UnityEngine;
/// <summary>
/// Serves as the head of the invincibility trail renderer
/// </summary>
public class InvincibilityTrailRenderer : MonoBehaviour
{
    public Player player;
    [Tooltip("The target of the invincibility trail"), SerializeField]
    public Transform target;
    public static string resetFadoutFunctionMessage = "ResetFadeOut";
    public static string fadeOutFunctionMessage = "FadeOut";
    /// <summary>
    /// Set the target for the invincibility trail
    /// <param name="transform">The new target for the trail</param>
    /// <param ref name="player"> Where applicable the player this power up is granted to</param>
    /// </summary>
    public void SetTarget(Transform transform, Player player = null)
    {
        this.target = transform;
        this.player = player;
    }

    /// <summary>
    /// Get the target for the invincibility trail
    /// </summary>
    public Transform GetTarget()
    {
        if (this.target == null)
        {
            this.target = this.transform;
        }

        return this.target;
    }
}
