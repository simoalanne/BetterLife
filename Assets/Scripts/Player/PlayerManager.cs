using System.Linq;
using Helpers;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInteract))]
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private SceneLoadTrigger _loadToPlayerBed;
        private PlayerMovement _playerMovement;
        private PlayerInteract _playerInteract;
        private OpenPlayerInventory _openPlayerInventory;
        private SpriteRenderer _spriteRenderer;
        private DisplayMoney _displayMoney;
        private float _moneyBeforeGambling;
        private string[] _scenesToDisableHUD = { "MainMenu", "Roulette", "BlackJack", "Slots" };
        [SerializeField] private float _moneyInBankAccount = 100f;
        private float _originalMoney;
        private bool _hasTalkedToLoanShark; // Shouln't be in this script but it's what it is.
        private bool _hasTalkedToShopkeeper;
        private bool _hasPlayerPassedOut;
        private bool _hasReadGoodbyeNote;
        public bool HasTalkedToLoanShark
        {
            get => _hasTalkedToLoanShark;
            set => _hasTalkedToLoanShark = value;
        }
        public bool HasPlayerPassedOut
        {
            get => _hasPlayerPassedOut;
            set => _hasPlayerPassedOut = value;
        }
        public bool HasTalkedToShopkeeper
        {
            get => _hasTalkedToShopkeeper;
            set => _hasTalkedToShopkeeper = value;
        }
        public bool HasReadGoodbyeNote
        {
            get => _hasReadGoodbyeNote;
            set => _hasReadGoodbyeNote = value;
        }
        public float MoneyInBankAccount
        {
            get => _moneyInBankAccount;
            set
            {
                if (!_scenesToDisableHUD.Contains(SceneManager.GetActiveScene()
                        .name)) // If the scene has hud active, update the money text.
                {
                    _displayMoney.UpdateMoneyText(_moneyInBankAccount, value);
                }

                _moneyInBankAccount = value;
            }
        }

        void Awake()
        {
            Services.Register(this, dontDestroyOnLoad: true);
            _playerMovement = GetComponent<PlayerMovement>();
            _playerInteract = GetComponent<PlayerInteract>();
            _openPlayerInventory = FindObjectOfType<OpenPlayerInventory>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _displayMoney = FindObjectOfType<DisplayMoney>();
            _moneyBeforeGambling = _moneyInBankAccount;
            _originalMoney = _moneyInBankAccount;
        }

        private void Start()
        {
            _displayMoney.UpdateMoneyText(0, _moneyInBankAccount);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void ResetMoney() => _moneyInBankAccount = _originalMoney;

        public void DisablePlayerMovement() => _playerMovement.enabled = false;

        public void EnablePlayerMovement() => _playerMovement.enabled = true;

        public void DisableSpriteRenderer()
        {
            _spriteRenderer.enabled = false;
        }

        public void EnableSpriteRenderer()
        {
            _spriteRenderer.enabled = true;
        }

        public void DisableInventoryOpen()
        {
            _openPlayerInventory.CanOpenInventory = false;
        }

        public void EnableInventoryOpen()
        {
            _openPlayerInventory.CanOpenInventory = true;
        }

        public void DisablePlayer()
        {
            DisableInputs();
            DisableSpriteRenderer();
        }
        
        public void EnablePlayer()
        {
            EnableInputs();
            EnableSpriteRenderer();
        }

        public void DisableInputs()
        {
            DisablePlayerMovement();
            DisableInventoryOpen();
        }

        public void EnableInputs()
        {
            EnablePlayerMovement();
            EnableInventoryOpen();
        }

        public void Teleport(SpawnPoint spawnPoint)
        {
            transform.position = spawnPoint?.spawnPoint ?? transform.position;
            _playerMovement.SetFacingDirection(spawnPoint?.facingDirection.ToVector2() ?? Vector2.zero);
        }

        void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (current.name == "MainMenu")
            {
                _displayMoney.UpdateMoneyText(0, _moneyInBankAccount);
                return;
            }

            if (_scenesToDisableHUD.Contains(next.name))
            {
                _moneyBeforeGambling =
                    _moneyInBankAccount; // Save the money before the hud is disabled so the animation can be played when hud is re-enabled.
            }

            if (!_scenesToDisableHUD.Contains(next.name))
            {
                if (_scenesToDisableHUD.Contains(current.name))
                {
                    _displayMoney.UpdateMoneyText(_moneyBeforeGambling, _moneyInBankAccount);
                }
            }
        }

        public void LoadToPlayerBed()
        {
            if (SceneManager.GetActiveScene().name == "PlayerHome")
            {
                Debug.Log("Player is already in the player house.");
                FindObjectOfType<Sleep>().SleepInBed();
                return;
            }

            _loadToPlayerBed.Interact();
        }

        void OnEnable()
        {
            var spawnPoints = Resources.LoadAll<SpawnPoint>("SpawnPoints");
            var matchingSpawnPoint =
                spawnPoints.FirstOrDefault(sp => sp.sceneName == SceneManager.GetActiveScene().name);
            if (matchingSpawnPoint != null)
            {
                Debug.Log($"Found spawn point for scene {SceneManager.GetActiveScene().name}");
                transform.position = matchingSpawnPoint.spawnPoint;
            }
        }

        void OnDestroy()
        {
            Debug.Log("PlayerManager destroyed, unregistering from Services");
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }
}
