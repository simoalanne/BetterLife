using System;
using System.Collections;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private float animDuration = 0.075f;
        [SerializeField] private Color onColor = new(0f, 0.5f, 0f);
        [SerializeField] private Color offColor = new(100f/255f, 100f/255f, 100f/255f);
        [SerializeField] private RectTransform knob;
        [SerializeField] private Vector2 onPosition, offPosition;
        private Button _toggleButton;
        private Image _knobImage;
        private bool _isOn;

        public Action<bool> OnToggle;

        private void Awake()
        {
            _knobImage = knob.GetComponent<Image>();
            _toggleButton = GetComponent<Button>();
            _toggleButton.onClick.AddListener(() => StartCoroutine(MoveButton()));
        }

        public void SetInitialStatus(bool isOn)
        {
            _isOn = !isOn;
            StartCoroutine(MoveButton());
        }

        private IEnumerator MoveButton()
        {
            _isOn = !_isOn;
            _toggleButton.interactable = false;
            yield return knob.Move(_isOn ? onPosition : offPosition, animDuration);
            _toggleButton.interactable = true;
            OnToggle?.Invoke(_isOn);
            _knobImage.color = _isOn ? onColor : offColor;
        }
    }
}
