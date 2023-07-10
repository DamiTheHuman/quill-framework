using System.Collections;
using UnityEngine;
/// <summary>
/// A slider which moves the player througout the player stage
/// </summary>
public class SpecialStageSliderController : MonoBehaviour
{
    [SerializeField]
    private bool isMoving = false;
    [FirstFoldOutItem("Dependencies")]
    [SerializeField, Tooltip("The center pivot")]
    private Transform cameraPivot;
    [SerializeField, Tooltip("The parent object of the player"), LastFoldoutItem()]
    private Transform playerParent;

    [Tooltip("How fast the player travels through the special stage"), SerializeField]
    private float currentTravelSpeed = 1;
    [SerializeField, Tooltip("The base travel speed")]
    private float baseTravelSpeed = 3;
    [SerializeField, Tooltip("The speed the player slides on the pivot")]
    private float basePivotSlideSpeed = 1;
    [SerializeField, Tooltip("The speed of the player when moving to the final position")]
    private float actClearTravelSpeed = 1;
    [Tooltip("The time it takes to go from stalled back to regular speed"), SerializeField, FirstFoldOutItem("Stall Parameters")]
    private float stallTimeInSeconds = 1;
    [Tooltip("The current travel speed of the player after getting stalled"), SerializeField]
    private float stallTravelSpeed = 1;
    [Tooltip("Whether the player travel speed is currently being stalled"), SerializeField, LastFoldoutItem()]
    private bool isStalled = false;

    [SerializeField]
    [Tooltip("The current delta of the platform movement affecting its position within its lifetime"), FirstFoldOutItem("Half Pipe Settings")]
    private float angle = 0;
    [Tooltip("The radius of length of the platforms movement boundaries"), SerializeField]
    private float range = 120;
    [SerializeField, LastFoldoutItem(), Tooltip("Position of the slider on start")]
    private Vector2 startPosition;

    private void Start()
    {
        this.startPosition = this.transform.position;
        HedgehogCamera.Instance().SetCameraTarget(this.gameObject);
        GMSpecialStageManager.Instance().GetPlayer().transform.parent = this.playerParent;
        this.isStalled = false;
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;
        this.CalculateTravelSpeed();

        if (this.currentTravelSpeed > 0)
        {
            this.isMoving = true;
        }

        if (GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Idle)
        {
            return;
        }

        this.transform.position += (Vector3)(new Vector2(0, this.currentTravelSpeed) * GMStageManager.Instance().GetPhysicsMultiplier()) * delta;//Move the player by the current velocity

        Vector2 position = this.transform.position;
        position.x = this.startPosition.x + (Mathf.Cos(delta * Mathf.Deg2Rad) * this.range);
        position.y = this.startPosition.y + (Mathf.Sin(delta * Mathf.Deg2Rad) * this.range);
        this.angle += GMSpecialStageManager.Instance().GetPlayer().GetInputManager().GetCurrentInput().x * (this.currentTravelSpeed * 0.75f) * GMStageManager.Instance().GetPhysicsMultiplier() * delta;
        this.angle = General.ClampDegreeAngles(this.angle);

        if (this.angle is < 270 and >= 180)
        {
            if (GMSpecialStageManager.Instance().GetPlayer().GetGrounded())
            {
                this.HalfPipeJump();
            }

            this.angle = 270;
        }
        else if (this.angle is <= 180 and > 90)
        {
            if (GMSpecialStageManager.Instance().GetPlayer().GetGrounded())
            {
                this.HalfPipeJump();
            }

            this.angle = 90;
        }

        this.CalculatePlayerPosition();
    }

    /// <summary>
    /// Get the end travel speed
    /// </summary>
    public float GetEndTravelSpeed() => this.actClearTravelSpeed;

    /// <summary>
    /// Allow the player to jump vertically while on the half pipe
    /// </summary>
    private void HalfPipeJump()
    {
        SpecialStagePlayer player = GMSpecialStageManager.Instance().GetPlayer();
        player.GetActionManager().PerformAction<SpecialStageJump>();
        player.velocity.y = -6;
        player.velocity.x = 0;
        this.StartCoroutine(this.WatchAndStartHalfPipeSlide());
    }

    /// <summary>
    /// Starts the slide the player to the middle coroutine
    /// </summary>
    public void SlideToMiddle() => this.StartCoroutine(this.SlideToMiddleCoroutine());
    /// <summary>
    /// Slides the player to the middle of the half pope
    /// </summary>
    private IEnumerator SlideToMiddleCoroutine()
    {
        SpecialStagePlayer player = GMSpecialStageManager.Instance().GetPlayer();

        yield return new WaitUntil(() => player.GetGrounded());

        while (true)
        {
            float delta = Time.deltaTime;
            float speed = GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Clear ? this.actClearTravelSpeed : this.basePivotSlideSpeed * GMStageManager.Instance().GetPhysicsMultiplier() * delta;
            if (this.angle >= 270)
            {
                this.angle = Mathf.MoveTowards(this.angle, 360, speed);
            }
            else if (this.angle >= 0)
            {
                this.angle = Mathf.MoveTowards(this.angle, 0, speed);
            }

            this.CalculatePlayerPosition();

            if (this.angle is 0 or 360)
            {
                this.angle = 0;

                break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    /// <summary>
    /// Slides the player down the half pipe after successfullly performing a half pipe jump
    /// </summary>
    private IEnumerator WatchAndStartHalfPipeSlide()
    {
        SpecialStagePlayer player = GMSpecialStageManager.Instance().GetPlayer();

        yield return new WaitUntil(() => player.GetGrounded());

        player.GetInputManager().SetLockControls(10, InputRestriction.All);
        //While the player input is still locked move them towards the center
        IEnumerator slideToMiddleCoroutine = this.SlideToMiddleCoroutine();

        if (this.angle is (>= 75 and <= 90) or (>= 255 and <= 270))
        {
            this.StartCoroutine(slideToMiddleCoroutine);
            while (slideToMiddleCoroutine != null)
            {
                if (player.GetInputManager().GetCurrentInput().x == 0 && player.GetGrounded() && player.GetInputManager().GetInputRestriction() == InputRestriction.All)
                {
                    yield return new WaitForFixedUpdate();

                    continue;
                }
                this.StopCoroutine(slideToMiddleCoroutine);
                slideToMiddleCoroutine = null;

                break;
            }
        }

        player.GetInputManager().SetInputRestriction(InputRestriction.None);

        yield return null;
    }

    /// <summary>
    /// Calculate the speed the player moves at
    /// </summary>
    public void CalculateTravelSpeed()
    {
        if (GMSpecialStageManager.Instance().GetSpecialStageState() != SpecialStageState.Running)
        {
            this.currentTravelSpeed = 0;

            return;
        }
        else if (GMSpecialStageManager.Instance().GetSpecialStageState() == SpecialStageState.Clear)
        {
            this.currentTravelSpeed = this.actClearTravelSpeed;

            return;
        }

        this.currentTravelSpeed = this.isStalled == false ? this.baseTravelSpeed : this.stallTravelSpeed;
    }

    /// <summary>
    /// Get the position of the player relative to  the camera position and angle;
    /// </summary>
    public Vector3 GetPositionAtAngle(float angle)
    {
        Vector3 position = this.cameraPivot.position;

        Vector3 playerPivotPosition = position;

        playerPivotPosition.x = position.x + (Mathf.Cos((angle - 90) * Mathf.Deg2Rad) * this.range);
        playerPivotPosition.z = position.z + (Mathf.Sin((angle + 90) * Mathf.Deg2Rad) * this.range);

        return playerPivotPosition;
    }

    /// <summary>
    /// Get the range of the pivot circle
    /// </summary>
    public float GetRange() => this.range;
    /// <summary>
    /// Sets the angle in degrees
    /// </summary>
    public void SetAngle(float angleInDegrees) => this.angle = General.ClampDegreeAngles(angleInDegrees);
    /// <summary>
    /// Getts the current angle
    /// </summary>
    public float GetAngle() => this.angle;
    /// <summary>
    /// Calculates the player poistition relative to the angle and camera pivot
    /// </summary>
    public Vector2 CalculatePlayerPosition()
    {
        Vector3 position = this.cameraPivot.position;

        Vector3 rotation = this.cameraPivot.eulerAngles;
        Vector3 playerPivotPosition = this.GetPositionAtAngle(this.angle);

        if (GMSpecialStageManager.Instance().GetPlayer() != null)
        {
            this.playerParent.position = playerPivotPosition;
            rotation.x += -90;
            rotation.y += -45;

            if (GMSpecialStageManager.Instance().GetPlayer().GetGrounded())
            {
                GMSpecialStageManager.Instance().GetPlayer().GetSpriteController().transform.localEulerAngles = new Vector3(0, 0, this.angle);
                GMSpecialStageManager.Instance().GetPlayer().transform.eulerAngles = rotation;
            }
        }

        return playerPivotPosition;
    }

    public Transform GetPlayerParent() => this.playerParent;

    /// <summary>
    /// Sets the speed the player will be moved at
    /// <param name="travelSpeed">The current travel speed of the player</param>
    /// </summary>
    public void SetTravelSpeed(float travelSpeed)
    {
        if (travelSpeed == 0)
        {
            this.isMoving = false;
        }

        this.currentTravelSpeed = travelSpeed;
    }

    /// <summary>
    /// Gets the current speed the player will be moved at
    /// </summary>
    public float GetTravelSpeed() => this.currentTravelSpeed;
    /// <summary>
    /// Set the is moving state
    /// <param name="isMoving">The new value of is moving</param>
    /// </summary>
    public void SetIsMoving(bool isMoving) => this.isMoving = isMoving;
    /// <summary>
    /// Gets the current value for is moving
    /// </summary>
    public bool GetIsMoving() => this.isMoving;

    /// <summary>
    /// Stall the player briefly after getting hit
    /// </summary>
    public void StartDamageStall()
    {
        this.isStalled = true;
        this.StartCoroutine(this.DamageStallCoroutine(this.currentTravelSpeed));
    }

    /// <summary>
    /// Slowly ramp up the player speed back to normal after getting hit
    /// <param name="targetTravelSpeed">The target speed to reach</param>
    /// </summary>
    private IEnumerator DamageStallCoroutine(float targetTravelSpeed)
    {
        this.stallTravelSpeed = 0;

        for (float t = 0; t < 1; t += Time.deltaTime / this.stallTimeInSeconds)
        {
            this.stallTravelSpeed = Mathf.Lerp(0, targetTravelSpeed, t);

            yield return new WaitForEndOfFrame();
        }

        this.stallTravelSpeed = 1;
        this.isStalled = false;

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Vector3 position = Application.isPlaying ? (Vector2)HedgehogCamera.Instance().GetStartPosition() : (Vector2)HedgehogCamera.Instance().transform.position;

        Vector3 rotation = this.cameraPivot.transform.eulerAngles;
        position = this.cameraPivot.transform.position;
        GizmosExtra.Draw3DCircle(position, rotation, this.range, Color.red);

        Vector3 playerPivotPosition = position;

        playerPivotPosition.x = position.x + (Mathf.Cos((this.angle - 90) * Mathf.Deg2Rad) * this.range);
        playerPivotPosition.z = position.z + (Mathf.Sin((this.angle + 90) * Mathf.Deg2Rad) * this.range);
        GizmosExtra.Draw3DCircle(playerPivotPosition, rotation, 8, Color.yellow);
    }
}
