using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class SceneLoadTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private SceneLoader.PlayerVisibility _playerVisibilityInNewScene;
    [SerializeField] private SceneLoader.TransitionType _transitionType;
    [SerializeField] private LoadTriggerType _loadTriggerType;
    [SerializeField] private string _playerSpawnPoint;

    [Header("Game world interact options")]
    [SerializeField] private Vector2 _interactMinDistance = new(0.5f, 0.5f);
    [SerializeField] private bool _isInteractable = true;

    public Vector2 InteractMinDistance { get; set; }
    public bool IsInteractable { get; set; }

    public enum LoadTriggerType
    {
        OnGameWorldInteract,
        OnGameWorldTriggerEnter,
        OnUIButtonClick,
    }

    private const string PLAYER_TAG = "Player";

    void Awake()
    {
        if (_loadTriggerType == LoadTriggerType.OnUIButtonClick)
        {
            GetComponent<Button>().onClick.AddListener(() => LoadScene());
        }

        InteractMinDistance = _interactMinDistance;
        IsInteractable = _isInteractable;
    }

    public void Interact()
    {
        if (_loadTriggerType == LoadTriggerType.OnGameWorldInteract)
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "BlackJack")
        {
            PlayerManager.Instance.MoneyInBankAccount = FindObjectOfType<PlayerScript>().GetMoney();
        }

        if (SceneLoader.Instance != null)
        {
            Vector2 spawnPointPosition = GetSpawnPointPosition(_sceneToLoad, _playerSpawnPoint);
            SceneLoader.Instance.LoadScene(_sceneToLoad, _playerVisibilityInNewScene, _transitionType, spawnPointPosition);
        }
    }

    private Vector2 GetSpawnPointPosition(string sceneName, string spawnPointName)
    {
        Debug.Log($"Scene name: {sceneName}, Spawn point name: {spawnPointName}");
        SpawnPointData spawnPointData = Resources.Load<SpawnPointData>("SpawnPointData");
        if (spawnPointData != null)
        {
            var spawnPoint = spawnPointData.spawnPoints
                .FirstOrDefault(sp => sp.sceneName == sceneName && sp.spawnPointName == spawnPointName);
            if (spawnPoint != null)
            {
                return spawnPoint.position;
            }
        }
        return default;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && _loadTriggerType == LoadTriggerType.OnGameWorldTriggerEnter)
        {
            LoadScene();
        }
    }
}
