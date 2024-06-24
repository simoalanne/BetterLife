using UnityEngine;

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
        
        [SerializeField] private float _moneyInBankAccount = 1000f;
        public float MoneyInBankAccount { get; set; }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _playerMovement = GetComponent<PlayerMovement>();
            _playerInteract = GetComponent<PlayerInteract>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            MoneyInBankAccount = _moneyInBankAccount;
        }

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

        public void SetPlayerPosition(Vector2 position)
        {
            transform.position = position;
        }
    }
}
