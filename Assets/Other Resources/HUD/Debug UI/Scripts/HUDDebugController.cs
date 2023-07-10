using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// The class which handles the debug controller
/// </summary>
public class HUDDebugController : MonoBehaviour
{
    [SerializeField, Tooltip("The parent object to attach the debug elements to")]
    private Transform parent;
    [SerializeField, Tooltip("The template the debug text will follow")]
    private GameObject debugTemplatePrefab;
    [SerializeField, Tooltip("The list of active debug elements")]
    private List<DebugElementData> debugElements = new List<DebugElementData>();

    private void Awake() => this.InstantiateRegularStageDebugElements();

    // Update is called once per frame
    private void Update()
    {
        Player player = GMStageManager.Instance().GetPlayer();

        if (player == null || player.GetPlayerState() == PlayerState.Sleep)
        {
            return;
        }

        this.UpdateRegularStageDebugElements(player);
    }

    /// <summary>
    /// Instantiate the debugger for regular stage elements
    /// </summary>
    private void InstantiateRegularStageDebugElements()
    {
        foreach (DebugElementNames debugElementName in Enum.GetValues(typeof(DebugElementNames)))
        {
            GameObject debugElement = Instantiate(this.debugTemplatePrefab);
            debugElement.transform.SetParent(this.parent.transform);
            debugElement.name = "Debug UI: " + debugElementName;
            debugElement.transform.localPosition = new Vector3(debugElement.transform.localPosition.x, debugElement.transform.localPosition.y, 0);
            debugElement.transform.localScale = Vector3.one;
            this.debugElements.Add(new DebugElementData(debugElementName, debugElement.GetComponent<TextMeshPro>()));
        }
    }

    /// <summary>
    /// Update the debug elements on screen
    /// <param name="player">The player holding our data</param>
    /// </summary>
    private void UpdateRegularStageDebugElements(Player player)
    {
        if (player.gameObject.activeSelf == false)
        {
            return;
        }

        foreach (DebugElementData debugElement in this.debugElements)
        {
            switch (debugElement.tag)
            {
                case DebugElementNames.Character:
                    debugElement.SetUiText(GMCharacterManager.Instance().currentCharacter);
                    break;
                case DebugElementNames.GSP:
                    debugElement.SetUiText(General.RoundToDecimalPlaces(player.groundVelocity));
                    break;
                case DebugElementNames.XSP:
                    debugElement.SetUiText(General.RoundToDecimalPlaces(player.velocity.x));
                    break;
                case DebugElementNames.YSP:
                    debugElement.SetUiText(General.RoundToDecimalPlaces(player.velocity.y));
                    break;
                case DebugElementNames.GRND:
                    debugElement.SetUiText(player.GetGrounded());
                    break;
                case DebugElementNames.GroundAngle:
                    debugElement.SetUiText(player.GetSensors().groundCollisionInfo.GetCurrentCollisionInfo().GetAngleInDegrees());
                    break;
                case DebugElementNames.SpriteAngle:
                    debugElement.SetUiText(player.GetSpriteController().GetSpriteAngle());
                    break;
                case DebugElementNames.Action:
                    if (player.GetActionManager().currentPrimaryAction == null)
                    {
                        debugElement.SetUiText("N/A");
                        continue;
                    }
                    debugElement.SetUiText(player.GetActionManager().currentPrimaryAction.GetType());
                    break;
                case DebugElementNames.SubAction:
                    if (player.GetActionManager().currentSubAction == null)
                    {
                        debugElement.SetUiText("N/A");
                        continue;
                    }
                    debugElement.SetUiText(player.GetActionManager().currentSubAction.GetType());
                    break;
                case DebugElementNames.Gimmick:
                    debugElement.SetUiText(player.GetGimmickManager().GetActiveGimmickMode());
                    break;
                case DebugElementNames.Shield:
                    debugElement.SetUiText(player.GetHedgePowerUpManager().GetShieldPowerUp().GetShieldType());
                    break;
                case DebugElementNames.SpeedShoes:
                    debugElement.SetUiText(player.GetHedgePowerUpManager().GetPowerSneakersPowerUp().PowerUpIsActive());
                    break;
                case DebugElementNames.Super:
                    debugElement.SetUiText(player.GetHedgePowerUpManager().GetSuperPowerUp());
                    break;
                case DebugElementNames.Health:
                    debugElement.SetUiText(player.GetHealthManager().GetHealthStatus());
                    break;
                case DebugElementNames.Oxygen:
                    debugElement.SetUiText(General.RoundToDecimalPlaces(player.GetOxygenManager().currentOxygen));
                    break;
                case DebugElementNames.Input:
                    debugElement.SetUiText(player.GetInputManager().GetCurrentInput());
                    break;
                case DebugElementNames.Animation:
                    debugElement.SetUiText(player.GetAnimatorManager().GetCurrentAnimationName().Replace('_', ' ').Replace(GMCharacterManager.Instance().currentCharacter.ToString(), ""));
                    break;
                default:
                    break;
            }
        }
    }
}
