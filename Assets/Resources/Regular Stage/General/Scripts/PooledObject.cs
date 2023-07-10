using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Handles object within the scene
/// </summary>
public class PooledObject : MonoBehaviour
{
    public static string repoolFunction = "RepoolObject";
    [Help("Watch disabled objects so we can keep the scene clean")]
    public Transform parentPool = null;
    public ObjectToSpawn objectToSpawn;

    private void Start()
    {
        SceneManager.sceneUnloaded += this.OnSceneUnloaded;
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    private void OnSceneUnloaded(Scene current)
    {
        this.RepoolObject();
        GMSceneManager.OnDismountSceneEvent -= this.OnDismountScene;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => GMSceneManager.OnDismountSceneEvent += this.OnDismountScene;
    private void OnEnable() => GMSceneManager.OnDismountSceneEvent += this.OnDismountScene;

    private void OnDisable()
    {
        //If the object is not in the parent do not unsubscribe from the event
        if (this.transform.parent == this.parentPool)
        {
            GMSceneManager.OnDismountSceneEvent -= this.OnDismountScene;
        }

        DontDestroyOnLoad(this.gameObject);//Apply this after parenting to ensure the object is back in the pool even if the object is waiting to repool
        this.Invoke(nameof(WaitTillUnAdd), Time.fixedDeltaTime);//Add a delay to repool cause we cant reparent during an onDisable which causes errors
    }

    /// <summary>
    /// Performed when a scene is about to be loaded
    /// </summary>
    private void OnDismountScene()
    {
        this.transform.parent = this.parentPool.transform;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Set the parent of the pool
    /// </summary>
    public void SetParent(Transform parentPol, ObjectToSpawn objectToSpawn)
    {
        this.parentPool = parentPol;
        this.objectToSpawn = objectToSpawn;
    }

    /// <summary>
    /// Check to see if an object is abut to be unadded before clean up
    /// </summary>
    private void WaitTillUnAdd()
    {
        if (this.transform.parent == null)
        {
            this.RepoolObject();
        }
    }

    /// <summary>
    /// Parents an object back to its parent pool to keep the scene clean
    /// </summary>
    private void RepoolObject()
    {
        if (this.parentPool == null)
        {
            return;
        }

        this.transform.parent = this.parentPool.transform;
        this.gameObject.SetActive(false);
    }
}
