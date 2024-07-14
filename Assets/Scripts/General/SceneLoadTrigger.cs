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
        if (SceneLoader.Instance != null)
        {
            Vector2 spawnPointPosition = GetSpawnPointPosition(_sceneToLoad, _playerSpawnPoint);
            SceneLoader.Instance.LoadScene(_sceneToLoad, _playerVisibilityInNewScene, _transitionType, spawnPointPosition);
        }
        else
        {
            SceneManager.LoadSceneAsync(_sceneToLoad);
            Debug.LogWarning("SceneLoader instance is null. Loading scene without transition.");
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
