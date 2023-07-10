using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuasarLakeBossController : Boss
{
    [Tooltip("The animator of the eggmans"), FirstFoldOutItem("Part Animators"), SerializeField]
    private Animator eggmanBodyAnimator;
    [LastFoldoutItem(), SerializeField]
    private Animator eggmanPodFlameAnimator;

    [Tooltip("The current attack phase for the boss"), FirstFoldOutItem("General Info")]
    private QuasarLakeBossAttackPhase attackPhase = QuasarLakeBossAttackPhase.Phase1;
    [Tooltip("The acceleration of the boss")]
    private float acceleration = 1f;
    [Tooltip("The delay the boss has when turning around"), SerializeField]
    private float turningDelay = 35f;
    [Tooltip("The current delta of the boss used when hovering"), SerializeField, LastFoldoutItem()]
    private float hoverDelta = 0;

    [Tooltip("The speed of the boss when the boss is defeated"), SerializeField, FirstFoldOutItem("End Movement Info")]
    private Vector2 endAcceleration = new Vector2(4, 1);
    [LastFoldoutItem(), SerializeField]
    private float actionTimer = 0;

    [Tooltip("The left most point the boss can reach"), SerializeField, FirstFoldOutItem("Boundaries")]
    private Vector2 leftBoundary;
    [Tooltip("The right most point the boss can reach"), SerializeField, LastFoldoutItem()]
    private Vector2 rightBoundary;

    [SerializeField]
    private GameObject bossSpritesPivot;

    [SerializeField, FirstFoldOutItem("Pseudo Spike Info")]
    private float pseudoSpikeShowSpeed = 60f;
    [SerializeField]
    private Vector2 pseudoSpikeShowPositionOffset = new Vector2(0, 16f);
    [SerializeField]
    private Pseudo3DBallController horizontalPseudo3DSpike;
    [SerializeField]
    private Pseudo3DBallController verticalPseudo3DSpike;
    [SerializeField]
    private List<InitializedObjectData> pseudo3DHorizontalSpikeObjectData;
    [SerializeField, LastFoldoutItem()]
    private List<InitializedObjectData> pseudo3DVerticalSpikeObjectData;

    [FirstFoldOutItem("Debug parameters"), SerializeField, LastFoldoutItem()]
    private Color boundaryDebugColor = Color.red;

    protected override void Start()
    {
        base.Start();

        this.pseudo3DHorizontalSpikeObjectData = this.GetSpikeControllerInfo(this.horizontalPseudo3DSpike);
        this.pseudo3DVerticalSpikeObjectData = this.GetSpikeControllerInfo(this.verticalPseudo3DSpike);
        this.horizontalPseudo3DSpike.gameObject.SetActive(false);
        this.verticalPseudo3DSpike.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        switch (this.bossPhase)
        {
            case BossPhase.Entry:
                this.BossEntryMovement(this.transform.position);

                break;
            case BossPhase.Attacking:
                if (this.attackPhase == QuasarLakeBossAttackPhase.Phase1)
                {
                    this.Phase1Movement(this.transform.position);

                    if (this.healthPoints <= 3)
                    {
                        this.attackPhase = QuasarLakeBossAttackPhase.Phase2;
                        this.StartCoroutine(this.ShowPseudo3DSpikeBall(this.verticalPseudo3DSpike, this.pseudo3DVerticalSpikeObjectData));
                    }
                }
                else if (this.attackPhase == QuasarLakeBossAttackPhase.Phase2)
                {
                    this.Phase2Movement(this.transform.position);
                }
                else if (this.attackPhase == QuasarLakeBossAttackPhase.Turning)
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
        this.bossSpritesPivot.transform.localScale = new Vector3(this.currentDirection, 1, 1);
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

            this.StartCoroutine(this.ShowPseudo3DSpikeBall(this.horizontalPseudo3DSpike, this.pseudo3DHorizontalSpikeObjectData));
            this.currentDirection = -1;
        }
    }

    /// <summary>
    /// The movement of the boss when phase 1 is active
    /// <param name="position">The current position of the boss</param>
    /// </summary>
    private void Phase1Movement(Vector2 position)
    {
        if (position.x < this.leftBoundary.x + this.startPosition.x && this.currentDirection == -1)
        {
            this.Flip();
        }
        else if (position.x > this.rightBoundary.x + this.startPosition.x && this.currentDirection == 1)
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
        QuasarLakeBossAttackPhase previousPhase = this.attackPhase;
        this.attackPhase = QuasarLakeBossAttackPhase.Turning;

        yield return new WaitForSeconds(General.StepsToSeconds(this.turningDelay));

        yield return new WaitForEndOfFrame();

        this.currentDirection = this.currentDirection == 1 ? -1 : 1;
        this.bossSpritesPivot.transform.localScale = new Vector3(this.currentDirection, 1, 1);
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
        this.ExplodeInitializedObjects(this.pseudo3DHorizontalSpikeObjectData);
        this.ExplodeInitializedObjects(this.pseudo3DVerticalSpikeObjectData);
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
        this.bossSpritesPivot.transform.localScale = new Vector3(this.currentDirection, 1, 1);
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
    /// Gathers the spike controller data at the start to aid initialization
    /// <param name="pseudo3DSpikeController">The spike controller to gather data from</param>
    /// </summary>
    private List<InitializedObjectData> GetSpikeControllerInfo(Pseudo3DBallController pseudo3DSpikeController)
    {
        List<InitializedObjectData> pseudo3DSpikeControllerData = new List<InitializedObjectData>
        {
            new InitializedObjectData(pseudo3DSpikeController.gameObject, pseudo3DSpikeController.transform.localPosition)
        };

        foreach (Pseudo3DBallLinkController linkController in pseudo3DSpikeController.GetPseudo3DBallLinks())
        {
            pseudo3DSpikeControllerData.Add(new InitializedObjectData(linkController.gameObject, linkController.transform.localPosition));
        }

        pseudo3DSpikeControllerData.Add(new InitializedObjectData(pseudo3DSpikeController.GetPseudo3DBallSpikeController().gameObject, pseudo3DSpikeController.GetPseudo3DBallSpikeController().transform.localPosition));

        return pseudo3DSpikeControllerData;
    }

    /// <summary>
    /// Slowly shows the the objects relating to the spike controller
    /// <param name="pseudo3DSpike">The main pseudo 3d spike object/controller </param>
    /// <param name="initializedObjectData">The list of objects initialized by the pseudo3D Ball<</param>
    /// </summary>
    private IEnumerator ShowPseudo3DSpikeBall(Pseudo3DBallController pseudo3DSpike, List<InitializedObjectData> initializedObjectData)
    {
        this.SetActiveSpikeControllerObjects(pseudo3DSpike, false);

        foreach (Pseudo3DBallLinkController linkController in pseudo3DSpike.GetPseudo3DBallLinks())
        {
            linkController.transform.localPosition = this.pseudoSpikeShowPositionOffset;
        }

        pseudo3DSpike.GetPseudo3DBallSpikeController().transform.localPosition = this.pseudoSpikeShowPositionOffset;
        pseudo3DSpike.transform.localPosition = this.pseudoSpikeShowPositionOffset;
        pseudo3DSpike.gameObject.SetActive(true);

        //Move the spike to its position
        while (true)
        {
            int objectsAtTargetPosition = 0;

            foreach (InitializedObjectData initializedObject in initializedObjectData)
            {
                Vector2 position = initializedObject.GetGameObject().transform.localPosition;
                position = Vector2.MoveTowards(position, initializedObject.GetStartPosition(), this.pseudoSpikeShowSpeed * Time.deltaTime);
                initializedObject.GetGameObject().transform.localPosition = position;

                if (position == initializedObject.GetStartPosition())
                {
                    objectsAtTargetPosition++;
                }
            }

            //All Objects are at the target position
            if (objectsAtTargetPosition == initializedObjectData.Count)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        this.SetActiveSpikeControllerObjects(pseudo3DSpike, true);

        yield return null;
    }

    /// <summary>
    /// Disable the monobehaviours attached related to the spike controller
    /// <param name="pseudo3DSpike">The main pseudo 3d spike object/controller </param>
    /// <param name="value">The value of the objects relating to the spike controller</param>
    /// </summary>
    private void SetActiveSpikeControllerObjects(Pseudo3DBallController pseudo3DSpike, bool value)
    {
        foreach (Pseudo3DBallLinkController linkController in pseudo3DSpike.GetPseudo3DBallLinks())
        {
            linkController.enabled = value;
        }

        pseudo3DSpike.GetPseudo3DBallSpikeController().enabled = value;
        pseudo3DSpike.enabled = value;

    }

    /// <summary>
    /// Disable and spwwan an explosion at the position of the initialzed object
    /// <param name="initializedObjectData">The list of objects initialized by the pseudo3D Ball</param>
    /// </summary>
    private void ExplodeInitializedObjects(List<InitializedObjectData> initializedObjectData)
    {
        foreach (InitializedObjectData initializedObject in initializedObjectData)
        {
            initializedObject.GetGameObject().SetActive(false);
            GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.BossExplosion, initializedObject.GetGameObject().transform.position);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Vector2 debugPosition = Application.isPlaying ? (Vector3)this.startPosition : this.transform.position;
        Vector2 pos1 = debugPosition + this.leftBoundary;
        Vector2 pos2 = debugPosition + this.rightBoundary;
        Gizmos.color = this.boundaryDebugColor;
        Gizmos.DrawLine(pos1, pos2);
        GizmosExtra.Draw2DArrow(pos1, 90);
        GizmosExtra.Draw2DArrow(pos2, 270);
    }
}
