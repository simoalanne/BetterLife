using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInteract))]
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        private PlayerMovement _playerMovement;
        private PlayerInteract _playerInteract;
        private SpriteRenderer _spriteRenderer;
        private DisplayMoney _displayMoney;
        private float _moneyBeforeGambling;
        private string[] _scenesToDisableHUD = { "MainMenu", "Roulette", "BlackJack", "Slots" };
        [SerializeField] private float _moneyInBankAccount = 1000f;
        private float _originalMoney;
        public float MoneyInBankAccount
        {
            get => _moneyInBankAccount;
            set
            {
                if (!_scenesToDisableHUD.Contains(SceneManager.GetActiveScene().name)) // If the scene has hud active, update the money text.
                {
                    _displayMoney.UpdateMoneyText(_moneyInBankAccount, value);
                }

                _moneyInBankAccount = value;
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _playerMovement = GetComponent<PlayerMovement>();
                _playerInteract = GetComponent<PlayerInteract>();
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _displayMoney = FindObjectOfType<DisplayMoney>();
                _moneyBeforeGambling = _moneyInBankAccount;
                _originalMoney = _moneyInBankAccount;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            _displayMoney.UpdateMoneyText(0, _moneyInBankAccount);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void ResetMoney() => _moneyInBankAccount = _originalMoney;

        public void DisablePlayerMovement()
        {
            _playerMovement.CanMove = false;
        }

        public void EnablePlayerMovement()
        {
            _playerMovement.CanMove = true;
        }

        public void DisablePlayerInteract()
        {
            _playerInteract.CanInteract = false;
        }

        public void EnablePlayerInteract()
        {
            _playerInteract.CanInteract = true;
        }

        public void DisableSpriteRenderer()
        {
            _spriteRenderer.enabled = false;
        }

        public void EnableSpriteRenderer()
        {
            _spriteRenderer.enabled = true;
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
                _moneyBeforeGambling = _moneyInBankAccount; // Save the money before the hud is disabled so the animation can be played when hud is re-enabled.
            }
            if (!_scenesToDisableHUD.Contains(next.name))
            {
                if (_scenesToDisableHUD.Contains(current.name))
                {
                    _displayMoney.UpdateMoneyText(_moneyBeforeGambling, _moneyInBankAccount);
                }
            }
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }
}
