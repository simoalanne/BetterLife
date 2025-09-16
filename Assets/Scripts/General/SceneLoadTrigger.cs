using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private SceneLoader.PlayerVisibility _playerVisibilityInNewScene;
    [SerializeField] private SceneLoader.TransitionType _transitionType;
    [SerializeField] private LoadTriggerType _loadTriggerType;
    [SerializeField, Tooltip("Leave empty if doesnt apply")] private SpawnPoint _playerSpawnPoint;
    
    public bool CanInteract { get; set; } = true;

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
            if (TryGetComponent(out OpeningHours openingHours))
            {
                if (openingHours.IsOpen)
                {
                    LoadScene();
                }
                else
                {
                    openingHours.ShowClosedSign();
                }
            }
            else
            {
                LoadScene();
            }
            
        }
    }

    private void LoadScene()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(_sceneToLoad, _playerVisibilityInNewScene, _transitionType, _playerSpawnPoint);
        }
        else
        {
            SceneManager.LoadSceneAsync(_sceneToLoad);
            Debug.LogWarning("SceneLoader instance is null. Loading scene without transition.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && _loadTriggerType == LoadTriggerType.OnGameWorldTriggerEnter)
        {
            LoadScene();
        }
    }
}
