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
        public bool CanMove { get; set; } = true;
        private Keyboard _keyboard;
        private Vector3 _playerScale = Vector3.zero;
        [SerializeField] private Sprite _idleSpriteFrontLeft;
        [SerializeField] private Sprite _idleSpriteBackLeft;
        [SerializeField] private Sprite _idleSpriteFrontRight;
        [SerializeField] private Sprite _idleSpriteBackRight;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _keyboard = Keyboard.current;
            _animator.enabled = false;
            _playerScale = transform.localScale;
        }

        void Update()
        {
            if (!CanMove)
            {
                _movement = Vector2.zero;
                _rigidbody2D.velocity = Vector2.zero;
                _animator.enabled = false;
                return;
            }

            if (_keyboard != null)
            {
                _movement.x = _keyboard.dKey.isPressed ? 1 : _keyboard.aKey.isPressed ? -1 : 0; // If D key is pressed, move right. If A key is pressed, move left. Otherwise, don't move
                _movement.y = _keyboard.wKey.isPressed ? 1 : _keyboard.sKey.isPressed ? -1 : 0; // If W key is pressed, move up. If S key is pressed, move down. Otherwise, don't move
            }

            if (_movement != Vector2.zero)
            {
                _animator.enabled = true;
                _lastMovement = _movement; // Store the last movement direction to determine which idle animation to play when the player stops moving

                if (_movement != Vector2.zero)
                {
                    _animator.enabled = true;
                    _lastMovement = _movement; // Store the last movement direction
                    _animator.Play(_movement.x > 0 ? _movement.y <= 0 ? "FrontRunRight" : "BackRunRight" : _movement.y <= 0 ? "FrontRunLeft" : "BackRunLeft");
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
    }
}