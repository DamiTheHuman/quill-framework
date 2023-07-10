using UnityEngine;

public enum InputRestriction
{
    [Tooltip("Stop accepting input on the X Axis only")]
    XAxis,
    [Tooltip("Stop accepting input on the Y Axis only")]
    YAxis,
    [Tooltip("Stop accepting input on both the X and Y Axis")]
    All,
    [Tooltip("Allow Input")]
    None,
}
