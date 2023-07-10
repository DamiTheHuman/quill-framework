using UnityEngine;

/// <summary>
/// A sub action depends on its primary/parent action to be performed for example
/// A player cannot dropdash or instashield without the jumping
/// </summary>
public class HedgeSubAction : HedgeAction
{
    public SubActionSubState subAnimationID;
    [Tooltip("The parent action in which the sub action depends on to be performed")]
    public HedgePrimaryAction parentAction;

    public void SetParentAction(HedgePrimaryAction hedgePrimaryAction) => this.parentAction = hedgePrimaryAction;
}
