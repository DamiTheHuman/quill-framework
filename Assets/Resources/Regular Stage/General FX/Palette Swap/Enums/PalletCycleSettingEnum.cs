using UnityEngine;

public enum PalletCycleSetting
{
    [Tooltip("The Palette will not change cycles unless a color set is set via script")]
    Static,
    [Tooltip("The Set palette will attempt to cycle between color sets at the set frequency")]
    SwapToNextColorSet,
    [Tooltip("The color set will cycle between colorsets by lerping the colors towards the next cycle")]
    LerpToNextColorSet,
}
