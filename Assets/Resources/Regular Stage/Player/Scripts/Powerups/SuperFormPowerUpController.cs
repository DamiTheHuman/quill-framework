using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the activity of the super form
/// Author - Nihil [Core Framework] code used for both super sparkle positions
/// </summary>
public class SuperFormPowerUpController : HedgePowerUp
{
    [Tooltip("The mninimum speed the player must be going at to spawn a large sparkle")]
    public float largeSparkleSpeedCondition = 6;
    [Tooltip("The interval in which a super sparkle is spawned within the coroutine"), SerializeField]
    private float superSparkleSpawnInterval = 0.35f;
    private IEnumerator largeSuperSparkleCoroutine;

    /// <summary>
    /// Begins the coroutine which spawns a large sparkle at the set interval
    /// </summary>
    public void BeginLargeSparkleCoroutine()
    {
        if (this.largeSuperSparkleCoroutine != null)
        {
            this.StopCoroutine(this.largeSuperSparkleCoroutine);
        }

        this.largeSuperSparkleCoroutine = this.SpawnLargeSuperSparkleCoroutine(this.superSparkleSpawnInterval);
        this.StartCoroutine(this.largeSuperSparkleCoroutine);
    }
    /// <summary>
    /// Spawns a sparkle effect round the player
    /// </summary>
    public void SpawnSmallSuperSparkle()
    {
        bool shouldSpawnSparkle = Random.Range(0, 16) == 0;

        if (shouldSpawnSparkle)
        {
            Vector2 spawnPosition = this.player.transform.position;
            spawnPosition.x += Random.Range(12, -12);
            spawnPosition.y -= Random.Range(-16, 16);
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.SuperSparkSmall, spawnPosition);
        }
    }
    /// <summary>
    /// Spawns a large sparkle effect around the player as long as they reach the set conditions
    /// </summary>
    private void SpawnLargeSuperSparkle()
    {
        if ((Mathf.Abs(this.player.currentPlayerDirection) == 1 && Mathf.Abs(this.player.GetHorizontalVelocity()) > this.largeSparkleSpeedCondition) || Mathf.Abs(this.player.velocity.y) > this.largeSparkleSpeedCondition)
        {
            Vector2 spawnPosition = this.player.transform.position;
            SuperSparkController superSpark = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.SuperSparkLarge, spawnPosition).GetComponent<SuperSparkController>();
            superSpark.SetPlayer(this.player);
        }
    }
    /// <summary>
    /// Actions that take place when the powerup is ended
    /// </summary>
    public override void RemovePowerUp()
    {
        if (this.largeSuperSparkleCoroutine != null)
        {
            this.StopCoroutine(this.largeSuperSparkleCoroutine);
        }
    }

    /// <summary>
    /// A coroutine to manage the spawning of sparkles as long as the player is super
    /// <param name="superSparkSpawnTimeStep">The interval in which a sparkle should be spawned</param>
    /// </summary>
    private IEnumerator SpawnLargeSuperSparkleCoroutine(float superSparkSpawnTimeStep)
    {
        while (this.hedgePowerUpManager.GetSuperPowerUp() == SuperPowerUp.SuperForm)
        {
            yield return new WaitForSeconds(superSparkSpawnTimeStep);
            this.SpawnLargeSuperSparkle();
        }
        yield return null;
    }
}
