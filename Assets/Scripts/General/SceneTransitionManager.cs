using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Transition durations")]
    [SerializeField] private float _transitionInDuration = 0.5f;
    [SerializeField] private float _transitionOutDuration = 0.25f;

    [Header("Transition sprites")]
    [SerializeField] private Sprite _circleSprite;

    [Header("Materials")]
    [SerializeField] private Material _circleRevealMaterial;

    private CanvasGroup _canvasGroup;
    private Image _transitionImage;
    private Material _originalMaterial;
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = GetComponent<SceneLoader>();
        _transitionImage = GetComponentInChildren<Image>();
        _originalMaterial = _transitionImage.material;
        _canvasGroup = GetComponent<CanvasGroup>();
        _circleRevealMaterial = Instantiate(_circleRevealMaterial); // Instantiate the material so that the original material doesn't get changed and doesn't appear in version control.
    }

    public enum TransitionDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }

    public IEnumerator FadeTransition(SceneLoader.TransitionState state)
    {
        float timer = 0;
        if (state == SceneLoader.TransitionState.In)
        {
            while (timer < _transitionInDuration)
            {
                timer += Time.deltaTime;
                _canvasGroup.alpha = timer / _transitionInDuration;
                yield return null;
            }
            _canvasGroup.alpha = 1;
            _sceneLoader.TransitionInDone = true;
        }
        else if (state == SceneLoader.TransitionState.Out)
        {
            while (timer < _transitionOutDuration)
            {
                timer += Time.deltaTime;
                _canvasGroup.alpha = 1 - Mathf.Pow(timer / _transitionOutDuration, 3); // Exponential fade out to make the fade out look smoother
                yield return null;
            }
            _canvasGroup.alpha = 0;
            _sceneLoader.TransitionOutDone = true;
        }
    }

    public IEnumerator SlideTransition(TransitionDirection direction, SceneLoader.TransitionState state)
    {
        var rectTransform = _transitionImage.rectTransform;
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = Vector2.zero;

        var canvasRectTransform = GetComponent<RectTransform>();
        float canvasWidth = canvasRectTransform.sizeDelta.x;
        float canvasHeight = canvasRectTransform.sizeDelta.y;

        if (state == SceneLoader.TransitionState.In)
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
        float transitionDuration = state == SceneLoader.TransitionState.In ? _transitionInDuration : _transitionOutDuration;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / transitionDuration; // Normalize the time to a 0-1 range
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, progress); // Use the normalized time here
            yield return null;
        }

        rectTransform.anchoredPosition = endPos; // Make sure the position is exactly where it should be at the end
        if (state == SceneLoader.TransitionState.In)
        {
            _sceneLoader.TransitionInDone = true;
        }
        else
        {
            _sceneLoader.TransitionOutDone = true;
        }
    }

    public IEnumerator CircleTransition(SceneLoader.TransitionState state)
    {
        float timer = 0;

        if (state == SceneLoader.TransitionState.In)
        {
            _transitionImage.sprite = _circleSprite;
            _transitionImage.material = _circleRevealMaterial;
            _transitionImage.material.SetFloat("_Cutoff", 0.6f); // Circle is bigger than the screen so cutoff can start from this value
            _canvasGroup.alpha = 1;

            while (timer < _transitionInDuration)
            {
                timer += Time.deltaTime;
                _transitionImage.material.SetFloat("_Cutoff", 0.6f - timer / _transitionInDuration);
                yield return null;
            }

            _sceneLoader.TransitionInDone = true;
        }

        else if (state == SceneLoader.TransitionState.Out)
        {
            while (timer < _transitionOutDuration)
            {
                timer += Time.deltaTime;
                _transitionImage.material.SetFloat("_Cutoff", timer / _transitionOutDuration - 0.4f);
                yield return null;
            }
            _transitionImage.sprite = null;
            _transitionImage.material = _originalMaterial;
            _sceneLoader.TransitionOutDone = true;
            _canvasGroup.alpha = 0;
        }
    }
}
