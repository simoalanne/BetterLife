using System.Collections.Generic;
using Helpers;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadTrigger : MonoBehaviour, IInteractable
{
    [Dropdown("AllScenes")]
    [SerializeField] private string sceneToLoad;

    // ReSharper disable once UnusedMember.Local NaughtyAttributes can't resolve methods from other classes so this is needed
    private static List<string> AllScenes => FunctionLibrary.GetAllSceneNames();

    [SerializeField] private PlayerVisibility playerVisibilityInNewScene;
    [SerializeField] private LoadTriggerType loadTriggerType;
    [SerializeField, Tooltip("Leave empty if doesnt apply")]
    private SpawnPoint playerSpawnPoint;

    public bool CanInteract { get; set; } = true;

    private enum LoadTriggerType
    {
        OnGameWorldInteract,
        OnGameWorldTriggerEnter,
        OnUIButtonClick,
        Manual
    }

    private const string PlayerTag = "Player";

    private void Reset()
    {
        if (GetComponent<Button>() != null)
        {
            loadTriggerType = LoadTriggerType.OnUIButtonClick;
        }
    }

    private void OnValidate()
    {
        if (loadTriggerType == LoadTriggerType.OnGameWorldInteract)
        {
            if (GetComponents<IInteractable>().Length > 1)
            {
                loadTriggerType = LoadTriggerType.Manual;
                Debug.LogWarning(
                    $"{name}: Multiple IInteractable components detected. SceneLoadTrigger set to Manual.");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
        }
        else if (GetComponents<IInteractable>().Length <= 1)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void Awake()
    {
        if (loadTriggerType == LoadTriggerType.OnUIButtonClick)
        {
            GetComponent<Button>().onClick.AddListener(LoadScene);
        }
    }

    public void Interact()
    {
        if (loadTriggerType == LoadTriggerType.OnGameWorldInteract) LoadScene();
    }

    public void LoadScene()
    {
        if (Services.TryGet<SceneLoader>(out var sceneLoader))
        {
            sceneLoader.LoadScene(sceneToLoad, playerVisibilityInNewScene, playerSpawnPoint);
            return;
        }

        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag) && loadTriggerType is LoadTriggerType.OnGameWorldTriggerEnter)
        {
            LoadScene();
        }
    }
}
