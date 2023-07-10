using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuSaveSlotStateData
{
    [SerializeField]
    protected bool isDisabled;
    [SerializeField, Tooltip("List of objects to show when the save slot is in this state")]
    protected List<GameObject> onActiveShowObjects;
    [SerializeField, Tooltip("List of objects to show when debug is activated on this save slot state where avaialble")]
    protected List<GameObject> OnDebugOnlyObjects;
    [SerializeField]
    protected Animator characterAnimator;

    /// <summary>
    /// Display the following objects based on the state passed and debug passed
    /// </summary>
    public virtual void SetDisplayState(PlayerData playerData, bool state)
    {
        if (state == false && this.isDisabled)
        {
            return;
        }

        foreach (GameObject activeOnlyObjects in this.onActiveShowObjects)
        {
            if (activeOnlyObjects.activeSelf == state)
            {
                continue;
            }

            activeOnlyObjects.SetActive(state);
        }


        foreach (GameObject debugOnlyObjects in this.OnDebugOnlyObjects)
        {
            if (debugOnlyObjects.activeSelf == state && debugOnlyObjects.activeSelf == playerData.GetPlayerSettings().GetDebugMode())
            {
                continue;
            }

            debugOnlyObjects.SetActive(state && playerData.GetPlayerSettings().GetDebugMode());
        }

        this.characterAnimator.SetInteger("Character", (int)playerData.GetCharacter());

        this.isDisabled = state == false;
    }
}
