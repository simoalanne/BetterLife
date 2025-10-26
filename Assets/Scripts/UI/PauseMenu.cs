using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Serializable]
        private struct DisabledInScene
        {
            [Scene] public string sceneName;
        }

        [SerializeField] private List<DisabledInScene> disabledInScenes = new()
        {
            new DisabledInScene { sceneName = "MainMenu" },
            new DisabledInScene { sceneName = "EndGameCutscene" },
            new DisabledInScene { sceneName = "GameOverCutscene" }
        };

        [SerializeField] private HideableElement pauseMenu;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitToMainMenuButton;

        public bool IsPaused { get; private set; }
        private bool _pauseAllowed = true;
        
        // Ensures that input is only re-enabled if the pause menu initiated the disabling. This prevents conflicts with
        // other systems that may have disabled player input and not yet re-enabled it.
        private bool _initiatedDisableInput;

        private void Awake()
        {
            Services.Register(this, persistent: true);
            var currentScene = SceneManager.GetActiveScene().name;
            _pauseAllowed = !disabledInScenes.Exists(d => d.sceneName == currentScene);
        }

        private void Start()
        {
            Services.SceneLoader.OnSceneChanged += changed =>
            {
                if (disabledInScenes.Exists(d => d.sceneName == changed.NewScene))
                {
                    _pauseAllowed = false;
                    if (IsPaused)
                    {
                        Resume();
                    }
                    return;
                }

                _pauseAllowed = true;
            };

            pauseMenu.InitialVisibility(false);
            Services.InputManager.Controls.General.OpenPauseMenu.performed += _ =>
            {
                if (!_pauseAllowed) return;

                if (IsPaused)
                {
                    Resume();
                    return;
                }

                Pause();
            };

            resumeButton.onClick.AddListener(Resume);
            quitToMainMenuButton.onClick.AddListener(() =>
            {
                AudioListener.pause = false;
                pauseMenu.InitialVisibility(false);
                Time.timeScale = 1;
                Services.SceneLoader.LoadScene("MainMenu");
            });
        }

        private void Pause()
        {
            var inputManager = Services.InputManager;
            // if input was already disabled by another system, don't re-enable it on resume
            _initiatedDisableInput = inputManager.IsInputActive; 
            AudioListener.pause = true;
            Services.InputManager.EnablePlayerInput(false);
            pauseMenu.Toggle(true);
            Time.timeScale = 0;
            IsPaused = true;
        }

        private void Resume()
        {
            AudioListener.pause = false;
            if (_initiatedDisableInput)
            {
                Services.InputManager.EnablePlayerInput(true);
            }
            pauseMenu.Toggle(false);
            Time.timeScale = 1;
            IsPaused = false;
        }
    }
}
