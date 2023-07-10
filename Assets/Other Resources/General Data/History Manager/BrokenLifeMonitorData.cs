using System;
using UnityEngine;

[Serializable]
public class BrokenLifeMonitorData
{
    [HideInInspector]
    public string name;
    [Tooltip("The type of monitor broken"), SerializeField]
    private PowerUp monitorType;
    [Tooltip("The position the monitor was broken at"), SerializeField]
    private Vector2 position;

    public BrokenLifeMonitorData(MonitorController monitorController, Vector2 position, int index = 0)
    {
        this.monitorType = monitorController.GetPowerUpToGrant();
        this.position = position;
        this.name = General.TransformSpacesToUpperCaseCharacters(this.monitorType.ToString() + " " + (index + 1));
    }

    /// <summary>
    /// Get the current position of the monitor
    /// </summary>
    public Vector2 GetPosition() => this.position;
}
