using UnityEngine;
/// <summary>
/// A class that manges the players health at any given time
/// </summary
public class HealthManager : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField, LastFoldoutItem()]
    private Hurt hurt;

    [Tooltip("The current health status of the player"), SerializeField]
    private HealthStatus healthStatus = HealthStatus.Vulnerable;
    [SerializeField]
    [Help("Time to wait before rings are collected after getting hit in Steps")]
    private float ringCollectionLockOutTimeInSteps = 64;
    [SerializeField]
    private float timeBeforeRingsCanBeCollected = 0;
    [SerializeField]
    [Tooltip("The number of rings to spawn")]
    private float numberOfRingsToSpawn = 0;
    [SerializeField]
    [Tooltip("The maximum number of rings that can be spawned at any given instance")]
    private float maxRingCount = 32;
    [SerializeField, Tooltip("The base speed of a spilled ring")]
    private float spilledRingSpeed = 4;

    private void Reset()
    {
        this.player = this.GetComponent<Player>();
        this.hurt = this.player.GetComponentInChildren<Hurt>();
    }

    private void Start()
    {
        if (this.player == null)
        {
            this.Reset();
        }
    }

    private void Update()
    {
        if (this.healthStatus == HealthStatus.Death)
        {
            return;
        }

        this.UpdateCollectRingsTimer();
        //If the player is super they are always invincible no matter the conditions except dearh
        if (this.player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm && this.healthStatus != HealthStatus.Death)
        {
            this.healthStatus = HealthStatus.Invincible;
        }
    }

    /// <summary>
    /// Updates the ring collection timer which determines whether rings can be picked up
    /// </summary>
    private void UpdateCollectRingsTimer()
    {
        if (this.timeBeforeRingsCanBeCollected > 0)
        {
            this.timeBeforeRingsCanBeCollected -= Time.deltaTime;
        }

        this.timeBeforeRingsCanBeCollected = Mathf.Clamp(this.timeBeforeRingsCanBeCollected, 0, General.StepsToSeconds(this.ringCollectionLockOutTimeInSteps));
    }

    /// <summary>
    /// Checks if the player can collect rings based on the timer
    /// </summary>
    public bool CanCollectRings() => this.timeBeforeRingsCanBeCollected == 0;
    /// <summary>
    /// Set the health status of the player
    /// <param name="healthStatus"> the new health status of the player</param>
    /// </summary>
    public void SetHealthStatus(HealthStatus healthStatus)
    {
        //If the player is super they are always invincible no matter the conditions except dearh
        if (this.player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm && healthStatus != HealthStatus.Death)
        {
            this.healthStatus = HealthStatus.Invincible;
            return;
        }

        this.healthStatus = healthStatus;
    }

    /// <summary>
    /// Get the health status of the player
    /// </summary>
    public HealthStatus GetHealthStatus() => this.healthStatus;
    /// <summary>
    /// Sets the amount of rings to spawn when spill rings is about to be called
    /// <param name="numberOfRingsToSpawn">The to be set amount of rings to be spawned on the next hit</param>
    /// </summary
    public void SetAmountOfRingsToSpawn(float numberOfRingsToSpawn)
    {
        this.numberOfRingsToSpawn = numberOfRingsToSpawn;

        if (this.numberOfRingsToSpawn > this.maxRingCount)
        {
            this.numberOfRingsToSpawn = this.maxRingCount;
        }
    }

    /// <summary>
    /// Performs the spill action based on the number of rings set to spawn
    /// </summary
    public void SpillRings()
    {
        this.timeBeforeRingsCanBeCollected = General.StepsToSeconds(this.ringCollectionLockOutTimeInSteps);
        bool flipSpawnDirection = false;
        float currentRingAngle = 101.25f;
        float ringSpeed = this.spilledRingSpeed;

        for (int i = 0; i < this.numberOfRingsToSpawn; i++)
        {
            //Instantiate a ring each loop
            GameObject ring = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.SpilledRing, this.transform.position);
            RingController ringController = ring.GetComponent<RingController>();
            ringController.SetVelocity(General.AngleToVector(currentRingAngle * Mathf.Deg2Rad) * ringSpeed);
            //When the rings spawned = to change the angle and speed
            if (i == 16)
            {
                currentRingAngle = 101.25f;
                ringSpeed /= 2;
            }
            if (flipSpawnDirection)
            {
                ringController.SetVelocity(General.AngleToVector(currentRingAngle * Mathf.Deg2Rad) * ringSpeed * new Vector2(-1, 1));
                currentRingAngle += 22.5f;
            }
            flipSpawnDirection = !flipSpawnDirection;
        }
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().ringLoss);
    }

    /// <summary>
    /// Verify the players hit based on their current state and ring count
    /// <param name="hazardXPosition">The X position of the hazard</param>
    /// </summary>
    public bool VerifyHit(float hazardXPosition)
    {
        bool hitStatus = false;

        if (this.healthStatus == HealthStatus.Vulnerable)
        {
            if (this.player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType() == ShieldType.None)
            {
                if (GMRegularStageScoreManager.Instance().GetRingCount() <= 0) //Kill The player
                {
                    this.KillPlayer();
                    hitStatus = true;

                    return hitStatus;
                }
                this.SetAmountOfRingsToSpawn(GMRegularStageScoreManager.Instance().GetRingCount());//If it passess the max limit it is  
                this.SpillRings();
                GMRegularStageScoreManager.Instance().SetRingCount(0);//Set the ring count to zero
            }
            else
            {
                this.player.GetHedgePowerUpManager().GetShieldPowerUp().RemovePowerUp();
            }
            this.SetHealthStatus(HealthStatus.Invulnerable);//set the player to invulnerable so they cant be hit multiple times
            this.hurt.SetHurtFallDirection((int)Mathf.Sign(this.transform.position.x - hazardXPosition));
            this.player.GetActionManager().PerformAction<Hurt>();
            hitStatus = true;

        }

        return hitStatus;
    }

    /// <summary>
    /// Checks if the player is below the zone bounds
    /// </summary>
    public void CheckForBelowBoundaryDeath()
    {
        if (this.player.GetSpriteController().GetSpriteRenderer().bounds.max.y < HedgehogCamera.Instance().GetCameraBoundsHandler().GetDeathPlane() && HedgehogCamera.Instance().GetBounds().min.y <= HedgehogCamera.Instance().GetCameraBoundsHandler().GetDeathPlane() && this.GetHealthStatus() != HealthStatus.Death)
        {

            if (this.player.GetHedgePowerUpManager().GetSuperPowerUp() == SuperPowerUp.SuperForm)
            {
                this.player.GetHedgePowerUpManager().RevertSuperForm();
            }

            this.KillPlayer();
        }
    }

    /// <summary>
    /// Actually Kills the player
    /// </summary>
    public void KillPlayer()
    {
        this.transform.eulerAngles = Vector3.zero;
        this.player.GetSpriteController().SetSpriteAngle(0);
        this.SetHealthStatus(HealthStatus.Death);
        this.player.GetAnimatorManager().SwitchGimmickSubstate(0);
        this.player.SetMovementRestriction(MovementRestriction.None);
        this.player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        this.player.GetActionManager().EndCurrentAction();
        this.player.GetActionManager().PerformAction<Die>();
        this.player.SetPhysicsState(PhysicsState.Basic);
        this.player.StartDeathUpdate();
    }

}
