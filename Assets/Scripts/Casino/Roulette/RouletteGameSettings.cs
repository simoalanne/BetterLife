using System.Collections;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class RouletteGameSettings : MonoBehaviour
    {
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Image _dimBackground;
        [SerializeField] private RectTransform _settingsPanel;
        [SerializeField] private Button _showBetTypeSetting;
        [SerializeField] private Button _showBetOddsSetting;
        [SerializeField] private RectTransform _previousNumbersPanel;
        [SerializeField] private float _panelMoveDuration = 0.1f;
        [SerializeField] private RectTransform _rulesPanel;

        private RectTransform _settingsCanvas;

        private bool _isBetTypeShown;
        private bool _isBetOddsShown;

        private bool _isSettingsOpen;
        private bool _isRulesOpen;
        private bool _isPreviousNumbersOpen;

        private void Awake()
        {
            Services.Register(this);
            LoadSettings();
            _settingsCanvas = GetComponent<RectTransform>();
            _showBetTypeSetting.GetComponent<ToggleButton>().SetInitialStatus(_isBetTypeShown);
            _showBetOddsSetting.GetComponent<ToggleButton>().SetInitialStatus(_isBetOddsShown);
            _settingsPanel.anchoredPosition = new Vector2(_settingsPanel.sizeDelta.x, 0); // Settings panel is off screen on right side
            _dimBackground.gameObject.SetActive(false);
        }

        public void OnBackgroundClick()
        {
            if (_isSettingsOpen)
            {
                _isSettingsOpen = false;
                StartCoroutine(MovePanel(_settingsPanel, new Vector2(_settingsPanel.sizeDelta.x, 0)));
            }

            if (_isRulesOpen)
            {
                _isRulesOpen = false;
                StartCoroutine(MovePanel(_rulesPanel, new Vector2(0, _rulesPanel.sizeDelta.y)));
            }

            if (_isPreviousNumbersOpen)
            {
                _isPreviousNumbersOpen = false;
                StartCoroutine(MovePanel(_previousNumbersPanel, new Vector2(0, _previousNumbersPanel.sizeDelta.y)));
            }

            _dimBackground.gameObject.SetActive(false);
        }

        public void OpenRules()
        {
            _isRulesOpen = true;
            _isSettingsOpen = false;
            StartCoroutine(MovePanel(_settingsPanel, new Vector2(_settingsPanel.sizeDelta.x, 0))); // Move the settings panel off screen to the right
            StartCoroutine(MovePanel(_rulesPanel, new Vector2(0, -(_settingsCanvas.sizeDelta.y - _rulesPanel.sizeDelta.y) / 2))); // Move the panel to the center of the screen
        }

        public void CloseRules()
        {
            _isRulesOpen = false;
            _isSettingsOpen = true;
            StartCoroutine(MovePanel(_settingsPanel, new Vector2(0, 0))); // Move the settings panel back to the center of the screen
            StartCoroutine(MovePanel(_rulesPanel, new Vector2(0, _rulesPanel.sizeDelta.y)));
        }

        public void OpenSettings()
        {
            _isSettingsOpen = true;
            StartCoroutine(MovePanel(_settingsPanel, new Vector2(0, 0)));
            _dimBackground.gameObject.SetActive(true);
        }

        public void CloseSettings()
        {
            _isSettingsOpen = false;
            StartCoroutine(MovePanel(_settingsPanel, new Vector2(_settingsPanel.sizeDelta.x, 0)));
            _dimBackground.gameObject.SetActive(false);
        }

        public void OpenPreviousNumbers()
        {
            _isPreviousNumbersOpen = true;
            _isSettingsOpen = false;
            StartCoroutine(MovePanel(_settingsPanel, new Vector2(_settingsPanel.sizeDelta.x, 0))); // Move the settings panel off screen to the right
            StartCoroutine(MovePanel(_previousNumbersPanel, new Vector2(0, -(_settingsCanvas.sizeDelta.y - _previousNumbersPanel.sizeDelta.y) / 2)));
        }

        public void ClosePreviousNumbers()
        {
            _isPreviousNumbersOpen = false;
            _isSettingsOpen = true;
            StartCoroutine(MovePanel(_settingsPanel, new Vector2(0, 0))); // Move the settings panel back to the center of the screen
            StartCoroutine(MovePanel(_previousNumbersPanel, new Vector2(0, _previousNumbersPanel.sizeDelta.y))); // Move the panel off screen to the top
        }

        IEnumerator MovePanel(RectTransform panel, Vector2 endPosition)
        {
            float elapsedTime = 0;
            Vector2 startPosition = panel.anchoredPosition;

            while (elapsedTime < _panelMoveDuration)
            {
                panel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / _panelMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            panel.anchoredPosition = endPosition;
        }

        public void ToggleBetType()
        {
            _isBetTypeShown = !_isBetTypeShown;
            PlayerPrefs.SetInt("IsBetTypeShown", _isBetTypeShown ? 1 : 0);
        }

        public void ToggleBetOdds()
        {
            _isBetOddsShown = !_isBetOddsShown;
            PlayerPrefs.SetInt("IsBetOddsShown", _isBetOddsShown ? 1 : 0);
        }

        private void LoadSettings()
        {
            _isBetTypeShown = PlayerPrefs.GetInt("IsBetTypeShown", 1) == 1;
            _isBetOddsShown = PlayerPrefs.GetInt("IsBetOddsShown", 1) == 1;
        }
        
        public (bool showBetType, bool showBetOdds) GetDisplaySettings() => (_isBetTypeShown, _isBetOddsShown);
    }
}