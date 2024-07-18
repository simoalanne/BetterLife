using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPoint", menuName = "ScriptableObjects/SpawnPoint", order = 1)]
public class SpawnPoint : ScriptableObject
{
    public string sceneName;
    public Vector2 spawnPoint;
    public string spawnPointName;
}


