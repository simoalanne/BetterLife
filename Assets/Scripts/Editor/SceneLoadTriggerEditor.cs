using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

[CustomEditor(typeof(SceneLoadTrigger))]
public class SceneLoadTriggerEditor : Editor
{
    SerializedProperty sceneToLoad;
    SerializedProperty playerVisibilityInNewScene;
    SerializedProperty transitionType;
    SerializedProperty loadTriggerType;
    SerializedProperty spawnPoint;

    void OnEnable()
    {
        sceneToLoad = serializedObject.FindProperty("_sceneToLoad");
        playerVisibilityInNewScene = serializedObject.FindProperty("_playerVisibilityInNewScene");
        transitionType = serializedObject.FindProperty("_transitionType");
        loadTriggerType = serializedObject.FindProperty("_loadTriggerType");
        spawnPoint = serializedObject.FindProperty("_playerSpawnPoint");

        if (((SceneLoadTrigger)target).GetComponent<Button>() != null)
        {
            loadTriggerType.enumValueIndex = (int)SceneLoadTrigger.LoadTriggerType.OnUIButtonClick;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); 

        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled && scene.path != SceneManager.GetActiveScene().path)
            .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path))
            .ToArray();
        int currentSceneIndex = System.Array.IndexOf(scenes, sceneToLoad.stringValue);
        int newSceneIndex = EditorGUILayout.Popup("Scene To Load", currentSceneIndex, scenes);
        if (newSceneIndex != currentSceneIndex)
        {
            sceneToLoad.stringValue = scenes[newSceneIndex];
        }
        EditorGUILayout.PropertyField(spawnPoint); 
        EditorGUILayout.PropertyField(playerVisibilityInNewScene);

        EditorGUILayout.PropertyField(transitionType);
        EditorGUILayout.PropertyField(loadTriggerType);

        SceneLoadTrigger sceneLoadTrigger = (SceneLoadTrigger)target;

        if ((SceneLoadTrigger.LoadTriggerType)loadTriggerType.enumValueIndex == SceneLoadTrigger.LoadTriggerType.OnGameWorldInteract)
        {
            sceneLoadTrigger.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        else
        {
            sceneLoadTrigger.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        serializedObject.ApplyModifiedProperties();
    }
}
