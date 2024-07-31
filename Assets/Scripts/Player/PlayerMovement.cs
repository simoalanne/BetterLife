using Audio;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 5f;
        private bool _isPlayingStepSound = false;
        private SoundEffectPlayer _soundEffectPlayer;
        private Animator _animator;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _movement;
        private Vector2 _lastMovement;
        private bool _canMove = true;
        public bool CanMove { get => _canMove; set => _canMove = value; }

        [SerializeField] private Sprite _idleSpriteFrontLeft;
        [SerializeField] private Sprite _idleSpriteBackLeft;
        [SerializeField] private Sprite _idleSpriteFrontRight;
        [SerializeField] private Sprite _idleSpriteBackRight;

        private PlayerControls _playerControls;

        void OnEnable()
        {
            _playerControls.Player.Enable();
        }

        void OnDisable()
        {
            _playerControls.Player.Disable();
        }

        void Awake()
        {
            _soundEffectPlayer = GetComponent<SoundEffectPlayer>();
            _playerControls = new PlayerControls();
            _playerControls.Player.Move.performed += OnMovePerformed;
            _playerControls.Player.Move.canceled += OnMoveCanceled;

            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator.enabled = false;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _movement = Vector2.zero;
        }

        void Update()
        {
            if (!_canMove)
            {
                _isPlayingStepSound = false;
                _soundEffectPlayer.StopSoundEffect();
                _movement = Vector2.zero;
                _rigidbody2D.velocity = Vector2.zero;
                _animator.enabled = false;
                return;
            }

            if (_movement != Vector2.zero)
            {
                _animator.enabled = true;
                if (!_isPlayingStepSound)
                {
                    _isPlayingStepSound = true;
                    _soundEffectPlayer.PlaySoundEffect(0);
                }

                // Determine the animation based on movement direction, using last movement direction for purely vertical movement
                if (_movement.x == 0 && _movement.y != 0) // Moving purely vertically
                {
                    // Use _lastMovement.x to determine the horizontal direction for the animation
                    _animator.Play(_lastMovement.x > 0 ? _movement.y <= 0 ? "FrontRunRight" : "BackRunRight" : _movement.y <= 0 ? "FrontRunLeft" : "BackRunLeft");
                }
                else
                {
                    // Normal movement handling
                    _animator.Play(_movement.x > 0 ? _movement.y <= 0 ? "FrontRunRight" : "BackRunRight" : _movement.y <= 0 ? "FrontRunLeft" : "BackRunLeft");
                }

                // Update last movement direction
                _lastMovement = _movement;
            }
            else
            {
                _animator.enabled = false;
                if (_isPlayingStepSound)
                {
                    _isPlayingStepSound = false;
                    _soundEffectPlayer.StopSoundEffect();
                    SetFacingDirection(_lastMovement);
                }
            }
        }

        void FixedUpdate()
        {
            // Use normalized to prevent diagonal movement from being faster than horizontal or vertical movement.
            var speedMultiplier = PowerUpsInInventory.HasPowerUp(PowerUpsInInventory.PowerUpType.MovementSpeedBoost) ? 1.5f : 1f;
            if (speedMultiplier > 1f)
            {
                GetComponent<AnimationSpeedController>().SetAnimationSpeed(1.5f);
            }
            _rigidbody2D.velocity = _movement.normalized * _movementSpeed * speedMultiplier;
        }

        private void SetFacingDirection(Vector2 direction)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (direction.x > 0)
            {
                spriteRenderer.sprite = direction.y <= 0 ? _idleSpriteFrontRight : _idleSpriteBackRight;
            }
            else
            {
                spriteRenderer.sprite = direction.y <= 0 ? _idleSpriteFrontLeft : _idleSpriteBackLeft;
            }
        }

        public void ChangeIdleSprite(SpawnPoint.FacingDirection facingDirection)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            switch (facingDirection)
            {
                case SpawnPoint.FacingDirection.BackLeft:
                    spriteRenderer.sprite = _idleSpriteBackLeft;
                    break;
                case SpawnPoint.FacingDirection.BackRight:
                    spriteRenderer.sprite = _idleSpriteBackRight;
                    break;
                case SpawnPoint.FacingDirection.Left:
                    spriteRenderer.sprite = _idleSpriteFrontLeft;
                    break;
                case SpawnPoint.FacingDirection.Right:
                    spriteRenderer.sprite = _idleSpriteFrontRight;
                    break;
            }
        }
    }
}
