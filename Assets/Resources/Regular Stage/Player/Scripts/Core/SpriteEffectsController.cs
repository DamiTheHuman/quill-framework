using System.Collections.Generic;
using UnityEngine;
/// <summary>
///A Controller for sprite effects which comprises of prefabs that spawn at a specific position parented to the player Sprite object
/// </summary>
public class SpriteEffectsController : MonoBehaviour
{
    [SerializeField]
    private Transform bubbleSpawnPoint;
    [Tooltip("A list of all the sprite effects the player can have"), SerializeField]
    private List<SpriteEffectsData> spriteEffects;
    [HideInInspector]
    public SpriteEffectsDictionary spriteEffectsDictionary;
    [Tooltip("A reference to the active sprite effect "), SerializeField]
    private ActiveSpriteEffectData currentSpriteEffect;

    // Start is called before the first frame update
    private void Start() => this.PopulateSpriteEffectsPool();
    /// <summary>
    /// Populates the pool of common sprite effects used by the player
    /// </summary>
    private void PopulateSpriteEffectsPool()
    {
        this.spriteEffectsDictionary = new SpriteEffectsDictionary();  //Create an empty dictonary
        Transform mainPoolHolder = new GameObject().transform;//A parent object to keep the inspector tidy
        mainPoolHolder.transform.parent = this.transform;
        mainPoolHolder.transform.position = this.transform.parent.position;
        mainPoolHolder.name = "Sprite Effects";
        //Populate the pools
        foreach (SpriteEffectsData pool in this.spriteEffects)
        {
            Transform objectPoolParent = new GameObject().transform; //A parent object to keep the inspector tidy
            objectPoolParent.transform.parent = this.transform;
            objectPoolParent.name = "Effect: " + pool.GetPrefab().name;
            objectPoolParent.transform.parent = mainPoolHolder;
            objectPoolParent.transform.localPosition = pool.GetRelativeSpawnPosition();

            SerializableQue objectPool = new SerializableQue();
            //Unlike The Spawn Manager this only creates one effect
            if (pool.GetPrefab() != null)
            {
                GameObject obj = Instantiate(pool.GetPrefab());
                obj.transform.parent = objectPoolParent;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.eulerAngles = Vector3.zero;
                obj.SetActive(false);
                objectPool.Enqueue(obj);//Save the object
            }

            this.spriteEffectsDictionary.Add(pool.GetTag(), objectPool);//Add the pooled objects to the dictionary
            this.spriteEffectsDictionary.OnBeforeSerialize();//Serializes our data
        }
    }

    /// <summary>
    /// Gets a currently spawned object and sets its toggle value
    /// <param name="status">The of the toggled item </param>
    /// <param name="spriteEffect">The gameobject to spawn </param>
    /// </summary>
    public GameObject ToggleEffect(SpriteEffectToggle spriteEffect, bool status)
    {
        this.transform.localPosition = Vector3.zero;

        if (this.spriteEffectsDictionary.Count == 0)
        {
            this.spriteEffectsDictionary.OnAfterDeserialize();
        }

        if (this.spriteEffectsDictionary.ContainsKey(spriteEffect))
        {
            GameObject objectToSpawn = this.spriteEffectsDictionary[spriteEffect].Dequeue();

            if (status == false)
            {
                this.spriteEffectsDictionary[spriteEffect].Enqueue(objectToSpawn);
                this.currentSpriteEffect = new ActiveSpriteEffectData();
            }
            else
            {
                objectToSpawn.transform.eulerAngles = Vector3.zero;
                this.SetCurrentSpriteEffect(spriteEffect, objectToSpawn);
            }

            objectToSpawn.SetActive(status);
            this.spriteEffectsDictionary[spriteEffect].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        Debug.LogError("Could not toggle effect " + spriteEffect);

        return null;
    }

    /// <summary>
    /// Sets the current active sprite effect
    /// <param name="spawnTarget">The tag of the gameobject being spawned </param>
    /// <param name="objectToSpawn">The gameObject being spawned</param>
    /// </summary>
    public void SetCurrentSpriteEffect(SpriteEffectToggle spawnTarget, GameObject objectToSpawn)
    {
        this.currentSpriteEffect.SetPrefab(objectToSpawn);
        this.currentSpriteEffect.SetRelativeSpawnPosition(objectToSpawn.transform.localPosition);
        this.currentSpriteEffect.SetTag(spawnTarget);

        if (objectToSpawn.GetComponent<Animator>() != null)
        {
            this.currentSpriteEffect.SetAnimator(objectToSpawn.GetComponent<Animator>());
        }
    }

    /// <summary>
    /// Gets the current active sprite effect
    /// </summary>
    public ActiveSpriteEffectData GetCurrentSpriteEffect() => this.currentSpriteEffect.GetPrefab() != null ? this.currentSpriteEffect : null;

    /// <summary>
    /// Gets the spawn point of the bubble
    /// </summary>
    public Transform GetBubbleSpawnPoint() => this.bubbleSpawnPoint;

    private void OnValidate()
    {
        foreach (SpriteEffectsData spriteEffect in this.spriteEffects)
        {
            spriteEffect.SetName(General.TransformSpacesToUpperCaseCharacters(spriteEffect.GetTag().ToString()));
        }
    }
}
