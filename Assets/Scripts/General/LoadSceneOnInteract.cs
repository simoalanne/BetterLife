using UnityEngine;

/// <summary>
/// This script can be used for objects that load a scene when interacted with
/// </summary>
[RequireComponent(typeof(Collider2D))]

public class LoadSceneOnInteract : MonoBehaviour, IInteractable
{
    [Header("Scene options")]
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private bool _playerVisibleInNewScene = true;

    [Header("Interact options")]
    [SerializeField] private Vector2 _interactMinDistance = new(0.5f, 0.5f);
    [SerializeField] private bool _isInteractable = true;

    public Vector2 InteractMinDistance { get; set;}
    public bool IsInteractable { get; set; }

    void Awake()
    {
        InteractMinDistance = _interactMinDistance;
        IsInteractable = _isInteractable;
    }

    public void Interact()
    {
        SceneLoader.Instance.LoadScene(_sceneToLoad, _playerVisibleInNewScene);
    }
}
