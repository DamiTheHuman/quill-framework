using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class HomingAttack : HedgeSubAction
{
    [SerializeField]
    private TargetLockController targetLockController;
    [SerializeField, Tooltip("The current homing attack target if available")]
    private GameObject currentTarget;
    [SerializeField, Tooltip("The item at index 0 is usually the target")]
    private List<GameObject> potentialTargets = new List<GameObject>();
    [SerializeField, Tooltip("The mode of the homing attack")]
    private HomingAttackMode homingAttackMode = HomingAttackMode.None;
    [SerializeField, Tooltip("Reason why the homing attack was ended")]
    private HomingAttackEndReason homingAttackEndReason = HomingAttackEndReason.None;
    [SerializeField]
    private Vector2 homingAttackDirection = new Vector2();
    [FirstFoldOutItem("Debug"), SerializeField]
    private Color homableRadiusDebugColor = General.RGBToColour(255, 77, 84, 170);
    [SerializeField]
    private Color playerToTargetDebugColor = Color.green;
    [LastFoldoutItem(), SerializeField]
    private Color playerToPotentialTargetDebugColor = Color.yellow;
    private IEnumerator homingAttackTimeOutCoroutine;

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 1.11f;
        this.parentAction = this.GetComponentInParent<Jump>();
        this.homingAttackMode = HomingAttackMode.None;
        this.currentTarget = null;
        this.potentialTargets.Clear();
        if (this.homingAttackTimeOutCoroutine != null)
        {
            this.StopCoroutine(this.homingAttackTimeOutCoroutine);
            this.homingAttackTimeOutCoroutine = null;
        }
    }

    /// <summary>
    /// Can only perform the dropdash while jumping and only if the Jump sub action has not been used up
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Jump>() && this.parentAction.usedSubAction == false)
        {
            this.ScanForHomableTarget();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Waits untill the user inputs the jump button again before running the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed())
        {
            return true;
        }

        return false;
    }
    /// <summary>
    /// Locsk the player input and begin moving them towards the target
    /// </summary>
    public override void OnActionStart()
    {
        this.SetHomingAttackEndReason(HomingAttackEndReason.None);
        if (this.ScanForHomableTarget())
        {
            this.homingAttackDirection = new Vector2();
            this.player.GetSpriteController().SetSpriteAngle(0);
            this.player.velocity = new Vector2();
            this.currentTarget = this.potentialTargets[0];
            this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
            this.SetHomingAttackMode(HomingAttackMode.Homing);
            this.BeginHomingAttackTimeOutCoroutine();
        }
        else
        {
            this.player.velocity.x = this.player.GetPlayerPhysicsInfo().homingAttackSpeed * this.player.currentPlayerDirection;
            this.player.velocity.y = 0;
            HedgehogCamera.Instance().GetCameraLookHandler().BeginDashLag();
            this.SetHomingAttackMode(HomingAttackMode.Dash);
        }

        this.player.GetInputManager().GetJumpButton().SetButtonPressed(false);
        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().homingAttack);
    }

    /// <summary>
    /// Move the player towards the current target
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.homingAttackMode == HomingAttackMode.Homing)
        {
            Vector2 prevPosition = this.player.transform.position;
            Vector3 position = this.player.transform.position;
            Vector3 targetPosition = this.currentTarget.transform.position;
            position = Vector2.MoveTowards(position, targetPosition, this.player.GetPlayerPhysicsInfo().homingAttackSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime);
            this.player.velocity = new Vector2();
            this.player.groundVelocity = 0;
            this.player.transform.position = position;

            Vector2 direction = targetPosition - position;
            float angleBetweenPositionsInRadians = Mathf.Atan2(direction.y, direction.x);

            this.homingAttackDirection = General.AngleToVector(angleBetweenPositionsInRadians);
            this.SetHomingAttackEndReason(this.UpdateHomingAttackEndReason());
        }
    }

    /// <summary>
    /// Exits the dropdash when the player hits the ground, wall or ceiling so they don't get stuck
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.homingAttackEndReason != HomingAttackEndReason.None)
        {
            return true;
        }
        else if (this.player.GetGrounded() || this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetHit() || this.player.GetSensors().ceilingCollisionInfo.GetCurrentCollisionInfo().GetHit())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Set the <see cref="HomingAttackMode"/>
    /// </summary>
    /// <param name="homingAttackMode"></param>
    public void SetHomingAttackMode(HomingAttackMode homingAttackMode)
    {
        if (homingAttackMode == HomingAttackMode.None)
        {
            this.potentialTargets.Clear();
            this.currentTarget = null;
            this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
            if (this.homingAttackTimeOutCoroutine != null)
            {
                this.StopCoroutine(this.homingAttackTimeOutCoroutine);
            }
        }

        this.homingAttackMode = homingAttackMode;
    }
    /// <summary>
    /// Checks to update the homing attack reason
    /// </summary>
    private HomingAttackEndReason UpdateHomingAttackEndReason()
    {
        if (this.homingAttackMode != HomingAttackMode.Homing)
        {
            return this.homingAttackEndReason;
        }
        HomingAttackEndReason homingAttackEndReason = HomingAttackEndReason.None;

        if (this.currentTarget && this.HasPlayerReachedTarget(this.currentTarget))
        {
            homingAttackEndReason = HomingAttackEndReason.ReachedTarget;
        }
        else if (this.player.GetGrounded()
           || this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetHit()
           || this.player.GetSensors().ceilingCollisionInfo.GetCurrentCollisionInfo().GetHit()
           )
        {
            homingAttackEndReason = HomingAttackEndReason.HitAWall;

        }
        else if (this.currentTarget == null || this.currentTarget.activeSelf == false)
        {
            homingAttackEndReason = HomingAttackEndReason.LostTarget;

        }
        else if (this.IsThereAWallBetweenPlayerAndTarget(this.currentTarget))
        {
            homingAttackEndReason = HomingAttackEndReason.WallBetweenPlayerAndTrget;

        }
        else if (this.player.velocity != Vector2.zero)
        {
            homingAttackEndReason = HomingAttackEndReason.VelocityChanged;

        }

        return homingAttackEndReason;
    }

    /// <summary>
    /// Get the <see cref="homingAttackMode"/>
    /// </summary>
    /// <returns></returns>
    public HomingAttackMode GetHomingAttackMode() => this.homingAttackMode;

    /// <summary>
    /// Set the <see cref="homingAttackEndReason"/>
    /// </summary>
    /// <returns></returns>
    public void SetHomingAttackEndReason(HomingAttackEndReason homingAttackEndReason) => this.homingAttackEndReason = homingAttackEndReason;

    /// <summary>
    /// Get the <see cref="homingAttackDirection"/>
    /// </summary>
    /// <returns></returns>
    public Vector2 GetHomingAttackDirection() => this.homingAttackDirection;

    /// <summary>
    /// Check if the player has reached the target
    /// </summary>
    /// <param name="currentTarget"></param>
    /// <returns></returns>
    private bool HasPlayerReachedTarget(GameObject currentTarget) => Vector2.Distance(this.player.transform.position, currentTarget.transform.position) <= 16;

    /// <summary>
    /// The commands performed at the end of the homing attack
    /// </summary>
    public override void OnActionEnd()
    {
        if (this.homingAttackEndReason is HomingAttackEndReason.LostTarget or HomingAttackEndReason.TookToLong or HomingAttackEndReason.WallBetweenPlayerAndTrget)
        {
            this.player.velocity = this.player.GetPlayerPhysicsInfo().homingAttackSpeed * this.GetHomingAttackDirection();
        }

        this.homingAttackDirection = new Vector2();
        this.SetHomingAttackEndReason(HomingAttackEndReason.None);
        this.SetHomingAttackMode(HomingAttackMode.None);
    }

    /// <summary>
    /// Scans the players vicinity for homable objects
    /// </summary>
    /// <returns></returns>
    private bool ScanForHomableTarget()
    {
        this.currentTarget = null;
        Collider2D[] results = new Collider2D[100];

        //cast a circle cast the radius of the player for
        int hits = Physics2D.OverlapCircleNonAlloc(this.player.transform.position, this.player.GetPlayerPhysicsInfo().homingAttackRadius, results, this.player.GetPlayerPhysicsInfo().homingAttackCollisionMask);
        this.potentialTargets = new List<GameObject>();
        for (int i = 0; i < hits; i++)
        {
            string potentialTargetName = results[i].gameObject.name;
            try
            {
                GameObject potentialTarget = results[i].gameObject;
                if (this.IsPlayerLookingAtObject(potentialTarget) == false)
                {
                    continue;
                }
                if (this.IsTargetObjectIsHomable(potentialTarget))
                {
                    if (this.IsThereAWallBetweenPlayerAndTarget(potentialTarget))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                this.potentialTargets.Add(potentialTarget);
            }
            catch (Exception exception)
            {
                Debug.Log("Could not scan potential target: " + potentialTargetName);
            }
        }

        if (this.potentialTargets.Count == 0)
        {
            return false;
        }

        this.potentialTargets = this.potentialTargets.OrderBy(
           homableObject => Vector2.Distance(this.player.transform.position, homableObject.transform.position)
          ).ToList();

        if (this.targetLockController == null)
        {
            this.targetLockController = GMSpawnManager.Instance().SpawnGameObject(ObjectToSpawn.TargetLock, this.potentialTargets[0].transform.position).GetComponent<TargetLockController>();
            this.targetLockController.SetPlayer(this.player);
            this.targetLockController.SetHomingAttack(this);
        }

        if (this.targetLockController.gameObject.activeSelf == false)
        {
            this.targetLockController.gameObject.SetActive(true);
        }

        return true;
    }

    /// <summary>
    /// Gets the current target
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentTarget() => this.currentTarget;

    /// <summary>
    /// An event triggered when the player hits the currents <see cref="currentTarget"/>
    /// And triggers a <see cref="ContactEvent.HedgeOnCollisionEnter(Player)"/>
    /// </summary>
    /// <param name="targetObject"></param>
    public void OnHitTargetObject(ContactEvent targetObject)
    {
        if (this.GetHomingAttackMode() != HomingAttackMode.Homing)
        {
            return;
        }

        if (this.currentTarget.gameObject != targetObject.gameObject)
        {
            Debug.Log("THIS WAS NOT THE TARGET");
        }

        this.SetHomingAttackEndReason(HomingAttackEndReason.HitTarget);

        ///Any attack rebound kind of event should be registered here <see cref="Player.AttackRebound"></see>
        if (targetObject is BadnikController or MonitorController)
        {
            this.player.velocity.y = this.player.GetBasePhysicsInfo().basicJumpVelocity;
            //When not holding down set button up to true so the user doesn't get a max height jump unless they choose to
            //TODO: is there a better way to handle this
            if (this.player.GetInputManager().GetJumpButton().GetButtonDown() == false)
            {
                this.player.GetInputManager().GetJumpButton().SetButtonUp(true);
            }

            this.parentAction.ResetCurrentSubAction();
        }
        else if (targetObject is BalloonController)
        {
            this.parentAction.ResetCurrentSubAction();
        }
        else if (targetObject is Boss)
        {
            float direction = Mathf.Sign(targetObject.transform.position.x - this.player.transform.position.x);
            this.player.velocity.x = this.player.GetPlayerPhysicsInfo().homingAttackSpeed * direction;
            this.player.velocity.y = 0;
        }

        this.SetHomingAttackMode(HomingAttackMode.None);
    }

    /// <summary>
    /// Get the <see cref="potentialTargets"/>
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetPotentialTargets() => this.potentialTargets;

    /// <summary>
    /// Check if the player is looking at the <see cref="GameObject"/>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    /// TODO: should the player beable to home directly above or below them?
    private bool IsPlayerLookingAtObject(GameObject target) => this.player.currentPlayerDirection != Math.Sign(this.player.transform.position.x - target.transform.position.x);

    /// <summary>
    /// Check the <see cref="GameObject"/> homable
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsTargetObjectIsHomable(GameObject target)
    {
        MonoBehaviour targetObjectClass = target.GetComponent<MonoBehaviour>();

        if (targetObjectClass is Boss or BadnikController)
        {
            return true;
        }

        return this.player.GetPlayerPhysicsInfo().homableObjectControllers.Any(homableObject => homableObject == targetObjectClass.GetType().ToString());
    }

    /// <summary>
    /// Check if a there is a wall between the player and the <see cref="GameObject"/>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsThereAWallBetweenPlayerAndTarget(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("Target was null");
            return true;
        }

        RaycastHit2D wallBetweenPlayerAndObjectHit = Physics2D.Linecast(this.player.transform.position, target.transform.position, this.player.GetSensors().groundCollisionInfo.GetCollisionMask());
        Debug.DrawLine(this.player.transform.position, target.transform.position, this.currentTarget == target ? this.playerToTargetDebugColor : this.playerToPotentialTargetDebugColor);

        if (wallBetweenPlayerAndObjectHit && wallBetweenPlayerAndObjectHit.transform != target.transform)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Begins the <see cref="homingAttackTimeOutCoroutine"/>
    /// </summary>
    private void BeginHomingAttackTimeOutCoroutine()
    {
        if (this.homingAttackTimeOutCoroutine != null)
        {
            this.StopCoroutine(this.homingAttackTimeOutCoroutine);
        }

        this.homingAttackTimeOutCoroutine = this.UpdateHomingAttackTimeOut();
        this.StartCoroutine(this.homingAttackTimeOutCoroutine);
    }

    /// <summary>
    /// Ends the homing attack forcefully after a certain amout of time has passed
    /// This ideally should never happen
    /// </summary>
    private IEnumerator UpdateHomingAttackTimeOut()
    {
        yield return new WaitForSeconds(General.StepsToSeconds(this.player.GetPlayerPhysicsInfo().homingAttackTimeout));
        this.SetHomingAttackMode(HomingAttackMode.None);
        this.SetHomingAttackEndReason(HomingAttackEndReason.TookToLong);
        Debug.LogError("Homing Attack too long!");
    }

    private void OnDrawGizmos()
    {
        if (this.CanPerformAction())
        {
            Gizmos.color = this.homableRadiusDebugColor;
            Gizmos.DrawWireSphere(this.player.transform.position, this.player.GetPlayerPhysicsInfo().homingAttackRadius);
        }
    }
}
