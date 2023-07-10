using UnityEngine;
/// <summary>
/// A scriptable object which stores information about the character
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterInfoScriptableObject", order = 1)]
public class CharacterInfoScriptableObject : ScriptableObject
{
    [Tooltip("How tall the character is when standing"), FirstFoldOutItem("General Details")]
    public float characterHeight = 40;
    [Tooltip("How wide the character is when standing")]
    public float characterWidth = 20;
    [Tooltip("The pivot pixel point is used to declare the players position when standing")]
    public float playerPixelPivotPoint = 15;
    [Tooltip("The height to identify a low ceiling")]
    public float lowCeilingYRange = 24;
    [Tooltip("How strong the character is when pushing"), LastFoldoutItem()]
    public float characterStrength = 0;

    [Tooltip("Width radius value for sensor positioning when standing"), FirstFoldOutItem("Regular Details")]
    public float bodyWidthRadius = 8;
    [Tooltip("Height radius value for sensor positioning when standing"), LastFoldoutItem()]
    public float bodyHeightRadius = 20;

    [Tooltip("How tall the character is when rolling"), FirstFoldOutItem("Rolling Details")]
    public float rollingCharacterHeight = 30;
    [Tooltip("How wide the character is when rolling")]
    public float rollingCharacterWidth = 16;
    [Tooltip("Width radius value for sensor positioning when rolling")]
    public float rollingBodyWidthRadius = 6;
    [Tooltip("Height radius value for sensor positioning when rolling"), LastFoldoutItem()]
    public float rollingBodyHeightRadius = 15;

    [Tooltip("The Push radius of the character"), FirstFoldOutItem("Push & Wall Sensor Details")]
    public float pushRadius = 10;
    [Tooltip("The wall sensor offset when on flat ground")]
    public float pushRadiusOffsetOnFlatGround = -8;
    [Tooltip("The wall sensor offset when on slopes")]
    public float pushRadiusOffsetOnSlopes = 0;

    [Tooltip("How tall the character is when gliding"), FirstFoldOutItem("Gliding Details")]
    public float glidingCharacterHeight = 21;
    [Tooltip("How wide the character is when gliding")]
    public float glidingCharacterWidth = 21;
    [Tooltip("Width radius value for sensor positioning when gliding")]
    public float glidingBodyWidthRadius = 10;
    [Tooltip("Height radius value for sensor positioning when glding"), LastFoldoutItem()]
    public float glidingBodyHeightRadius = 10;

    [Tooltip("How far the ground and vertical sensors extend"), FirstFoldOutItem("Sensor Details")]
    public float sensorExtension = 16f;
    [Help("If collision are too inconsistent at super high speeds, try lowering the speed limits to something like 16 [Like Sonic CD] before tryng to turn this off")]
    [Tooltip("Determines whether highspeed movement will be used when player attains higher speeds to help consistency")]
    public bool allowHighSpeedMovement = true;
    [Tooltip("Whether the player can adjust to curved via the wall sensors")]
    public bool useWallLanding = false;
}
