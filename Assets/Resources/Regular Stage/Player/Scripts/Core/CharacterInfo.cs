using UnityEngine;
/// <summary>
/// This class stores inormation about the current character being used 
/// </summary>
/// 
public class CharacterData : MonoBehaviour
{
    [Tooltip("How tall the character is when standing")]
    public float characterHeight = 40;
    [Tooltip("How wide the character is when standing")]
    public float characterWidth = 20;
    [Tooltip("The pivot pixel point is used to declare the players position when standing")]
    public float playerPixelPivotPoint = 15;
    [Tooltip("How strong the character is when pushing")]
    public float characterStrength = 0;
    [Tooltip("How tall the character is when rolling")]
    public float rollingCharacterHeight = 30;
    [Tooltip("How wide the character is when rolling")]
    public float rollingCharacterWidth = 16;
    [Tooltip("Width radius value for sensor positioning when standing")]
    public float bodyWidthRadius = 8;
    [Tooltip("Height radius value for sensor positioning when standing")]
    public float bodyHeightRadius = 20;
    [Tooltip("Width radius value for sensor positioning when rolling")]
    public float rollingBodyWidthRadius = 8;
    [Tooltip("Height radius value for sensor positioning when rolling")]
    public float rollingBodyHeightRadius = 20;
    [Tooltip("The Push radius of the character")]
    public float pushRadius = 10;
    [Tooltip("The wall sensor offset when on flat ground")]
    public float pushRadiusOffsetOnFlatGround = -8;
    [Tooltip("The wall sensor offset when on slopes")]
    public float pushRadiusOffsetOnSlopes = 0;
    [Tooltip("The height to identify a low ceiling")]
    public float lowCeilingYRange = 24;
    [Tooltip("The amount to shrink the camera by when performing shrink based actions ")]
    public float specialRollDepression = -5;
}
