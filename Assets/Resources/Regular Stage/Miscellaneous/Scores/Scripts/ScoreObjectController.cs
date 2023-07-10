using UnityEngine;

/// <summary>
/// The simple movement of the score manager that is displayed when an enemy is destroyed
/// </summary>
public class ScoreObjectController : MonoBehaviour, IPooledObject
{
    [SerializeField]
    private float floatSpeed = 1;
    [SerializeField]
    private float killTime = 0.53333333333f;
    [SerializeField]
    private float currentLife = 0;
    public void OnObjectSpawn() => this.currentLife = 0;

    private void Update() => this.currentLife += Time.deltaTime;
    private void FixedUpdate()
    {
        if (this.currentLife < 0.43333333333f)
        {
            this.transform.position += new Vector3(0, this.floatSpeed, 0f) * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
        }
        if (this.currentLife >= this.killTime)
        {
            this.gameObject.SetActive(false);
        }
    }
}
