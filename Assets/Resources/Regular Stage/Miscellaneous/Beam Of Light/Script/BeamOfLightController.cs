using UnityEngine;
/// <summary>
/// A beam of light spawned when a teleporter is activated
/// Author  - AZU and Damizien -in Sonic Worlds
/// Changes - Converted to unity with some minor tweaks for customization
/// </summary>
public class BeamOfLightController : MonoBehaviour, IPooledObject
{
    [SerializeField]
    private Vector2 startPosition;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private bool activated = false;

    private void Reset()
    {
        this.animator = this.GetComponent<Animator>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    private void Awake()
    {
        if (this.animator == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// When the object is spawned set the activated state and store the set spawn position
    /// </summary>
    public void OnObjectSpawn() => this.startPosition = this.transform.position;

    private void OnEnable() => this.SetActivated(true);

    private void FixedUpdate() => this.animator.SetBool("Activated", this.activated);

    private void LateUpdate()
    {
        this.LightFlash();
        this.StickToBottom(this.transform.position);
    }

    /// <summary>
    /// Flash the beam of lgiht in relation to the active timer
    /// </summary>
    private void LightFlash()
    {
        int alpha = (int)(40 + (Mathf.Abs(Mathf.Cos(GMRegularStageScoreManager.Instance().GetTimerCount() * 2f)) * 32f));
        this.spriteRenderer.color = General.RGBToColour(255, 255, 255, alpha);
    }

    /// <summary>
    /// Sticks the beam of lgiht to the bottom of the screen as it moves
    /// <param name="position">The current positon of the beam of light</param>
    /// </summary>
    private void StickToBottom(Vector2 position)
    {
        if (HedgehogCamera.Instance().GetBounds().min.y > this.startPosition.y - (this.spriteRenderer.size.y / 2))
        {
            position.y = HedgehogCamera.Instance().GetBounds().min.y + (this.spriteRenderer.size.y / 2);
            this.transform.position = position;
        }
    }

    /// <summary>
    /// Sets the activated status of the beam of light
    /// <param name="activated">The new value for activated</param>
    /// </summary>
    public void SetActivated(bool activated) => this.activated = activated;
}
