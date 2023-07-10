using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Manages the characters that can be played and swapped to
/// </summary>

[Serializable]
public class SpawnCharacterData
{
    public Player player;
    public SpecialStagePlayer specialStagePlayer;
    public SpawnCharacterData(Player player, SpecialStagePlayer specialStagePlayer)
    {
        this.player = player;
        this.specialStagePlayer = specialStagePlayer;
    }

}
public class GMCharacterManager : MonoBehaviour
{

    [Help("If you want to change character spawned when the scene loads update the character value in the current save which is in the '_SAVEMANAGER' not this value!")]
    [Tooltip("The currently character"), IsDisabled]
    public PlayableCharacter currentCharacter;
    #region
    [Serializable]
    /// <summary>
    /// A class which contains information for a specified main character
    /// </summary>
    public class Character
    {
        [HideInInspector]
        public string name;
        public PlayableCharacter tag;
        public GameObject prefab;
        public GameObject specialStagePrefab;
        public GameObject activeObject;
        public GameObject activeSpecialStageObject;
    }
    [Serializable]
    public class SerializableQue : Queue<GameObject> { }//Helps force unity to serialse queues
    [Serializable]
    public class PoolDictionary : SerializableDictionary<PlayableCharacter, SerializableQue> { } //Helps force unity to serialize dictionaries so no need to restart on recompile :D
    #endregion
    [Tooltip("A pool of all the main characters that can be played"), SerializeField]
    private List<Character> characters = new List<Character>();
    private List<SpawnCharacterData> readyToUsePlayers = new List<SpawnCharacterData>();
    [Tooltip("Informs other objects that the character on the scene was instantiated from here"), SerializeField]
    private bool characterInstatedFromCharacterManager = false;
    private static GMCharacterManager instance;
    private void Awake()
    {
        if (Instance() != null && Instance() != this)
        {
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);//Destroy duplicate character managers
            return;
        }

        this.PopulateCharacterPool();
        this.transform.parent = null;
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Everytime a new scene is loaded this is called, This will server as the Start function
    /// </summary>
    private void OnSceneLoaded() => instance = this;

    /// <summary>
    /// Get a reference to the static instance of the character manager
    /// </summary>
    public static GMCharacterManager Instance()
    {
        //Ensures instance is never null when requested for
        if (instance == null)
        {
            instance = FindObjectOfType<GMCharacterManager>();
            SceneManager.sceneLoaded += instance.OnLoadCallback;
        }

        return instance;
    }

    /// <summary>
    /// The actions performed on every scene reload
    /// </summary>
    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode) => this.OnSceneLoaded();

    /// <summary>
    /// Populates the pool of main characters for the scene
    /// </summary>
    private void PopulateCharacterPool()
    {
        GameObject characterParent = new GameObject();
        characterParent.transform.parent = this.transform;
        characterParent.transform.position = new Vector2(-99999, 99999);
        characterParent.name = "Characters";
        GameObject specialStageCharacterParent = new GameObject();
        specialStageCharacterParent.transform.parent = this.transform;
        specialStageCharacterParent.transform.position = new Vector2(-99999, 99999);
        specialStageCharacterParent.name = "Special Stage Characters";

        foreach (Character character in this.characters)
        {
            //Create regular Player
            Player newCharacter = Instantiate(character.prefab).GetComponent<Player>();
            newCharacter.transform.localPosition = Vector3.zero;
            newCharacter.GetComponent<Player>().SetPlayerState(PlayerState.Sleep);
            character.activeObject = newCharacter.gameObject;
            newCharacter.name = character.prefab.name;
            newCharacter.transform.parent = characterParent.transform;

            //Create Special Stage Player
            SpecialStagePlayer newSpecialStageCharacter = Instantiate(character.specialStagePrefab).GetComponent<SpecialStagePlayer>();
            newSpecialStageCharacter.transform.localPosition = Vector3.zero;
            newSpecialStageCharacter.GetComponent<SpecialStagePlayer>().SetSleep(true);
            character.activeSpecialStageObject = newSpecialStageCharacter.gameObject;
            newSpecialStageCharacter.name = character.prefab.name + " [Special Stage]";
            newSpecialStageCharacter.transform.parent = specialStageCharacterParent.transform;

            this.readyToUsePlayers.Add(new SpawnCharacterData(newCharacter, newSpecialStageCharacter));
        }
    }

    /// <summary>
    /// Get a character in <see cref="characters"/> by its tag
    /// <param name="characterTag">The tag of the character</param>
    /// </summary>
    public Character GetCharacter(PlayableCharacter characterTag) => Array.Find(this.characters.ToArray(), c => c.tag == characterTag);

    /// <summary>
    /// Swaps to a different character based on the character tag
    /// <param name="characterTo">The character to swap to</param>
    /// </summary>
    public void SwapCharacter(PlayableCharacter characterTo)
    {
        Player currentPlayer = GMStageManager.Instance().GetPlayer();
        Player targetPlayer = this.GetCharacter(characterTo).activeObject.GetComponent<Player>();

        if (this.currentCharacter == characterTo)
        {
            Debug.Log("Character is already: " + characterTo);

            return;
        }

        if (targetPlayer == null)
        {
            return;
        }

        this.currentCharacter = characterTo;
        currentPlayer.name = targetPlayer.gameObject.name;
        currentPlayer.SwapPlayerData(targetPlayer);
        currentPlayer.GetSensors().SwapSensorsData(targetPlayer.GetSensors());
        currentPlayer.GetSpriteController().SwapSpriteControllerData(targetPlayer.GetSpriteController());
        currentPlayer.GetAfterImageController().SwapAfterImageControler(targetPlayer.GetAfterImageController());
        currentPlayer.GetAnimatorManager().SwapAnimatorController(targetPlayer.GetAnimatorManager());
        currentPlayer.GetActionManager().SwapActionData(targetPlayer.GetActionManager());
        currentPlayer.GetHedgePowerUpManager().GetShieldPowerUp().Start();
        currentPlayer.GetPaletteController().SwapPalleteController(targetPlayer.GetPaletteController());
        currentPlayer.GetSolidBoxController().SwapSolidBoxController(targetPlayer.GetSolidBoxController());
        currentPlayer.GetHitBoxController().SwapHitBoxController(targetPlayer.GetHitBoxController());
        currentPlayer.GetSpriteController().SwapSpriteControllerData(targetPlayer.GetSpriteController());
        GMStageHUDManager.Instance().SetLifeIconMaterial(currentPlayer.GetSpriteController().GetSpriteRenderer().sharedMaterial);//update the HUD material to match the player
    }

    /// <summary>
    /// Checks whether to instantiate a new player within the context of the scene
    /// </summary>
    public Player CheckToInstantiatePlayer()
    {
        Debug.Log("No Player found in scene, will attempt to instantiate player based on save data");
        Player player = GMStageManager.Instance().GetPlayer();//Get the stages player object to see if it exists

        for (int x = 0; x < this.characters.Count; x++)
        {
            if (this.characters[x].tag == GMSaveSystem.Instance().GetCurrentPlayerData().GetCharacter())//Found the player so time to create them
            {
                if (player != null)
                {
                    if (player.GetPlayerPhysicsInfo().character == this.characters[x].tag && player.gameObject.activeSelf && player.GetPlayerState() == PlayerState.Awake)
                    {
                        if (General.TransformSpacesToUpperCaseCharacters(player.gameObject.name) != this.characters[x].tag.ToString())
                        {
                            Debug.Log("Player name doesnt match character tag If this is intentional please ignore");
                        }

                        Debug.Log("Found active player with tag!");
                        this.currentCharacter = player.GetPlayerPhysicsInfo().character;

                        return player;
                    }

                    Debug.Log("Player object found in scene and is being set to sleep, Please set the character from the character Manager!");
                    player.SetPlayerState(PlayerState.Sleep);
                    player.gameObject.SetActive(false);
                    player.gameObject.name = "<Deactivated by GM> " + player.gameObject.name;
                    player.transform.parent = null;
                    this.currentCharacter = player.GetPlayerPhysicsInfo().character;
                }

                Player character = Instantiate(this.characters[x].prefab).GetComponent<Player>();
                this.characterInstatedFromCharacterManager = true;
                character.SetPlayerState(PlayerState.Awake);
                character.name = this.characters[x].prefab.gameObject.name;
                this.currentCharacter = character.GetPlayerPhysicsInfo().character;
                GMStageManager.Instance().SetPlayer(character);
                Debug.Log("Match found! player instantaited from character manager");

                return character;
            }
        }

        return null;
    }

    /// <summary>
    /// Checks whether to instantiate a special stage player within the context of the scene
    /// </summary>
    public SpecialStagePlayer CheckToInstantiateSpecialStagePlayer()
    {
        Debug.Log("No Player found in scene, will attempt to instantiate player based on save data");
        SpecialStagePlayer player = GMSpecialStageManager.Instance().GetPlayer();//Get the stages player object to see if it exists
        for (int x = 0; x < this.characters.Count; x++)
        {
            if (this.characters[x].tag == GMSaveSystem.Instance().GetCurrentPlayerData().GetCharacter())//Found the player so time to create them
            {
                if (player != null)
                {
                    if (player.GetPlayerPhysicsInfo().character == this.characters[x].tag && player.gameObject.activeSelf)
                    {
                        if (General.TransformSpacesToUpperCaseCharacters(player.gameObject.name) != this.characters[x].tag.ToString())
                        {
                            Debug.Log("Special Stage Player name doesnt match character tag If this is intentional please ignore");
                        }

                        Debug.Log("Special Stage Player object with name similar to character tag, aborting automatic player load and switching to manual");
                        this.currentCharacter = player.GetPlayerPhysicsInfo().character;

                        return player;
                    }
                    Debug.Log("Special Stage Player object found in scene and is being set to sleep, Please set the character from the character Manager!");
                    player.SetSleep(true);
                    player.gameObject.SetActive(false);
                    player.gameObject.name = "<Deactivated by GM> " + player.gameObject.name;
                    player.transform.parent = null;
                    this.currentCharacter = player.GetPlayerPhysicsInfo().character;
                }

                SpecialStagePlayer character = Instantiate(this.characters[x].specialStagePrefab).GetComponent<SpecialStagePlayer>();
                this.characterInstatedFromCharacterManager = true;
                character.SetSleep(false);
                character.name = this.characters[x].prefab.gameObject.name + " [Special Stage]";
                this.currentCharacter = character.GetPlayerPhysicsInfo().character;
                character.transform.parent = GMSpecialStageManager.Instance().GetSpecialStageSlider().transform;
                character.transform.localPosition = Vector3.zero;
                GMSpecialStageManager.Instance().SetPlayer(character);

                Debug.Log("Match found! Special Stage Player instantaited from character manager");

                return character;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the current status of character instated from
    /// </summary>
    public bool GetCharacterInstantiated() => this.characterInstatedFromCharacterManager;

    private void OnValidate()
    {
        foreach (Character character in this.characters)
        {
            character.name = General.TransformSpacesToUpperCaseCharacters(character.tag.ToString());
        }
    }
}
