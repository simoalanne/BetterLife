using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private HideableElement pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitToMainMenuButton;
        [SerializeField] private Button quitGameButton;

        private bool _isPaused;

        private void Start()
        {
            pauseMenu.InitialVisibility(false);
            Services.InputManager.Controls.Player.OpenPauseMenu.performed += _ =>
            {
                if (_isPaused)
                {
                    Resume();
                    return;
                }

                Pause();
            };

            resumeButton.onClick.AddListener(Resume);
            quitToMainMenuButton.onClick.AddListener(() =>
            {
                pauseMenu.InitialVisibility(false);
                Services.SceneLoader.LoadMainMenu();
            });
            quitGameButton.onClick.AddListener(Application.Quit);
        }

        private void Pause()
        {
            pauseMenu.Toggle(true);
            Time.timeScale = 0;
            _isPaused = true;
        }

        private void Resume()
        {
            pauseMenu.Toggle(false);
            Time.timeScale = 1;
            _isPaused = false;
        }
    }
}
