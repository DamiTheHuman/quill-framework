using System.Collections;
using UnityEngine;

/// <summary>
/// A controller class that handles the way the static on a monitor is displayed allowing it to flicker infrequently
/// </summary>
public class MonitorStaticController : MonoBehaviour
{
    private Animator animator;
    public float timeMultiplier = 2f;
    public float currentWaitTime = 1;
    private int replayHash = 0;

    private void Start()
    {
        this.animator = this.GetComponent<Animator>();
        this.replayHash = Animator.StringToHash("Replay");
    }

    /// <summary>
    /// Adjust the animation timer at the end of monitor static controller
    /// This is called via the components animator
    /// </summary>
    public void UpdateAnimationReplayTimer()
    {
        this.StartCoroutine(this.ReplayAnimationTimer(this.currentWaitTime * this.timeMultiplier));
        this.currentWaitTime = this.currentWaitTime switch
        {
            1 => 0.5f,
            0.5f => 0.25f,
            _ => 1f,
        };
    }

    /// <summary>
    /// A coroutine which aims to wait a while before restarting the animation
    /// </summary>
    private IEnumerator ReplayAnimationTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.animator.SetTrigger(this.replayHash);
    }
}
