using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SceneListScriptableObject", order = 6)]
public class SceneListScriptableObject : ScriptableObject
{
    [Button(nameof(AutoCompleteNextScenes)), Tooltip("Use this automaticially populate the next scene object list"), SerializeField]
    private bool autoCompleteNextScenes;
    [Tooltip("List of Start Scenes which are played when the game is launched or restarted")]
    public List<SceneData> introScenes = new List<SceneData>();
    [Tooltip("The main menu scene")]
    public SceneData menuScene;
    [Tooltip("List of Regular Stage Scenes")]
    public List<SceneData> stageScenes = new List<SceneData>();
    [Tooltip("List of Special Stage Scenes")]
    public List<SceneData> specialStageScenes = new List<SceneData>();
    [Tooltip("The act clear special stage scene")]
    public SceneData specialStageActClearScene;
    [Tooltip("The end credits scene")]
    public SceneData creditScene;

    private void OnValidate()
    {
        Dictionary<int, string> scenes = this.LoadGameScenes();
        this.introScenes = this.SetSceneNames(scenes, this.introScenes);
        this.menuScene.name = scenes[this.menuScene.GetSceneId()];
        this.stageScenes = this.SetSceneNames(scenes, this.stageScenes);
        this.specialStageScenes = this.SetSceneNames(scenes, this.specialStageScenes);
        this.specialStageActClearScene.name = scenes[this.specialStageActClearScene.GetSceneId()];
    }


    /// <summary>
    /// Auto completes the next scene id
    /// </summary>
    private void AutoCompleteNextScenes()
    {
        this.OnValidate();

        this.introScenes = this.UpdateNextSceneForScenelist(this.introScenes, this.menuScene);
        this.menuScene.SetNextSceneId(this.stageScenes[0].GetSceneId());
        this.stageScenes = this.UpdateNextSceneForScenelist(this.stageScenes, this.creditScene);

        //All special stage next scenes are act clear
        for (int x = 0; x < this.specialStageScenes.Count; x++)
        {
            this.specialStageScenes[x].SetNextSceneId(this.specialStageActClearScene.GetSceneId());
        }

        this.creditScene.SetNextSceneId(this.menuScene.GetNextSceneId());
    }

    /// <summary>
    /// Autocomplete the next scene for scene data that make use of lists
    /// </summary>
    private List<SceneData> UpdateNextSceneForScenelist(List<SceneData> scenes, SceneData lastScene = null)
    {
        for (int x = 0; x < scenes.Count; x++)
        {
            if (x + 1 >= this.stageScenes.Count && lastScene != null)
            {
                this.stageScenes[x].SetNextSceneId(this.creditScene.GetSceneId());
                break;
            }

            scenes[x].SetNextSceneId(scenes[x + 1].GetSceneId());
        }

        return scenes;
    }

    /// <summary>
    /// Ensures that our variables array id's have a name relative to the scene id 
    /// </summary>
    private List<SceneData> SetSceneNames(Dictionary<int, string> scenes, List<SceneData> startScenes)
    {
        for (int x = 0; x < startScenes.Count; x++)
        {
            startScenes[x].name = scenes[startScenes[x].GetSceneId()];
        }

        return startScenes;
    }

    /// <summary>
    /// Load all the available scenes that are available within build
    /// </summary>
    private Dictionary<int, string> LoadGameScenes()
    {
        Dictionary<int, string> availableScenes = new Dictionary<int, string>();
#if UNITY_EDITOR

        EditorBuildSettingsScene[] scenesInBuild = EditorBuildSettings.scenes;
        string path;

        for (int i = 0; i < scenesInBuild.Length; i++)
        {
            path = Path.GetFileNameWithoutExtension(scenesInBuild[i].path);
            availableScenes.Add(i, path);
        }
#endif

        return availableScenes;
    }

    /// <summary>
    /// Get the scene data of the index passed
    /// </summary>
    public SceneData GetSceneData(int index)
    {
        List<SceneData> allScenes = new List<SceneData>();

        allScenes.AddRange(this.introScenes);
        allScenes.Add(this.menuScene);
        allScenes.AddRange(this.stageScenes);
        allScenes.AddRange(this.specialStageScenes);
        allScenes.Add(this.specialStageActClearScene);
        allScenes.Add(this.creditScene);

        return allScenes.FirstOrDefault(x => x.GetSceneId() == index);
    }
}
