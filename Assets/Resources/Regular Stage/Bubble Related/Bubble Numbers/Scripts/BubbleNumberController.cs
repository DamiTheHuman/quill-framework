using System.Collections;
using UnityEngine;
/// <summary>
/// Control the bubble numbers when they are spawned
/// </summary>
public class BubbleNumberController : MonoBehaviour, IPooledObject
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform poolParent;
    [SerializeField, LastFoldoutItem()]
    private Animator animator;
    [SerializeField]
    [Tooltip("The time in seconds between each bubble number")]
    private float waitInterval = 2;

    public void OnObjectSpawn() { }

    private void Reset()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.animator = this.GetComponent<Animator>();
    }

    private void Awake()
    {
        if (this.transform.parent != null)
        {
            this.poolParent = this.transform.parent;
        }
        if (this.spriteRenderer == null)
        {
            this.Reset();
        }
    }

    private void Update()
    {
        this.transform.eulerAngles = Vector3.zero;

        if (this.transform.parent != null)
        {
            this.spriteRenderer.flipX = this.transform.parent.localScale.x == -1;//Ensures it always faces forward
        }
    }

    /// <summary>
    /// Set the time of the bubble to be displayed
    /// <param name="playerOxygen">The current players oxygen level</param>
    /// </summary>
    public void StartBubbleNumberCountDown(int playerOxygen) => this.StartCoroutine(this.CountDownBubbleNumbers(playerOxygen));

    /// <summary>
    /// Hides the bubbles sprite renderer
    /// </summary>
    public void HideBubble() => this.spriteRenderer.enabled = false;

    /// <summary>
    /// Displays the bubble number in relative sequence to the wait interval
    /// <param name="playerOxygen">The playerx oxygen the moment that countdown can begin</param>
    /// </summary>
    private IEnumerator CountDownBubbleNumbers(int playerOxygen)
    {
        int intervals = (int)(playerOxygen / this.waitInterval);

        for (int x = 0; x < intervals + 1; x++)
        {
            this.spriteRenderer.enabled = true;
            this.animator.SetInteger("Time", intervals - x);
            this.animator.SetTrigger("Grow");
            yield return new WaitForSeconds(this.waitInterval);

        }

        yield return null;
    }
}
