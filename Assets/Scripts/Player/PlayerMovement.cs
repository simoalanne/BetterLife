using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 5f;

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
                _movement = Vector2.zero;
                _rigidbody2D.velocity = Vector2.zero;
                _animator.enabled = false;
                return;
            }

            if (_movement != Vector2.zero)
            {
                _animator.enabled = true;

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
                    _lastMovement = _movement; // Update last movement direction since there's horizontal movement
                }
            }
            else
            {
                _animator.enabled = false;
                GetComponent<SpriteRenderer>().sprite =
                    _lastMovement.x > 0 ? _lastMovement.y <= 0 ? _idleSpriteFrontRight : _idleSpriteBackRight : _lastMovement.y <= 0 ? _idleSpriteFrontLeft : _idleSpriteBackLeft;
            }
        }

        void FixedUpdate()
        {
            // Use normalized to prevent diagonal movement from being faster than horizontal or vertical movement.
            _rigidbody2D.velocity = _movement.normalized * _movementSpeed;
        }

        public void SetFacingDirection(Vector2 direction)
        {
            Debug.Log("Direction: " + direction);
            GetComponent<SpriteRenderer>().sprite =
                direction.x > 0 ? direction.y <= 0 ? _idleSpriteFrontRight : _idleSpriteBackRight : direction.y <= 0 ? _idleSpriteFrontLeft : _idleSpriteBackLeft;
        }
    }
}
