using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewSpawnPoint", menuName = "ScriptableObjects/SpawnPoint", order = 1)]
    public class SpawnPoint : ScriptableObject
    {
        public enum FacingDirection
        {
            FrontLeft,
            FrontRight,
            BackLeft,
            BackRight,
        }

        public string sceneName;
        public Vector2 spawnPoint;
        [SerializeField, Tooltip("Direction the player will face when spawned")]
        public FacingDirection facingDirection;
    }

    public static class SpawnPointExtensions
    {
        public static Vector2 ToVector2(this SpawnPoint.FacingDirection facingDirection)
        {
            return facingDirection switch
            {
                SpawnPoint.FacingDirection.FrontLeft => new Vector2(-1, -1).normalized,
                SpawnPoint.FacingDirection.FrontRight => new Vector2(1, -1).normalized,
                SpawnPoint.FacingDirection.BackLeft => new Vector2(-1, 1).normalized,
                SpawnPoint.FacingDirection.BackRight => new Vector2(1, 1).normalized,
                _ => Vector2.zero
            };
        }
    }
}
