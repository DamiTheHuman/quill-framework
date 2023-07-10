using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// The Corkscrew Gimmick - By transforming  the player through a specified route based on his speed
///  Original Author - Lark SS [Sonic Worlds] 
///  Added functionality - The corkscrew runs responsively to the angle of the transform
/// </summary>
public class CorkscrewController : MonoBehaviour
{
    private Player player;
    [Tooltip("The Layer of the players solid box")]
    [SerializeField, LayerList] public int solidBoxLayer = 27;
    [Tooltip("The direction the corkscrew is currently going in")]
    public CorkscrewSpinDirection currentCorkscrewSpinDirection = CorkscrewSpinDirection.Left;

    [Tooltip("The left most point of the corkscrew"), FirstFoldOutItem("Corkscrew Integral Points")]
    public Transform leftIntegralPoint;
    [Tooltip("The right most point of the corkscrew"), LastFoldoutItem()]
    public Transform rightIntegralPoint;

    [Tooltip("The Spin Height Of The Corkscrew"), FirstFoldOutItem("Corkscrew Info")]
    public float spinHeight = 36;
    [Tooltip("The Spin width of the corkscrew")]
    public float spinWidth = 384;
    [Tooltip("The total length of the corkscrew spin"), LastFoldoutItem()]
    public int spinLength = 46;

    [Tooltip("The slowest we can be to enter and stick to the corkscrew"), FirstFoldOutItem("Active Spin Info")]
    public float minCorkscrewStickSpeed = 3f;
    [Tooltip("Check how far the player is on the corkscrew")]
    public float spinProgress;
    [Tooltip("The current player angle on the corkscrew"), SerializeField]
    private float progressAngleInDegrees;
    [Tooltip("Check if the player is on the corkscrew")]
    public bool onCorkscrew = false;
    [Tooltip("Check if the player was rolling while coming in contact with the corkscrew so we maintain the state"), LastFoldoutItem]
    public bool rollingOnContact;

    [Tooltip("Values in to move the player along the corkscrew"), SerializeField, FirstFoldOutItem("Debug Corkscrew"), LastFoldoutItem]
    private Vector2 spinValues;

    public void Start()
    {
        CorkscrewIntegralPointController leftCorkscrewController = this.leftIntegralPoint.GetComponent<CorkscrewIntegralPointController>();
        CorkscrewIntegralPointController rightCorkscrewController = this.rightIntegralPoint.GetComponent<CorkscrewIntegralPointController>();
        leftCorkscrewController.SetCorkScrewController(this, CorkscrewSpinDirection.Left);
        rightCorkscrewController.SetCorkScrewController(this, CorkscrewSpinDirection.Right);
    }

    private void FixedUpdate()
    {
        if (this.onCorkscrew)
        {
            this.UpdateCorkScrewSpin();
        }
    }
    /// <summary>
    /// Begin the corkscrew spin if the required conditions are met
    /// </summary/>
    public void BeginCorkScrewSpin(Player player, CorkscrewSpinDirection corkscrewDirection, float startProgress)
    {
        if (corkscrewDirection == CorkscrewSpinDirection.Left && player.groundVelocity > this.minCorkscrewStickSpeed)
        {
            this.spinProgress = startProgress;
        }
        else if (corkscrewDirection == CorkscrewSpinDirection.Right && player.groundVelocity < this.minCorkscrewStickSpeed)
        {
            this.spinProgress = (360 * this.spinLength / 46) + startProgress;
        }

        if (Mathf.Abs(player.groundVelocity) > this.minCorkscrewStickSpeed)
        {
            this.player = player;
            this.currentCorkscrewSpinDirection = corkscrewDirection;
            player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.OnCorkscrew);
            player.SetMovementRestriction(MovementRestriction.Both);
            this.onCorkscrew = true;
            this.rollingOnContact = player.GetActionManager().CheckActionIsBeingPerformed<Roll>();
            if (this.rollingOnContact == false)
            {
                player.GetAnimatorManager().SwitchGimmickSubstate(GimmickSubstate.CorkscrewSpin);
            }
        }
    }
    /// <summary>
    /// Updates the players movement while on the corkscrew
    /// </summary/>
    public void UpdateCorkScrewSpin()
    {
        //Ends the spin if the player jumps and falls below the minimum speed
        if (this.player.GetComponent<ActionManager>().CheckActionIsBeingPerformed<Jump>() || Mathf.Abs(this.player.groundVelocity) < this.minCorkscrewStickSpeed)
        {
            this.EndCorkscrewSpin();

            return;
        }
        //Move smoothly across the corkscrew based on the players speed
        float calculatedVelocity = this.player.groundVelocity;
        /// HighSpeedMovement Similar to the version in  <see cref="Sensors"/> 
        if (this.player.GetSensors().GetAllowHighSpeedMovement() && Mathf.Abs(calculatedVelocity) > GMStageManager.Instance().GetMaxBlockSize())
        {
            this.PerformHighSpeedCorkscrewMovement(calculatedVelocity);
        }
        else
        {
            this.spinProgress += calculatedVelocity * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
            this.CalculateAndMovePlayer(this.spinProgress);
        }

    }

    /// <summary>
    /// Calculates the players position and animations and moves the player
    /// <param name="spinProgress">The current spin progress</param>
    /// </summary>
    private void CalculateAndMovePlayer(float spinProgress)
    {
        this.UpdateXAxisMovement(spinProgress);
        this.UpdateYAxisMovement(spinProgress);
        this.UpdateCorkscrewAngle(spinProgress);

        if (this.rollingOnContact == false)
        {
            this.UpdatePlayerCorkscrewAnimation(spinProgress);
        }

        //When not rolling update the corkscrew animation based on movement
        this.UpdatePlayerValues();
    }

    /// <summary>
    /// Update the active spin values along the X Axis
    /// <param name="spinProgress">The current spin progress relative to the corkscrew</param>
    /// </summary/>
    private void UpdateXAxisMovement(float spinProgress)
    {
        this.spinValues.x = this.leftIntegralPoint.transform.position.x;
        this.spinValues.x += Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad) * spinProgress / 360 * this.spinWidth;
        this.spinValues.x += Mathf.Cos((this.transform.eulerAngles.z + 90) * Mathf.Deg2Rad) * Mathf.Cos(spinProgress * Mathf.Deg2Rad) * -this.spinHeight;
        this.spinValues.x += this.spinHeight * Mathf.Cos((this.transform.eulerAngles.z + 90) * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Update the active spin values along the Y Axis
    /// <param name="spinProgress">The current spin progress relative to the corkscrew</param>
    /// </summary/>
    private void UpdateYAxisMovement(float spinProgress)
    {
        this.spinValues.y = this.leftIntegralPoint.transform.position.y;
        this.spinValues.y += Mathf.Cos((this.transform.eulerAngles.z - 90) * Mathf.Deg2Rad) * spinProgress / 360 * this.spinWidth;
        this.spinValues.y += Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad) * Mathf.Cos(spinProgress * Mathf.Deg2Rad) * -this.spinHeight;
        this.spinValues.y += this.spinHeight * Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Calculates a pseudo angle for the player based on the current spin progress 
    /// <param name="spinProgress">The current spin progress relative to the corkscrew</param>
    /// </summary/>
    private void UpdateCorkscrewAngle(float spinProgress)
    {
        this.progressAngleInDegrees = (360 + (22.5f * Mathf.Sin(spinProgress * Mathf.Deg2Rad))) % 360;

        if (this.progressAngleInDegrees > 180)
        {
            this.progressAngleInDegrees -= 360;
        }
    }

    /// <summary>
    /// Update the index of the corkscrew animation based on the current spin progress
    /// <param name="spinProgress">The current spin progress relative to the corkscrew</param>
    /// </summary/>
    private void UpdatePlayerCorkscrewAnimation(float spinProgress)
    {
        float animationFrameIndex = (int)Mathf.Abs(spinProgress / 15) % this.player.GetAnimatorManager().corkscrewSpinAnimationLength;
        animationFrameIndex /= this.player.GetAnimatorManager().corkscrewSpinAnimationLength - 1;
        this.player.GetAnimatorManager().PlayAnimation("Run Corkscrew Spin", animationFrameIndex);
    }

    /// <summary>
    /// Updates the players gimmickangle, sprite angle and positions based on the spin values
    /// </summary/>
    private void UpdatePlayerValues()
    {
        this.player.GetGimmickManager().SetGimmickAngle(this.progressAngleInDegrees + this.transform.eulerAngles.z);
        this.player.GetSpriteController().SetSpriteAngle(this.transform.eulerAngles.z);
        this.player.transform.position = this.spinValues;
    }

    /// <summary>
    /// Handles the player movement at incredibly high speeds exceeding 16 pixels per step while grounded
    /// This is  used to help avoid clipping to level terrain and missing interacts with gimmicks alongside mainting footing on high speed slopes similarly to the one in sensors
    /// <param name="corkscrewVelocity">The players speed on the corkscrew</param>
    /// </summary>
    private void PerformHighSpeedCorkscrewMovement(float corkscrewVelocity)
    {
        int iterations = (int)((Mathf.Abs(corkscrewVelocity) / GMStageManager.Instance().GetMaxBlockSize()) + 1);
        int direction = (int)Mathf.Clamp(corkscrewVelocity, -1, 1);
        float[] velocitySplts = Enumerable.Repeat(16f, iterations).ToArray();
        float sumOfVelocitySplits = velocitySplts.Sum();

        if (sumOfVelocitySplits > corkscrewVelocity)
        {
            velocitySplts[iterations - 1] = sumOfVelocitySplits - corkscrewVelocity;
        }

        this.player.velocity = this.player.CalculateSlopeMovement(this.player.groundVelocity);

        for (int x = 0; x < velocitySplts.Length; x++)
        {
            //Cause we are breaking convention if the player is ever null break  cause it means they have exceeded the corkscrew
            velocitySplts[x] *= direction;
            if (this.player == null)
            {
                break;
            }

            this.spinProgress += velocitySplts[x] * GMStageManager.Instance().GetPhysicsMultiplier() * Time.deltaTime;
            this.CalculateAndMovePlayer(this.spinProgress);
            this.player.GetHitBoxController().CheckHitBoxCollisions();
            this.player.GetSensors().MoveAndCollide(this.spinValues);
        }
    }
    /// <summary>
    /// Énd the corkscrew spin and revert the player back to the regular state
    /// </summary/>
    public void EndCorkscrewSpin()
    {
        this.player.GetAnimatorManager().SwitchGimmickSubstate(0);
        this.player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        this.player.SetMovementRestriction(MovementRestriction.None);
        this.onCorkscrew = false;
        this.currentCorkscrewSpinDirection = CorkscrewSpinDirection.None;
        this.rollingOnContact = false;
        this.spinProgress = 0;
        this.player.GetGimmickManager().SetGimmickAngle(0);
        this.player = null;
    }

    private void OnDrawGizmos()
    {
        float debugSpinProgress = 0;
        Vector2 debugSpinValues = new Vector2(0, 0);
        List<Vector2> spinValueLog = new List<Vector2>();

        for (int x = 0; x < this.spinLength; x++)
        {
            debugSpinValues.x = this.leftIntegralPoint.transform.position.x;
            debugSpinValues.x += Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad) * debugSpinProgress / 360 * this.spinWidth;
            debugSpinValues.x += Mathf.Cos((this.transform.eulerAngles.z + 90) * Mathf.Deg2Rad) * Mathf.Cos(debugSpinProgress * Mathf.Deg2Rad) * -this.spinHeight;
            debugSpinValues.x += this.spinHeight * Mathf.Cos((this.transform.eulerAngles.z + 90) * Mathf.Deg2Rad);

            debugSpinValues.y = this.leftIntegralPoint.transform.position.y;
            debugSpinValues.y += Mathf.Cos((this.transform.eulerAngles.z - 90) * Mathf.Deg2Rad) * debugSpinProgress / 360 * this.spinWidth;
            debugSpinValues.y += Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad) * Mathf.Cos(debugSpinProgress * Mathf.Deg2Rad) * -this.spinHeight;
            debugSpinValues.y += this.spinHeight * Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad);

            if (x == 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(debugSpinValues, 2f);
            }
            else if (x == this.spinLength - 1)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(debugSpinValues, 2f);
            }

            Gizmos.color = Color.white;
            spinValueLog.Add(debugSpinValues);

            if (x > 0)
            {
                Gizmos.DrawLine(spinValueLog[x], spinValueLog[x - 1]);
            }

            debugSpinProgress += 8;
        }

        Gizmos.color = Color.red;
    }
}
