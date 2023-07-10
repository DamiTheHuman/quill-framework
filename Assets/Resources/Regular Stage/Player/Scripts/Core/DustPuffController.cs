using System.Collections;
using UnityEngine;
/// <summary>
/// Originally conceptgotten from Orbinaut Framework
/// </summary>
public class DustPuffController : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [Tooltip("The interval in steps between each dust puff in steps"), SerializeField]
    private float dustPuffSpawnIntervalInSteps = 4;
    [Tooltip("Spawns dust puff coroutine")]
    private IEnumerator spawnDustPuffsCoroutine;

    private void Reset() => this.player = this.GetComponentInParent<Player>();

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Checks and spawns dust puffs when appropriate
    /// </summary>
    public void CheckToSpawnDustPuffs()
    {
        if (this.ShouldSpawnDustPuffs() == false)
        {
            if (this.spawnDustPuffsCoroutine != null)
            {
                this.StopCoroutine(this.spawnDustPuffsCoroutine);
                this.spawnDustPuffsCoroutine = null;
            }

            return;
        }

        if (this.spawnDustPuffsCoroutine != null)
        {
            return;
        }

        this.spawnDustPuffsCoroutine = this.SpawnDustPuffsCoroutine();
        this.StartCoroutine(this.spawnDustPuffsCoroutine);
    }

    /// <summary>
    /// Validates if we can spawn dust puffs based on the players state
    /// </summary>
    private bool ShouldSpawnDustPuffs()
    {
        ActionManager actionManager = this.player.GetActionManager();

        return actionManager.CheckActionIsBeingPerformed<Skidding>() || (actionManager.GetAction<Glide>() != null && (actionManager.GetAction<Glide>() as Glide).GetGlideState() == GlideState.Sliding);
    }

    /// <summary>
    /// Spawn Dusts Coroutine
    /// </summary>
    private IEnumerator SpawnDustPuffsCoroutine()
    {
        while (true)
        {
            if (this.ShouldSpawnDustPuffs() == false)
            {
                break;
            }

            Vector2 spawnPosition = this.player.transform.position;
            spawnPosition.y -= this.player.GetSensors().characterBuild.bodyHeightRadius;
            GameObject dustPuff = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.DustPuff, spawnPosition);

            yield return new WaitForSeconds(General.StepsToSeconds(this.dustPuffSpawnIntervalInSteps));
        }

        yield return null;
    }
}
