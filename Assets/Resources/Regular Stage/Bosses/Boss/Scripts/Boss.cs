using System.Collections;
using UnityEngine;
/// <summary>
/// A parent class for all the bosses
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Boss : HitBoxContactEvent
{
    public Material bossMaterial;
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;
    [Tooltip("The current velocity of the Boss"), FirstFoldOutItem("Generic Boss Info"), SerializeField]
    protected Vector2 velocity;
    [Tooltip("The current phase of the boss phase"), SerializeField]
    protected BossPhase bossPhase = BossPhase.Idle;
    [Tooltip("The current Boss health status of the Boss")]
    protected HealthStatus bossHealthStatus = HealthStatus.Invulnerable;
    [Tooltip("The trigger that activated the boss"), SerializeField]
    protected BossTrigger bossTrigger;
    [Tooltip("The current direction of the Boss"), SerializeField]
    protected int currentDirection = 1;
    [Tooltip("The audio source off the boss explosion"), SerializeField]
    protected AudioClip bossExplosionSound;
    [LastFoldoutItem(), Tooltip("The audio played when the Boss is hit"), SerializeField]
    protected AudioClip bossHitSound;
    [FirstFoldOutItem("Boss Health"), SerializeField]
    protected int healthPoints = 7;
    [FirstFoldOutItem("Invincibility Timer"), SerializeField]
    protected float invincibilityDuration = 30;
    [Tooltip("How many steps to update the material"), SerializeField, LastFoldoutItem()]
    protected float invertColorUpdateStep = 4f;
    private Color debugColor = General.RGBToColour(255, 77, 84, 170);

    private void Awake() => this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    protected override void Start()
    {
        base.Start();

        if (this.spriteRenderer == null)
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }

        GMStageManager.Instance().SetBoss(this);//tell the stage manager GM that there is a boss in this stage
        this.boxCollider2D = this.GetComponent<BoxCollider2D>();
        this.bossHealthStatus = HealthStatus.Invulnerable;
        this.bossMaterial.SetFloat("_Threshold", 0);
    }

    /// <summary>
    /// Usually used to set defaults
    /// </summary/>
    public override void Reset()
    {
        if (this.GetComponent<Rigidbody2D>() != null)
        {
            this.GetComponent<Rigidbody2D>().isKinematic = true;
        }

        this.gameObject.layer = LayerMask.NameToLayer("Hitbox Collision Layer");
    }

    /// <summary>
    /// Performs basic confirmation to check if the Boss damaged the player Or is Damaged
    /// <param name="player">The player object to come in contact wit the boss </param>
    /// </summary/>
    public virtual bool CheckIfHitPlayer(Player player)
    {
        //If the player is vulnerable and not attacking take a hit
        if (player.GetHealthManager().GetHealthStatus() == HealthStatus.Vulnerable && player.GetAttackingState() == false)
        {
            return true; //Player has been hit
        }
        else if (this.bossHealthStatus == HealthStatus.Vulnerable && (player.GetAttackingState() || player.GetHealthManager().GetHealthStatus() == HealthStatus.Invincible))
        {
            if (player.GetGrounded() == false)
            {
                player.AttackRebound();
            }

            this.TakeDamage();//Boss has been hit

            return false;
        }

        return false;
    }

    /// <summary>
    /// If the player comes in contact with the Boss trigger its perform action
    /// <param name="player">The player object to check against  </param>
    /// <param name="solidBoxColliderBounds">The players solid box colliders bounds  </param>
    /// </summary>
    public override bool HedgeIsCollisionValid(Player player, Bounds solidBoxColliderBounds)
    {
        bool triggerContact = false;
        triggerContact = true;

        return triggerContact;
    }

    /// <summary>
    /// Choose to destroy the Boss if the player attacked it or is invincible or harm the player if they are vulnerable
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);

        if (player.GetHealthManager().GetHealthStatus() == HealthStatus.Vulnerable && player.GetAttackingState() == false)
        {
            if (player.GetHealthManager().VerifyHit(this.transform.position.x))
            {
                this.OnHitPlayer();
            }
        }
        else if (this.bossHealthStatus == HealthStatus.Vulnerable && (player.GetAttackingState() || player.GetHealthManager().GetHealthStatus() == HealthStatus.Invincible))
        {
            player.velocity.x *= -1;
            player.velocity.y *= -1;

            this.TakeDamage();
        }
    }

    /// <summary>
    /// Actions performed when the enemy hits the player
    /// </summary/>
    public virtual void OnHitPlayer() { }

    /// <summary>
    /// Actions performed when a player takes damage
    /// </summary/>
    public virtual void TakeDamage()
    {
        GMAudioManager.Instance().PlayOneShot(this.bossHitSound);
        this.healthPoints--;

        if (this.healthPoints <= 0)
        {
            this.OnBossDestruction();
        }
        else
        {
            this.StartCoroutine(this.HurtFlash());
        }
    }

    /// <summary>
    /// Set the active boss phase
    /// <param name="bossPhase">The new boss phase for the active boss</param>
    /// </summary>
    public void SetCurrentBossPhase(BossPhase bossPhase) => this.bossPhase = bossPhase;

    /// <summary>
    /// Get the current boss phase
    /// </summary>
    public BossPhase GetBossPhase() => this.bossPhase;

    /// <summary>
    /// Set the boss invincible for the set amount of time beofre making the boss vulnerable again while flashing
    /// </summary/>
    private IEnumerator HurtFlash()
    {
        this.bossHealthStatus = HealthStatus.Invulnerable;

        for (int x = 0; x < this.invincibilityDuration / this.invertColorUpdateStep; x++)
        {
            if (this.bossMaterial.GetFloat("_Threshold") != 0)
            {
                this.bossMaterial.SetFloat("_Threshold", 0);
            }
            else
            {
                this.bossMaterial.SetFloat("_Threshold", 1);
            }

            yield return new WaitForSeconds(General.StepsToSeconds(this.invertColorUpdateStep));
        }

        this.bossMaterial.SetFloat("_Threshold", 0);
        this.bossHealthStatus = HealthStatus.Vulnerable;
    }

    /// <summary>
    /// Determines whether the boss is defeated or not
    /// </summary/>
    public bool IsDefeated() => this.healthPoints <= 0 && this.bossPhase == BossPhase.Exit;

    /// <summary>
    /// Actions performed when the boss is activated
    /// </summary/>
    public virtual void OnActivation(BossTrigger bossTrigger)
    {
        this.bossTrigger = bossTrigger;
        this.bossHealthStatus = HealthStatus.Vulnerable;
        GMStageManager.Instance().SetStageState(RegularStageState.FightingBoss);
        GMAudioManager.Instance().PlayBGM(BGMToPlay.BossTheme);//Play boss theme
        GMAudioManager.Instance().StopBGM(BGMToPlay.MainBGM);//Stop the main stage theme
    }

    /// <summary>
    /// Actions performed when the boss is destroyed
    /// </summary/>
    public virtual void OnBossDestruction()
    {
        this.boxCollider2D.enabled = false;
        GMAudioManager.Instance().PlayOneShot(this.bossHitSound);
        this.bossHealthStatus = HealthStatus.Death;
    }

    /// <summary>
    /// Actions performed when the explosions spanwed have ended
    /// </summary/>
    public virtual void OnExplosionsEnd() { }

    /// <summary>
    /// Actions performed when the boss fight is over
    /// </summary/>
    public virtual void OnBossFightEnd()
    {
        HedgehogCamera.Instance().SetCameraMode(CameraMode.FollowTarget);
        GMAudioManager.Instance().ClearQueue();
        GMAudioManager.Instance().StopBGM(BGMToPlay.BossTheme);
        GMAudioManager.Instance().UnMuteBGM(BGMToPlay.MainBGM);
        GMAudioManager.Instance().PlayBGM(BGMToPlay.MainBGM);//Restore the main BGM
        GMStageManager.Instance().GetPlayer().GetHedgePowerUpManager().RevertSuperForm(); //Revert the player after being defeated
        GMStageManager.Instance().SetStageState(RegularStageState.Running);
        this.bossTrigger.gameObject.SetActive(false);//Disable the boss trigger for clean up
    }

    /// <summary>
    /// Get the boss trigger
    /// </summary/>
    public BossTrigger GetBossTrigger() => this.bossTrigger;

    /// <summary>
    /// Set the boss trigger
    /// </summary/>
    public void SetBossTrigger(BossTrigger bossTrigger) => this.bossTrigger = bossTrigger;

    protected virtual void OnDrawGizmos()
    {
        if(this.bossHealthStatus == HealthStatus.Vulnerable)
        {
            GizmosExtra.DrawRect(this.transform, this.boxCollider2D, this.debugColor, true);
        }
    }
}
