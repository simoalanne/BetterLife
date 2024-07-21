using Player;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private float _minX, maxX, _minY, _maxY;

    private Transform _playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = PlayerManager.Instance.transform;
    }

    void LateUpdate()
    {
        if (_playerTransform == null) return;

        var targetPos = new Vector3(Mathf.Clamp(_playerTransform.position.x, _minX, maxX),
        Mathf.Clamp(_playerTransform.position.y, _minY, _maxY), transform.position.z);
        transform.position = targetPos;
    }
}
