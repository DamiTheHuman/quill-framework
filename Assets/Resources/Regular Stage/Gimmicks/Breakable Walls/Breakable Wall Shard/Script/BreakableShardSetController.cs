using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakableShardSetController : MonoBehaviour, IPooledObject
{
    [Tooltip("A Find all the shard controllers parented to this game object"), Button(nameof(FindAllShardControllers)), SerializeField]
    private bool findChildShardControllers = true;

    [Tooltip("Calculate the velocity to apply based on shard set positions"), Button(nameof(CalculatShardSetStartVelocities)), SerializeField]
    private bool autoCalculateShardVelocities = true;

    [FirstFoldOutItem("Dependencies"), SerializeField]
    private List<BreakableWallShardController> breakableShardSetControllers;
    [LastFoldoutItem(), SerializeField]
    private SpriteRenderer debugSroteRenderer;

    [SerializeField, FirstFoldOutItem("Auto Calculation Setting"), Tooltip("The highest velocity for the shard at the top most left position")]
    private Vector2 startVelocity = new Vector2(1.5f, 2f);
    [SerializeField, Tooltip("The amount the shard velocities degrade from top left to bottom right")]
    private Vector2 degradtion = new Vector2(2f, 4f);
    [SerializeField, Tooltip("The amount of shard in a column"), LastFoldoutItem()]
    private int columnLength = 2;

    [SerializeField, Tooltip("List of shards that have been deactivated")]
    private int deactivatedShartCounter;

    public void OnObjectSpawn()
    {
        foreach (BreakableWallShardController breakableWallShardController in this.breakableShardSetControllers)
        {
            breakableWallShardController.gameObject.SetActive(true);
            breakableWallShardController.OnObjectSpawn();
        }

        this.deactivatedShartCounter = 0;
    }

    /// <summary>
    /// Deactivate the set shard and increment the counter
    /// </summary>
    public void DeactivatedShard()
    {
        this.deactivatedShartCounter++;

        if (this.deactivatedShartCounter >= this.breakableShardSetControllers.Count) //All the shards are deactivated
        {
            this.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Find all the shard controlleres that are children of this component
    /// </summary>
    public void FindAllShardControllers()
    {
        this.breakableShardSetControllers.Clear();

        foreach (BreakableWallShardController breakableWallShardController in this.GetComponentsInChildren<BreakableWallShardController>())
        {
            this.breakableShardSetControllers.Add(breakableWallShardController);
        }

        this.breakableShardSetControllers = this.breakableShardSetControllers.OrderByDescending(
            shard => shard.transform.position.y
          ).ThenBy(
            shard => shard.transform.position.x
          ).ToList();
    }
    /// <summary>
    /// Auto calculate the shard velocities based on the start velocity and degradationv alues
    /// </summary>
    public void CalculatShardSetStartVelocities()
    {
        this.FindAllShardControllers();

        if (this.breakableShardSetControllers.Count == 0)
        {
            return;
        }

        Vector2 pointerPosition = new Vector2(0, 0);

        bool applyExtra = false;
        for (int x = 0; x < this.breakableShardSetControllers.Count; x++)
        {
            Vector2 velocity = new Vector2
            {
                x = this.startVelocity.x - (this.startVelocity.x / this.degradtion.x * pointerPosition.x),
                y = this.startVelocity.y / ((pointerPosition.y + 1) / this.degradtion.y)
            };

            //On even rows add an extra offset to the velocity so it smoothens out
            if (velocity.x == 0 && pointerPosition.x > (this.columnLength - 1) / 2 && this.columnLength % 2 == 0)
            {
                applyExtra = true;
            }

            if (applyExtra)
            {
                velocity.x -= this.startVelocity.x / this.degradtion.x;
            }

            this.breakableShardSetControllers[x].projectileData.SetStartVelocity(velocity);

            if (pointerPosition.x > this.columnLength)
            {
                pointerPosition.y++;
                pointerPosition.x = 0;
                applyExtra = false;

                continue;
            }

            pointerPosition.x++;
        }

        General.SetDirty(this);
    }
}
