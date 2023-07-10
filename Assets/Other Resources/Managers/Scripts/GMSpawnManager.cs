using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A Manager class that handles how objects are spawned making use of pools
/// This provides a more efficient alternatives to Instantiate and Destroy methods
/// </summary>
public class GMSpawnManager : MonoBehaviour
{
    public Dictionary<ObjectToSpawn, Transform> repoolArray = new Dictionary<ObjectToSpawn, Transform>();
    [SerializeField]
    private SpawnablePrefabsScriptableObjects spawnablePrefabs;
    [HideInInspector, SerializeField]
    private PoolDictionary poolDictionary;

    [IsDisabled, SerializeField]
    private GameObject otherObjectsPile;
    /// <summary>
    /// The single instance of the spawn manager
    /// </summary>
    private static GMSpawnManager instance;

    private void Awake()
    {
        this.transform.parent = null;
        DontDestroyOnLoad(this);

        if (Instance() != null && Instance() != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);//Destroy duplicate spawn managers

            return;
        }

        this.PopulatePool();
    }

    /// <summary>
    /// Everytime a new scene is loaded this is called, This will server as the Start function
    /// </summary>
    private void OnSceneLoaded() => instance = this;

    /// <summary>
    /// The actions performed on every scene reload
    /// </summary>
    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode) => this.OnSceneLoaded();
    /// <summary>
    /// Get a reference to the static instance of the spawn manager
    /// </summary>
    public static GMSpawnManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMSpawnManager>();
            instance.poolDictionary.OnAfterDeserialize();//Reloads the serialized data if needed
        }

        return instance;
    }

    /// <summary>
    /// Populates the pools based on the gameobjects type and amount set
    /// </summary>
    private void PopulatePool()
    {
        this.poolDictionary = new PoolDictionary();  //Create an empty dictonary
        Transform mainPoolHolder = new GameObject().transform;//A parent object to keep the inspector tidy
        mainPoolHolder.transform.parent = this.transform;
        mainPoolHolder.name = "Object Pool Holder";

        //Populate the pools
        foreach (SpawnablePoolData pool in this.spawnablePrefabs.pool)
        {
            Transform objectPoolParent = new GameObject().transform; //A parent object to keep the inspector tidy
            objectPoolParent.transform.parent = this.transform;
            objectPoolParent.name = "Pool: " + pool.GetPrefab().name;
            objectPoolParent.transform.parent = mainPoolHolder;

            SerializableQue objectPool = new SerializableQue();

            for (int i = 0; i < pool.GetSize(); i++)
            {
                if (pool.GetPrefab() != null)
                {
                    PooledObject obj = Instantiate(pool.GetPrefab()).AddComponent<PooledObject>();
                    obj.transform.parent = objectPoolParent;
                    obj.SetParent(objectPoolParent, pool.GetTag());
                    obj.gameObject.SetActive(false);
                    objectPool.Enqueue(obj.gameObject);//Save the object
                }
            }

            this.poolDictionary.Add(pool.GetTag(), objectPool);//Add the pooled objects to the dictionary
            this.poolDictionary.OnBeforeSerialize();//Serializes our data
        }
    }

    /// <summary>
    /// Spawns a gameobject from the pool of game objects based on an enumarator and spawn position which can be called anywhere
    /// <param name="spawnTarget">The gameobject to spawn </param>
    /// <param name="spawnPosition">The position to spawn the game object </param>
    /// </summary>
    public GameObject SpawnGameObject(ObjectToSpawn spawnTarget, Vector3 spawnPosition)
    {
        if (this.poolDictionary.ContainsKey(spawnTarget))
        {
            GameObject objectToSpawn = this.poolDictionary[spawnTarget].Dequeue();//Retrieves the first element in the queue

            if (objectToSpawn == null)
            {
                Debug.Log("Could not Spawn Object " + spawnTarget);

                return null;
            }

            objectToSpawn.transform.position = spawnPosition;
            IPooledObject pooledObject = objectToSpawn.GetComponent<IPooledObject>();

            if (pooledObject != null)
            {
                pooledObject.OnObjectSpawn();//Call the insantiazion like an awake
            }

            objectToSpawn.SetActive(true);
            this.poolDictionary[spawnTarget].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        return null;
    }

    /// <summary>
    /// Spawns a random Animal from the list of possible animals usually 3 per stage
    /// <param name="spawnPosition">The position to spawn the game object </param>
    /// </summary>
    public GameObject SpawnRandomAnimal(Vector2 spawnPosition)
    {
        int animalToSpawn = UnityEngine.Random.Range(1, 4);
        GameObject animalSpawned = null;
        animalSpawned = animalToSpawn switch
        {
            1 => this.SpawnGameObject(ObjectToSpawn.Animal1, spawnPosition),
            2 => this.SpawnGameObject(ObjectToSpawn.Animal2, spawnPosition),
            3 => this.SpawnGameObject(ObjectToSpawn.Animal3, spawnPosition),
            _ => this.SpawnGameObject(ObjectToSpawn.Animal1, spawnPosition),
        };

        return animalSpawned;
    }

    /// <summary>
    /// Create the other objects pile when needed
    /// </summary>
    private void CreateOtherObjectsPile()
    {
        this.otherObjectsPile = new GameObject("Other Objects Pile");
        this.otherObjectsPile.transform.parent = null;

    }

    /// <summary>
    /// A pile of objects used to keep the inspector clean
    /// </summary>
    public GameObject GetOtherObjectsPile()
    {
        if (this.otherObjectsPile == null)
        {
            this.CreateOtherObjectsPile();
        }

        return this.otherObjectsPile;
    }
}
