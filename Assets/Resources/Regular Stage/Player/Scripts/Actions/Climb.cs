using System;
using System.Collections;
using System.Linq;
using UnityEngine;
/// <summary>
/// A unique action that allows players to climb certain objects within the <see cref="Sensors.climbableCollisionMask"/>
/// </summary>
public class Climb : HedgePrimaryAction
{
    [SerializeField]
    private Glide glide;
    [SerializeField]
    private ClimbState climbState = ClimbState.None;
    [SerializeField, DirectionList, Tooltip("The direction of the wall being climbed")]
    private int wallDirection = 1;
    [SerializeField, Tooltip("The info of the wall being climbed")]
    private CollisionInfoData currentWallInfo;
    [SerializeField, Tooltip("The current position of the ledge")]
    private Vector2 ledgePosition;
    [SerializeField, Range(0, 1), FirstFoldOutItem("Pull Up Info")]
    private float currentAnimationTime = 0;
    [SerializeField, Tooltip("Calculate the final position of the player and camera once the pull up animation is done playing")]
    private Vector2 finalPositionAfterPullUp;

    [FirstFoldOutItem("Sensor Colours")]
    public Color ledgeSensorColor = General.RGBToColour(0, 240, 0);
    public Color fallSensorColor = General.RGBToColour(56, 255, 162);
    public Color findLedgeSensorColor = General.RGBToColour(0, 174, 239);
    [LastFoldoutItem]
    public Color ceilingSensorColor = General.RGBToColour(255, 242, 56);
    [SerializeField, Tooltip("Radius in which a wall can be climbed")]
    private float noClimbRadius = 0;
    [Tooltip("Current wall collision info"), SerializeField]
    private CollisionInfoData wallLedgeCollisionInfo = new CollisionInfoData();

    public override void Start()
    {
        base.Start();

        if (this.glide == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Mainly used to set the defaults
    /// </summary>
    public override void Reset()
    {
        base.Reset();
        this.actionID = 17f;
        this.climbState = ClimbState.None;
        this.attackingAction = false;
        this.wallDirection = 1;
        this.primaryAnimationID = ActionSubState.Climbing;
        this.glide = this.player.GetActionManager().GetAction<Glide>() as Glide;
    }

    /// <summary>
    /// If we are currently gliding or turning within  a glid action we can perform the action
    /// </summary>
    public override bool CanPerformAction()
    {
        if (this.player.GetActionManager().GetAction<Glide>() == null)
        {
            return false;
        }

        if ((this.player.GetActionManager().CheckActionIsBeingPerformed<Glide>()
            && this.glide.GetGlideState() == GlideState.Gliding) || this.glide.GetGlideState() == GlideState.Turning)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Wait until the player comes in contact with climbable object and if so perform the action
    /// </summary>
    public override bool LaunchActionConditon()
    {
        if (
            this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetHit()
            && this.IsClimbable(this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetHit().transform.gameObject)
            && General.CheckAngleIsWithinOrEqualRange(Mathf.RoundToInt(this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees()), 90 + this.noClimbRadius, 270 - this.noClimbRadius)
            )
        {
            if (Mathf.Sign(this.player.velocity.x) == this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Loop through the players positions to calculate the final position post pull up
    /// </summary>
    private Vector2 CalculatePlayerFinalPosition()
    {
        Vector2 finalPosition = this.player.transform.position;

        foreach (Vector2 position in this.player.GetPlayerPhysicsInfo().pullUpPositionIncrements)
        {
            finalPosition += position * new Vector2(this.wallDirection, 1);
        }

        return finalPosition;
    }

    /// <summary>
    /// Sets the player to the climb state
    /// </summary>
    public override void OnActionStart()
    {
        this.player.GetAnimatorManager().SwitchActionSubstate(this.primaryAnimationID);
        this.climbState = ClimbState.Climb;
        this.wallDirection = this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo().GetSensorHitDirection();
        this.currentWallInfo = this.player.GetSensors().wallCollisionInfo.GetCurrentCollisionInfo();
        this.player.velocity.x = 0;
        this.player.velocity.y = 0;

        GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().grab);
    }

    /// <summary>
    /// Perform different actions based on the players current climb state
    /// </summary>
    public override void OnPerformingAction()
    {
        if (this.climbState == ClimbState.Climb)
        {
            this.HandleClimbingState();
        }
        else if (this.climbState == ClimbState.PullUp)
        {
            this.player.GetAnimatorManager().PlayAnimation("Clambering", this.currentAnimationTime);
        }
        else if (this.climbState == ClimbState.Drop)
        {
            this.HandleDroppingState();
        }
    }

    /// <summary>
    /// Exits the climb state when the climb state is set to none or they touch the ground
    /// </summary>
    public override bool ExitActionCondition()
    {
        if (this.climbState == ClimbState.None || (this.player.GetGrounded() && this.climbState == ClimbState.Climb))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// The commands performed at the end of the climb action
    /// </summary>
    public override void OnActionEnd()
    {
        this.player.GetAnimatorManager().SwitchActionSecondarySubstate(0);
        this.player.GetAnimatorManager().SwitchActionSubstate(0);
        this.player.GetAnimatorManager().SwitchGimmickSubstate(0);
        this.player.GetAnimatorManager().SetOtherAnimationSubstate(0);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.SetClimbState(ClimbState.None);
    }

    /// <summary>
    /// Set the current climb state of the action and update the animation where available
    /// <param name="climbState">The new climb state for the climb action </param>
    /// </summary>
    public void SetClimbState(ClimbState climbState)
    {
        if (this.climbState == climbState)
        {
            return;
        }

        this.climbState = climbState;

        if (climbState != ClimbState.None)
        {
            int index = (int)climbState;
            this.player.GetAnimatorManager().SetOtherAnimationSubstate(index);

            if (climbState == ClimbState.PullUp)
            {
                this.OnSwitchToPullUpState();
            }
            else if (climbState == ClimbState.StandUp)
            {
                this.OnSwitchToStandUpState();
            }
        }
        else
        {
            this.player.GetActionManager().EndCurrentAction();
            this.player.GetAnimatorManager().SetOtherAnimationSubstate(0);
        }
    }

    /// <summary>
    /// Checks to slide of the wall or climb, the movement of the player while on the wall and whether the jump button was pressed to get off the wall
    /// </summary>
    private void HandleClimbingState()
    {
        this.player.velocity.y = this.player.GetInputManager().GetCurrentInput().y * this.player.GetPlayerPhysicsInfo().climbSpeed;

        //If the player tries to move upwards while the ceiling info is set neutralize the vertical velocity
        if (this.player.velocity.y > 0)
        {
            if (this.CheckForCeiling(this.player.transform.position)
                || this.player.PlayerIsAboveTopBounds()
                || (this.wallLedgeCollisionInfo.GetHit() && !General.CheckAngleIsWithinOrEqualRange(Mathf.RoundToInt(this.wallLedgeCollisionInfo.GetAngleInDegrees()), 90 + this.noClimbRadius, 270 - this.noClimbRadius)))
            {
                this.player.velocity.y = 0;
            }
        }


        this.player.SetPlayerDirection(this.wallDirection);//Face the wall D:
        if (this.player.GetInputManager().GetJumpButton().GetButtonPressed() && this.player.PlayerIsAboveTopBounds() == false)
        {
            int currentWallDirection = this.wallDirection;//This action is about to be exit and reset() so save the wall direction
            this.player.GetActionManager().PerformAction<Jump>();
            this.player.velocity.x = 4 * -currentWallDirection;//Jump away from the wall
            this.player.velocity.y = 4;
            this.player.SetPlayerDirection((int)Mathf.Sign(this.player.velocity.x));//Be free

            return;
        }

        this.CheckForWallLedge(this.transform.position);

        if (this.player.velocity.y < 0)
        {
            this.CheckToFallOfLedge(this.transform.position);
        }
    }

    /// <summary>
    /// While in the pull up state force the animation frame based on the animation time of the coroutine
    /// </summary>
    private void HandlePullUpState()
    {
        this.player.GetAnimatorManager().PlayAnimation("Pull Up", this.currentAnimationTime);
        this.player.GetAnimatorManager().GetAnimator().Update(Time.deltaTime);
    }

    /// <summary>
    /// While the player is dropping from a climb we need to wait for them to hit the ground before standing up
    /// </summary>
    private void HandleDroppingState()
    {
        if (this.player.GetGrounded())
        {
            this.player.GetAnimatorManager().GetAnimator().Update(Time.deltaTime);
            this.player.groundVelocity = 0;
            this.player.velocity.x = 0;
            this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
            GMAudioManager.Instance().PlayOneShot(this.player.GetPlayerActionAudio().land);
            this.SetClimbState(ClimbState.StandUp);
        }
    }

    /// <summary>
    /// Calculate the actual position of the ledge and adjust the player accordingly
    /// </summary>
    private void FindLedgePosition(Vector2 position)
    {
        float distance = GMStageManager.Instance().GetMaxBlockSize();
        SensorData sensorData = new SensorData(0, -90, 0, distance);

        Vector2 findLedgeSensorPositon = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2((this.player.GetSensors().characterBuild.pushRadius + 1) * this.wallDirection, distance)); // B - Right Ground Sensor
        RaycastHit2D findLedgeSensor = Physics2D.Raycast(findLedgeSensorPositon, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.player.GetSensors().wallCollisionInfo.GetClimbableCollisionMask());
        Debug.DrawLine(findLedgeSensorPositon, findLedgeSensorPositon + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.findLedgeSensorColor);

        if (findLedgeSensor)
        {
            position.y = findLedgeSensor.point.y - this.player.GetSensors().currentBodyHeightRadius + 1f;
            this.ledgePosition = findLedgeSensor.point;
        }

        this.player.transform.position = position;
    }

    /// <summary>
    /// Check if the player has reached the top of the ledge
    /// </summary>
    private void CheckForWallLedge(Vector2 position)
    {
        SensorData sensorData = new SensorData(0, 0, 0, (this.player.GetSensors().characterBuild.pushRadius + 1) * this.wallDirection);
        Vector2 ledgeSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(0, this.player.GetSensors().currentBodyHeightRadius)); //Setup sensor position
        RaycastHit2D ledgeSensor = Physics2D.Raycast(ledgeSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.player.GetSensors().wallCollisionInfo.GetClimbableCollisionMask());
        Debug.DrawLine(ledgeSensorPosition, ledgeSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.ledgeSensorColor);

        if (ledgeSensor)
        {
            this.wallLedgeCollisionInfo = new CollisionInfoData(ledgeSensor, this.wallDirection == 1 ? SensorHitDirectionEnum.Right : SensorHitDirectionEnum.Left);
        }
        else
        {
            this.wallLedgeCollisionInfo = new CollisionInfoData();
        }

        if (!ledgeSensor)
        {
            if (this.wallLedgeCollisionInfo.GetHit() && !General.CheckAngleIsWithinRange(Mathf.RoundToInt(this.wallLedgeCollisionInfo.GetAngleInDegrees()), 90 + this.noClimbRadius, 270 - this.noClimbRadius))
            {
                return;
            }

            this.FindLedgePosition(position);
            this.SetClimbState(ClimbState.PullUp);
        }
    }

    /// <summary>
    /// Check if the player has reached the bottom of the ledge
    ///
    private void CheckToFallOfLedge(Vector2 position)
    {
        SensorData sensorData = new SensorData(0, 0, 0, (this.player.GetSensors().characterBuild.pushRadius + 1) * this.wallDirection);
        Vector2 fallSensorPosition = General.CalculateAngledObjectPosition(position, sensorData.GetAngleInRadians(), new Vector2(0, -this.player.GetSensors().currentBodyHeightRadius)); //Setup sensor position
        RaycastHit2D fallSensor = Physics2D.Raycast(fallSensorPosition, sensorData.GetCastDirection(), sensorData.GetCastDistance(), this.player.GetSensors().wallCollisionInfo.GetClimbableCollisionMask());
        Debug.DrawLine(fallSensorPosition, fallSensorPosition + (sensorData.GetCastDirection() * sensorData.GetCastDistance()), this.fallSensorColor);

        if (!fallSensor)
        {
            this.player.velocity.y = 0;
            this.SetClimbState(ClimbState.Drop);
        }
    }

    public ClimbState GetClimbState() => this.climbState;

    /// <summary>
    /// Checks if there is a ceiling above the player
    /// <param name="position"> The current position of the player</param>
    /// </summary>
    private bool CheckForCeiling(Vector2 position)
    {
        float angleInRadians = 0;
        float sensorAngle = angleInRadians * Mathf.Rad2Deg;//The angle the sensors will be cast in
        Vector2 direction = General.AngleToVector(sensorAngle * Mathf.Deg2Rad); //Set the ray direction

        Vector2 ceilingCheckRange = new Vector2(this.player.GetSensors().characterBuild.bodyWidthRadius * 2, this.player.GetSensors().characterBuild.bodyHeightRadius);
        Vector2 lowCeilingSensorPosition = General.CalculateAngledObjectPosition(position, angleInRadians, new Vector2(-this.player.GetSensors().characterBuild.bodyWidthRadius, ceilingCheckRange.y));
        RaycastHit2D lowCeilingSensor = Physics2D.Raycast(lowCeilingSensorPosition, direction, ceilingCheckRange.x, this.player.GetSensors().ceilingCollisionInfo.GetCollisionMask());
        Debug.DrawLine(lowCeilingSensorPosition, lowCeilingSensorPosition + (direction * ceilingCheckRange), this.ceilingSensorColor);

        return lowCeilingSensor;
    }


    /// <summary>
    /// Actions performed when the pull up state is switched to
    /// </summary>
    private void OnSwitchToPullUpState()
    {
        this.finalPositionAfterPullUp = this.CalculatePlayerFinalPosition();
        this.player.velocity.y = 0;
        this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
        this.currentAnimationTime = 0;
        this.StartCoroutine(this.BeginPullUp());
        this.StartCoroutine(this.PullUpCameraMovement());
    }

    /// <summary>
    /// Actions performed when the stand up state is switched to
    /// </summary>
    private void OnSwitchToStandUpState()
    {
        this.player.GetSensors().SetSizeMode(SizeMode.Regular);
        this.StartCoroutine(this.WaitTillStandingCoroutine());
    }

    /// <summary>
    /// Moves the player through the motions and position of the pull up animation
    /// </summary>
    private IEnumerator BeginPullUp()
    {
        yield return new WaitForEndOfFrame();
        this.player.GetAnimatorManager().SetAnimatorSpeed(0);

        for (int x = 0; x < this.player.GetPlayerPhysicsInfo().pullUpPositionIncrements.Count; x++)
        {
            Vector2 position = this.player.transform.position;
            position += this.player.GetPlayerPhysicsInfo().pullUpPositionIncrements[x] * new Vector2(this.wallDirection, 1);
            this.player.transform.position = position;
            this.currentAnimationTime = x / (float)this.player.GetPlayerPhysicsInfo().pullUpPositionIncrements.Count;
            this.player.GetAnimatorManager().GetAnimator().playbackTime = this.currentAnimationTime;
            this.HandlePullUpState();
            yield return new WaitForSeconds(this.player.GetPlayerPhysicsInfo().pullUpAnimationClip.length / 4f);
        }

        this.player.GetAnimatorManager().SetAnimatorSpeed(1);
        this.SetClimbState(ClimbState.StandUp);

        yield return null;
    }

    /// <summary>
    /// While doing a pullup we want to hijack the camera movement so it can perform a smooth follow to the end pull up point
    /// </summary>
    private IEnumerator PullUpCameraMovement()
    {
        //Hijack the camera movement and control it on this side in parallel to pull up
        HedgehogCamera hedgehogCamera = HedgehogCamera.Instance();
        hedgehogCamera.SetCameraMode(CameraMode.Freeze);
        float currentTime = 0;
        Vector3 startPosition = hedgehogCamera.transform.position;
        Vector3 target = this.finalPositionAfterPullUp;
        target.z = hedgehogCamera.transform.position.z;

        while (currentTime < 1)
        {
            currentTime += Time.deltaTime / this.player.GetPlayerPhysicsInfo().pullUpAnimationClip.length;
            hedgehogCamera.SetCameraPosition(Vector3.Lerp(startPosition, target, currentTime));

            yield return new WaitForFixedUpdate();
        }

        hedgehogCamera.SetCameraMode(CameraMode.FollowTarget);

        yield return null;
    }

    /// <summary>
    /// Waits until the players standing animation is played out before moving on
    /// </summary>
    private IEnumerator WaitTillStandingCoroutine()
    {
        yield return new WaitForEndOfFrame();

        while (this.climbState == ClimbState.StandUp && this.player.GetAnimatorManager().GetCurrentAnimationNormalizedTime() < 1)
        {
            yield return new WaitForFixedUpdate();
        }

        this.SetClimbState(ClimbState.None);

        yield return null;
    }

    /// <summary>
    /// Check to see if the <see cref="GameObject"/> can be climbed
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool IsClimbable(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        bool objectIsInClimbableMask = General.ContainsLayer(this.player.GetSensors().wallCollisionInfo.GetClimbableCollisionMask(), target.layer);
        target.TryGetComponent<MonoBehaviour>(out MonoBehaviour targetObjectClass);

        //If its the object is not a MonoBehaviour checking if its in the climbable mask will suffice
        if (targetObjectClass == null)
        {
            return objectIsInClimbableMask;
        }

        //Checks if the object belongs to a class in our non climbable list
        bool cannotBeClimbed = this.player.GetPlayerPhysicsInfo().nonClimbableObjectControllers.Any(climbableObject => climbableObject == targetObjectClass.GetType().ToString());
        if (cannotBeClimbed)
        {
            return false;
        }

        return objectIsInClimbableMask;
    }
}
