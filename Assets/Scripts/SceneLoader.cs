using System;
using System.Collections;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

public record SceneChanged(string OldScene, string NewScene);

[RequireComponent(typeof(CanvasGroup))]
public class SceneLoader : MonoBehaviour
{
    [Header("Customization")]
    [SerializeField] private float transitionInDuration = 0.5f;
    [SerializeField] private float transitionOutDuration = 0.25f;

    private CanvasGroup _canvasGroup;
    public bool IsLoading { get; private set; }

    public Action<SceneChanged> OnSceneChanged;

    // Used to prevent stopping the transition coroutine before the old scene is unloaded
    // this is needed because some scripts call TemporaryFade on Awake which, if cancelled too early
    // would result in the old scene never being unloaded which is obviously not desired.
    private bool _safeToStopTransitionCoroutine = true;
    private Coroutine _sceneLoadCoroutine;

    private void Awake()
    {
        Services.Register(this, persistent: true);
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0;
    }

    /// <summary> Can be used for fading the screen to black or back for other purposes than scene loading. </summary>
    public IEnumerator TemporaryFade(bool fadeIn, bool instant = false, float? overrideDuration = null)
    {
        yield return new WaitUntil(() => _safeToStopTransitionCoroutine);
        IsLoading = false;
        if (_sceneLoadCoroutine != null) StopCoroutine(_sceneLoadCoroutine);
        _sceneLoadCoroutine = null;
        _canvasGroup.blocksRaycasts = true;
        var targetAlpha = fadeIn ? 1 : 0;
        if (instant)
        {
            _canvasGroup.alpha = targetAlpha;
            _canvasGroup.blocksRaycasts = false;
            yield break;
        }

        var duration = overrideDuration ?? (fadeIn ? transitionInDuration : transitionOutDuration);
        yield return _canvasGroup.Fade(targetAlpha, duration);
        _canvasGroup.blocksRaycasts = false;
    }

    public void LoadScene(string sceneName)
    {
        if (IsLoading) return;

        IsLoading = true;

        _sceneLoadCoroutine = StartCoroutine(SceneTransition(sceneName));
    }

    private IEnumerator SceneTransition(string sceneName)
    {
        _safeToStopTransitionCoroutine = false;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0;
        var oldScene = SceneManager.GetActiveScene().name;

        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (op is null) throw new Exception("Scene " + sceneName + " not found");
        op.allowSceneActivation = false;
        yield return _canvasGroup.Fade(1, transitionInDuration);
        op.allowSceneActivation = true;
        yield return op;
        var unloadOp = SceneManager.UnloadSceneAsync(oldScene);
        yield return unloadOp;
        _safeToStopTransitionCoroutine = true;
        OnSceneChanged?.Invoke(new SceneChanged(oldScene, sceneName));
        yield return _canvasGroup.Fade(0, transitionOutDuration);
        _canvasGroup.blocksRaycasts = false;
        IsLoading = false;
        _sceneLoadCoroutine = null;
    }
}
