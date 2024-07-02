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
    [Header("Fade animation durations")]
    [SerializeField] private float _fadeInDuration = 0.75f;
    [SerializeField] private float _fadeOutDuration = 0.75f;

    [Header("Slide animation durations")]
    [SerializeField] private float _slideInDuration = 2.5f;
    [SerializeField] private float _slideOutDuration = 2.5f;

    [Header("Circle animation durations")]
    [SerializeField] private float _circleInDuration = 1.5f;
    [SerializeField] private float _circleOutDuration = 1.5f;

    [Header("Transition sprites")]
    [SerializeField] private Sprite _circleSprite;

    [Header("Transition settings")]
    [SerializeField] private Color _transitionColor = Color.black;

    [Header("Materials")]
    [SerializeField] private Material _circleRevealMaterial;

    private CanvasGroup _canvasGroup;
    private Image _transitionImage;
    private Coroutine _activeCoroutine;
    private bool _transitionInDone;
    private bool _transitionOutDone;
    private Material _originalMaterial;

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

    private enum TransitionDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
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
        _originalMaterial = _transitionImage.material;
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0; // Set the canvas to be invisible initially
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts initially
        _transitionImage.color = _transitionColor;
        _circleRevealMaterial = Instantiate(_circleRevealMaterial); // Instantiate the material so that the original material doesn't get changed and doesn't appear in version control.
        _circleRevealMaterial.SetColor("_TransitionColor", _transitionColor);
    }

    public void LoadScene(string sceneName, PlayerVisibility visibility, TransitionType transitionType = TransitionType.Random)
    {
        if (_activeCoroutine != null)
        {
            Debug.LogWarning("A scene is already being loaded!\n Player shouldn't be able to trigger multiple scene loads at once.");
            Debug.LogWarning("Don't let the player trigger scene load again until the current scene is loaded.");
            return;
        }

        Player.PlayerManager.Instance.DisablePlayerInteract(); // Disable the player interact script while the scene is being loaded
        Player.PlayerManager.Instance.DisablePlayerMovement(); // Disable the player movement script while the scene is being loaded

        if (transitionType == TransitionType.Random)
        {
            transitionType = (TransitionType)Random.Range(0, 6); // Randomize the transition type if it's set to random
        }

        _activeCoroutine = StartCoroutine(SceneTransition(sceneName, visibility, transitionType));
    }

    IEnumerator SceneTransition(string sceneName, PlayerVisibility visibility, TransitionType transitionType)
    {
        var rectTransform = _transitionImage.rectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0;
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
                StartCoroutine(SlideTransition(TransitionDirection.LeftToRight, TransitionState.In));
                break;
            case TransitionType.SlideRight:
                StartCoroutine(SlideTransition(TransitionDirection.RightToLeft, TransitionState.In));
                break;
            case TransitionType.SlideDown:
                StartCoroutine(SlideTransition(TransitionDirection.TopToBottom, TransitionState.In));
                break;
            case TransitionType.SlideUp:
                StartCoroutine(SlideTransition(TransitionDirection.BottomToTop, TransitionState.In));
                break;
            case TransitionType.Circle:
                StartCoroutine(CircleTransition(TransitionState.In));
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
                StartCoroutine(SlideTransition(TransitionDirection.LeftToRight, TransitionState.Out));
                break;
            case TransitionType.SlideRight:
                StartCoroutine(SlideTransition(TransitionDirection.RightToLeft, TransitionState.Out));
                break;
            case TransitionType.SlideDown:
                StartCoroutine(SlideTransition(TransitionDirection.TopToBottom, TransitionState.Out));
                break;
            case TransitionType.SlideUp:
                StartCoroutine(SlideTransition(TransitionDirection.BottomToTop, TransitionState.Out));
                break;
            case TransitionType.Circle:
                StartCoroutine(CircleTransition(TransitionState.Out));
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
            Player.PlayerManager.Instance.EnablePlayerInteract(); // Enable the player interact script in the new scene
            Player.PlayerManager.Instance.EnablePlayerMovement(); // Enable the player movement script in the new scene
        }

        _transitionInDone = false;
        _transitionOutDone = false;

        IEnumerator FadeTransition(TransitionState state)
        {
            float timer = 0;
            if (state == TransitionState.In)
            {
                while (timer < _fadeInDuration)
                {
                    timer += Time.deltaTime;
                    _canvasGroup.alpha = timer / _fadeInDuration;
                    yield return null;
                }
                _canvasGroup.alpha = 1;
                _transitionInDone = true;
            }
            else if (state == TransitionState.Out)
            {
                while (timer < _fadeOutDuration)
                {
                    timer += Time.deltaTime;
                    _canvasGroup.alpha = 1 - Mathf.Pow(timer / _fadeOutDuration, 3); // Exponential fade out to make the fade out look smoother
                    yield return null;
                }
                _canvasGroup.alpha = 0;
                _transitionOutDone = true;
            }
        }

        IEnumerator SlideTransition(TransitionDirection direction, TransitionState state)
        {
            var rectTransform = _transitionImage.rectTransform;
            Vector2 startPos = Vector2.zero;
            Vector2 endPos = Vector2.zero;

            var canvasRectTransform = GetComponent<RectTransform>();
            float canvasWidth = canvasRectTransform.sizeDelta.x;
            float canvasHeight = canvasRectTransform.sizeDelta.y; ;

            if (state == TransitionState.In)
            {
                switch (direction)
                {
                    case TransitionDirection.LeftToRight:
                        startPos = new Vector2(-canvasWidth, 0);
                        break;
                    case TransitionDirection.RightToLeft:
                        startPos = new Vector2(canvasWidth, 0);
                        break;
                    case TransitionDirection.TopToBottom:
                        startPos = new Vector2(0, canvasHeight);
                        break;
                    case TransitionDirection.BottomToTop:
                        startPos = new Vector2(0, -canvasHeight);
                        break;
                    default:
                        Debug.LogError("Invalid transition direction");
                        yield break; // Exit the coroutine if the direction is invalid
                }
                _canvasGroup.alpha = 1;
                rectTransform.anchoredPosition = startPos;
            }
            else
            {
                switch (direction)
                {
                    case TransitionDirection.LeftToRight:
                        endPos = new Vector2(canvasWidth, 0);
                        break;
                    case TransitionDirection.RightToLeft:
                        endPos = new Vector2(-canvasWidth, 0);
                        break;
                    case TransitionDirection.TopToBottom:
                        endPos = new Vector2(0, -canvasHeight);
                        break;
                    case TransitionDirection.BottomToTop:
                        endPos = new Vector2(0, canvasHeight);
                        break;
                    default:
                        Debug.LogError("Invalid transition direction");
                        yield break; // Exit the coroutine if the direction is invalid
                }
            }

            float timer = 0;
            float transitionDuration = state == TransitionState.In ? _slideInDuration : _slideOutDuration;

            while (timer < transitionDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / transitionDuration; // Normalize the time to a 0-1 range
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, progress); // Use the normalized time here
                yield return null;
            }

            rectTransform.anchoredPosition = endPos; // Make sure the position is exactly where it should be at the end
            if (state == TransitionState.In) _transitionInDone = true;
            else _transitionOutDone = true;
        }

        IEnumerator CircleTransition(TransitionState state)
        {
            float timer = 0;

            if (state == TransitionState.In)
            {
                _transitionImage.sprite = _circleSprite;
                _transitionImage.material = _circleRevealMaterial;
                _transitionImage.material.SetFloat("_Cutoff", 0.6f); // Circle is bigger than the screen so cutoff can start from this value
                _canvasGroup.alpha = 1;

                while (timer < _circleInDuration)
                {
                    timer += Time.deltaTime;
                    _transitionImage.material.SetFloat("_Cutoff", 0.6f - timer / _circleInDuration);
                    yield return null;
                }

                _transitionInDone = true;
            }

            else if (state == TransitionState.Out)
            {
                while (timer < _circleOutDuration)
                {
                    timer += Time.deltaTime;
                    _transitionImage.material.SetFloat("_Cutoff", timer / _circleOutDuration - 0.4f);
                    yield return null;
                }
                _transitionImage.sprite = null;
                _transitionImage.material = _originalMaterial;
                _transitionOutDone = true;
                _canvasGroup.alpha = 0;
                _transitionOutDone = true;
            }
        }
    }
}
