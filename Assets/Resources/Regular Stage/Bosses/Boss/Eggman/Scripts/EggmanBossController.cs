using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A simple eggman boss to serve as a base template for other bosses
/// Author - Nihil - Core Framework
/// </summary>
public class EggmanBossController : Boss
{
    [Tooltip("The animator of the eggmans"), FirstFoldOutItem("Part Animators")]
    public Animator eggmanBodyAnimator;
    [LastFoldoutItem()]
    public Animator eggmanPodFlameAnimator;

    [Tooltip("The current attack phase for the boss"), FirstFoldOutItem("General Info")]
    private EggmanBossAttackPhase attackPhase = EggmanBossAttackPhase.Phase1;
    [Tooltip("The acceleration of the boss")]
    private float acceleration = 1f;
    [Tooltip("The delay the boss has when turning around"), SerializeField]
    private float turningDelay = 35f;
    [Tooltip("The current delta of the boss used when hovering"), SerializeField, LastFoldoutItem()]
    private float hoverDelta = 0;

    [Tooltip("The speed of the boss when the boss is defeated"), SerializeField, FirstFoldOutItem("End Movement Info")]
    private Vector2 endAcceleration = new Vector2(4, 1);
    [LastFoldoutItem()]
    public float actionTimer = 0;

    [Tooltip("The left most point the boss can reach"), SerializeField, FirstFoldOutItem("Boundaries")]
    private Vector2 leftBoundary;
    [Tooltip("The right most point the boss can reach"), SerializeField, LastFoldoutItem()]
    private Vector2 rightBoundary;

    [Tooltip("The list of Spikes to cycle through"), SerializeField, FirstFoldOutItem("Cycle Spikes")]
    private List<MovingPlatformController> cycleSpikes;
    [Tooltip("The speed it takes for the Spike to fully appear "), SerializeField]
    private float cycleSpikeSpeed = 20;
    [Tooltip("The size of the Spike radius"), SerializeField, LastFoldoutItem()]
    private float cycleSpikeRadius = 50;

    [FirstFoldOutItem("Debug parameters"), SerializeField, LastFoldoutItem()]
    private Color rangeDebugColor = Color.red;

    private void FixedUpdate()
    {
        switch (this.bossPhase)
        {
            case BossPhase.Entry:
                this.BossEntryMovement(this.transform.position);

                break;
            case BossPhase.Attacking:
                if (this.attackPhase == EggmanBossAttackPhase.Phase1)
                {
                    this.Phase1Movement(this.transform.position);
                    if (this.healthPoints <= 3)
                    {
                        this.attackPhase = EggmanBossAttackPhase.Phase2;
                        this.cycleSpikes[0].transform.parent.gameObject.SetActive(true);
                        this.StartCoroutine(this.GrowSpikes());
                    }
                }
                else if (this.attackPhase == EggmanBossAttackPhase.Phase2)
                {
                    this.Phase2Movement(this.transform.position);
                }
                else if (this.attackPhase == EggmanBossAttackPhase.Turning)
                {
                    this.velocity = Vector2.zero;
                }
                break;
            case BossPhase.Exploding:
                this.ExplosionMovement();

                break;
            case BossPhase.Exit:
                this.ExitMovement();

                break;
            case BossPhase.Idle:
                break;
            default:
                break;
        }

        this.Move(this.velocity);
        this.eggmanPodFlameAnimator.SetBool("Moving", Mathf.Abs(this.velocity.x) > 0);
        this.HoverMovement(this.transform.position);
    }

    /// <summary>
    /// Moves the boss in the direction of its current velocity
    /// <param name="velocity">The boss current velocity</param>
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime * new Vector3(velocity.x, velocity.y, 0f);//Move the boss by the current velocity

    /// <summary>
    /// Moves the boss upwards and downards continually while moving
    /// <param name="position">The current position of the boss</param>
    /// </summary>
    private void HoverMovement(Vector2 position)
    {
        position.y -= Mathf.Sin(this.hoverDelta * 6) * 0.3f;
        this.transform.position = position;
        this.hoverDelta += Time.deltaTime;
    }

    /// <summary>
    /// The actions that take place when the player has activated the trigger
    /// <param name="bossTrigger">The trigger/bounds that activated the boss fight</param>
    /// </summary>
    public override void OnActivation(BossTrigger bossTrigger)
    {
        base.OnActivation(bossTrigger);
        GMStageManager.Instance().SetBoss(this);
        this.SetCurrentBossPhase(BossPhase.Entry);
        this.currentDirection = -1;
        this.transform.localScale = new Vector3(this.currentDirection, 1, 1);
    }

    /// <summary>
    /// The movement of the boss when entering the scene
    /// <param name="position">The current position of the boss</param>
    /// </summary>
    private void BossEntryMovement(Vector2 position)
    {
        if (position.y <= this.rightBoundary.y + this.startPosition.y)
        {
            if (this.velocity.y != 0 && this.velocity.x == 0)
            {
                this.eggmanBodyAnimator.SetTrigger("HitPlayer");
            }
            this.velocity.y = 0;
            this.velocity.x = this.acceleration * this.currentDirection;
        }
        else
        {
            this.velocity.y = -this.acceleration;
        }

        if (position.x <= this.bossTrigger.GetBossFightBounds().GetCenterPosition().x)
        {
            this.SetCurrentBossPhase(BossPhase.Attacking);
            this.currentDirection = -1;
        }
    }

    /// <summary>
    /// The movement of the boss when phase 1 is active
    /// <param name="position">The current position of the boss</param>
    /// </summary>
    private void Phase1Movement(Vector2 position)
    {
        if (this.transform.position.x < this.leftBoundary.x + this.startPosition.x && this.currentDirection == -1)
        {
            this.Flip();
        }
        else if (this.transform.position.x > this.rightBoundary.x + this.startPosition.x && this.currentDirection == 1)
        {
            this.Flip();
        }

        this.velocity.x = this.acceleration * this.currentDirection;
    }

    /// <summary>
    /// The movement of the boss when phase 2 is active
    /// <param name="position">The current position of the boss</param>
    /// </summary>
    private void Phase2Movement(Vector2 position) => this.Phase1Movement(position);

    /// <summary>
    /// The movement off eggman while fleeing the scene
    /// </summary>
    private void ExitMovement()
    {
        if (this.actionTimer is < 120 and > 90)
        {
            this.velocity.y = -3;
        }
        else if (this.actionTimer is < 90 and > 70)
        {
            this.velocity.y = -0.5f;
        }
        else if (this.actionTimer is < 70 and > 40)
        {
            this.velocity.y = 2;
        }
        else if (this.actionTimer < 30)
        {
            this.velocity.y = 0.2f;
            this.velocity.x = this.endAcceleration.x;
            this.endAcceleration.x += 0.05f;
            if (this.actionTimer == 1)
            {
                this.OnBossFightEnd();//Restore the camera view port
            }
        }

        //If the boss exceeds the right zone view and is no longer visible, disable it
        if (this.transform.position.x > HedgehogCamera.Instance().GetCameraBoundsHandler().GetActBounds().GetRightBorderPosition() + 90)
        {
            this.gameObject.SetActive(false);
        }

        this.actionTimer--;
    }

    /// <summary>
    /// The way the boss moves while exploding
    /// <param name="bossTrigger">The trigger/bounds that activated the boss fight</param>
    /// </summary>
    private void ExplosionMovement()
    {
        this.actionTimer--;

        if (this.actionTimer == 120)
        {
            this.SetCurrentBossPhase(BossPhase.Exit);
        }
    }

    /// <summary>
    /// Prepares to flip the boss object
    /// </summary>
    private void Flip() => this.StartCoroutine(this.WaitAndFlip());

    /// <summary>
    /// Waits the specified amount of time before flipping the boss sprite
    /// </summary>
    private IEnumerator WaitAndFlip()
    {
        EggmanBossAttackPhase previousPhase = this.attackPhase;
        this.attackPhase = EggmanBossAttackPhase.Turning;

        yield return new WaitForSeconds(General.StepsToSeconds(this.turningDelay));

        yield return new WaitForEndOfFrame();

        this.currentDirection = this.currentDirection == 1 ? -1 : 1;
        this.transform.localScale = new Vector3(this.currentDirection, 1, 1);
        this.attackPhase = previousPhase;
    }

    public override void OnHitPlayer()
    {
        base.OnHitPlayer();
        this.eggmanBodyAnimator.SetTrigger("HitPlayer");
    }

    public override void TakeDamage()
    {
        base.TakeDamage();
        this.eggmanBodyAnimator.SetTrigger("GotHit");
    }

    /// <summary>
    /// Perform the basic boss destruction actions and prepare the spawn of explosions
    /// </summary>
    public override void OnBossDestruction()
    {
        base.OnBossDestruction();
        this.actionTimer = 240;
        this.StartCoroutine(this.SpawnBossExplosions());
        this.SetCurrentBossPhase(BossPhase.Exploding);
        this.acceleration = 0;
        this.velocity = Vector2.zero;
    }

    /// <summary>
    /// Spawn explosions randomly around the object
    /// </summary/>
    private IEnumerator SpawnBossExplosions()
    {
        this.eggmanBodyAnimator.SetBool("Exploding", true);

        foreach (MovingPlatformController movingPlatformController in this.cycleSpikes)
        {
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BossExplosion, movingPlatformController.transform.position);
            movingPlatformController.gameObject.SetActive(false);
        }
        while (this.actionTimer > 120)
        {
            //Spawn random explosions around the boss
            this.SpawnExplosion();
            GMAudioManager.Instance().PlayOneShot(this.bossExplosionSound);

            yield return new WaitForSeconds(General.StepsToSeconds(6));
        }

        this.OnExplosionsEnd();
        this.eggmanBodyAnimator.SetBool("Exploding", false);
        this.currentDirection = 1;
        this.transform.localScale = new Vector3(this.currentDirection, 1, 1);
    }

    /// <summary>
    /// Spawns a random explosion around the boss object
    /// </summary/>
    private void SpawnExplosion()
    {
        Vector2 spawnPosition;
        spawnPosition.x = this.transform.position.x + Random.Range(-32, 32);
        spawnPosition.y = this.transform.position.y - Random.Range(-32, 32);
        GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BossExplosion, spawnPosition);
    }

    /// <summary>
    /// Grows the Spikes that circle the boss
    /// </summary/>
    private IEnumerator GrowSpikes()
    {
        while (true)
        {
            foreach (MovingPlatformController cycleSpike in this.cycleSpikes)
            {
                cycleSpike.SetRange(Mathf.MoveTowards(cycleSpike.GetRange(), this.cycleSpikeRadius, this.cycleSpikeSpeed * Time.deltaTime));
            }

            if (this.cycleSpikes[this.cycleSpikes.Count - 1].GetRange() == this.cycleSpikeRadius)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Vector2 debugPosition = Application.isPlaying ? (Vector3)this.startPosition : this.transform.position;
        Vector2 pos1 = debugPosition + this.leftBoundary;
        Vector2 pos2 = debugPosition + this.rightBoundary;
        Gizmos.color = this.rangeDebugColor;
        Gizmos.DrawLine(pos1, pos2);
        GizmosExtra.Draw2DArrow(pos1, 90);
        GizmosExtra.Draw2DArrow(pos2, 270);
    }
}
