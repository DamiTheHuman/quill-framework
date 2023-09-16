using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Player), typeof(Animator))]
/// <summary>
/// The class which handles the animations of the player
/// </summary>
public class AnimatorManager : MonoBehaviour
{
    [SerializeField, FirstFoldOutItem("Dependencies")]
    private Player player;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Sensors sensors;
    [SerializeField]
    private GimmickManager gimmickManager;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField, LastFoldoutItem()]
    private ActionManager actionManager;

    [SerializeField]
    private AnimatorUpdateMode updateMode = AnimatorUpdateMode.Regular;

    [FirstFoldOutItem("Animation Information")]
    [Tooltip("The current animation frame for the animation")]
    public int currentAnimationFrame = 0;
    [Tooltip("The length of the characters air walk animation")]
    public int airWalkAnimationLength = 10;
    [Tooltip("The length of the characters corkscrew spin animation")]
    public int corkscrewSpinAnimationLength = 24;
    [Tooltip("The Speed multiplier for the walk animation")]
    public float walkSpeedMultiplier = 6f;
    [Tooltip("The Speed multiplier for the jog animation")]
    public float jogSpeedMultiplier = 9f;
    [Tooltip("The Speed multiplier for the jump animation")]
    public float jumpSpeedMultiplier = 16f;
    [Tooltip("The Speed multiplier for the run animation")]
    public float runSpeedMultiplier = 10f;
    [Tooltip("The Speed multiplier for the dash animation")]
    public float dashSpeedMultiplier = 12.5f;
    [Tooltip("The Speed multiplier for the spindash animation")]
    public float spinDashMultiplier = 6f;
    [Tooltip("The Speed multiplier for flight animation"), LastFoldoutItem]
    public float flySpeedMultiplier = 40f;

    [FirstFoldOutItem("Animation End Status")]
    public bool waitTillAnimationEnds = false;
    [LastFoldoutItem]
    public string currentlyCheckAnimation = "";
    private float currentSpeedMultiplier;

    //Animator Values hashes
    private int characterHash;
    private int characterFloatHash;
    private int physicsStateHash;
    private int speedMultiplierHash;
    private int absGroundVelocityHash;
    private int groundVelocityHash;
    private int velocityXHash;
    private int velocityYHash;
    private int yInputHash;
    private int xInputHash;
    private int groundedHash;
    private int substateHash;
    private int actionSubstateHash;
    private int secondaryActionSubstateHash;
    private int switchSubstateHash;
    private int gimmickSubstateHash;
    private int switchGimmickSubstateHash;
    private int otherAnimationSubstateHash;
    private int extraCharacterInfoHash;

    private void Reset()
    {
        this.player = this.GetComponent<Player>();
        this.animator = this.GetComponent<Animator>();
        this.sensors = this.GetComponent<Sensors>();
        this.inputManager = this.GetComponent<InputManager>();
        this.actionManager = this.GetComponent<ActionManager>();
        this.gimmickManager = this.GetComponent<GimmickManager>();
    }
    private void Start()
    {
        this.SetAnimatorHashValues();

        if (this.player == null)
        {
            this.Reset();
        }
    }
    /// <summary>
    /// Gets a reference to the animator
    /// </summary>
    public Animator GetAnimator() => this.animator;

    /// <summary>
    /// Sets the hash values for the animator parameters
    /// </summary>
    private void SetAnimatorHashValues()
    {
        this.characterHash = Animator.StringToHash("Character");
        this.characterFloatHash = Animator.StringToHash("CharacterFloat");
        this.physicsStateHash = Animator.StringToHash("PhysicsState");
        this.speedMultiplierHash = Animator.StringToHash("SpeedMultiplier");
        this.absGroundVelocityHash = Animator.StringToHash("AbsGroundVelocity");
        this.groundVelocityHash = Animator.StringToHash("GroundVelocity");
        this.velocityYHash = Animator.StringToHash("VelocityY");
        this.velocityXHash = Animator.StringToHash("VelocityX");
        this.yInputHash = Animator.StringToHash("YInput");
        this.xInputHash = Animator.StringToHash("XInput");
        this.groundedHash = Animator.StringToHash("Grounded");
        this.substateHash = Animator.StringToHash("Substate");
        this.actionSubstateHash = Animator.StringToHash("ActionSubstate");
        this.secondaryActionSubstateHash = Animator.StringToHash("SecondaryActionSubstate");
        this.switchSubstateHash = Animator.StringToHash("SwitchSubstate");
        this.gimmickSubstateHash = Animator.StringToHash("GimmickSubstate");
        this.switchGimmickSubstateHash = Animator.StringToHash("SwitchGimmickSubstate");
        this.otherAnimationSubstateHash = Animator.StringToHash("OtherAnimationSubstate");
        this.extraCharacterInfoHash = Animator.StringToHash("ExtraCharacterInfo");
    }
    /// <summary>
    /// Updates the current player animation
    /// </summary>
    public void UpdatePlayerAnimations()
    {
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<Victory>())
        {
            return;
        }

        //If the victory is playing no more updates
        this.UpdateCoreStateAnimatorVariables();
        this.UpdateSubStates();
        this.CalculateAnimationSpeedMultiplier();
        this.UpdateAnimatorVariables();
        this.UpdateVariableSpriteEffects();
    }

    /// <summary>
    /// Calculate the values for current speed multiplier based on the playrs conditions
    /// </summary>
    private void CalculateAnimationSpeedMultiplier()
    {
        float absoluteGroundVelocity = Mathf.Abs(this.player.groundVelocity);
        this.currentSpeedMultiplier = 20 + (Mathf.Abs(this.player.groundVelocity) * this.walkSpeedMultiplier);

        if (absoluteGroundVelocity >= 6 && absoluteGroundVelocity < 14 && this.player.GetGrounded())
        {
            this.currentSpeedMultiplier = 20 + (absoluteGroundVelocity * this.jogSpeedMultiplier);
        }
        else if (absoluteGroundVelocity >= 14 && absoluteGroundVelocity < 15 && this.player.GetGrounded())
        {
            this.currentSpeedMultiplier = 20 + (absoluteGroundVelocity * this.runSpeedMultiplier);
        }
        else if (absoluteGroundVelocity >= 15 && this.player.GetGrounded())
        {
            this.currentSpeedMultiplier = 30 + (absoluteGroundVelocity * this.dashSpeedMultiplier);
        }
        else if (this.actionManager.CheckActionIsBeingPerformed<Jump>() || this.animator.GetInteger("GimmickSubstate") == 7)
        {
            this.currentSpeedMultiplier = 20 + (absoluteGroundVelocity * this.jumpSpeedMultiplier);

            if (this.actionManager.currentSubAction is Fly)
            {
                this.currentSpeedMultiplier = 40 + Mathf.Max(0, this.player.velocity.y * this.flySpeedMultiplier);
            }
        }
        else if (this.actionManager.CheckActionIsBeingPerformed<Spindash>())
        {
            float spinRev = ((Spindash)this.actionManager.currentPrimaryAction).currentSpinRev;
            float minSpinRev = ((Spindash)this.actionManager.currentPrimaryAction).minSpindashRev;
            this.currentSpeedMultiplier = Mathf.Abs(minSpinRev + Mathf.Floor(spinRev / 2)) * this.spinDashMultiplier;
        }
    }
    /// <summary>
    /// Updates the core animator variables
    /// </summary>
    private void UpdateCoreStateAnimatorVariables()
    {
        int character = (int)GMCharacterManager.Instance().currentCharacter;
        this.animator.SetInteger(this.characterHash, character);
        this.animator.SetFloat(this.characterFloatHash, character);
        this.animator.SetInteger(this.physicsStateHash, (int)this.player.GetPhysicsState());
    }

    /// <summary>
    /// Updates all the values of the animator variables
    /// </summary>
    private void UpdateAnimatorVariables()
    {
        this.animator.SetFloat(this.speedMultiplierHash, General.ConvertAnimationSpeed(this.animator, this.currentSpeedMultiplier));
        this.animator.SetFloat(this.absGroundVelocityHash, Mathf.Abs(this.player.groundVelocity));
        this.animator.SetFloat(this.groundVelocityHash, this.player.groundVelocity);
        this.animator.SetFloat(this.velocityXHash, this.player.velocity.x);
        this.animator.SetFloat(this.velocityYHash, this.player.velocity.y);
        this.animator.SetInteger(this.yInputHash, (int)this.inputManager.GetCurrentInput().y);
        this.animator.SetInteger(this.xInputHash, (int)this.inputManager.GetCurrentInput().x);

        if (this.animator.GetBool(this.groundedHash) != this.player.GetGrounded() && this.player.GetGrounded() && this.player.GetGimmickManager().GetGroundedGimicks().Contains(this.player.GetGimmickManager().GetActiveGimmickMode()))
        {
            this.animator.SetInteger(this.gimmickSubstateHash, 0);
        }

        //Apply the super peel out to sonic alone
        if(GMCharacterManager.Instance().currentCharacter == PlayableCharacter.Sonic && this.player.GetActionManager().GetAction<SuperPeelOut>() != null)
        {
            this.animator.SetInteger(this.extraCharacterInfoHash, (int)ExtraCharacterInfo.HasSuperPeelOut);
        }

        this.animator.SetBool(this.groundedHash, this.player.GetGrounded());
    }

    /// <summary>
    /// Updates the animator for sprite effects that change according to the players state
    /// </summary>
    private void UpdateVariableSpriteEffects()
    {
        if (this.player.GetSpriteEffectsController().GetCurrentSpriteEffect() != null && this.player.GetSpriteEffectsController().GetCurrentSpriteEffect().GetTag() == SpriteEffectToggle.WaterRun)
        {
            this.player.GetSpriteEffectsController().GetCurrentSpriteEffect().SetAnimatorSpeedMultiplier(this.currentSpeedMultiplier);
        }
    }

    /// <summary>
    /// Updates the player substates allowing switching between branches instead of having a messy inspector
    /// </summary>
    private void UpdateSubStates()
    {
        this.currentAnimationFrame = this.GetCurrentAnimationFrame();

        //When going super always compute the air state... just incase 
        if (this.player.GetActionManager().CheckActionIsBeingPerformed<SuperTransform>())
        {
            this.SwitchSubstate(SubState.Aerial);

            return;
        }

        if (this.player.GetGrounded() && this.player.groundVelocity == 0)
        {
            this.SwitchSubstate(SubState.Idle);
        }
        else if (Mathf.Abs(this.player.groundVelocity) > 0 && this.player.GetGrounded())
        {
            this.AirWalkToWalkTransfer();
            this.SwitchSubstate(SubState.Moving);
        }
        else if (this.player.GetGrounded() == false)
        {
            this.SwitchSubstate(SubState.Aerial);
        }
    }

    /// <summary>
    /// Finds the animation frame of the current animation playing
    /// </summary>
    private int GetCurrentAnimationFrame()
    {
        if (this.animator.GetCurrentAnimatorClipInfo(0).Length > 0)
        {
            AnimationClip animatorClipInfo = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip;

            return (int)(animatorClipInfo.length * (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) * animatorClipInfo.frameRate);
        }

        return 0;
    }

    /// <summary>
    /// Get the normalized time of the currently playing animation
    /// </summary>
    public float GetCurrentAnimationNormalizedTime() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    /// <summary>
    /// Provides a smooth transition between the air walk and the walk state and vice versa
    /// </summary>
    private void AirWalkToWalkTransfer()
    {
        if (this.player.GetGrounded() == false && this.animator.GetInteger(this.actionSubstateHash) == 0)
        {
            //Transfer to the air walk animation smoothly
            if (this.AnimationIsPlaying("Air Walk") == false && this.player.groundVelocity < 276.99)
            {
                //The walk animations has 11 frames while air walk only has 10 frames so if the last frame is playing reduce by 1
                if (this.currentAnimationFrame == 11)
                {
                    this.currentAnimationFrame = 10;
                }
                this.animator.Play("Air Walk", 0, (float)this.currentAnimationFrame / this.airWalkAnimationLength);
            }
        }
        else if (this.player.GetGrounded())
        {
            //Transfer from the walk animation to the ground smoothly
            if (this.AnimationIsPlaying("Air Walk") && this.player.groundVelocity < 276.99)
            {
                this.animator.Play("Movement Cycles", 0, (float)this.currentAnimationFrame / this.airWalkAnimationLength);
            }
        }
    }

    /// <summary>
    /// Play an animation by its name in the animator
    /// <param name="animationName">The name of the animation to play</param>
    /// <param name="animationIndex">The offset in which the animation will be played from</param>
    /// </summary>
    public void PlayAnimation(string animationName, float animationIndex = 0) => this.animator.Play(animationName, 0, animationIndex);

    /// <summary>
    /// Sets the speed of the animator with 1 being normal, 0 being frozen and -1 being in reverse
    /// <param name="animationName">The speed to be set</param>
    /// </summary>
    public void SetAnimatorSpeed(float animatorSpeed) => this.animator.speed = animatorSpeed;
    /// <summary>
    /// Checks if a certain animation int eh animator is being played
    /// <param name="animationName">The name of the animation being played</param>
    /// </summary>
    public bool AnimationIsPlaying(string animationName)
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the current animation is at its end
    /// </summary>
    public bool CheckIfCurrentAnimationIsAtEnd() => this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;

    /// <summary>
    /// Get the name of the current animation being played
    /// </summary>
    public string GetCurrentAnimationName()
    {
        if (this.animator.GetCurrentAnimatorClipInfo(0).Length == 0)
        {
            return "";
        }
        return this.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    /// <summary>
    /// Gets the current animations length in frames
    /// </summary>
    public int GetCurrentAnimationLengthInFrames()
    {
        AnimationClip animatorClipInfo = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        return (int)(animatorClipInfo.length * 1 * animatorClipInfo.frameRate);
    }

    /// <summary>
    /// Updates the main substate of the animator which could be whether the player is grounded or in an aerial state
    /// <param name="newState">The new substate to branc to </param>
    /// <param name="switchAfterAnimation">What animation to play on the end if applicable</param>
    /// </summary>
    public void SwitchSubstate(SubState newState, string switchAfterAnimation = "") => this.BranchUpdate((int)newState, AnimationSubstateType.PrimarySubstate, switchAfterAnimation);

    /// <summary>
    /// Updates the action branch between primary actions such as jumps, rolls e.t.c
    /// <param name="newState">The new substate to branc to </param>
    /// <param name="switchAfterAnimation">What animation to play on the end if applicable</param>
    /// </summary>
    public void SwitchActionSubstate(ActionSubState newState, string switchAfterAnimation = "") => this.BranchUpdate((int)newState, AnimationSubstateType.ActionSubState, switchAfterAnimation);

    /// <summary>
    /// Updates the secondary action branch between  actions such as the dropdash,skid flip e.t.c
    /// <param name="newState">The new substate to branc to </param>
    /// <param name="switchAfterAnimation">What animation to play on the end if applicable</param>
    /// </summary>
    public void SwitchActionSecondarySubstate(SubActionSubState newState, string switchAfterAnimation = "") => this.BranchUpdate((int)newState, AnimationSubstateType.SecondaryActionSubstate, switchAfterAnimation);

    /// <summary>
    /// Set the other animation substate which stands for values the arent represented within the scopes of the substate
    /// <param name="value">The new value for the other animation substate </param>
    /// </summary>
    public void SetOtherAnimationSubstate(int value) => this.GetAnimator().SetInteger(this.otherAnimationSubstateHash, value);

    /// <summary>
    /// Switches beetween gimmick substates on the animator while also making use of triggers to force an update
    /// <param name="gimmickState">The new gimmick substate to branch to </param>
    /// </summary>
    public void SwitchGimmickSubstate(GimmickSubstate gimmickState)
    {
        if (this.animator.GetInteger(this.gimmickSubstateHash) != (int)gimmickState)
        {
            this.animator.SetInteger(this.gimmickSubstateHash, (int)gimmickState);
        }

        this.animator.SetTrigger(this.switchGimmickSubstateHash);

        //When switching from gimmick back to normal substate
        if (gimmickState == 0)
        {
            this.animator.SetTrigger(this.switchSubstateHash);
        }
    }

    /// <summary>
    /// Switches been substates on the animator while also making use of triggers to force an update
    /// <param name="substateValue">The new substate to branc to </param>
    /// <param name="animationSubstate">The action substate to update to </param>
    /// <param name="switchAfterAnimation">What animation to play on the end if applicable</param>
    /// </summary>
    private void BranchUpdate(int substateValue, AnimationSubstateType animationSubstate = AnimationSubstateType.PrimarySubstate, string switchAfterAnimation = "", bool skipTrigger = false)
    {
        if (this.updateMode == AnimatorUpdateMode.NoUpdates)
        {
            return;
        }

        int subState = 0;
        subState = animationSubstate switch
        {
            AnimationSubstateType.PrimarySubstate => this.substateHash,
            AnimationSubstateType.ActionSubState => this.actionSubstateHash,
            AnimationSubstateType.SecondaryActionSubstate => this.secondaryActionSubstateHash,
            _ => this.substateHash,
        };

        if (this.animator.GetInteger(subState) != substateValue)
        {
            if (switchAfterAnimation == "")
            {
                this.animator.SetInteger(subState, substateValue);

                if (this.updateMode != AnimatorUpdateMode.UpdateWithoutFiringSwitchTrigger)
                {
                    this.animator.SetTrigger(this.switchSubstateHash);
                }
            }
            else
            {
                this.StartCoroutine(this.WaitTillAnimationEnds(substateValue, switchAfterAnimation, animationSubstate));//Begin waiting
            }
        }
    }

    /// <summary>
    /// Get a reference to the main sub state hash in the animator
    /// </summary>
    public int GetSubstateHash() => this.substateHash;

    /// <summary>
    /// Get a reference to the action sub state hash in the animator
    /// </summary>
    public int GetActionSubStateHash() => this.actionSubstateHash;

    /// <summary>
    /// Get a reference to the secondary action substate hash in the animator
    /// </summary>
    public int GetSecondaryActionSubstateHash() => this.secondaryActionSubstateHash;

    /// <summary>
    /// Get a reference to the gimmick sub state hash in the animator
    /// </summary>
    public int GetGimmickSubstateHash() => this.gimmickSubstateHash;

    /// <summary>
    /// Get a reference to the main sub state value
    /// </summary>
    public int GetSubstate() => this.animator.GetInteger(this.substateHash);

    /// <summary>
    /// Get a reference to the action sub state value
    /// </summary>
    public int GetActionSubState() => this.animator.GetInteger(this.actionSubstateHash);

    /// <summary>
    /// Get a reference to the secondary action substate value
    /// </summary>
    public int GetSecondaryActionSubstate() => this.animator.GetInteger(this.secondaryActionSubstateHash);

    /// <summary>
    /// Get a reference to the gimmick sub state value
    /// </summary>
    public int GetGimmickSubstate() => this.animator.GetInteger(this.gimmickSubstateHash);

    /// <summary>
    /// Set the updat emode
    /// </summary>
    public void SetUpdateMode(AnimatorUpdateMode updateMode) => this.updateMode = updateMode;

    /// <summary>
    /// This function is called by unity animation events to perform a specific action during any frame of the animation
    /// </summary>
    public void AnimationToActionEvent(float eventID) { }

    /// <summary>
    /// Ensures the current substate branch is not updated till the animation is done playing
    /// <param name="nextState">The new substate to branc to after copmpletion </param>
    /// <param name="animationToWatch">The animation to wait for completion</param>
    /// </summary>
    private IEnumerator WaitTillAnimationEnds(int nextState, string animationToWatch, AnimationSubstateType newSubstate)
    {
        this.animator.Play(animationToWatch);
        this.waitTillAnimationEnds = true;
        this.currentlyCheckAnimation = animationToWatch;

        yield return new WaitForEndOfFrame();//Wait for the next frame to begin
        yield return new WaitUntil(() => this.CheckIfCurrentAnimationIsAtEnd());
        this.waitTillAnimationEnds = false;
        this.currentlyCheckAnimation = "";
        this.BranchUpdate(nextState, newSubstate);
    }

    /// <summary>
    /// Swaps the animator data with another animator object
    /// <param name="targetAnimatorManager">The Animator manager to swap to</param>
    /// </summary>
    public void SwapAnimatorController(AnimatorManager targetAnimatorManager)
    {
        int animation = this.animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float animationNormalizedTime = this.GetCurrentAnimationNormalizedTime();
        int pastSubstateHash = this.animator.GetInteger(this.substateHash);
        int pastSecondarySubstateHash = this.animator.GetInteger(this.secondaryActionSubstateHash);
        int pastActionSubStateHash = this.animator.GetInteger(this.actionSubstateHash);
        int pastGimmickSubstateHash = this.animator.GetInteger(this.gimmickSubstateHash);
        this.animator.runtimeAnimatorController = targetAnimatorManager.animator.runtimeAnimatorController;
        this.animator.SetInteger(this.substateHash, pastSubstateHash);
        this.animator.SetInteger(this.secondaryActionSubstateHash, pastSecondarySubstateHash);
        this.animator.SetInteger(this.actionSubstateHash, pastActionSubStateHash);
        this.animator.SetInteger(this.gimmickSubstateHash, pastGimmickSubstateHash);

        if (animationNormalizedTime > this.animator.GetCurrentAnimatorStateInfo(0).length * 2 && this.animator.GetCurrentAnimatorStateInfo(0).loop)
        {
            int incrementAmount = (int)(animationNormalizedTime / this.animator.GetCurrentAnimatorStateInfo(0).length);
            animationNormalizedTime /= incrementAmount;
        }

        this.animator.Play(animation, 0, animationNormalizedTime);
    }
}


