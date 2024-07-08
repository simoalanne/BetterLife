using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SpawnPointUpdater
{
    static SpawnPointUpdater()
    {
        EditorSceneManager.sceneSaved += scene => EditorApplication.delayCall += () => UpdateSpawnPoints(scene);
    }

    private static void UpdateSpawnPoints(Scene scene)
    {
        string spawnPointDataPath = "Assets/Resources/SpawnPointData.asset";
        Debug.Log($"Updating spawn points for scene: {scene.name}");
    
        SpawnPointData spawnPointData = AssetDatabase.LoadAssetAtPath<SpawnPointData>(spawnPointDataPath);
        if (spawnPointData == null)
        {
            Debug.Log("SpawnPointData not found, creating new one.");
            spawnPointData = ScriptableObject.CreateInstance<SpawnPointData>();
            AssetDatabase.CreateAsset(spawnPointData, spawnPointDataPath);
        }
        else
        {
            Debug.Log("SpawnPointData found, updating.");
        }
    
        // Remove existing spawn points for the scene
        spawnPointData.spawnPoints.RemoveAll(sp => sp.sceneName == scene.name);
    
        // Find all spawn points in the scene
        foreach (GameObject obj in scene.GetRootGameObjects())
        {
            foreach (var spawnPointComponent in obj.GetComponentsInChildren<PlayerSpawnPoint>())
            {
                SpawnPoint spawnPoint = new()
                {
                    sceneName = scene.name,
                    spawnPointName = spawnPointComponent.name,
                    position = spawnPointComponent.transform.position,
                    playerFacingDirection = spawnPointComponent.playerFacingDirection
                };
                spawnPointData.spawnPoints.Add(spawnPoint);
            }
        }
        EditorUtility.SetDirty(spawnPointData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); // Ensure the AssetDatabase is up-to-date
    
        Debug.Log("Spawn points updated successfully.");
    }
}