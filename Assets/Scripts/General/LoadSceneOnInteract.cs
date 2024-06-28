using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script can be used for objects that load a scene when interacted with
/// </summary>

public class LoadSceneOnInteract : MonoBehaviour, IInteractable
{
    [Header("Scene options")]
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private SceneLoader.PlayerVisibility _playerVisibilityInNewScene;
    [SerializeField] private SceneLoader.TransitionType _transitionType;

    [Header("Interact options")]
    [SerializeField] private Vector2 _interactMinDistance = new(0.5f, 0.5f);
    [SerializeField] private bool _isInteractable = true;

    public Vector2 InteractMinDistance { get; set; }
    public bool IsInteractable { get; set; }

    void Awake()
    {
        InteractMinDistance = _interactMinDistance;
        IsInteractable = _isInteractable;
    }

    public void Interact()
    {
        // Placeholder code. remove asap
        if (SceneManager.GetActiveScene().name == "BlackJack")
        {
            PlayerManager.Instance.MoneyInBankAccount = FindObjectOfType<PlayerScript>().GetMoney();
        }

        SceneLoader.Instance.LoadScene(_sceneToLoad, _playerVisibilityInNewScene, _transitionType);
    }
}
