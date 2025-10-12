using System;
using System.Collections;
using Helpers;
using NaughtyAttributes;
using UnityEngine;

namespace UI
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(CanvasGroup))]
    public class HideableElement : MonoBehaviour
    {
        private enum State
        {
            Hiding,
            Showing,
            Idle
        }

        [Header("Effects")]
        [SerializeField] private float effectDuration = 0.25f;

        [Header("Effects|Move")]
        [SerializeField] private bool useMoveEffect;
        [SerializeField, ShowIf(nameof(useMoveEffect))] private Vector2 visiblePosition;
        [SerializeField, ShowIf(nameof(useMoveEffect))] private Vector2 hiddenPosition;

        [Header("Effects|Scale")]
        [SerializeField] private bool useScaleEffect;
        [SerializeField, ShowIf(nameof(useScaleEffect))] private Vector3 visibleScale = Vector3.one;

        [Header("Effects|Fade")]
        [SerializeField] private bool useFadeEffect;
        [SerializeField, ShowIf(nameof(useFadeEffect))] private float visibleAlpha = 1f;

        private RectTransform _rt;
        private CanvasGroup _cg;

        private State _state = State.Idle;
        private float _cachedTime;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _cg = GetComponent<CanvasGroup>();
            _state = State.Idle;
        }

        public void InitialVisibility(bool visible) => SetFinalValues(visible);

        public void Toggle(bool show, bool instant = false)
        {
            if (instant)
            {
                SetFinalValues(show);
                return;
            }

            var animationCancelled = _state is State.Hiding && show || _state is State.Showing && !show;
            var animationContinuing = _state is State.Hiding && !show || _state is State.Showing && show;
            var adjustedDuration = (animationCancelled, animationContinuing) switch
            {
                (true, _) => _cachedTime,
                (_, true) => effectDuration - _cachedTime,
                _ => effectDuration
            };
            StopAllCoroutines();
            StartCoroutine(ToggleElement(show, adjustedDuration));
        }

        private void SetFinalValues(bool show)
        {
            StopAllCoroutines();
            _state = State.Idle;
            _cachedTime = 0f;

            if (useMoveEffect)
            {
                var endPos = show ? visiblePosition : hiddenPosition;
                _rt.anchoredPosition = endPos;
            }

            if (useScaleEffect)
            {
                var endScale = show ? visibleScale : Vector3.zero;
                _rt.localScale = endScale;
            }

            if (!useFadeEffect) return;

            var endAlpha = show ? visibleAlpha : 0f;
            _cg.alpha = endAlpha;
        }

        private IEnumerator ToggleElement(bool show, float duration)
        {
            _cg.blocksRaycasts = false;
            _state = show ? State.Showing : State.Hiding;

            if (useMoveEffect)
            {
                var endPos = show ? visiblePosition : hiddenPosition;
                StartCoroutine(_rt.Move(endPos, duration));
            }

            if (useScaleEffect)
            {
                var endScale = show ? visibleScale : Vector3.zero;
                StartCoroutine(_rt.Scale(endScale, duration));
            }

            if (useFadeEffect)
            {
                var endAlpha = show ? visibleAlpha : 0f;
                StartCoroutine(_cg.Fade(endAlpha, duration));
            }

            yield return FunctionLibrary.DoOverTime(duration, progress => _cachedTime = progress * duration);
            _cg.blocksRaycasts = true;
            SetFinalValues(show);
        }
    }
}
