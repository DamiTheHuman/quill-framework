using System.Collections;
using UnityEngine;
/// <summary>
/// A manager class for the players breathing while underwater
/// </summary
public class OxygenManager : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies"), LastFoldoutItem]
    private Player player;

    [FirstFoldOutItem("Players Oxygen")]
    [Tooltip("The maximum amount of oxygen the player can have")]
    public float maximumOxygen = 32;
    [Tooltip("The current oxygen the player has")]
    [LastFoldoutItem]
    public float currentOxygen = 32;
    private IEnumerator breathingCoroutine;

    [SerializeField, FirstFoldOutItem("Bubble related")]
    private BubbleNumberController bubbleNumberController;
    [Tooltip("The interval in which the player breaths which effects the bubble spawn time"), SerializeField]
    private float breathInterval = 0.8f;
    [LastFoldoutItem()]
    [Tooltip("The Position in which the bubble is spawned"), SerializeField]
    private Vector2 relativeBubbleNumberPosition = new Vector2(0, 32);

    private void Reset() => this.player = this.GetComponent<Player>();
    // Start is called before the first frame update
    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }

        this.currentOxygen = this.maximumOxygen;
    }

    // Update is called once per frame
    private void Update() => this.UpdatePlayerBreath();

    /// <summary>
    /// Updates the current player breath when the player is underwater
    /// </summary>
    private void UpdatePlayerBreath()
    {
        if (this.player.GetPhysicsState() == PhysicsState.Underwater)
        {
            if (this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.BubbleShield)  //Underwater and without a bubble shield thus reduce the players breath
            {
                if (this.breathingCoroutine == null)
                {
                    this.breathingCoroutine = this.BreathingCoroutine();
                    this.StartCoroutine(this.breathingCoroutine);
                }
                if (this.currentOxygen > 0 && this.player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.InTeleporter)
                {
                    this.currentOxygen -= Time.deltaTime;

                    if (this.currentOxygen <= 12)
                    {
                        this.CheckBubbleTime();
                    }
                }
                if (this.currentOxygen == 0)
                {
                    this.DrownPlayer();
                }
            }
            else
            {
                this.EndBreathingCoroutine();
                this.ReplenishPlayerBreath();
                this.StopDrowningSequence();
            }
        }
        else
        {
            this.EndBreathingCoroutine();
            this.StopDrowningSequence();
            this.ReplenishPlayerBreath();
        }

        this.currentOxygen = Mathf.Clamp(this.currentOxygen, 0, this.maximumOxygen);
    }
    /// <summary>
    /// Actions that take place when the player dies bby dorwning
    /// </summary>
    private void DrownPlayer()
    {
        if (this.bubbleNumberController != null)
        {
            this.bubbleNumberController.gameObject.SetActive(false);
        }

        this.player.GetHealthManager().KillPlayer();
    }
    /// <summary>
    /// Spawns the approrpiate bubble number relative to the current time
    /// </summary>
    private void CheckBubbleTime()
    {
        if (this.bubbleNumberController == null)
        {
            Vector2 bubbleNumperSpawnPosition = this.relativeBubbleNumberPosition + (Vector2)this.transform.position;
            this.bubbleNumberController = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BubbleNumber, bubbleNumperSpawnPosition).GetComponent<BubbleNumberController>();
            this.bubbleNumberController.StartBubbleNumberCountDown((int)this.currentOxygen);
            this.bubbleNumberController.transform.parent = this.player.GetSpriteController().transform;//Makes it follow the player
            GMAudioManager.Instance().PlayAndQueueBGM(BGMToPlay.DrowningTheme);
        }
    }

    /// <summary>
    /// Sets the players breath to its max
    /// </summary>
    public void ReplenishPlayerBreath()
    {
        if (this.currentOxygen < this.maximumOxygen)
        {
            this.currentOxygen = this.maximumOxygen;
        }

        this.StopDrowningSequence();
    }
    /// <summary>
    /// Unsets the bubble numbers if they are active and stops the drowning theme
    /// </summary>
    private void StopDrowningSequence()
    {
        if (this.bubbleNumberController != null)
        {
            this.bubbleNumberController.gameObject.SetActive(false);
            this.bubbleNumberController = null;
            GMAudioManager.Instance().StopBGM(BGMToPlay.DrowningTheme);
        }
    }
    /// <summary>
    /// Ends the bubble spawn breathing coroutine
    /// </summary>
    private void EndBreathingCoroutine()
    {
        if (this.breathingCoroutine != null)
        {
            this.StopCoroutine(this.breathingCoroutine);
            this.breathingCoroutine = null;
        }
    }

    /// <summary>
    /// Spawns a small bubble at the players mouth when ever the player exhales
    /// </summary>
    private IEnumerator BreathingCoroutine()
    {
        bool exhaling = false;

        while (this.player.GetPhysicsState() == PhysicsState.Underwater && this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() != ShieldType.BubbleShield)
        {
            yield return new WaitForSeconds(this.breathInterval);
            if (exhaling)
            {
                GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.SmallBubble, this.player.GetSpriteEffectsController().GetBubbleSpawnPoint().transform.position);
            }
            exhaling = !exhaling;//Switch between exhaling and inhaling
        }
        yield return null;
    }

}
