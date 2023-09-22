using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A scriptable object which stores information about the characters Physics
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterPhysicsScriptableObject", order = 1)]
public class CharacterPhysicsScriptableObject : ScriptableObject
{
    [Help("Primarly used so the character manager can know who is who")]
    [Tooltip("The character the physics set belongs to")]
    public PlayableCharacter character = PlayableCharacter.Sonic;
    [Tooltip("Whether the character can break walls by touching them")]
    public bool canRamThroughWalls = false;
    [FirstFoldOutItem(" General - Velocity Limits")]
    [Tooltip("The X and Y limits the velocity can be at any given time")]
    [Help("When exceeding speeds of  16 pixels per step be mindful of level design")]
    public Vector2 velocityLimits = Vector2.positiveInfinity;
    [Tooltip("The top speed that can be achieved by the player on their own without slopes or gimmicks")]
    public float basicTopSpeed = 6f;
    [Tooltip("Recommended Velocity limit which also affects sprite rotation"), LastFoldoutItem]
    public float recommendedVelocityLimit = 16;

    [Tooltip("The current slope factor being used"), FirstFoldOutItem("General - Slope Variables")]
    public float slopeFactor = 0.125f;
    [Tooltip("The value of slope when going uphill")]
    public float basicSlopeRollUpwards = 0.078125f;
    [Tooltip("The value of slope when going downhil")]
    public float basicSlopeRollDownards = 0.3125f;
    [Tooltip("The minimum velocity needed before the player is kicked off a slope while not in GROUND mode"), LastFoldoutItem()]
    public float basicFall = 2.5f;

    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Basic - Ground Movement")]
    public float basicAcceleration = 0.046875f;
    [Tooltip("How much speed reduced when moving away from the direction of the current velocity")]
    public float basicDeceleration = 0.5f;
    [Tooltip("How speed is subtracted when the player input is not in the direction of the current velocity")]
    public float basicFriction = 0.046875f;

    [Tooltip("The downard force that pulls the player towards the ground"), FirstFoldOutItem("Basic - Air Movement")]
    public float basicGravity = 0.21875f;
    [Tooltip("How much the player accelerates by in the air which is 2 x acceleration")]
    public float basicAirAcceleration = 0.09375f;
    [Tooltip("The air drag effect on the players air velocity")]
    public float basicAirDrag = 0.96875f;
    [Tooltip("The Conditions that must be met before air drag can be applied"), LastFoldoutItem()]
    public Vector2 basicAirDragCondition = new Vector2(0.125f, 4f);

    [Tooltip("How speed is subtracted when the player input is not in the direction of the current velocity when rolling "), FirstFoldOutItem("Basic - Rolling Variables")]
    public float basicRollingFriction = 0.0234375f;
    [Tooltip("How much speed reduced when moving away from the direction of the current velocity when rolling")]
    public float basicRollingDeceleration = 0.125f;
    [Tooltip("The minimum ground velocity before the player unrolls"), LastFoldoutItem()]
    public float basicMinRollThreshold = 0.2f;

    [Tooltip("How much velocity is added to the player when they jump"), FirstFoldOutItem("Basic - Jump Variables")]
    public float basicJumpVelocity = 6.5f;
    [Tooltip("The velocity set when the jump button is released during a jump"), LastFoldoutItem()]
    public float basicJumpReleaseThreshold = 4f;

    //Underwater variables
    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Underwater - Ground Movement")]
    public float underwaterAcceleration = 0.0234375f;
    [Tooltip("How much speed reduced when moving away from the direction of the current velocity")]
    public float underwaterDeceleration = 0.25f;
    [Tooltip("How speed is subtracted when the player input is not in the direction of the current velocity")]
    public float underwaterFriction = 0.0234375f;
    [Tooltip("The top speed limit when the player is underwater"), LastFoldoutItem()]
    public float underwaterTopSpeed = 3f;

    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Underwater - Air Movement")]
    public float underwaterGravity = 0.0625f;
    [Tooltip("How much the player accelerates by in the air which is 2 x acceleration")]
    public float underwaterAirAcceleration = 0.046875f;

    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Underwater - Jump Variables")]
    public float underwaterJumpVelocity = 3.5f;
    [Tooltip("The velocity set when the jump button is released during a jump"), LastFoldoutItem()]
    public float underwateJumpReleaseThreshold = 2f;

    [Tooltip("How speed is subtracted when the player input is not in the direction of the current velocity when rolling "), FirstFoldOutItem("Underwater - Rolling Variables")]
    public float underwaterRollingFriction = 0.703125f;
    [Tooltip("How much speed reduced when moving away from the direction of the current velocity when rolling"), LastFoldoutItem()]
    public float underwaterRollingDeceleration = 0.125f;

    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Underwater Exit & Entry")]
    public Vector2 underWaterEntryMultiplier = new Vector2(0.5f, 0.25f);
    [Tooltip("How much speed added when moving in the direction of the current velocity"), LastFoldoutItem()]
    public Vector2 underWaterExitMultiplier = new Vector2(1, 2);

    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Speed Shoes - Ground Movement")]
    public float speedShoesAcceleration = 0.09375f;
    [Tooltip("How much speed reduced when moving away from the direction of the current velocity")]
    public float speedShoesDeceleration = 0.25f;
    [Tooltip("How speed is subtracted when the player input is not in the direction of the current velocity")]
    public float speedShoesFriction = 0.09375f;
    [Tooltip("The top speed that can be achieved by the player on their own"), LastFoldoutItem()]
    public float speedShoesTopSpeed = 12f;

    [Tooltip("How much speed added when moving in the direction of the current velocity"), FirstFoldOutItem("Speed Shoes - Air Movement")]
    public float speedShoesAirAcceleration = 0.1875f;

    [Tooltip("How speed is subtracted when the player input is not in the direction of the current velocity when rolling "), FirstFoldOutItem("Speed Shoes - Rolling Variables")]
    public float speedShoesRollingFriction = 0.046875f;
    [Tooltip("How much speed reduced when moving away from the direction of the current velocity when rolling"), LastFoldoutItem()]
    public float speedShoesRollingDeceleration = 0.125f;

    [Tooltip("How much velocity to apply to the player when they die"), FirstFoldOutItem("Hurt and Death Velocities")]
    public Vector2 deathVelocity = new Vector2(0, 7);
    [Tooltip("How much velocity to apply to the hurt player")]
    public Vector2 knockBackVelocity = new Vector2(2, 4);
    [Tooltip("The gravity applied to the player during the hurt state"), LastFoldoutItem()]
    public float knockBackGravity = 0.1875f;

    [Tooltip("The velocity added to the player jumps during his animation"), SerializeField, FirstFoldOutItem("Victory Velocity")]
    public Vector2 victoryVelocity = new Vector2(1.4f, 3f);

    [Tooltip("How long the player can fly for in steps"), FirstFoldOutItem("Flight Variables")]
    public float maxFlightDuration = 480f;
    [Tooltip("The gravity applied when flying upwards")]
    public float flightUpwardsGravity = -0.125f;
    [Tooltip("The gravity applied when flying regularly")]
    public float flightBaseGravity = 0.03125f;
    [Tooltip("How long the player must wait before they can make the first flight upwards"), LastFoldoutItem()]
    public float firstFlightDelayDuration = 10f;

    [Tooltip("The players horizontal velocity at teh start of a glide"), FirstFoldOutItem("Glide Variables")]
    public float glideStartVelocity = 4f;
    [Tooltip("The value the player horizontal velocity is multiplied by when they drop")]
    public float glideDropVelocityMultiplier = 0.25f;
    [Tooltip("The acceleration of the player while gliding")]
    public float glideAcceleration = 0.015625f;
    [Tooltip("The top velocity that can be attained while gliding")]
    public Vector2 glideTopVelocity = new Vector2(24f, 0.5f);
    [Tooltip("The value of gravity when gliding")]
    public float glideGravity = 0.125f;
    [Tooltip("The velocity of the player when they hit the ground while gliding")]
    public float glideSlideFriction = 0.125f;
    [Tooltip("The speed the player turns at")]
    public float glideTurnSpeed = 2.8125f;
    [Tooltip("The speed the player climbs walls at"), FirstFoldOutItem("Climb Variables")]
    public float climbSpeed = 1;
    [Tooltip("Positions that are added to the player while performing a pull up"), FirstFoldOutItem("Pull Up Variables"), LastFoldoutItem()]
    public List<Vector2> pullUpPositionIncrements = new List<Vector2> { new Vector2(4, -6), new Vector2(4, 9), new Vector2(4, 8), new Vector2(4, 7), new Vector2(4, 7) };
    [Tooltip("The pull up animation"), LastFoldoutItem()]
    public AnimationClip pullUpAnimationClip;
    [Tooltip("List of classes of that cannot be climbed"), IsDisabled]
    public readonly List<string> nonClimbableObjectControllers = new List<string> {
        typeof(SpikeController).ToString(),
    };

    [Tooltip("Special Stage Gravity"), FirstFoldOutItem("Special Stage Variables")]
    public float specialStageCurrentGravity = 0.21875f;
    [Tooltip("How high the player jumps in a special stage")]
    public float specialStageJumpVelocity = 6.5f;
    [Tooltip("The acceleration of the player in the air in  a special stage"), LastFoldoutItem()]
    public float specialStageAirAcceleration = 0.09375f;


    [FirstFoldOutItem("Homing Attack Variables")]
    public LayerMask homingAttackCollisionMask = new LayerMask();
    [Tooltip("The radius of the homing Attack")]
    public readonly float homingAttackRadius = 96f;
    [Tooltip("The speed of the homing attack")]
    public float homingAttackSpeed = 4f;
    [Tooltip("List of classes of objects that are considered homable"), IsDisabled]
    public readonly List<string> homableObjectControllers = new List<string> {
        typeof(BadnikController).ToString(),
        typeof(MonitorController).ToString(),
        typeof(SpringController).ToString(),
        typeof(Boss).ToString(),
        typeof(BalloonController).ToString(),
    };
    [Help("Once this time is exceeded while performing a homing attack it will end forcefully"), LastFoldoutItem()]
    public float homingAttackTimeout = 60f;
}
