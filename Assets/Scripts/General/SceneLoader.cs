using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Player;

/// <summary>
/// This should be the only script used for scene loading.
/// The gameobject containing this script should be in the first scene 
/// that is loaded when the game starts and nowhere else.
/// </summary>
[RequireComponent(typeof(SceneTransitionManager))]
public class SceneLoader : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private Image _transitionImage;
    private Coroutine _activeCoroutine;
    private bool _transitionInDone;
    private bool _transitionOutDone;
    private bool _isLoading = false;
    private SceneTransitionManager _transitionManager;

    public bool TransitionInDone { get => _transitionInDone; set => _transitionInDone = value; }
    public bool TransitionOutDone { get => _transitionOutDone; set => _transitionOutDone = value; }
    public bool IsLoading => _isLoading;

    public enum PlayerVisibility
    {
        Visible,
        Invisible
    }

    public enum TransitionType
    {
        Fade,
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        Circle,
        Random,
        None
    }

    public enum TransitionState
    {
        In,
        Out
    }

    public static SceneLoader Instance { get; private set; } // Make this script a singleton so that it can be easily called from other scripts

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _transitionManager = GetComponent<SceneTransitionManager>();
        _transitionImage = GetComponentInChildren<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0; // Set the canvas to be invisible initially
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts initially
    }

    public void LoadScene(string sceneName, PlayerVisibility visibility, TransitionType transitionType = TransitionType.Random, SpawnPoint spawnPoint = null)
    {
        if (_activeCoroutine != null)
        {
            return;
        }
        Time.timeScale = 1;
        _isLoading = true;

        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.DisablePlayerInteract(); // Disable the player interact script while the scene is being loaded
            PlayerManager.Instance.DisablePlayerMovement(); // Disable the player movement script while the scene is being loaded
        }

        if (transitionType == TransitionType.Random)
        {
            transitionType = (TransitionType)Random.Range(0, 6); // Randomize the transition type if it's set to random
        }

        _activeCoroutine = StartCoroutine(SceneTransition(sceneName, visibility, transitionType, spawnPoint));
    }

    IEnumerator SceneTransition(string sceneName, PlayerVisibility visibility, TransitionType transitionType, SpawnPoint spawnPoint)
    {
        var rectTransform = _transitionImage.rectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0;
        var audioListener = FindObjectOfType<AudioListener>(); // Find the audio listener in the scene
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
        {
            FindObjectOfType<UnityEngine.EventSystems.EventSystem>().enabled = false; // Disable the event system so that the player can't interact with UI while the scene is transitioning
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); // Start scene loading in the background
        operation.allowSceneActivation = false; // Prevent the scene from being activated until the fade in is done
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Get the index of the current scene for unloading later

        switch (transitionType)
        {
            case TransitionType.Fade:
                StartCoroutine(_transitionManager.FadeTransition(TransitionState.In));
                break;
            case TransitionType.SlideLeft:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.LeftToRight, TransitionState.In));
                break;
            case TransitionType.SlideRight:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.RightToLeft, TransitionState.In));
                break;
            case TransitionType.SlideDown:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.TopToBottom, TransitionState.In));
                break;
            case TransitionType.SlideUp:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.BottomToTop, TransitionState.In));
                break;
            case TransitionType.Circle:
                StartCoroutine(_transitionManager.CircleTransition(TransitionState.In));
                break;
            case TransitionType.None:
                _transitionInDone = true;
                break;
            default:
                Debug.LogError("Invalid transition type");
                break;
        }

        yield return new WaitUntil(() => _transitionInDone); // Wait until the transition in is done.
        audioListener.enabled = false;
        operation.allowSceneActivation = true;
        yield return operation; // Wait for the scene to be fully loaded in case it's not ready after the fade in.
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentSceneIndex); // Unload the old scene
        yield return unloadOperation; // Wait for the old scene to be fully unloaded
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)); // Set the newly loaded scene as the active scene
        // Change the the player visibility once the screen is fully black to prevent player from being suddenly visible or invisible in the old scene.
        if (visibility == PlayerVisibility.Invisible && PlayerManager.Instance != null)
        {
            PlayerManager.Instance.DisableSpriteRenderer(); // Disable the player sprite renderer if the player is not supposed to be visible in the new scene
        }
        else if (visibility == PlayerVisibility.Visible)
        {
            PlayerManager.Instance.EnableSpriteRenderer(); // Enable the player sprite renderer if the player is supposed to be visible in the new scene
            if (spawnPoint != null)
            {
                PlayerManager.Instance.transform.position = spawnPoint.spawnPoint; // Move the player to the spawn point
                PlayerManager.Instance.SetSpawnSprite(spawnPoint.facingDirection); // Set the player sprite based on the spawn point facing direction
            }
        }
        switch (transitionType)
        {
            case TransitionType.Fade:
                StartCoroutine(_transitionManager.FadeTransition(TransitionState.Out));
                break;
            case TransitionType.SlideLeft:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.LeftToRight, TransitionState.Out));
                break;
            case TransitionType.SlideRight:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.RightToLeft, TransitionState.Out));
                break;
            case TransitionType.SlideDown:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.TopToBottom, TransitionState.Out));
                break;
            case TransitionType.SlideUp:
                StartCoroutine(_transitionManager.SlideTransition(SceneTransitionManager.TransitionDirection.BottomToTop, TransitionState.Out));
                break;
            case TransitionType.Circle:
                StartCoroutine(_transitionManager.CircleTransition(TransitionState.Out));
                break;
            case TransitionType.None:
                _transitionOutDone = true;
                break;
            default:
                Debug.LogError("Invalid transition type");
                break;
        }

        _activeCoroutine = null; // Set the active coroutine to null so that a new scene load can be triggered

        yield return new WaitUntil(() => _transitionOutDone); // Wait until the transition out is done
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts after the fade out
        // Enable the player interact and movement last so that the player can't trigger another scene load while this coroutine is still running
        if (visibility == PlayerVisibility.Visible)
        {
            PlayerManager.Instance.EnablePlayerInteract(); // Enable the player interact script in the new scene
            PlayerManager.Instance.EnablePlayerMovement(); // Enable the player movement script in the new scene
        }

        _transitionInDone = false;
        _transitionOutDone = false;
        _isLoading = false;
    }
}
