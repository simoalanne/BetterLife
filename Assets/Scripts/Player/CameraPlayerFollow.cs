using Helpers;
using Player;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private float _minX, maxX, _minY, _maxY;

    private Transform _playerTransform;

    private void LateUpdate()
    {
        _playerTransform ??= Services.PlayerManager.transform;
        var targetPos = new Vector3(Mathf.Clamp(_playerTransform.position.x, _minX, maxX),
        Mathf.Clamp(_playerTransform.position.y, _minY, _maxY), transform.position.z);
        transform.position = targetPos;
    }
}
