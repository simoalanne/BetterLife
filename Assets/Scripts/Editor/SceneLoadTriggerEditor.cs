using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI; // Required for checking if the component is attached to a UI Button

[CustomEditor(typeof(SceneLoadTrigger))]
public class SceneLoadTriggerEditor : Editor
{
    SerializedProperty sceneToLoad;
    SerializedProperty loadTriggerType;
    SerializedProperty interactMinDistance;
    SerializedProperty isInteractable;

    void OnEnable()
    {
        sceneToLoad = serializedObject.FindProperty("_sceneToLoad");
        loadTriggerType = serializedObject.FindProperty("_loadTriggerType");
        interactMinDistance = serializedObject.FindProperty("_interactMinDistance");
        isInteractable = serializedObject.FindProperty("_isInteractable");

        // Automatically set trigger type to UIButton if attached to a button
        if (((SceneLoadTrigger)target).GetComponent<Button>() != null)
        {
            loadTriggerType.enumValueIndex = (int)SceneLoadTrigger.LoadTriggerType.OnUIButtonClick;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Dynamic scene selection (excluding active scene)
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled && scene.path != SceneManager.GetActiveScene().path) // Exclude active scene
            .Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path))
            .ToArray();
        int currentSceneIndex = System.Array.IndexOf(scenes, sceneToLoad.stringValue);
        int newSceneIndex = EditorGUILayout.Popup("Scene To Load", currentSceneIndex, scenes);
        if (newSceneIndex != currentSceneIndex)
        {
            sceneToLoad.stringValue = scenes[newSceneIndex];
        }

        // Existing fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_playerVisibilityInNewScene"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_transitionType"));
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

        // Collider management remains unchanged
        ManageColliders((SceneLoadTrigger.LoadTriggerType)loadTriggerType.enumValueIndex);
    }

    private void ManageColliders(SceneLoadTrigger.LoadTriggerType triggerType)
    {
        SceneLoadTrigger SceneLoadTrigger = (SceneLoadTrigger)target;
        Collider2D[] colliders2D = SceneLoadTrigger.GetComponents<Collider2D>();

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
                collider2D.isTrigger = true; // Collider is always a trigger
            }
        }
    }
}