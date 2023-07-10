using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// Handles the spawning of bubbles small, medium or large for visuals or to replenish the players health depending on the type
/// Author - Sonic Worlds Creators
/// </summary>
public class BubblerController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    protected BubbleData smallBubbleData;
    [SerializeField]
    protected BubbleData mediumBubbleData;
    [SerializeField]
    protected BubbleData largeBubbleData;

    private void Reset() => this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    // Start is called before the first frame update
    private void Start()
    {
        if (this.spriteRenderer == null)
        {
            this.Reset();
        }

        this.StartCoroutine(this.BubbleSpawnCoroutine(this.smallBubbleData));
        this.StartCoroutine(this.BubbleSpawnCoroutine(this.mediumBubbleData));
        this.StartCoroutine(this.BubbleSpawnCoroutine(this.largeBubbleData));
    }

    /// <summary>
    /// A coroutine which checks a bubbles spawn probability at a regular interval
    /// <param name="bubbleData"> The current bubble info being analyzed</param>
    /// </summary>
    private IEnumerator BubbleSpawnCoroutine(BubbleData bubbleData)
    {
        while (true)
        {
            yield return new WaitForSeconds(bubbleData.GetSpawnInterval());

            //Ensures nothing is performed when not in view
            if (HedgehogCamera.Instance().IsSpriteWithinCameraView(this.spriteRenderer) == false)
            {
                continue;
            }

            this.CheckBubbleProbability(bubbleData);
        }
    }

    /// <summary>
    /// Checks the possiblity of spawning a bubble and if sucessful spanws a bubble of the set type
    /// <param name="bubbleData"> The current bubble info being analyzed</param>
    /// </summary>
    private void CheckBubbleProbability(BubbleData bubbleData)
    {
        bubbleData.SetActiveProbability(Random.Range(0, bubbleData.GetProbabilityLimit()));

        if (bubbleData.GetBubbleType() == BubbleType.Large)
        {
            bubbleData.SetActiveProbability(bubbleData.GetActiveProbability() + 1); // add 1
            bubbleData.SetCurrentProbability(bubbleData.GetCurrentProbability() - 1); //Ensures after a certain time a large bubble will spawn
        }

        //If the medium probability is 1 spawn a bubble
        if (bubbleData.GetActiveProbability() == 1 || bubbleData.GetCurrentProbability() == -1)
        {
            bubbleData.SetActiveProbability(0);
            bubbleData.SetCurrentProbability(bubbleData.GetProbabilityLimit());
            this.SpawnBubble(bubbleData.GetBubbleType());
        }
    }

    /// <summary>
    /// Spawns the appropriate bubble based on the type
    /// <param name="bubbleType"> The type of bubble to spawn </param>
    /// </summary>
    private void SpawnBubble(BubbleType bubbleType)
    {
        switch (bubbleType)
        {
            case BubbleType.Small:
                GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.SmallBubble, this.transform.position);

                break;
            case BubbleType.Medium:
                GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.MediumBubble, this.transform.position);

                break;
            case BubbleType.Large:
                GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.LargeBubble, this.transform.position);

                break;
        }
    }
}
