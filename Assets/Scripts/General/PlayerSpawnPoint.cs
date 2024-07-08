using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private Vector2 _playerFacingDirection = Vector2.right;
    public Vector2 playerFacingDirection => _playerFacingDirection;
}
