using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This should be the only script used for scene loading.
/// The gameobject containing this script should be in the first scene 
/// that is loaded when the game starts and nowhere else.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private float _fadeInDuration = 0.75f;
    [SerializeField] private float _fadeOutDuration = 0.75f;

    private CanvasGroup _canvasGroup;
    private Coroutine _activeCoroutine;

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

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0; // Set the canvas to be invisible initially
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts initially
    }

    public void LoadScene(string sceneName, bool playerVisible)
    {
        if (_activeCoroutine != null)
        {
            Debug.LogWarning("A scene is already being loaded!\n Player shouldn't be able to trigger multiple scene loads at once.");
            Debug.LogWarning("Don't let the player trigger scene load again until the current scene is loaded.");
            return;
        }

        Player.PlayerManager.Instance.DisablePlayerInteract(); // Disable the player interact script while the scene is being loaded
        Player.PlayerManager.Instance.DisablePlayerMovement(); // Disable the player movement script while the scene is being loaded
        _activeCoroutine = StartCoroutine(FadeAndLoadScene(sceneName, playerVisible));
    }

    IEnumerator FadeAndLoadScene(string sceneName, bool playerVisible)
    {
        _canvasGroup.blocksRaycasts = true;
        var audioListener = FindObjectOfType<AudioListener>(); // Find the audio listener in the scene
        FindObjectOfType<UnityEngine.EventSystems.EventSystem>().gameObject.SetActive(false);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); // Start scene loading in the background
        operation.allowSceneActivation = false; // Prevent the scene from being activated until the fade in is done

        float timer = 0;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Get the index of the current scene for unloading later

        while (timer < _fadeInDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = timer / _fadeInDuration;
            yield return null;
        }

        _canvasGroup.alpha = 1; // Make sure the alpha is exactly 1 at the end of the fade in

        audioListener.enabled = false; // Disable the audio listener in the current scene to prevent 2 audio listeners from being active at the same time
        operation.allowSceneActivation = true; // allow the scene to be activated when fade in is done.

        yield return operation; // Wait for the scene to be fully loaded in case it's not ready after the fade in.
        SceneManager.UnloadSceneAsync(currentSceneIndex); // Unload the current scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName)); // Set the newly loaded scene as the active scene

        // Change the the player visibility once the screen is fully black to prevent player from being suddenly visible or invisible in the old scene.
        if (!playerVisible)
        {
            Player.PlayerManager.Instance.DisableSpriteRenderer(); // Disable the player sprite renderer if the player is not supposed to be visible in the new scene
        }
        else
        {
            Player.PlayerManager.Instance.EnableSpriteRenderer(); // Enable the player sprite renderer if the player is supposed to be visible in the new scene
        }

        timer = 0;
        while (timer < _fadeInDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = 1 - Mathf.Pow(timer / _fadeOutDuration, 3); // Exponential fade out to make the fade out look smoother
            yield return null;
        }
        _canvasGroup.alpha = 0; // Make sure the alpha is exactly 0 at the end of the fade out
        _activeCoroutine = null; // Set the active coroutine to null so that a new scene load can be triggered
        _canvasGroup.blocksRaycasts = false; // Make sure the canvas group doesn't block raycasts after the fade out

        // Enable the player interact and movement last so that the player can't trigger another scene load while this coroutine is still running
        if (playerVisible)
        {
            Player.PlayerManager.Instance.EnablePlayerInteract(); // Enable the player interact script in the new scene
            Player.PlayerManager.Instance.EnablePlayerMovement(); // Enable the player movement script in the new scene
        }
    }
}
