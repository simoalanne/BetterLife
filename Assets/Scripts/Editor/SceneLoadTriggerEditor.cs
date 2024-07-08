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
    SerializedProperty interactMinDistance;
    SerializedProperty isInteractable;
    SerializedProperty playerSpawnPoint;

    void OnEnable()
    {
        sceneToLoad = serializedObject.FindProperty("_sceneToLoad");
        playerVisibilityInNewScene = serializedObject.FindProperty("_playerVisibilityInNewScene");
        transitionType = serializedObject.FindProperty("_transitionType");
        loadTriggerType = serializedObject.FindProperty("_loadTriggerType");
        interactMinDistance = serializedObject.FindProperty("_interactMinDistance");
        isInteractable = serializedObject.FindProperty("_isInteractable");
        playerSpawnPoint = serializedObject.FindProperty("_playerSpawnPoint");

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

        EditorGUILayout.PropertyField(playerVisibilityInNewScene);

        if ((SceneLoader.PlayerVisibility)playerVisibilityInNewScene.enumValueIndex == SceneLoader.PlayerVisibility.Visible)
        {
            string[] spawnPointsInScene = GetSpawnPointsInScene(sceneToLoad.stringValue);
            if (spawnPointsInScene.Length > 0) // Check if there are spawn points in the scene
            {
                string[] options = spawnPointsInScene.Concat(new string[] { "Previous Location" }).ToArray();

                int currentSpawnPointIndex = System.Array.IndexOf(options, playerSpawnPoint.stringValue);
                if (currentSpawnPointIndex == -1) currentSpawnPointIndex = 0;  // Default to first spawn point if none is set
                int newSpawnPointIndex = EditorGUILayout.Popup("Player Spawn Point", currentSpawnPointIndex, options);
                if (newSpawnPointIndex != currentSpawnPointIndex)
                {
                    playerSpawnPoint.stringValue = options[newSpawnPointIndex];
                }
            }
        }

        EditorGUILayout.PropertyField(transitionType);
        EditorGUILayout.PropertyField(loadTriggerType);

        SceneLoadTrigger sceneLoadTrigger = (SceneLoadTrigger)target;

        if ((SceneLoadTrigger.LoadTriggerType)loadTriggerType.enumValueIndex == SceneLoadTrigger.LoadTriggerType.OnGameWorldInteract)
        {
            EditorGUILayout.PropertyField(interactMinDistance);
            EditorGUILayout.PropertyField(isInteractable);
            sceneLoadTrigger.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        else
        {
            sceneLoadTrigger.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        serializedObject.ApplyModifiedProperties();

        ManageColliders((SceneLoadTrigger.LoadTriggerType)loadTriggerType.enumValueIndex);
    }


    private void ManageColliders(SceneLoadTrigger.LoadTriggerType triggerType)
    {
        SceneLoadTrigger sceneLoadTrigger = (SceneLoadTrigger)target;
        Collider2D[] colliders2D = sceneLoadTrigger.GetComponents<Collider2D>();

        if (colliders2D.Length == 0 && triggerType != SceneLoadTrigger.LoadTriggerType.OnUIButtonClick)
        {
            EditorGUILayout.HelpBox("You need a collider for this load trigger type!", MessageType.Error);
            return;
        }

        bool enableColliders = triggerType != SceneLoadTrigger.LoadTriggerType.OnUIButtonClick;

        foreach (var collider2D in colliders2D)
        {
            collider2D.enabled = enableColliders;
            if (enableColliders)
            {
                collider2D.isTrigger = true;
            }
        }
    }

    private string[] GetSpawnPointsInScene(string sceneName)
    {
        string spawnPointDataPath = $"Assets/Resources/SpawnPointData.asset";
        SpawnPointData spawnPointData = AssetDatabase.LoadAssetAtPath<SpawnPointData>(spawnPointDataPath);
        if (spawnPointData != null)
        {
            return spawnPointData.spawnPoints
                .Where(sp => sp.sceneName == sceneName)
                .Select(sp => sp.spawnPointName)
                .ToArray();
        }
        return new string[0];
    }
}
