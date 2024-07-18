using Player;
using UnityEditor.EditorTools;
using UnityEngine;

public class CameraPlayerFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private float _minXPos;
    [SerializeField] private float _maxXPos;
    [SerializeField] private float _minYPos;
    [SerializeField] private float _maxYPos;
    [SerializeField, Tooltip("Higher value means faster camera movement")] private float _smoothTime = 2f;

    private Transform _playerTransform;
    // Start is called before the first frame update
    void Awake()
    {
        _playerTransform = PlayerManager.Instance.transform;
        transform.position = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
    }

    void Update()
    {
        if (_playerTransform == null) return;
        Vector3 targetPosition = new(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
        targetPosition.x = Mathf.Clamp(targetPosition.x, _minXPos, _maxXPos);
        targetPosition.y = Mathf.Clamp(targetPosition.y, _minYPos, _maxYPos);
        transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothTime * Time.deltaTime);
    }
}
