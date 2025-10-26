using UI;
using UnityEngine;
namespace Casino.Roulette
{
    public class RouletteGameSettings : MonoBehaviour
    {
        [SerializeField] private HideableElement settingsPanel;
        [SerializeField] private HideableElement previousNumbersPanel;
        [SerializeField] private HideableElement rulesPanel;
        [SerializeField] private ToggleButton showBetTypeSetting;
        [SerializeField] private ToggleButton showBetOddsSetting;
        [SerializeField] private DimBackground dimBackground;

        private void Awake()
        {
            Services.Register(this);
            showBetOddsSetting.SetInitialStatus(Preferences.RouletteShowBetOdds);
            showBetTypeSetting.SetInitialStatus(Preferences.RouletteShowBetType);
            showBetOddsSetting.OnToggle += isOn => Preferences.RouletteShowBetOdds = isOn;
            showBetTypeSetting.OnToggle += isOn => Preferences.RouletteShowBetType = isOn;
            dimBackground.OnBackgroundClicked += CloseAllPanels;
        }
        
        private void CloseAllPanels()
        {
            dimBackground.UnDim();
            settingsPanel.Toggle(false);
            previousNumbersPanel.Toggle(false);
            rulesPanel.Toggle(false);
        }

        public void OpenRules()
        {
            rulesPanel.Toggle(true);
            settingsPanel.Toggle(false);
        }

        public void CloseRules()
        {
            rulesPanel.Toggle(false);
            settingsPanel.Toggle(true);
        }

        public void OpenSettings()
        {
            dimBackground.Dim();
            settingsPanel.Toggle(true);
        }

        public void CloseSettings()
        {
            dimBackground.UnDim();
            settingsPanel.Toggle(false);
        }

        public void OpenPreviousNumbers()
        {
            previousNumbersPanel.Toggle(true);
            settingsPanel.Toggle(false);
        }

        public void ClosePreviousNumbers()
        {
            previousNumbersPanel.Toggle(false);
            settingsPanel.Toggle(true);
        }
    }
}
