using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A class that handles the player input
/// </summary>
public class InputManager : MonoBehaviour
{
    private InputMaster inputMaster;
    private IEnumerator lockControlsCoroutine;
    [SerializeField]
    private InputRestriction currentInputRestricion = InputRestriction.None;
    [SerializeField]
    [Tooltip("The current input of the player")]
    private Vector2 currentInput;

    [SerializeField]
    private ButtonInputData jumpButton;
    [SerializeField]
    private ButtonInputData secondaryJumpButton;
    [SerializeField]
    private ButtonInputData alternativeButton;
    [SerializeField,]
    private ButtonInputData specialButton;
    [SerializeField]
    private ButtonInputData leftTrigger;
    [SerializeField]
    private ButtonInputData rightTrigger;
    [SerializeField]
    private ButtonInputData leftBumper;
    [SerializeField]
    private ButtonInputData rightBumper;

    [Tooltip("If this value is set all input will be ignored and this value will be used only"), SerializeField]
    private Vector2 inputOverride;

    public void Reset()
    {
        this.inputMaster = new InputMaster();
        this.currentInput = new Vector2();
        this.jumpButton.Reset();
        this.secondaryJumpButton.Reset();
        this.alternativeButton.Reset();
        this.leftTrigger.Reset();
        this.rightTrigger.Reset();
        this.leftBumper.Reset();
        this.rightBumper.Reset();
        this.specialButton.Reset();
    }

    private void Awake()
    {
        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        this.inputMaster = new InputMaster();
        this.inputMaster.Player.AButton.started += ctx => this.jumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.AButton.performed += ctx => this.jumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.AButton.canceled += ctx => this.jumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.secondaryJumpButton.SetAction(() => this.SecondaryJumpButtonAction(this.secondaryJumpButton.GetInputActionPhase()));

        this.inputMaster.Player.BButton.started += ctx => this.secondaryJumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.BButton.performed += ctx => this.secondaryJumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.BButton.canceled += ctx => this.secondaryJumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.alternativeButton.SetAction(() => this.SecondaryJumpButtonAction(this.alternativeButton.GetInputActionPhase()));
        this.inputMaster.Player.XButton.started += ctx => this.alternativeButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.XButton.performed += ctx => this.alternativeButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.XButton.canceled += ctx => this.alternativeButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.inputMaster.Player.YButton.started += ctx => this.specialButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.YButton.performed += ctx => this.specialButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.YButton.canceled += ctx => this.specialButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.inputMaster.Player.LeftTrigger.started += ctx => this.leftTrigger.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.LeftTrigger.performed += ctx => this.leftTrigger.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.LeftTrigger.canceled += ctx => this.leftTrigger.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.RightTrigger.started += ctx => this.rightTrigger.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.RightTrigger.performed += ctx => this.rightTrigger.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.RightTrigger.canceled += ctx => this.rightTrigger.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.LeftBumper.started += ctx => this.leftBumper.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.LeftBumper.performed += ctx => this.leftBumper.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.LeftBumper.canceled += ctx => this.leftBumper.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.RightBumper.started += ctx => this.rightBumper.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.RightBumper.performed += ctx => this.rightBumper.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.inputMaster.Player.RightBumper.canceled += ctx => this.rightBumper.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.inputMaster.Player.StartButton.performed += ctx => this.OnPauseButtonPressed(ctx);
        this.inputMaster.Player.Movement.performed += ctx => this.SetDirectionalInput(ctx.ReadValue<Vector2>());
        this.inputMaster.Enable();
    }
    private void OnEnable()
    {
        if (this.inputMaster == null && Time.timeSinceLevelLoad != 0)
        {
            this.Awake();
        }

        this.inputMaster.Enable();
    }

    private void OnDisable() => this.inputMaster.Disable();

    /// <summary>
    /// When the paush button is pressed and what it means based on the state of the current scene
    /// </summary>
    private void OnPauseButtonPressed(InputAction.CallbackContext ctx)
    {
        if (GMCutsceneManager.Instance().ActClearCutsceneIsPlaying() || GMCutsceneManager.Instance().ActStartCutsceneIsPlaying())
        {
            return;
        }
        //Some controllers trigger the pause and jump button at the same input
        else if (this.jumpButton.GetInputActionPhase() == InputActionPhase.Performed)
        {
            return;
        }

        GMPauseMenuManager.Instance().OnPauseButtonPress(ctx.phase);//Monitor the pause button
    }

    /// <summary>
    /// The moment the secondary jump button is pressed
    /// <param name="phase">The phase of the input</param>
    /// </summary>
    private void SecondaryJumpButtonAction(InputActionPhase phase)
    {
        if (this.IgnorePlayerInput())
        {
            return;
        }

        if (GMRegularStageDebugger.Instance() != null && GMRegularStageDebugger.Instance().GetDebugState() == DebugState.Active)
        {
            return;
        }

        this.jumpButton.OnInputPress(phase, this.IgnorePlayerInput());
    }
    /// <summary>
    /// Gets the current player input regardless of the active restriction
    /// </summary>
    public Vector2 ForceGetCurrentInput()
    {
        if (this.IgnorePlayerInput())
        {
            return new Vector2();
        }

        return this.currentInput;
    }

    /// <summary>
    /// Set the directional inut of the player object
    /// <param name="input">The input to be set</param>
    /// </summary>
    private void SetDirectionalInput(Vector2 input) => this.currentInput = input;

    /// <summary>
    /// Get the directional input the user is currently moving in
    /// </summary>
    public Vector2 GetCurrentInput()
    {
        if (this.inputOverride != Vector2.zero)
        {
            return this.inputOverride;
        }

        if (this.IgnorePlayerInput())
        {
            return new Vector2();
        }

        if (this.currentInputRestricion == InputRestriction.None)
        {
            return this.currentInput;
        }
        else if (this.currentInputRestricion == InputRestriction.XAxis)
        {
            return new Vector2(0, this.currentInput.y);
        }
        else if (this.currentInputRestricion == InputRestriction.YAxis)
        {
            return new Vector2(this.currentInput.x, 0);
        }
        else if (this.currentInputRestricion == InputRestriction.All)
        {
            return new Vector2(0, 0);
        }

        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    /// <summary>
    /// Get the jump button
    /// </summary>
    public ButtonInputData GetJumpButton() => this.jumpButton;

    /// <summary>
    /// Get the secondary Jump button
    /// </summary>
    public ButtonInputData GetSecondaryJumpButton() => this.secondaryJumpButton;

    /// <summary>
    /// Get the alternative button
    /// </summary>
    public ButtonInputData GetAlternativeButton() => this.alternativeButton;

    /// <summary>
    /// Get the special button 
    /// </summary>
    public ButtonInputData GetSpecialButton() => this.specialButton;

    /// <summary>
    /// Get the right trigger
    /// </summary>
    public ButtonInputData GetLeftTrigger() => this.leftTrigger;

    /// <summary>
    /// Get the right trigger
    /// </summary>
    public ButtonInputData GetRightTrigger() => this.rightTrigger;

    /// <summary>
    /// Get the left bumper
    /// </summary>
    public ButtonInputData GetLeftBumper() => this.leftBumper;

    /// <summary>
    /// Get the right bumper
    /// </summary>
    public ButtonInputData GetRightBumper() => this.rightBumper;

    /// <summary>
    /// Set the status of the players directional input
    /// </summary>
    public void SetInputRestriction(InputRestriction inputRestriction) => this.currentInputRestricion = inputRestriction;

    /// <summary>
    /// Get the current input restriction
    /// </summary>
    public InputRestriction GetInputRestriction() => this.currentInputRestricion;

    /// <summary>
    /// Locks the players directional input for a specified time
    /// </summary>
    public void SetLockControls(float lockTime, InputRestriction inputRestriction)
    {
        if (this.lockControlsCoroutine != null)
        {
            this.StopCoroutine(this.lockControlsCoroutine);
        }

        this.lockControlsCoroutine = this.LockControls(lockTime, inputRestriction);
        this.StartCoroutine(this.lockControlsCoroutine);
    }

    /// <summary>
    /// Locks the directional input based on the specified conditions
    /// </summary>
    private IEnumerator LockControls(float lockTime, InputRestriction inputRestriction)
    {
        this.SetInputRestriction(inputRestriction);

        yield return new WaitForSeconds(General.StepsToSeconds(lockTime));
        this.SetInputRestriction(InputRestriction.None);
    }
    /// <summary>
    /// Vibrate the active gamepade
    /// </summary>
    public void VibrateController(float lowFrequency, float highFrequency, float duration)
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
            this.Invoke(nameof(StopVibration), duration);
        }
    }
    /// <summary>
    /// Vibrate the active gamepade
    /// </summary>
    public void StopVibration()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    /// <summary>
    /// Overrides the players input with the values set in this object
    /// </summary>
    public void SetInputOverride(Vector2 input) => this.inputOverride = input;

    /// <summary>
    /// Get the value for input override
    /// </summary>
    public Vector2 GetInputOverride() => this.inputOverride;

    /// <summary>
    /// Stops inputs from being reading player input when these scenarios are set
    /// </summary>
    public bool IgnorePlayerInput()
    {
        bool actClearHUDIsActive = GMStageManager.Instance().GetStageState() == RegularStageState.ActClear && GMStageHUDManager.Instance().GetActClearUI().activeSelf;
        return GMPauseMenuManager.Instance().GameIsPaused() || actClearHUDIsActive;
    }
}
