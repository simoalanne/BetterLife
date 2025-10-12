using System.Collections;
using Helpers;
using Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerVisibility
{
    Visible,
    Invisible
}

/// <summary> Centralized scene loading and transition manager. </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Circle Transition")]
    [SerializeField] private Material circleRevealMaterial;
    [SerializeField] private float maxCutOff = 0.6f;


    [Header("Customization")]
    [SerializeField] private float transitionInDuration = 0.5f;
    [SerializeField] private float transitionOutDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    private Image _transitionImage;

    public bool IsLoading { get; private set; }

    private void Awake()
    {
        Services.Register(this, dontDestroyOnLoad: true);
        _canvasGroup = GetComponent<CanvasGroup>();
        _transitionImage = GetComponentInChildren<Image>();
        circleRevealMaterial = Instantiate(circleRevealMaterial);
    }

    public void LoadMainMenu()
    {
        LoadScene("MainMenu", PlayerVisibility.Invisible);
    }

    public void LoadScene(string sceneName, PlayerVisibility visibility, SpawnPoint spawnPoint = null)
    {
        if (IsLoading) return;

        IsLoading = true;

        StartCoroutine(SceneTransition(sceneName, visibility, spawnPoint));
    }

    private IEnumerator SceneTransition(string sceneName, PlayerVisibility visibility, SpawnPoint spawnPoint)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0;
        var oldScene = SceneManager.GetActiveScene().name;
        
        // Step 1: Fade to black and load the new scene in the background
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (op is null) throw new System.Exception("Scene " + sceneName + " not found");
        op.allowSceneActivation = false;
        yield return _canvasGroup.Fade(1, transitionInDuration);
        op.allowSceneActivation = true;
        yield return op;
        
        // Step 2: Fade from black to the new scene and handle player visibility and positioning
        var playerManager = Services.PlayerManager;
        if (visibility is PlayerVisibility.Visible)
        {
            playerManager.Teleport(spawnPoint);
            playerManager.EnablePlayer();
        }
        else
        {
            playerManager.DisablePlayer();
        }

        var unloadOp = SceneManager.UnloadSceneAsync(oldScene);
        yield return _canvasGroup.Fade(0, transitionOutDuration);
        yield return unloadOp;
        
        _canvasGroup.blocksRaycasts = false;
        IsLoading = false;
        // Handle player visibility and positioning
    }
}
