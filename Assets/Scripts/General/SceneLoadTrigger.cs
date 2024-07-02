using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadTrigger : MonoBehaviour, IInteractable
{
    [Header("Scene options")]
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private SceneLoader.PlayerVisibility _playerVisibilityInNewScene;
    [SerializeField] private SceneLoader.TransitionType _transitionType;

    [Header("Which event triggers the scene load?")]
    [SerializeField] private LoadTriggerType _loadTriggerType;

    [Header("Game world interact options")]
    [SerializeField] private Vector2 _interactMinDistance = new Vector2(0.5f, 0.5f);
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
        InteractMinDistance = _interactMinDistance;
        IsInteractable = _isInteractable;
    }

    /// <summary>
    /// This method is called when the player interacts with an object,
    /// containing this script.
    /// </summary>
    public void Interact()
    {
        if (_loadTriggerType == LoadTriggerType.OnGameWorldInteract)
        {
            LoadScene();
        }
    }

    public void LoadScene()
    {
        if (SceneManager.GetActiveScene().name == "BlackJack")
        {
            PlayerManager.Instance.MoneyInBankAccount = FindObjectOfType<PlayerScript>().GetMoney();
        }

        SceneLoader.Instance.LoadScene(_sceneToLoad, _playerVisibilityInNewScene, _transitionType);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && _loadTriggerType == LoadTriggerType.OnGameWorldTriggerEnter)
        {
            LoadScene();
        }
    }
}
