using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// Handles the functionality of a registered button
/// </summary>
[Serializable]
public class ButtonInputData
{
    [SerializeField]
    private Action action;
    [SerializeField, Tooltip("The phase of the input")]
    private InputActionPhase phase;
    [Tooltip("Determines whether the button has been pressed"), SerializeField, FirstFoldOutItem("Jump Button Status")]
    private bool buttonPressed;
    [Tooltip("Determines whether the button has been released"), SerializeField]
    private bool buttonUp;
    [Tooltip("Determines whether the button has been held down"), SerializeField, LastFoldoutItem()]
    private bool buttonDown;

    private InputActionPhase prevphase;
    public void Reset()
    {
        this.buttonPressed = false;
        this.buttonUp = false;
        this.buttonDown = false;
        this.phase = InputActionPhase.Waiting;
    }

    /// <summary>
    /// Perform an action based on the input
    /// <param name="phase">the current phase of the input</param>
    /// <param name="ignoreInput">Wherther the player input is to be ignoredt</param>
    /// </summary>
    public void OnInputPress(InputActionPhase phase, bool ignoreInput = false)
    {
        if (ignoreInput)
        {
            return;
        }

        this.phase = phase;
        this.action?.Invoke();

        if (this.phase == InputActionPhase.Started)
        {
            this.SetButtonPressed(true);
        }
        else if (this.prevphase == InputActionPhase.Performed)
        {
            this.SetButtonPressed(false);
        }

        this.SetButtonUp(this.buttonUp || phase == InputActionPhase.Canceled);

        if (this.GetButtonPressed() && this.GetButtonUp() == false)
        {
            this.SetButtonDown(true);
        }
        else if (this.GetButtonUp())
        {
            this.SetButtonDown(false);
        }

        if (this.buttonPressed || this.buttonUp)
        {
            GMStageManager.Instance().DelegateTillNextFixedUpdateCycles(this.ResetInputs);
        }

        this.prevphase = phase;
    }

    /// <summary>
    /// Resets inputs that last longer than a frame
    /// </summary>
    private void ResetInputs()
    {
        this.SetButtonPressed(false);
        this.SetButtonUp(false);
    }

    /// <summary>
    /// Set the action value
    /// </summary>
    public void SetAction(Action action) => this.action = action;
    /// <summary>
    /// Get the current input action phase
    /// </summary>
    public InputActionPhase GetInputActionPhase() => this.phase;
    /// <summary>
    /// Check if the button is currently being held 
    /// </summary>
    public bool GetButtonPressed() => this.buttonPressed;
    /// <summary>
    /// Check if the button is being held down
    /// </summary>
    public bool GetButtonDown() => this.buttonDown;
    /// <summary>
    /// Check if the button has been released this frame
    /// </summary>
    public bool GetButtonUp() => this.buttonUp;
    /// <summary>
    /// Set the up flag to the specified value
    /// </summary>
    public void SetButtonUp(bool value) => this.buttonUp = value;

    /// <summary>
    /// Set the pressed flag to the specified value
    /// </summary>
    public void SetButtonPressed(bool value) => this.buttonPressed = value;
    /// <summary>
    /// Set thereleased flag to the specified value
    /// </summary>
    public void SetButtonDown(bool value) => this.buttonDown = value;
}
