using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class RouletteGameSettings : MonoBehaviour
    {
        [SerializeField] private DisplayBetInfo _displayBetInfo;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Image _dimBackground;
        [SerializeField] private RectTransform _settingsPanel;
        [SerializeField] private Button _showBetTypeSetting;
        [SerializeField] private Button _showBetOddsSetting;
        [SerializeField] private RectTransform _previousNumbersPanel;
        [SerializeField] private float _settingsPanelMoveDuration = 0.1f;

        private bool _isBetTypeShown;
        private bool _isBetOddsShown;

        void Awake()
        {
            LoadSettings();

            _showBetTypeSetting.GetComponent<ToggleButton>().SetInitialStatus(_isBetTypeShown);
            _showBetOddsSetting.GetComponent<ToggleButton>().SetInitialStatus(_isBetOddsShown);
            _settingsPanel.anchoredPosition = new Vector2(_settingsPanel.sizeDelta.x, 0); // Settings panel is off screen on right side
            _dimBackground.gameObject.SetActive(false);
            _displayBetInfo.ShowBetType = _isBetTypeShown;
            _displayBetInfo.ShowBetOdds = _isBetOddsShown;
        }

        public void OpenSettings()
        {
            StartCoroutine(MoveSettingsPanel(new Vector2(0, 0)));
        }

        public void CloseSettings()
        {
            StartCoroutine(MoveSettingsPanel(new Vector2(_settingsPanel.sizeDelta.x, 0)));
            _dimBackground.gameObject.SetActive(false);
        }

        public void OpenPreviousNumbers()
        {
            StartCoroutine(MoveSettingsPanel(new Vector2(_settingsPanel.sizeDelta.x, 0)));
            StartCoroutine(MovePreviousNumbersPanel(new Vector2(0, 0)));
        }

        public void ClosePreviousNumbers()
        {
            StartCoroutine(MovePreviousNumbersPanel(new Vector2(0, _previousNumbersPanel.sizeDelta.y))); // Move the panel off screen to the top
            StartCoroutine(MoveSettingsPanel(new Vector2(0, 0)));
        }

        IEnumerator MovePreviousNumbersPanel(Vector2 endPosition)
        {
            float elapsedTime = 0;
            Vector2 startPosition = _previousNumbersPanel.anchoredPosition;

            while (elapsedTime < _settingsPanelMoveDuration)
            {
                _previousNumbersPanel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / _settingsPanelMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _previousNumbersPanel.anchoredPosition = endPosition;
        }

        IEnumerator MoveSettingsPanel(Vector2 endPosition)
        {
            float elapsedTime = 0;
            Vector2 startPosition = _settingsPanel.anchoredPosition;

            while (elapsedTime < _settingsPanelMoveDuration)
            {
                _settingsPanel.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / _settingsPanelMoveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _settingsPanel.anchoredPosition = endPosition;
        }

        public void ToggleBetType()
        {
            _isBetTypeShown = !_isBetTypeShown;
            _displayBetInfo.ShowBetType = _isBetTypeShown;
            PlayerPrefs.SetInt("IsBetTypeShown", _isBetTypeShown ? 1 : 0);
        }

        public void ToggleBetOdds()
        {
            _isBetOddsShown = !_isBetOddsShown;
            _displayBetInfo.ShowBetOdds = _isBetOddsShown;
            PlayerPrefs.SetInt("IsBetOddsShown", _isBetOddsShown ? 1 : 0);
        }

        void LoadSettings()
        {
            _isBetTypeShown = PlayerPrefs.GetInt("IsBetTypeShown", 1) == 1;
            _isBetOddsShown = PlayerPrefs.GetInt("IsBetOddsShown", 1) == 1;
        }
    }
}
