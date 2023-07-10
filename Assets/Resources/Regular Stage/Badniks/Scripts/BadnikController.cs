using UnityEngine;
/// <summary>
/// A parent class for all the enemeis
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class BadnikController : HitBoxContactEvent
{
    [Tooltip("The current velocity of the badnik"), FirstFoldOutItem("Generic Badnik Info")]
    public Vector2 velocity;
    [Tooltip("The current direction of the badnik")]
    public int currentDirection = 1;
    [LastFoldoutItem(), Tooltip("The audio played when an badnik is destroyed")]
    public AudioClip destroyBadnikSound;
    [FirstFoldOutItem("Badnik Health")]
    public int healthPoints = 1;
    [Tooltip("The current badnik state of the badnik"), LastFoldoutItem]
    public BadnikState badnikState = BadnikState.Vulnerable;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        if (this.spriteRenderer == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Usually used to set defaults
    /// </summary
    public override void Reset()
    {
        if (this.GetComponent<Rigidbody2D>() != null)
        {
            this.GetComponent<Rigidbody2D>().isKinematic = true;
            this.gameObject.layer = LayerMask.NameToLayer("Hitbox Collision Layer");
        }

        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Performs basic confirmation to check if the Badnik damaged the player Or is Damaged
    /// <param name="player">The player object to come in contact with the boss  </param>
    /// </summary
    public virtual bool CheckIfHitPlayer(Player player)
    {
        //If the player is vulnerable and not attacking take a hit
        if (player.GetHealthManager().GetHealthStatus() == HealthStatus.Vulnerable && player.GetAttackingState() == false)
        {
            return true;
        }
        else if (this.badnikState == BadnikState.Vulnerable && (player.GetAttackingState() || player.GetHealthManager().GetHealthStatus() == HealthStatus.Invincible))
        {
            if (player.GetGrounded() == false)
            {
                player.AttackRebound();
            }

            this.TakeDamage();

            return false;
        }

        return false;
    }

    /// <summary>
    /// If the player comes in contact with the badnik trigger its perform action
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
    /// Choose to destroy the badnik if the player attacked it or is invincible or harm the player if they are vulnerable
    /// <param name="player">The player object</param>
    /// </summary>
    public override void HedgeOnCollisionEnter(Player player)
    {
        base.HedgeOnCollisionEnter(player);
        if (player.GetHealthManager().GetHealthStatus() == HealthStatus.Vulnerable && player.GetAttackingState() == false)
        {
            player.GetHealthManager().VerifyHit(this.transform.position.x);//Verify the hit
        }
        else if (this.badnikState == BadnikState.Vulnerable && (player.GetAttackingState() || player.GetHealthManager().GetHealthStatus() == HealthStatus.Invincible))
        {
            if (player.GetGrounded() == false)
            {
                player.AttackRebound();
            }
            this.TakeDamage();//Badnik has been hit
        }
    }

    /// <summary>
    /// Actions performed when a badnik interacts with a secondaryhitbox object
    /// </summary
    public override void SecondaryHitBoxObjectAction(SecondaryHitBoxController secondaryHitBoxController)
    {
        if (this.badnikState == BadnikState.Vulnerable)
        {
            this.TakeDamage();//Badnik has been hit
        }
    }

    /// <summary>
    /// Actions performed when a player takes damage
    /// </summary
    public void TakeDamage()
    {
        this.healthPoints--;

        if (this.healthPoints <= 0)
        {
            this.OnBadnikDestruction();
        }
    }

    /// <summary>
    /// Actions performed when the badnik is finally destroyed
    /// </summary
    public virtual void OnBadnikDestruction()
    {
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BadnikExplosion, this.transform.position);
        GMRegularStageScoreManager.Instance().IncrementCombo(this.transform);//Incremenet the active combo
        GMAudioManager.Instance().PlayOneShot(this.destroyBadnikSound);
        GameObject animal = GMSpawnManager.Instance().SpawnRandomAnimal(this.transform.position);//Spawn an animal

        if (this.spriteRenderer != null) //Place the animal on the same layer as the sprite renderer
        {
            animal.GetComponent<SpriteRenderer>().sortingLayerID = this.spriteRenderer.sortingLayerID;
        }

        this.gameObject.SetActive(false);
    }
}
