using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private HideableElement mainMenu;
        [SerializeField] private HideableElement settingsMenu;
        [SerializeField] private HideableElement creditsMenu;
        [SerializeField] private HideableElement selectGameModeMenu;

        private void Awake()
        {
            mainMenu.InitialVisibility(true);
            settingsMenu.InitialVisibility(false);
            creditsMenu.InitialVisibility(false);
            selectGameModeMenu.InitialVisibility(false);
        }
        
        public void ShowMainMenu()
        {
            settingsMenu.Toggle(false);
            creditsMenu.Toggle(false);
            selectGameModeMenu.Toggle(false);
        }
        
        // These panels will have higher layer order than the main menu, so they naturally appear on top of it.
        public void ShowSettingsMenu() => settingsMenu.Toggle(true);
        public void ShowCreditsMenu() => creditsMenu.Toggle(true);
        public void ShowSelectGameModeMenu() => selectGameModeMenu.Toggle(true);
    }
}
