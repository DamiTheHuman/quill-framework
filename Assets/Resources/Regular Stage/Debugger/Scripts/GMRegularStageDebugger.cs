using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// An object for debugging regular stages
/// </summary>
public class GMRegularStageDebugger : MonoBehaviour
{
    [SerializeField, Button("UpdateSpawners")]
    private bool updateSpawners;
    public void UpdateSpawnners() => this.LoadPrefabs();

    [SerializeField, FirstFoldOutItem("Dependencies")]
    private InputManager inputManager;
    [SerializeField]
    private DebugInputManager debugInputManager;
    [SerializeField]
    private Transform debugParent;
    [SerializeField, LastFoldoutItem()]
    private Player player;
    [SerializeField, Tooltip("The state of the debugger game object")]
    private DebugState debugState = DebugState.Inactive;
    [SerializeField, Tooltip("Prefabs shown no matter what the stage is"), Help("Prefabs shown no matter what the stage is")]
    private List<DebugItemData> spawnListConstants;
    [SerializeField, Tooltip("These prefabs should never show up in the debugger cause they have dependenices that might error"), Help("These prefabs should never show up in the debugger cause they have dependenices that might error")]
    private List<Type> spawnListBlacklist = new List<Type>() { typeof(WaterController), typeof(SwingPlatformController), typeof(BossTrigger), typeof(CameraTriggerController), typeof(LayerSwitch), };
    [SerializeField, Tooltip("A list of objects the debugger can make")]
    private List<DebugItemData> spawnList;

    [SerializeField, Tooltip("The index of the object to place in soon"), FirstFoldOutItem("Indexes")]
    private int index = 0;
    [SerializeField, Tooltip("The sub index of the object to place in soon"), LastFoldoutItem()]
    private int variantIndex = 0;

    [SerializeField, Tooltip("The current velocity of the debugger"), FirstFoldOutItem("Movement Variables")]
    private Vector2 velocity;
    [SerializeField, Tooltip("The max velocity the debugger can attain")]
    private float maxVelocity = 8;
    [SerializeField, Tooltip("The acceleration of the debugger in any given direction")]
    private float acceleration = 0.1875f;
    [SerializeField, Tooltip("The decceleration of the debugger in any given direction"), LastFoldoutItem()]
    private float decceleration = 0.5f;

    [SerializeField, FirstFoldOutItem("Parent Objects")]
    private GameObject selectedDebugObject;
    [SerializeField, LastFoldoutItem()]
    private GameObject debugObjectsParent;

    [SerializeField, Tooltip("The clip played when a character swap takes place"), FirstFoldOutItem("Sounds")]
    private AudioClip swapPlayerClip;
    [SerializeField, Tooltip("The clip played when a failed character swap takes place"), LastFoldoutItem()]
    private AudioClip swapPlayerFailedClip;

    [SerializeField, FirstFoldOutItem("Power up states"), Tooltip("Whether the player had invincibility before powering up")]
    private bool hasInvincibility;
    [SerializeField, LastFoldoutItem(), Tooltip("Whether the playyer had the power sneakers before powering up")]
    private bool hasPowerSneakers;

    /// <summary>
    /// The single instance of the stage debugger
    /// </summary>
    private static GMRegularStageDebugger instance;

    /// <summary>
    /// Get a reference to the static instance of the stage debugger
    /// </summary>
    public static GMRegularStageDebugger Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMRegularStageDebugger>();
        }

        return instance;
    }

    private void Start()
    {
        if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() != SceneType.RegularStage || GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetDebugMode() == false)
        {
            this.SetDebugState(DebugState.Disabled);
        }
        else
        {
            this.SetDebugState(DebugState.Inactive);
        }

        this.debugObjectsParent = new GameObject("Debug Object Pile");
        this.debugParent.gameObject.SetActive(false);
        this.UpdateSelectedPrefab();
        this.index = 0;
        this.variantIndex = 0;
        this.player = GMStageManager.Instance().GetPlayer();
    }

    private void Update() => this.DebugActions();

    private void FixedUpdate()
    {
        if (this.debugState == DebugState.Disabled)
        {
            if (GMSceneManager.Instance().GetCurrentSceneData().GetSceneType() == SceneType.RegularStage && GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetDebugMode())
            {
                this.SetDebugState(DebugState.Inactive);
            }

            return;
        }

        if (this.debugState == DebugState.Active)
        {
            this.ApplyDecceleration();
            this.ApplyAcceleration();
            this.ApplyFriction();
            this.LimitVelocity();
            this.Move(this.velocity);
            this.ClampPositionToStageBounds();
        }
    }

    /// <summary>
    /// Gets the current debug state
    /// </summary>
    public DebugState GetDebugState() => this.debugState;

    /// <summary>
    /// Update the state of the debugger
    /// <param name="debugState">The debug state to set the stage to </param>
    /// </summary>
    public void SetDebugState(DebugState debugState)
    {
        if (this.debugState == debugState)
        {
            return;
        }

        this.debugState = debugState;

        if (!this.player)
        {
            this.player = GMStageManager.Instance().GetPlayer();
        }

        if (this.player == null)
        {
            return;
        }

        if (debugState == DebugState.Active)
        {
            this.hasPowerSneakers = this.player.GetHedgePowerUpManager().GetPowerSneakersPowerUp().PowerUpIsActive();
            this.hasInvincibility = this.player.GetHedgePowerUpManager().GetInvincibilityPowerUp().PowerUpIsActive();

            if (this.hasInvincibility)
            {
                this.player.GetHedgePowerUpManager().GetInvincibilityPowerUp().RemovePowerUp();
            }
            if (this.hasPowerSneakers)
            {
                this.player.GetHedgePowerUpManager().GetPowerSneakersPowerUp().RemovePowerUp();
            }


            this.AttachPlayerToDebugger();
            this.UpdateSelectedPrefab();
        }
        else if (debugState == DebugState.Inactive)
        {
            this.DettachPlayerFromDebugger();

            if (this.hasPowerSneakers)
            {
                GMGrantPowerUpManager.Instance().GrantPowerUp(this.player, PowerUp.PowerSneakers);
                this.hasPowerSneakers = false;
            }

            if (this.hasInvincibility)
            {
                GMGrantPowerUpManager.Instance().GrantPowerUp(this.player, PowerUp.Invincibility);
                this.hasInvincibility = false;
            }
        }
    }

    /// <summary>
    /// Moves the debug ring based on the velocity
    /// </summary>
    private void Move(Vector2 velocity) => this.transform.position += (Vector3)(velocity * GMStageManager.Instance().GetPhysicsMultiplier()) * Time.deltaTime;

    /// <summary>
    /// Reduce the debuggers velocity based on the current decceleration value
    /// </summary>
    private void ApplyDecceleration()
    {
        float deltaDecceleration = GMStageManager.Instance().ConvertToDeltaValue(this.decceleration);

        if (this.inputManager.GetCurrentInput().x == 1)
        {
            this.velocity.x = this.ApplyPositiveVelocityDecceleration(this.velocity.x, deltaDecceleration);
        }
        else if (this.inputManager.GetCurrentInput().x == -1)
        {
            this.velocity.x = this.ApplyNegativeVelocityDecceleration(this.velocity.x, deltaDecceleration);
        }

        if (this.inputManager.GetCurrentInput().y == 1)
        {
            this.velocity.y = this.ApplyPositiveVelocityDecceleration(this.velocity.y, deltaDecceleration);
        }
        else if (this.inputManager.GetCurrentInput().y == -1)
        {
            this.velocity.y = this.ApplyNegativeVelocityDecceleration(this.velocity.y, deltaDecceleration);
        }
    }

    /// <summary>
    /// Limit the players velocity based on the max values set
    /// </summary>
    private void LimitVelocity()
    {
        this.velocity.x = Mathf.Clamp(this.velocity.x, -this.maxVelocity, this.maxVelocity);
        this.velocity.y = Mathf.Clamp(this.velocity.y, -this.maxVelocity, this.maxVelocity);
    }

    /// <summary>
    /// Apply decceleration to the debugger when moving in the opposite direction
    /// </summary>
    private float ApplyPositiveVelocityDecceleration(float velocity, float deltaDecceleration)
    {
        if (velocity < 0)
        {
            velocity += deltaDecceleration;
            if (velocity >= 0)
            {
                velocity = this.decceleration;
            }
        }

        return velocity;
    }

    /// <summary>
    /// Apply negative decceleration when moving in the opposite direction
    /// </summary>
    private float ApplyNegativeVelocityDecceleration(float velocity, float deltaDecceleration)
    {
        if (velocity > 0)
        {
            velocity -= deltaDecceleration;
            if (velocity <= 0)
            {
                velocity = -this.decceleration;
            }
        }

        return velocity;
    }

    /// <summary>
    /// Apply friction to the players ground velocity based on the set conditions
    /// </summary>
    private void ApplyFriction()
    {
        if (this.inputManager.GetCurrentInput().x == 0)
        {
            this.velocity.x = 0;
        }

        if (this.inputManager.GetCurrentInput().y == 0)
        {
            this.velocity.y = 0;
        }
    }

    /// <summary>
    /// Apply acceleration to the debug ring
    /// </summary>
    private void ApplyAcceleration() => this.velocity += this.inputManager.GetCurrentInput() * this.acceleration;

    /// <summary>
    /// Limits the debugger position to the horizontal boundaries of the stage
    /// </summary>
    public void ClampPositionToStageBounds()
    {
        Vector2 position = this.transform.position;
        CameraBounds stageBounds = HedgehogCamera.Instance().GetCameraBoundsHandler().GetActiveStageBounds();

        if (position.x - this.player.GetSensors().characterBuild.pushRadius < stageBounds.GetLeftBorderPosition() && this.velocity.x <= 0)
        {
            position.x = stageBounds.GetLeftBorderPosition() + this.player.GetSensors().characterBuild.bodyWidthRadius;
            this.velocity.x = 0;
        }
        else if (position.x + this.player.GetSensors().characterBuild.pushRadius > stageBounds.GetRightBorderPosition() && this.velocity.x >= 0)
        {
            position.x = stageBounds.GetRightBorderPosition() - this.player.GetSensors().characterBuild.bodyWidthRadius;
            this.velocity.x = 0;
        }

        this.transform.position = position;
    }

    /// <summary>
    /// Attach the player to the debugger
    /// </summary>
    private void AttachPlayerToDebugger()
    {
        this.player.SetMovementRestriction(MovementRestriction.Both);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.All);
        this.player.GetActionManager().EndCurrentAction();
        this.player.GetSpriteController().SetSpriteAngle(0);
        this.player.SetGrounded(false);
        this.player.transform.eulerAngles = new Vector3(0, 0, 0);
        this.player.GetGimmickManager().SetActiveGimmickMode(GimmickMode.None);
        this.player.velocity = Vector2.zero;
        this.player.groundVelocity = 0;
        this.player.SetPlayerState(PlayerState.Sleep);
        this.transform.position = this.player.transform.position;
        this.player.transform.parent = this.transform;
        this.player.transform.localPosition = Vector3.zero;
        this.debugParent.gameObject.SetActive(true);
    }

    /// <summary>
    /// Dettach the player from the debugger
    /// </summary>
    private void DettachPlayerFromDebugger()
    {
        this.player.SetPlayerState(PlayerState.Awake);
        this.player.GetSpriteController().SetSpriteAngle(0);
        this.player.GetInputManager().SetInputRestriction(InputRestriction.None);
        this.player.SetMovementRestriction(MovementRestriction.None);
        this.player.GetAfterImageController().SetAfterImagesPositions(this.player.transform.position);
        this.player.gameObject.SetActive(true);
        this.player.transform.parent = null;
        Color color = this.player.GetSpriteController().GetSpriteRenderer().color;
        color.a = 1;

        if (this.player.GetHealthManager().GetHealthStatus() == HealthStatus.Invulnerable)
        {
            this.player.GetHealthManager().SetHealthStatus(HealthStatus.Vulnerable);
        }

        this.player.GetSpriteController().GetSpriteRenderer().color = color;
        this.debugParent.gameObject.SetActive(false);
        this.velocity = Vector2.zero;
        this.player.GetInputManager().Reset();
    }

    /// <summary>
    /// Instantiate the prefab, placing it within the scene
    /// </summary>
    private void PlaceObject()
    {
        GameObject debugObject = Instantiate(this.spawnList[this.index].GetVariants()[this.variantIndex]);
        debugObject.transform.position = this.selectedDebugObject.transform.position;
        debugObject.transform.parent = this.debugObjectsParent.transform;
    }

    /// <summary>
    /// Updates the selected prefab
    /// </summary>
    private void UpdateSelectedPrefab()
    {
        if (this.debugState != DebugState.Active)
        {
            return;
        }

        if (this.selectedDebugObject != null)
        {
            Destroy(this.selectedDebugObject);
        }

        this.selectedDebugObject = Instantiate(this.spawnList[this.index].GetVariants()[this.variantIndex]);
        //Disable all the scripts except the sprite
        this.SetomponentsOfType(this.selectedDebugObject, false, true);
        this.selectedDebugObject.transform.position = this.debugParent.transform.position;
        this.selectedDebugObject.transform.parent = this.debugParent.transform;
    }

    /// <summary>
    /// Disables or enables components of the set type
    /// </summary>
    private void SetomponentsOfType(GameObject obj, bool value = false, bool disableChildren = false)
    {
        List<Component> components = obj.GetComponents<Component>().ToList();

        if (disableChildren)
        {
            components.AddRange(obj.GetComponentsInChildren<Component>());
        }

        foreach (Component component in components)
        {
            if (component is MonoBehaviour)
            {
                (component as MonoBehaviour).enabled = false;
            }
            if (component is Behaviour)
            {
                (component as Behaviour).enabled = false;
            }
            if (component is Collider2D)
            {
                (component as Collider2D).enabled = false;
            }
        }
    }

    /// <summary>
    /// Switch the object to place in the scene
    /// <param name="index">The index of the object to update</param>
    /// <param name="direction">The direction to mvoe the index</param>
    /// <param name="list">The list the index operates against, used to find the limits/param>

    /// </summary>
    private int CycleObjectToPlace<T>(int index, int direction, List<T> list)
    {
        if (list.Count == 1)
        {
            return 0;
        }
        if (index + direction > list.Count - 1 && direction > 0)
        {
            return 0;
        }
        else if (index - direction < 0 && direction < 0)
        {
            return list.Count - 1;
        }

        index += direction;

        return index;
    }

    /// <summary>
    /// Debug actions that activate when the player makes certain inputs
    /// </summary>
    private void DebugActions()
    {
        if (GMSceneManager.Instance().IsLoadingNextScene() || this.debugState == DebugState.Disabled)
        {
            return;
        }

        if (GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetDebugMode() == false)
        {
            return;
        }

        if (this.player == null)
        {
            return;
        }

        if (GMStageManager.Instance().GetStageState() is RegularStageState.Died or RegularStageState.Cutscene or RegularStageState.ActClear)
        {
            return;
        }

        this.ToggleShowDebugUI();
        this.PlayerDebugActions();
        this.PowerUpDebugActions();
        this.CharacterDebugActions();
        this.ObjectDebugActions();
    }

    /// <summary>
    /// Toggles the show debug ui setting
    /// </summary>
    private void ToggleShowDebugUI()
    {
        if (this.debugInputManager.GetToggleDebugUIButton().GetButtonPressed())
        {
            GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().SetShowDebugUI(!GMSaveSystem.Instance().GetCurrentPlayerData().GetPlayerSettings().GetShowDebugUI());
            this.debugInputManager.GetToggleDebugUIButton().SetButtonPressed(false);
        }
    }

    /// <summary>
    /// Allows for placing objects within the scene
    /// </summary>
    private void ObjectDebugActions()
    {
        if (this.inputManager.GetAlternativeButton().GetButtonPressed())
        {
            this.SetDebugState(this.debugState == DebugState.Inactive ? DebugState.Active : DebugState.Inactive);
            this.inputManager.GetAlternativeButton().SetButtonPressed(false);
            this.inputManager.GetJumpButton().SetButtonPressed(false);

            return;
        }

        if (this.debugState == DebugState.Active)
        {
            if (this.inputManager.GetJumpButton().GetButtonPressed())
            {
                this.index = this.CycleObjectToPlace(this.index, 1, this.spawnList);
                this.variantIndex = 0;
                this.UpdateSelectedPrefab();
                this.inputManager.GetJumpButton().SetButtonPressed(false);
            }

            if (this.inputManager.GetSecondaryJumpButton().GetButtonPressed())
            {
                this.variantIndex = this.CycleObjectToPlace(this.variantIndex, 1, this.spawnList[this.index].GetVariants());
                this.inputManager.GetSecondaryJumpButton().SetButtonPressed(false);
                this.inputManager.GetJumpButton().SetButtonPressed(false);
                this.UpdateSelectedPrefab();
            }

            if (this.inputManager.GetSpecialButton().GetButtonPressed())
            {
                this.inputManager.GetSpecialButton().SetButtonPressed(false);
                this.PlaceObject();
            }
        }
    }

    /// <summary>
    /// Allows for switching between characters for debugging purposes
    /// </summary>
    private void CharacterDebugActions()
    {
        Player player = GMStageManager.Instance().GetPlayer();

        if (player != null && player.GetPlayerState() == PlayerState.Awake)
        {
            PlayableCharacter character = GMCharacterManager.Instance().currentCharacter;
            bool swapped = false;

            if (this.debugInputManager.GetSwapCharacterButton().GetButtonPressed() && !this.debugInputManager.GetSwapCharacterButton().GetButtonUp())
            {
                this.debugInputManager.GetSwapCharacterButton().SetButtonPressed(false);
                character = General.CycleEnumValue(GMCharacterManager.Instance().currentCharacter, 1);
                swapped = true;
            }

            if (swapped)
            {
                if (GMCharacterManager.Instance().currentCharacter != character)
                {
                    GMAudioManager.Instance().PlayOneShot(this.swapPlayerClip);
                }
                else
                {
                    GMAudioManager.Instance().PlayOneShot(this.swapPlayerFailedClip);
                }

                GMCharacterManager.Instance().SwapCharacter(character);
            }

        }
    }

    /// <summary>
    /// Allows for switching between power ups easily for debugging purposes
    /// </summary>
    private void PowerUpDebugActions()
    {
        Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
        Player player = GMStageManager.Instance().GetPlayer();

        if (this.debugInputManager.GetPowerUp0Button().GetButtonPressed())
        {
            GMGrantPowerUpManager.Instance().GrantPowerUp(player, PowerUp.RegularShield);
        }
        else if (this.debugInputManager.GetPowerUp1Button().GetButtonPressed())
        {
            GMGrantPowerUpManager.Instance().GrantPowerUp(player, PowerUp.BubbleShield);
        }
        else if (this.debugInputManager.GetPowerUp2Button().GetButtonPressed())
        {
            GMGrantPowerUpManager.Instance().GrantPowerUp(player, PowerUp.ElectricShield);
        }
        else if (this.debugInputManager.GetPowerUp3Button().GetButtonPressed())
        {
            GMGrantPowerUpManager.Instance().GrantPowerUp(player, PowerUp.FlameShield);
        }
        else if (this.debugInputManager.GetPowerUp4Button().GetButtonPressed())
        {
            GMGrantPowerUpManager.Instance().GrantPowerUp(player, PowerUp.Invincibility);
        }
        else if (this.debugInputManager.GetPowerUp5Button().GetButtonPressed())
        {
            GMGrantPowerUpManager.Instance().GrantPowerUp(player, PowerUp.PowerSneakers);
        }
    }

    /// <summary>
    /// Allows new debugging options for the player
    /// </summary>
    private void PlayerDebugActions()
    {
        if (this.debugInputManager.GetMoonJumpButton().GetButtonPressed())
        {
            this.player.SetGrounded(false);
            this.player.velocity.y = this.player.currentJumpVelocity;
        }

        if (this.debugInputManager.GetBreakButton().GetButtonPressed())
        {
            Debug.Break();
        }

        if (this.debugInputManager.GetSwapPaletteButton().GetButtonPressed())
        {
            this.player.GetPaletteController().LoadNextPallete();
            this.debugInputManager.GetSwapPaletteButton().SetButtonPressed(false);
        }

        if (this.debugInputManager.GetGrantAllEmeraldsButton().GetButtonPressed())
        {
            GMSaveSystem.Instance().GetCurrentPlayerData().IncrementChaosEmeraldCount(7);
            GMRegularStageScoreManager.Instance().IncrementRingCount(50);
            this.debugInputManager.GetGrantAllEmeraldsButton().SetButtonPressed(false);
        }

        if (this.debugInputManager.GetTimeSlowDownButton().GetButtonDown() && Time.timeScale == 1f)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            if (Time.timeScale == 0.1f)
            {
                Time.timeScale = 1f;
            }
        }

        if (this.debugInputManager.GetSpeedBurstButton().GetButtonPressed())
        {
            this.player.groundVelocity = 64 * this.player.currentPlayerDirection;
            this.debugInputManager.GetSpeedBurstButton().SetButtonPressed(false);
        }
    }

    /// <summary>
    /// Loads other prefabs alongside the constannts
    /// </summary>
    private void LoadPrefabs()
    {
        foreach (DebugItemData debugItemData in this.spawnListConstants)
        {
            if (debugItemData.GetVariants()[0].GetComponent<MonitorController>() != null)
            {
                debugItemData.SetName("Monitors");

                continue;
            }

            debugItemData.SetName(debugItemData.GetVariants()[0].name);
        }

        this.spawnList = new List<DebugItemData>();
        this.spawnList.AddRange(this.spawnListConstants);
        List<GameObject> badniks = FindObjectsOfType<BadnikController>().GroupBy(g => g.GetType()).Select(grg => General.GetPrefabAsset(grg.FirstOrDefault().gameObject)).Where(g => g != null && !this.spawnListBlacklist.Contains(g.GetType()) && !this.spawnListBlacklist.Contains(g.GetComponentInChildren<ContactEvent>().GetType())).ToList();

        foreach (GameObject badnik in badniks)
        {
            DebugItemData debugItem = new DebugItemData();
            debugItem.GetVariants().Add(badnik);
            debugItem.SetName(badnik.name);
            this.spawnList.Add(debugItem);
        }
    }
}
