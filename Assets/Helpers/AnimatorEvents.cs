using UnityEngine;
/// <summary>
/// Actions that can be performed during the animation cycle of a gameobject
/// </summary>
public class AnimatorEvents : MonoBehaviour
{
    [SerializeField, Tooltip("The audio clip to play when play audio function is called")]
    private AudioClip audioClipToPlay;

    /// <summary>
    /// Deactivate the game object the current object is attached to
    /// </summary>
    private void DeactivateGameObject() => this.gameObject.SetActive(false);

    /// <summary>
    /// Informs the parent object of the end of an animation
    /// </summary>
    private void InformParentOfAnimationEnd()
    {
        if (this.transform.parent != null)
        {
            this.transform.parent.BroadcastMessage("ChildAnimationEnd");
        }
    }

    /// <summary>
    /// Broadcasts a message to any of the game managers to perform the specified function
    /// <param name="animationEvent">The animation event type sent </param>
    /// </summary>
    private void MessageGameManagers(AnimationEvent animationEvent)
    {
        int id = animationEvent.intParameter;
        string eventToBroadcast = animationEvent.stringParameter;
        switch (id)
        {
            case 0:
                GMStageManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            case 1:
                GMSpawnManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            case 2:
                GMRegularStageScoreManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            case 3:
                GMGrantPowerUpManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            case 4:
                GMWaterLevelManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            case 5:
                GMPauseMenuManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            case 6:
                GMSceneManager.Instance().BroadcastMessage(eventToBroadcast);

                break;
            default:
                Debug.LogWarning("A Game Manager with the specified ID: " + id + " was not found");

                break;
        }
    }
}
