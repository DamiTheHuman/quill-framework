using UnityEngine;
public class DebugInputManager : MonoBehaviour
{
    private DebugInputMaster debugInputMaster;

    [SerializeField]
    private ButtonInputData moonJumpButton;
    [SerializeField]
    private ButtonInputData breakButton;
    [SerializeField]
    private ButtonInputData swapCharacterButton;
    [SerializeField,]
    private ButtonInputData swapPaletteButton;
    [SerializeField]
    private ButtonInputData powerUp0Button;
    [SerializeField]
    private ButtonInputData powerUp1Button;
    [SerializeField]
    private ButtonInputData powerUp2Button;
    [SerializeField]
    private ButtonInputData powerUp3Button;
    [SerializeField]
    private ButtonInputData powerUp4Button;
    [SerializeField]
    private ButtonInputData powerUp5Button;
    [SerializeField]
    private ButtonInputData speedBurstButton;
    [SerializeField]
    private ButtonInputData timeSlowDownButton;
    [SerializeField]
    private ButtonInputData grantAllEmeraldsButton;
    [SerializeField]
    private ButtonInputData toggleDebugUIButton;

    public void Reset()
    {
        this.debugInputMaster = new DebugInputMaster();
        this.moonJumpButton.Reset();
        this.breakButton.Reset();
        this.swapCharacterButton.Reset();
        this.swapPaletteButton.Reset();
        this.powerUp0Button.Reset();
        this.powerUp1Button.Reset();
        this.powerUp2Button.Reset();
        this.powerUp3Button.Reset();
        this.powerUp4Button.Reset();
        this.powerUp5Button.Reset();
        this.speedBurstButton.Reset();
        this.timeSlowDownButton.Reset();
        this.grantAllEmeraldsButton.Reset();
        this.toggleDebugUIButton.Reset();
    }

    private void Awake()
    {
        this.debugInputMaster = new DebugInputMaster();

        this.debugInputMaster.Player.MoonJump.started += ctx => this.moonJumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.MoonJump.performed += ctx => this.moonJumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.MoonJump.canceled += ctx => this.moonJumpButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.Break.started += ctx => this.breakButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.Break.performed += ctx => this.breakButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.Break.canceled += ctx => this.breakButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.SwapCharacter.started += ctx => this.swapCharacterButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.SwapCharacter.performed += ctx => this.swapCharacterButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.SwapCharacter.canceled += ctx => this.swapCharacterButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.SwapPalette.started += ctx => this.swapPaletteButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.SwapPalette.performed += ctx => this.swapPaletteButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.SwapPalette.canceled += ctx => this.swapPaletteButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.PowerUp0.started += ctx => this.powerUp0Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp0.performed += ctx => this.powerUp0Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp0.canceled += ctx => this.powerUp0Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.PowerUp1.started += ctx => this.powerUp1Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp1.performed += ctx => this.powerUp1Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp1.canceled += ctx => this.powerUp1Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.PowerUp2.started += ctx => this.powerUp2Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp2.performed += ctx => this.powerUp2Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp2.canceled += ctx => this.powerUp2Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.PowerUp3.started += ctx => this.powerUp3Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp3.performed += ctx => this.powerUp3Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp3.canceled += ctx => this.powerUp3Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.PowerUp4.started += ctx => this.powerUp4Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp4.performed += ctx => this.powerUp4Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp4.canceled += ctx => this.powerUp4Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.PowerUp5.started += ctx => this.powerUp5Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp5.performed += ctx => this.powerUp5Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.PowerUp5.canceled += ctx => this.powerUp5Button.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.SpeedBurst.started += ctx => this.speedBurstButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.SpeedBurst.performed += ctx => this.speedBurstButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.SpeedBurst.canceled += ctx => this.speedBurstButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.TimeSlowdown.started += ctx => this.timeSlowDownButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.TimeSlowdown.performed += ctx => this.timeSlowDownButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.TimeSlowdown.canceled += ctx => this.timeSlowDownButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.GrantAllEmeralds.started += ctx => this.grantAllEmeraldsButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.GrantAllEmeralds.performed += ctx => this.grantAllEmeraldsButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.GrantAllEmeralds.canceled += ctx => this.grantAllEmeraldsButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Player.ToggleDebugUI.started += ctx => this.toggleDebugUIButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.ToggleDebugUI.performed += ctx => this.toggleDebugUIButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());
        this.debugInputMaster.Player.ToggleDebugUI.canceled += ctx => this.toggleDebugUIButton.OnInputPress(ctx.phase, this.IgnorePlayerInput());

        this.debugInputMaster.Enable();
    }

    private void OnEnable()
    {
        if (this.debugInputMaster == null && Time.timeSinceLevelLoad != 0)
        {
            this.Awake();
        }

        this.debugInputMaster.Enable();
    }

    private void OnDisable() => this.debugInputMaster.Disable();

    /// <summary>
    /// Get the moon jump button
    /// </summary>
    public ButtonInputData GetMoonJumpButton() => this.moonJumpButton;

    /// <summary>
    /// Get the break button
    /// </summary>
    public ButtonInputData GetBreakButton() => this.breakButton;

    /// <summary>
    /// Get the swap charactger button
    /// </summary>
    public ButtonInputData GetSwapCharacterButton() => this.swapCharacterButton;

    /// <summary>
    /// Get the swap palette button
    /// </summary>
    public ButtonInputData GetSwapPaletteButton() => this.swapPaletteButton;

    /// <summary>
    /// Get the power up 0 button
    /// </summary>
    public ButtonInputData GetPowerUp0Button() => this.powerUp0Button;

    /// <summary>
    /// Get the power up 1 button
    /// </summary>
    public ButtonInputData GetPowerUp1Button() => this.powerUp1Button;

    /// <summary>
    /// Get the power up 2 button
    /// </summary>
    public ButtonInputData GetPowerUp2Button() => this.powerUp2Button;

    /// <summary>
    /// Get the power up 3 button
    /// </summary>
    public ButtonInputData GetPowerUp3Button() => this.powerUp3Button;


    /// <summary>
    /// Get the power up 4 button
    /// </summary>
    public ButtonInputData GetPowerUp4Button() => this.powerUp4Button;


    /// <summary>
    /// Get the power up 5 button
    /// </summary>
    public ButtonInputData GetPowerUp5Button() => this.powerUp5Button;

    /// <summary>
    /// Get the speed burst jump button
    /// </summary>
    public ButtonInputData GetSpeedBurstButton() => this.speedBurstButton;

    /// <summary>
    /// Get the time slow down button
    /// </summary>
    public ButtonInputData GetTimeSlowDownButton() => this.timeSlowDownButton;

    /// <summary>
    /// Get the grant all emeralds button
    /// </summary>
    public ButtonInputData GetGrantAllEmeraldsButton() => this.grantAllEmeraldsButton;

    /// <summary>
    /// Get the toggle debug UI Button
    /// </summary>
    public ButtonInputData GetToggleDebugUIButton() => this.toggleDebugUIButton;

    /// <summary>
    /// Stops inputs from being reading player input when these scenarios are set
    /// </summary>
    public bool IgnorePlayerInput()
    {
        bool actClearHUDIsActive = GMStageManager.Instance().GetStageState() == RegularStageState.ActClear && GMStageHUDManager.Instance().GetActClearUI().activeSelf;
        return GMPauseMenuManager.Instance().GameIsPaused() || actClearHUDIsActive;
    }
}
