using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ExistingSaveSlotStateData : MenuSaveSlotStateData
{
    [SerializeField] private Text lifeCountUI = null;
    [Tooltip("The object which holds the players chaos emerald amount"), SerializeField]
    protected GameObject emeraldHolder;
    [Tooltip("The current acts abbreviation"), SerializeField]
    protected TextMeshProUGUI currentActTextAbbreviation;
    [Tooltip("The current acts thumbnail"), SerializeField]
    protected Image currentActThumbnail;

    public override void SetDisplayState(PlayerData playerData, bool state)
    {
        base.SetDisplayState(playerData, state);

        if (!state)
        {
            return;
        }

        this.UpdateEmeralds(playerData.GetChaosEmeralds());
        this.UpdateCurrentActData(playerData);
        this.lifeCountUI.text = playerData.GetLives().ToString();
    }

    /// <summary>
    /// Updates the emrald UI based on the current player data
    /// </summary>
    protected void UpdateEmeralds(int count)
    {
        if (this.emeraldHolder == null)
        {
            return;
        }

        Transform[] emeralds = this.emeraldHolder.GetComponentsInChildren<Transform>(true);

        for (int x = 1; x < emeralds.Length; x++)
        {
            emeralds[x].gameObject.SetActive(x < count + 1);
        }
    }

    /// <summary>
    /// Updates the current act text and sprite based on the player data
    /// </summary>
    protected void UpdateCurrentActData(PlayerData playerData)
    {
        SceneData sceneData = GMSceneManager.Instance().GetSceneList().GetSceneData(playerData.GetCurrentScene().GetSceneId());
        this.currentActTextAbbreviation.text = GMSceneManager.Instance().GetSceneList().GetSceneData(playerData.GetCurrentScene().GetSceneId()).GetActAbbreviation() + " " + (int)GMSceneManager.Instance().GetSceneList().GetSceneData(playerData.GetCurrentScene().GetSceneId()).GetActNumber();
        this.currentActThumbnail.sprite = GMSceneManager.Instance().GetSceneList().GetSceneData(playerData.GetCurrentScene().GetSceneId()).GetThumbnail();
    }
}
