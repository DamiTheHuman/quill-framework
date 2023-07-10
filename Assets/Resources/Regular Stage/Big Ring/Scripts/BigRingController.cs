using System.Collections;
using UnityEngine;
/// <summary>
/// Handles transition to the special stage and the Big Ring
/// </summary>
public class BigRingController : HitBoxContactEvent
{
    [SerializeField]
    private MeshRenderer model;
    [SerializeField, Tooltip("Determines whether the ring is disabled which also affects the material of the ring")]
    private bool alreadyUsed = false;
    [FirstFoldOutItem("Dependencies"), SerializeField]
    private Animator animator;
    [LastFoldoutItem(), SerializeField]
    private BoxCollider2D boxCollider2D;

    [FirstFoldOutItem("Appear Settings"), SerializeField, Tooltip("The speed which the Big Ring appears")]
    private float appearSpeed = 1;
    [SerializeField, Tooltip("The scale the Big Ring is set to when spawned")]
    private Vector2 startScale = new Vector2(0, 0);
    [LastFoldoutItem(), SerializeField, Tooltip("The scale the Big Ring move towards when visible by the camera")]
    private Vector2 endScale = new Vector2(36f, 36f);

    [FirstFoldOutItem("Model Materials"), SerializeField, Tooltip("The material when the gold ring is active")]
    private Material activeModelMaterial;
    [LastFoldoutItem(), SerializeField, Tooltip("The material used when the model is disabled")]
    private Material disabledModelMaterial;

    [FirstFoldOutItem("General Big Ring Information"), Tooltip("The offsets for the ring sparkles"), SerializeField]
    private float bigRingSparkleOffset = 16;
    [SerializeField, Tooltip("Audio played when the player touches the big ring"), LastFoldoutItem()]
    private AudioClip collectedSound;
    private IEnumerator growRingCoroutine;

    public override void Reset()
    {
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.animator = this.GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        if (this.boxCollider2D == null)
        {
            this.Reset();
        }

        this.alreadyUsed = GMSaveSystem.Instance().GetCurrentPlayerData().CheckIfSpecialDataObjectExists(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId(), this.transform.position);

        //If we have more than 7 emeralds and the last emerald is not the current big ring deactivate
        bool existsInHistory = GMHistoryManager.Instance().GetRegularStageScoreHistory().CheckIfSpawnPointExists(SpawnPointType.SpecialStage, this.transform.position);

        if ((GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds() >= 7) && !existsInHistory)
        {
            //Allows us to test big ring while in debug mode and spawn big rings when we have more than 7 emeralds via the debugger
            if (!(GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetDebugMode() && GMStageManager.Instance().GetStageState() != RegularStageState.Idle))
            {
                this.gameObject.SetActive(false);
                this.boxCollider2D.enabled = false;
            }
        }

        this.model.material = this.alreadyUsed == false ? this.activeModelMaterial : this.disabledModelMaterial;
        this.boxCollider2D.enabled = !this.alreadyUsed;
        this.model.transform.parent.localScale = new Vector3(this.startScale.x, this.startScale.y, 1);
        this.growRingCoroutine = null;

        GMHistoryManager.Instance().GetRegularStageScoreHistory().RemoveSpawnPoint(SpawnPointType.SpecialStage, this.transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        if (HedgehogCamera.Instance().AreBoundsWithinCameraView(this.model.bounds) && this.growRingCoroutine == null)
        {
            this.growRingCoroutine = this.GrowRing();
            this.StartCoroutine(this.growRingCoroutine);
        }
    }

    /// <summary>
    /// If the player comes in contact with a Big Ring immedietly activate it 
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerContact = false;

        this.alreadyUsed = GMSaveSystem.Instance().GetCurrentPlayerData().CheckIfSpecialDataObjectExists(GMSceneManager.Instance().GetCurrentSceneData().GetSceneId(), this.transform.position);

        if (this.alreadyUsed)
        {
            triggerContact = false;
        }
        else
        {
            triggerContact = true;
        }

        return triggerContact;
    }

    /// <summary>
    /// Disable the player and spawn the requred items
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        this.BigRingTouched(player);
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BigRingFlash, this.transform.position);
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position);
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position + new Vector3(Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset), Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset)));
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position + new Vector3(Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset), Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset)));
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position + new Vector3(Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset), Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset)));
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.RingSparkle, this.transform.position + new Vector3(Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset), Random.Range(-this.bigRingSparkleOffset, this.bigRingSparkleOffset)));
        this.gameObject.SetActive(false);//Deactivate instead of destroy
        this.alreadyUsed = true;
        GMAudioManager.Instance().PlayOneShot(this.collectedSound);
    }

    /// <summary>
    /// The set of actions to be performed once the Big Ring is touched
    /// </summary>
    private void BigRingTouched(Player player)
    {
        if (GMSaveSystem.Instance().GetCurrentPlayerData().GetChaosEmeralds() >= 7)
        {
            GMRegularStageScoreManager.Instance().IncrementRingCount(50);

            return;
        }

        HedgehogCamera.Instance().SetCameraMode(CameraMode.Freeze);
        GMHistoryManager.Instance().GetRegularStageScoreHistory().AddSpawnPoint(SpawnPointType.SpecialStage, this.transform.position);//set the position the player goes back to after
        GMHistoryManager.Instance().SetActiveShieldType(player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType());
        GMSaveSystem.Instance().GetCurrentPlayerData().AddSpecialStageDataPosition(this.transform.position);
        GMSaveSystem.Instance().GetCurrentPlayerData().SetCharacter(GMCharacterManager.Instance().currentCharacter);
        player.GetHedgePowerUpManager().GetInvincibilityPowerUp().SetPowerUpVisbility(false);
        player.SetPlayerState(PlayerState.Sleep);
        this.Invoke(nameof(this.WarpToSpecialStage), this.collectedSound.length);
    }

    /// <summary>
    /// Warp the player to the special stage
    /// </summary>
    private void WarpToSpecialStage()
    {
        GMSpecialStageManager.Instance().LoadSpecialStage();
        GMSaveSystem.Instance().GetCurrentPlayerData().SetCharacter(GMCharacterManager.Instance().currentCharacter);
        GMSaveSystem.Instance().GetCurrentPlayerData().SerializeRegularStageData(GMStageManager.Instance().GetPlayer());
        GMSaveSystem.Instance().SaveAndOverwriteData();
        GMHistoryManager.Instance().GetRegularStageScoreHistory().SaveStageHistory();
    }

    /// <summary>
    /// Increases the Big Rings scale towards its end scale
    /// </summary>
    private IEnumerator GrowRing()
    {
        while ((Vector2)this.model.transform.parent.localScale != this.endScale)
        {
            Vector2 currentScale = this.model.transform.parent.localScale;
            currentScale = Vector2.MoveTowards(currentScale, this.endScale, Time.deltaTime * GMStageManager.Instance().GetPhysicsMultiplier() * this.appearSpeed);
            this.model.transform.parent.localScale = new Vector3(currentScale.x, currentScale.y, 1);

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }
}
