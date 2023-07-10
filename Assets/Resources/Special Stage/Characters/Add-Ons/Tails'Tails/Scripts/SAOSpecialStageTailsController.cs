using UnityEngine;

public class SAOSpecialStageTailsController : HedgeSpecialStageSpriteAddOnController
{
    [Tooltip("The current substate of the Tails'Tails"), SerializeField]
    private int currentSubstate = 0;
    [SerializeField, Tooltip("Current active speed multipler")]
    private float currentSpeedMultiplier;
    private int speedMultiplierHash;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        this.SwitchSubstate(0);
        this.speedMultiplierHash = Animator.StringToHash("SpeedMultiplier");
    }

    public override void UpdateAddOnInfo()
    {
        this.transform.localScale = new Vector3(1, 1, 0);
        this.UpdateAnimationSpeeds();
        this.currentSubstate = this.CalculateNewSubstate();
        this.SwitchSubstate(this.currentSubstate);
        this.animator.SetFloat(this.speedMultiplierHash, this.currentSpeedMultiplier);
    }

    /// <summary>
    /// Calculates the value for the substate the Tails'Tails should be in based on the player states
    /// </summary>
    public int CalculateNewSubstate()
    {
        SpecialStagePlayer player = this.GetPlayer();
        int newSubstate = 0;

        if (player.GetGrounded() == false)
        {
            newSubstate = -1;
        }

        return newSubstate;
    }

    /// <summary>
    /// Update the animation speed of the tails
    /// </summary>
    private void UpdateAnimationSpeeds()
    {
        SpecialStagePlayer player = this.GetPlayer();
        this.currentSpeedMultiplier = Mathf.Max(1, GMSpecialStageManager.Instance().GetSpecialStageSlider().GetTravelSpeed() * 0.67f);
    }
}
