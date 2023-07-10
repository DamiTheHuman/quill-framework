using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles the cutscenes via timeline triggers of an act
/// </summary>
public class GMCutsceneManager : MonoBehaviour
{
    [SerializeField]
    private InputMaster inputMaster;

    [Tooltip("For testing a scene without cutscenes"), SerializeField]
    private bool disableCutscenes;
    [Help("Start And End Act Time lines are set by the respective Start Point Controllers or Sign Post Controllers")]
    [Tooltip("The cutscene to play when an act starts"), SerializeField, IsDisabled]
    private CutsceneController actStartCutscene;
    [Help("This should be set by attaching the end time line object e.g Sign Post")]
    [Tooltip("The cutscene to play when an act is cleared"), SerializeField, IsDisabled]
    private CutsceneController actClearCutscene;
    [Tooltip("The bounds of the camera during the last cutscene"), SerializeField]
    private CameraBounds endCutsceneCameraBounds;

    [SerializeField, Tooltip("Cutscene debug object")]
    private Transform endCutsceneBoundsText;

    private Action onCutsceneEndAction;
    private IEnumerator actStartCutsceneCoroutine;
    private IEnumerator actClearCutsceneCoroutine;

    private void Awake()
    {
        this.actClearCutscene = null;
        this.inputMaster = new InputMaster();
    }

    protected virtual void OnEnable()
    {
        if (this.inputMaster == null)
        {
            this.inputMaster = new InputMaster();
        }

        this.inputMaster.Enable();
    }

    protected virtual void OnDisable()
    {
        if (this.inputMaster != null)
        {
            this.inputMaster.Disable();
        }
    }

    /// <summary>
    /// The single instance of the scene manager
    /// </summary>
    private static GMCutsceneManager instance;

    /// <summary>
    /// Get a reference to the static instance of the cut scene manager
    /// </summary>
    public static GMCutsceneManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMCutsceneManager>();
        }

        return instance;
    }

    /// <summary>
    /// Set the act cutscene to play at the end
    /// <param name="actClearCutscene">The cutscene to play at the end<</param>
    /// </summary>
    public void SetActClearCutscene(CutsceneController actClearCutscene) => this.actClearCutscene = actClearCutscene;

    /// <summary>
    /// Set the act timeline triggers to play at the start
    /// <param name="actStartCutscene">The cutscene to play at the start</param>
    /// </summary>
    public void SetActStartCutscene(CutsceneController actStartCutscene) => this.actStartCutscene = actStartCutscene;

    /// <summary>
    /// Plays the act start time line triggers with a callback at the end of the last trigger
    /// <param name="onCutsceneEndAction">The method to call when the time line is done playing</param>
    /// </summary>
    public void PlayActStartTimeLineTriggers(Action onCutsceneEndAction)
    {
        if (this.actStartCutscene != null && this.actStartCutscene.GetTimelineTriggers().Count == 0)
        {
            return;
        }

        HedgehogCamera.Instance().SetCameraTarget(GMStageManager.Instance().GetPlayer().gameObject);
        HedgehogCamera.Instance().SetCameraMode(CameraMode.FollowTarget);
        GMStageManager.Instance().GetPlayer().GetActionManager().EndCurrentAction(true);
        GMStageManager.Instance().GetPlayer().SetPlayerState(PlayerState.Cutscene);
        GMStageManager.Instance().SetStageState(RegularStageState.Cutscene);
        GMStageManager.Instance().GetPlayer().transform.position = this.actStartCutscene.GetTimelineTriggers()[0].transform.position;
        this.onCutsceneEndAction = onCutsceneEndAction;

        this.actStartCutsceneCoroutine = this.MangeTimeLineTriggers(this.actStartCutscene, onCutsceneEndAction);
        this.StartCoroutine(this.actStartCutsceneCoroutine);
    }

    /// <summary>
    /// Plays the act end time line triggers
    /// <param name="onCutsceneEndAction">The method to call when the time line is done playing</param>
    /// </summary>
    public void PlayActClearTimeLineTriggers(Action onCutsceneEndAction)
    {
        if (this.actClearCutscene != null && this.actClearCutscene.GetTimelineTriggers().Count == 0)
        {
            return;
        }

        HedgehogCamera.Instance().SetCameraTarget(GMStageManager.Instance().GetPlayer().gameObject);
        HedgehogCamera.Instance().SetCameraMode(CameraMode.FollowTarget);
        HedgehogCamera.Instance().GetCameraBoundsHandler().SetActBounds(this.endCutsceneCameraBounds);
        HedgehogCamera.Instance().GetCameraBoundsHandler().SetTargetActBounds(this.endCutsceneCameraBounds);
        GMStageManager.Instance().GetPlayer().GetActionManager().EndCurrentAction(true);
        GMStageManager.Instance().GetPlayer().SetPlayerState(PlayerState.Cutscene);
        GMStageManager.Instance().SetStageState(RegularStageState.Cutscene);
        this.onCutsceneEndAction = onCutsceneEndAction;
        this.actClearCutsceneCoroutine = this.MangeTimeLineTriggers(this.actClearCutscene, onCutsceneEndAction);
        this.StartCoroutine(this.actClearCutsceneCoroutine);
    }

    /// <summary>
    /// Determines whether the current act has time line triggers to play at the beginning before starting the zone
    /// </summary>
    public bool HasStartActTimeLineTriggers()
    {
        if (this.disableCutscenes)
        {
            return false;
        }

        return this.actStartCutscene != null && this.actStartCutscene.GetTimelineTriggers().Count > 0;
    }

    /// <summary>
    /// Determines whether the current act has time line triggers to play at the beginning before ending the zone
    /// </summary>
    public bool HasEndActTimeLineTriggers()
    {
        if (this.disableCutscenes)
        {
            return false;
        }

        return this.actClearCutscene != null && this.actClearCutscene.GetTimelineTriggers().Count > 0;
    }

    /// <summary>
    /// Activates the input Master
    /// </summary>
    private void EnableInputMaster()
    {
        this.inputMaster = new InputMaster();
        this.inputMaster.Player.AButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Player.BButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Player.XButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Player.YButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Player.StartButton.performed += ctx => this.OnInputPressed(ctx.phase);
        this.inputMaster.Enable();
    }

    /// <summary>
    /// Action that skips the current cutscene
    /// </summary>
    private void OnInputPressed(UnityEngine.InputSystem.InputActionPhase phase)
    {
        if (phase != UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            return;
        }

        if (this.ActStartCutsceneIsPlaying())
        {
            this.SkipActStartCutscene();
            this.inputMaster.Disable();
        }

        if (this.ActClearCutsceneIsPlaying())
        {
            this.SkipActClearCutscene();
            this.inputMaster.Disable();
        }
    }

    /// <summary>
    /// Checks whether the act start cutscene is playing
    /// </summary>
    public bool ActStartCutsceneIsPlaying() => this.actStartCutsceneCoroutine != null;

    /// <summary>
    /// Skips the current act start cutscene
    /// </summary>
    private void SkipActStartCutscene()
    {
        this.actClearCutsceneCoroutine = null;
        GMSceneManager.Instance().ReloadCurrentScene();
        this.onCutsceneEndAction();
    }

    /// <summary>
    /// Checks whether the act clear cutscene is playing
    /// </summary>
    public bool ActClearCutsceneIsPlaying() => this.actClearCutsceneCoroutine != null;

    /// <summary>
    /// Skips the current end cutscene
    /// </summary>
    private void SkipActClearCutscene()
    {
        this.actClearCutsceneCoroutine = null;
        this.onCutsceneEndAction();
    }


    /// <summary>
    /// Handle Interactions between the players and their timelines in sequence
    /// <param name="cutsceneController">The cutscene controller</param>
    /// <param name="onCutsceneEndAction">The method to call when the time line is done playing</param>
    /// </summary>
    private IEnumerator MangeTimeLineTriggers(CutsceneController cutsceneController, Action onCutsceneEndAction)
    {
        Player player = GMStageManager.Instance().GetPlayer();
        List<TimelineTriggerController> timeLineTriggers = cutsceneController.GetTimelineTriggers();
        player.SetPlayerState(PlayerState.Cutscene);
        player.GetInputManager().enabled = false;
        this.EnableInputMaster();

        for (int x = 0; x < timeLineTriggers.Count; x++)
        {
            switch (timeLineTriggers[x].GetCutsceneAudioType())
            {
                case CutsceneAudioType.UseBGM:
                    GMAudioManager.Instance().PlayBGM(timeLineTriggers[x].GetCutsceneBGM());
                    break;
                case CutsceneAudioType.UseCutsceneAudio:
                    cutsceneController.GetAudioSource().clip = timeLineTriggers[x].GetCutsceneAudioClip();
                    cutsceneController.GetAudioSource().Play();
                    break;
                case CutsceneAudioType.None:
                default:
                    break;
            }

            if (timeLineTriggers[x] == null)
            {
                continue;
            }


            switch (timeLineTriggers[x].GetTriggerType())
            {
                case TimelineTriggerType.MoveToPositionAndPlay:
                    IEnumerator movePlayerToTargetCoroutine = this.MovePlayerToTarget(player, timeLineTriggers[x].transform);
                    this.StartCoroutine(movePlayerToTargetCoroutine);

                    while (movePlayerToTargetCoroutine.Current != null)
                    {
                        yield return new WaitForFixedUpdate();
                    }

                    break;
                case TimelineTriggerType.PlayWhenPlayerIsGrounded:
                    yield return new WaitUntil(() => player.GetGrounded());
                    break;
                case TimelineTriggerType.HoldDirectionTillPositionAndPlay:
                    IEnumerator movePlayerToTargetExceedsCoroutine = this.MovePlayerToTargetExceeds(player, timeLineTriggers[x].transform);
                    this.StartCoroutine(movePlayerToTargetExceedsCoroutine);

                    while (movePlayerToTargetExceedsCoroutine.Current != null)
                    {
                        yield return new WaitForFixedUpdate();
                    }

                    break;
                case TimelineTriggerType.JustPlayAnimation:
                default:
                    break;
            }

            if (timeLineTriggers[x].GetTimelinePlayableDirector() != null)
            {
                player.currentPlayerDirection = timeLineTriggers[x].GetCurrentDirection();
                player.GetSpriteController().SetSpriteDirection(timeLineTriggers[x].GetCurrentDirection());
                timeLineTriggers[x].GetTimelinePlayableDirector().Play();

                while (timeLineTriggers[x].GetTimelinePlayableDirector().playableGraph.IsValid() && timeLineTriggers[x].GetTimelinePlayableDirector().playableGraph.IsPlaying())
                {
                    yield return new WaitForFixedUpdate();
                }

                if (timeLineTriggers[x].GetEndTriggerType() == TimelineTriggerEndCondition.EndWhenPlayerTouchesGroundAndNotInGimmick)
                {
                    while (player.GetGrounded() == false || player.GetGimmickManager().GetActiveGimmickMode() != GimmickMode.None)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
        }

        cutsceneController.GetAudioSource().Stop();
        player.GetInputManager().SetInputOverride(new Vector2(0, 0));
        player.GetInputManager().enabled = true;
        player.SetPlayerState(PlayerState.Awake);
        onCutsceneEndAction();

        if (this.actClearCutsceneCoroutine != null)
        {
            this.actClearCutsceneCoroutine = null;
        }

        if (this.actStartCutsceneCoroutine != null)
        {
            this.actStartCutsceneCoroutine = null;
        }

        this.inputMaster.Disable();

        yield return null;
    }

    /// <summary>
    /// Moves the player to the target position precistly
    /// <param name="player">The player object to move </param>
    /// <param name="target">The transform to move towards </param>
    /// </summary>
    private IEnumerator MovePlayerToTarget(Player player, Transform target)
    {
        bool slowingDown = false;
        float targetSnatchRange = player.currentTopSpeed;

        while (player.transform.position.x != target.transform.position.x)
        {
            int directionOfTarget = (int)Mathf.Sign(target.transform.position.x - player.transform.position.x);
            float playerToTargetDistance = target.transform.position.x - player.transform.position.x;
            int framesTillGoalIsReached = (int)(Mathf.Abs(playerToTargetDistance) / player.groundVelocity);

            if (slowingDown == false)
            {
                player.GetInputManager().SetInputOverride(new Vector2(directionOfTarget, 0));
            }

            if (framesTillGoalIsReached <= 64 && player.groundVelocity >= 6)
            {
                slowingDown = true;
                player.GetInputManager().SetInputRestriction(InputRestriction.All);
                player.GetInputManager().SetInputOverride(Vector2.zero);
            }
            else if (framesTillGoalIsReached > 128)
            {
                slowingDown = false;
            }

            if (Mathf.Abs(playerToTargetDistance) < targetSnatchRange)
            {
                player.GetInputManager().SetInputOverride(Vector2.zero);

                if (player.GetGrounded())
                {
                    //Calculate the amount of velocity to get to the position
                    player.groundVelocity = Mathf.Sqrt(2 * Mathf.Abs(playerToTargetDistance) * player.currentFriction) * directionOfTarget;
                }

                if (player.GetGrounded() == false)
                {
                    player.velocity.x = playerToTargetDistance;
                    player.groundVelocity = 0;
                }

                while (player.groundVelocity != 0 && player.GetGrounded() == false)
                {
                    yield return new WaitForFixedUpdate();
                }

                player.groundVelocity = 0;
                player.velocity.x = 0;
                player.transform.position = new Vector2(target.transform.position.x, player.transform.position.y); //At this point the difference should be minor so snap the player the rest

                break;
            }

            yield return new WaitForFixedUpdate();
        }
        player.GetInputManager().SetInputOverride(new Vector2(0, 0));

        yield return null;
    }

    /// <summary>
    /// Get the act start cutscene value
    /// </summary>
    public CutsceneController GetActStartCutscene() => this.actStartCutscene;

    /// <summary>
    /// Get the act clcear cutscene value
    /// </summary>
    public CutsceneController GetActClearCutscene() => this.actClearCutscene;

    /// <summary>
    /// Moves the player to the target position roughly and maintain the velocity
    /// <param name="player">The player object to move </param>
    /// <param name="target">The transform to move towards </param>
    /// </summary>
    private IEnumerator MovePlayerToTargetExceeds(Player player, Transform Target)
    {
        float targetSnatchRange = player.currentTopSpeed;

        while (player.transform.position.x != Target.transform.position.x)
        {
            int directionOfTarget = (int)Mathf.Sign(Target.transform.position.x - player.transform.position.x);
            float playerToTargetDistance = Target.transform.position.x - player.transform.position.x;
            int framesTillGoalIsReached = (int)(Mathf.Abs(playerToTargetDistance) / player.groundVelocity);

            player.GetInputManager().SetInputOverride(new Vector2(directionOfTarget, 0));

            float topSpeed = player.currentTopSpeed > player.groundVelocity ? player.currentTopSpeed : player.groundVelocity;
            if (Mathf.Abs(playerToTargetDistance) < topSpeed)
            {
                player.GetInputManager().SetInputOverride(Vector2.zero);

                if (player.GetGrounded())
                {
                    //Calculate the amount of velocity to get to the position
                    player.groundVelocity = Mathf.Sqrt(2 * Mathf.Abs(playerToTargetDistance) * player.currentFriction) * directionOfTarget;
                }

                if (player.GetGrounded() == false)
                {
                    player.velocity.x = playerToTargetDistance;
                    player.groundVelocity = 0;
                }

                while (player.groundVelocity != 0 && player.GetGrounded() == false)
                {
                    yield return new WaitForFixedUpdate();
                }

                player.transform.position = new Vector2(Target.transform.position.x, player.transform.position.y); //At this point the difference should be minor so snap the player the rest

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.endCutsceneCameraBounds.GetTopLeftBorderPosition(), this.endCutsceneCameraBounds.GetTopRightBorderPosition());//Top Horizontal Bounds
        Gizmos.DrawLine(this.endCutsceneCameraBounds.GetBottomLeftBorderPosition(), this.endCutsceneCameraBounds.GetBottomRightBorderPosition());//Bottom Horizontal Bounds
        Gizmos.DrawLine(this.endCutsceneCameraBounds.GetTopLeftBorderPosition(), this.endCutsceneCameraBounds.GetBottomLeftBorderPosition());//Left Vertical Bounds
        Gizmos.DrawLine(this.endCutsceneCameraBounds.GetTopRightBorderPosition(), this.endCutsceneCameraBounds.GetBottomRightBorderPosition());//Right Vertical Bounds

        if (this.endCutsceneBoundsText != null)
        {
            this.endCutsceneBoundsText.transform.position = this.endCutsceneCameraBounds.GetCenterPosition();
        }
    }
}
