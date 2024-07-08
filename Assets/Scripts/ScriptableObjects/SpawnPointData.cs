using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnPointData", menuName = "ScriptableObjects/SpawnPointData", order = 1)]
public class SpawnPointData : ScriptableObject
{
    public List<SpawnPoint> spawnPoints;
}

[System.Serializable]
public class SpawnPoint
{
    public string sceneName;
    public string spawnPointName;
    public Vector2 position;
    public Vector2 playerFacingDirection;
}
