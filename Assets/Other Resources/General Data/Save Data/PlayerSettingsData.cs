using UnityEngine;

[System.Serializable]
public class PlayerSettingsData
{
    [Tooltip("Whether the player has access to debug mode"), SerializeField]
    private bool debugMode = false;
    [Tooltip("Whether the player should show the debug UI if in debug mode"), SerializeField]
    private bool showDebugUI = false;

    /// <summary>
    /// Set the debug mode
    /// </summary
    public void SetDebugMode(bool debugMode) => this.debugMode = debugMode;

    /// <summary>
    /// Get the current debug mode
    /// </summary
    public bool GetDebugMode() => this.debugMode;

    /// <summary>
    /// Set the player show debug UI setting
    /// </summary
    public void SetShowDebugUI(bool showDebugUI) => this.showDebugUI = showDebugUI;

    /// <summary>
    /// Get the player show debug UI setting
    /// </summary
    public bool GetShowDebugUI() => this.showDebugUI;
}
