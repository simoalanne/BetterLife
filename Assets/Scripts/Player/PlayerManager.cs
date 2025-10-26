using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInteract))]
    public class PlayerManager : MonoBehaviour
    {
        [Serializable]
        private class FreePlaySceneRule
        {
            [Scene] public string fromScene;
            [Scene] public string toScene;
        }

        [Serializable]
        private enum VisibilityOption
        {
            HidePlayer,
            HidePlayerAndHUD,
        }

        [Serializable]
        private class PlayerAndHUDVisibilityRule
        {
            [Scene] public string sceneName;
            public VisibilityOption visibilityOption;
        }

        [Header("Settings")]
        [SerializeField, Scene, Tooltip("Loading this scene will reset the game back to initial state")]
        private string resetGameScene = "MainMenu";

        [SerializeField, Tooltip("All scenes where player and/or HUD should be disabled")]
        private List<PlayerAndHUDVisibilityRule> scenesToDisablePlayerOrHud = new()
        {
            new PlayerAndHUDVisibilityRule
            {
                sceneName = "MainMenu",
                visibilityOption = VisibilityOption.HidePlayerAndHUD
            }
        };

        [SerializeField, Tooltip("All from-to scene combinations that should put the game in free play mode")]
        private List<FreePlaySceneRule> freePlaySceneRules = new()
        {
            new FreePlaySceneRule
            {
                fromScene = "MainMenu",
                toScene = "Roulette"
            },

            new FreePlaySceneRule
            {
                fromScene = "MainMenu",
                toScene = "Blackjack"
            },

            new FreePlaySceneRule
            {
                fromScene = "MainMenu",
                toScene = "Slots"
            }
        };

        public bool IsInFreePlayMode { get; private set; }
        public string PreviousSceneName { get; private set; }

        private StoryProperties _storyProperties;
        public StoryProperties StoryProperties
        {
            get
            {
                _storyProperties ??= new StoryProperties();
                return _storyProperties;
            }
        }
        private PlayerMovement _playerMovement;
        private SpriteRenderer _spriteRenderer;
        private DisplayMoney _displayMoney;

        [SerializeField] private float moneyInBankAccount = 100f;
        private float _originalMoney;
        public float MoneyInBankAccount
        {
            get => moneyInBankAccount;
            set
            {
                _displayMoney.UpdateMoneyText(moneyInBankAccount, value);
                moneyInBankAccount = value;
            }
        }

        private void Awake()
        {
            Services.Register(this, persistent: true);
            _playerMovement = GetComponent<PlayerMovement>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _displayMoney = FindFirstObjectByType<DisplayMoney>();
            _originalMoney = moneyInBankAccount;
            SceneManager.activeSceneChanged += (old, _) => PreviousSceneName = old.name;
            
            var currentScene = SceneManager.GetActiveScene();

            var freePlayRule = freePlaySceneRules
                .FirstOrDefault(rule => rule.toScene == currentScene.name);

            if (freePlayRule is not null)
            {
                IsInFreePlayMode = true;
                EnablePlayer(enable: false, showHUD: false);
                return;
            }

            IsInFreePlayMode = false;

            var visibilityRule = scenesToDisablePlayerOrHud
                .FirstOrDefault(rule => rule.sceneName == currentScene.name);

            if (visibilityRule is not null)
            {
                (visibilityRule.visibilityOption switch
                {
                    VisibilityOption.HidePlayer => () => EnablePlayer(enable: false, showHUD: true),
                    VisibilityOption.HidePlayerAndHUD => () => EnablePlayer(enable: false, showHUD: false),
                    _ => new Action(() => { })
                }).Invoke();
            }

            var allSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
            if (allSpawnPoints.Length is 0) return;
            var targetSpawnPoint = allSpawnPoints
                                       .FirstOrDefault(spawnPoint =>
                                           spawnPoint.UseWhenPreviousSceneIs == currentScene.name) ??
                                   allSpawnPoints.First();
            Teleport(targetSpawnPoint);
        }

        private void Start()
        {
            _displayMoney.UpdateMoneyText(0, moneyInBankAccount);
            Services.SceneLoader.OnSceneChanged += HandleSceneChanged;
            
        }

        public void ResetMoney() => MoneyInBankAccount = _originalMoney;

        private void EnablePlayer(bool enable, bool showHUD)
        {
            _spriteRenderer.enabled = enable;
            Services.PlayerHUD.ShowHud(showHUD);
            if (enable)
            {
                Services.InputManager.EnablePlayerInput(true);
                return;
            }

            Services.InputManager.EnablePlayerInput(false);
        }


        private void Teleport(SpawnPoint spawnPoint)
        {
            transform.position = spawnPoint.transform.position;
            _playerMovement.SetFacingDirection(spawnPoint.FacingDirection);
        }

        private void HandleSceneChanged(SceneChanged sceneChanged)
        {
            var oldSceneName = sceneChanged.OldScene;
            var newSceneName = sceneChanged.NewScene;
            if (newSceneName == resetGameScene)
            {
                // Reset game state and disable player
                _spriteRenderer.color = Color.white; // see NPC/ShopKeeper.cs for why this is needed
                StoryProperties.ResetProperties();
                var gameTimer = Services.GameTimer;
                gameTimer.Reset();
                gameTimer.IsPaused = true;
                ResetMoney();
                EnablePlayer(false, false);
                Services.PlayerHUD.ResetHUD();
                return;
            }

            var freePlayRule = freePlaySceneRules
                .FirstOrDefault(rule => rule.fromScene == oldSceneName && rule.toScene == newSceneName);

            if (freePlayRule is not null)
            {
                IsInFreePlayMode = true;
                EnablePlayer(enable: false, showHUD: false);
                return;
            }

            Services.GameTimer.IsPaused = false;
            IsInFreePlayMode = false;

            var visibilityRule = scenesToDisablePlayerOrHud
                .FirstOrDefault(rule => rule.sceneName == newSceneName);

            if (visibilityRule is not null)
            {
                (visibilityRule.visibilityOption switch
                {
                    VisibilityOption.HidePlayer => () => EnablePlayer(enable: false, showHUD: true),
                    VisibilityOption.HidePlayerAndHUD => () => EnablePlayer(enable: false, showHUD: false),
                    _ => new Action(() => { })
                }).Invoke();
            }
            else
            {
                EnablePlayer(enable: true, showHUD: true);
            }

            var allSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
            if (allSpawnPoints.Length is 0) return;
            var targetSpawnPoint = allSpawnPoints.Length is 1
                ? allSpawnPoints.First()
                : allSpawnPoints.FirstOrDefault(spawnPoint => spawnPoint.UseWhenPreviousSceneIs == oldSceneName);
            Teleport(targetSpawnPoint);
        }
    }
}
