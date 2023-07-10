using UnityEngine;
/// <summary>
/// Duplicate of <see cref="AnimatorManager"/> but for special stages
/// </summary>
public class SpecialStageAnimatorManager : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private SpecialStagePlayer player;
    [SerializeField, LastFoldoutItem()]
    private Animator animator;
    private int substateHash;
    private int actionSubstateHash;
    private int switchSubstateHash;
    private int speedMultiplierHash;

    private void Reset()
    {
        this.player = this.GetComponent<SpecialStagePlayer>();
        this.animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.SetAnimatorHashValues();

        if (this.player == null)
        {
            this.Reset();
        }
    }

    /// <summary>
    /// Sets the hash values for the animator parameters
    /// </summary>
    private void SetAnimatorHashValues()
    {
        this.substateHash = Animator.StringToHash("Substate");
        this.actionSubstateHash = Animator.StringToHash("ActionSubstate");
        this.switchSubstateHash = Animator.StringToHash("SwitchSubstate");
        this.speedMultiplierHash = Animator.StringToHash("SpeedMultiplier");
    }

    public void UpdatePlayerAnimations() => this.UpdateSubStates();

    /// <summary>
    /// Updates the player substates allowing switching between branches instead of having a messy inspector
    /// </summary>
    private void UpdateSubStates()
    {
        if (this.player.velocity.x > 0 || GMSpecialStageManager.Instance().GetSpecialStageSlider().GetIsMoving())
        {
            this.animator.SetFloat(this.speedMultiplierHash, Mathf.Max(1, GMSpecialStageManager.Instance().GetSpecialStageSlider().GetTravelSpeed() * 0.35f));
            this.SwitchSubstate(SubState.Moving);
        }
        else
        {
            this.SwitchSubstate(SubState.Idle);
        }
    }

    /// <summary>
    /// Updates the main substate of the animator which could be whether the player is grounded or in an aerial state
    /// <param name="newState">The new substate to branc to </param>
    /// </summary>
    public void SwitchSubstate(SubState newState) => this.BranchUpdate((int)newState, AnimationSubstateType.PrimarySubstate);

    /// <summary>
    /// Updates the action branch between primary actions such as jumps, rolls e.t.c
    /// <param name="newState">The new substate to branc to </param>
    /// </summary>
    public void SwitchActionSubstate(ActionSubState newState) => this.BranchUpdate((int)newState, AnimationSubstateType.ActionSubState);
    /// <summary>
    /// Switches been substates on the animator while also making use of triggers to force an update
    /// <param name="substateValue">The new substate to branc to </param>
    /// <param name="animationSubstate">The action substate to update to </param>
    /// </summary>
    private void BranchUpdate(int substateValue, AnimationSubstateType animationSubstate)
    {
        int subState = 0;
        switch (animationSubstate)
        {
            case AnimationSubstateType.PrimarySubstate:
                subState = this.substateHash;

                break;
            case AnimationSubstateType.ActionSubState:
                subState = this.actionSubstateHash;

                break;
            case AnimationSubstateType.SecondaryActionSubstate:
                break;
            default:
                subState = this.substateHash;

                break;
        }
        if (this.animator.GetInteger(subState) != substateValue)
        {
            this.animator.SetInteger(subState, substateValue);
            this.animator.SetTrigger(this.switchSubstateHash);
        }
    }
}
