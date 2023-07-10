using UnityEngine;
/// <summary>
/// A helper to check whether the gizmos are active or not
/// </summary>
public class GizmosEnabledHelper : MonoBehaviour
{
    [Help("Checks if the gizmos button is toggled while the game is running")]
    private bool gizmosEnabled = false;
    [Tooltip("The frame where `OnDrawGizmos` was last called"), SerializeField]
    private float frameGizmoUpdate;
    [Tooltip("The frame where `Update` was last called"), SerializeField]
    private float frameGameUpdate;

#if UNITY_EDITOR
    // Update is called once per frame
    private void Update()
    {
        this.SetGizmosEnabled(this.frameGameUpdate == this.frameGizmoUpdate);
        this.frameGameUpdate = Time.renderedFrameCount;
    }

    /// <summary>
    /// Returns the value of gizmos enabled
    /// </summary>
    public bool GetGizmosEnabled() => this.gizmosEnabled;
    /// <summary>
    /// Set the value of gizmos enabled
    /// <param name="gizmosEnabled">The new value for gizmos enabled</param>
    /// </summary>
    private void SetGizmosEnabled(bool gizmosEnabled)
    {
        if (this.gizmosEnabled != gizmosEnabled)
        {
            if (gizmosEnabled)
            {
                //Fire on Gizmos enabled
            }
            else
            {
                //Fire On GIzmos disabled
            }
        }

        this.gizmosEnabled = gizmosEnabled;
    }

    private void OnDrawGizmos() => this.frameGizmoUpdate = Time.renderedFrameCount;
#endif

}
