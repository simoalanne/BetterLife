using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This should be the only script used for scene loading.
/// The gameobject containing this script should be in the first scene 
/// that is loaded when the game starts and nowhere else.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Transition durations")]
    [SerializeField] private float _transitionInDuration = 0.75f;
    [SerializeField] private float _transitionOutDuration = 0.75f;

    [Header("Transition sprites")]
    [SerializeField] private Sprite _circleSprite;

    private CanvasGroup _canvasGroup;
    private Image _transitionImage;
    private Coroutine _activeCoroutine;
    private bool _transitionInDone;
    private bool _transitionOutDone;

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
        Doorway,
        None
    }

    private enum TransitionDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    private enum TransitionState
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
        _transitionImage = GetComponentInChildren<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0; // Set the canvas to be invisible initially
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts initially
    }

    public void LoadScene(string sceneName, PlayerVisibility visibility, TransitionType transitionType = TransitionType.Fade)
    {
        if (_activeCoroutine != null)
        {
            Debug.LogWarning("A scene is already being loaded!\n Player shouldn't be able to trigger multiple scene loads at once.");
            Debug.LogWarning("Don't let the player trigger scene load again until the current scene is loaded.");
            return;
        }

        Player.PlayerManager.Instance.DisablePlayerInteract(); // Disable the player interact script while the scene is being loaded
        Player.PlayerManager.Instance.DisablePlayerMovement(); // Disable the player movement script while the scene is being loaded
        _activeCoroutine = StartCoroutine(SceneTransition(sceneName, visibility, transitionType));
    }

    IEnumerator SceneTransition(string sceneName, PlayerVisibility visibility, TransitionType transitionType)
    {
        _canvasGroup.blocksRaycasts = true;
        var audioListener = FindObjectOfType<AudioListener>(); // Find the audio listener in the scene
        FindObjectOfType<UnityEngine.EventSystems.EventSystem>().gameObject.SetActive(false); // Disable the event system so there aren't two active at the same time
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); // Start scene loading in the background
        operation.allowSceneActivation = false; // Prevent the scene from being activated until the fade in is done
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Get the index of the current scene for unloading later

        switch (transitionType)
        {
            case TransitionType.Fade:
                StartCoroutine(FadeTransition(TransitionState.In));
                break;
            case TransitionType.SlideLeft:
                StartCoroutine(SlideTransition(TransitionDirection.Left, TransitionState.In));
                break;
            case TransitionType.SlideRight:
                StartCoroutine(SlideTransition(TransitionDirection.Right, TransitionState.In));
                break;
            case TransitionType.SlideUp:
                StartCoroutine(SlideTransition(TransitionDirection.Up, TransitionState.In));
                break;
            case TransitionType.SlideDown:
                StartCoroutine(SlideTransition(TransitionDirection.Down, TransitionState.In));
                break;
            case TransitionType.Circle:
                StartCoroutine(CircleTransition(TransitionState.In));
                break;
            case TransitionType.Doorway:
                StartCoroutine(DoorwayTransition(TransitionState.In));
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
        SceneManager.UnloadSceneAsync(currentSceneIndex); // Unload the current scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)); // Set the newly loaded scene as the active scene

        // Change the the player visibility once the screen is fully black to prevent player from being suddenly visible or invisible in the old scene.
        if (visibility == PlayerVisibility.Invisible)
        {
            Player.PlayerManager.Instance.DisableSpriteRenderer(); // Disable the player sprite renderer if the player is not supposed to be visible in the new scene
        }
        else if (visibility == PlayerVisibility.Visible)
        {
            Player.PlayerManager.Instance.EnableSpriteRenderer(); // Enable the player sprite renderer if the player is supposed to be visible in the new scene
        }

        switch (transitionType)
        {
            case TransitionType.Fade:
                StartCoroutine(FadeTransition(TransitionState.Out));
                break;
            case TransitionType.SlideLeft:
                StartCoroutine(SlideTransition(TransitionDirection.Left, TransitionState.Out));
                break;
            case TransitionType.SlideRight:
                StartCoroutine(SlideTransition(TransitionDirection.Right, TransitionState.Out));
                break;
            case TransitionType.SlideUp:
                StartCoroutine(SlideTransition(TransitionDirection.Up, TransitionState.Out));
                break;
            case TransitionType.SlideDown:
                StartCoroutine(SlideTransition(TransitionDirection.Down, TransitionState.Out));
                break;
            case TransitionType.Circle:
                StartCoroutine(CircleTransition(TransitionState.Out));
                break;
            case TransitionType.Doorway:
                StartCoroutine(DoorwayTransition(TransitionState.Out));
                break;
            case TransitionType.None:
                _transitionOutDone = true;
                break;
            default:
                Debug.LogError("Invalid transition type");
                break;
        }

        _activeCoroutine = null; // Set the active coroutine to null so that a new scene load can be triggered
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts after the fade out
        _transitionInDone = false;
        _transitionOutDone = false;

        // Enable the player interact and movement last so that the player can't trigger another scene load while this coroutine is still running
        if (visibility == PlayerVisibility.Visible)
        {
            Player.PlayerManager.Instance.EnablePlayerInteract(); // Enable the player interact script in the new scene
            Player.PlayerManager.Instance.EnablePlayerMovement(); // Enable the player movement script in the new scene
        }


        IEnumerator FadeTransition(TransitionState state)
        {
            float timer = 0;
            if (state == TransitionState.In)
            {
                while (timer < _transitionInDuration)
                {
                    timer += Time.deltaTime;
                    _canvasGroup.alpha = timer / _transitionInDuration;
                    yield return null;
                }

                _transitionInDone = true;
            }
            else if (state == TransitionState.Out)
            {
                while (timer < _transitionOutDuration)
                {
                    timer += Time.deltaTime;
                    _canvasGroup.alpha = 1 - Mathf.Pow(timer / _transitionOutDuration, 3); // Exponential fade out to make the fade out look smoother
                    yield return null;
                }

                _transitionOutDone = true;
            }
        }

        IEnumerator SlideTransition(TransitionDirection direction, TransitionState state)
        {
            // WIP
            yield return null;
        }

        IEnumerator CircleTransition(TransitionState state)
        {
            // WIP
            _transitionImage.sprite = _circleSprite;
            yield return null;
        }

        IEnumerator DoorwayTransition(TransitionState state)
        {
            // WIP
            yield return null;
        }
    }
}
