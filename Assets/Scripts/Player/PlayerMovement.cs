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

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _keyboard = Keyboard.current;
            _animator.enabled = false;
        }

        void Update()
        {
            if (!CanMove)
            {
                _movement = Vector2.zero;
                _rigidbody2D.velocity = Vector2.zero;
                return;
            }

            if (_keyboard != null)
            {
                _movement.x = _keyboard.dKey.isPressed ? 1 : _keyboard.aKey.isPressed ? -1 : 0; // If D key is pressed, move right. If A key is pressed, move left. Otherwise, don't move
                _movement.y = _keyboard.wKey.isPressed ? 1 : _keyboard.sKey.isPressed ? -1 : 0; // If W key is pressed, move up. If S key is pressed, move down. Otherwise, don't move
            }

            if (_movement != Vector2.zero)
            {
                _lastMovement = _movement; // Store the last movement direction to determine which idle animation to play when the player stops moving

                if (_movement.x != 0)
                {
                    //_animator.Play(_movement.x > 0 ? "WalkRight" : "WalkLeft");
                }
                else if (_movement.y != 0)
                {
                    //_animator.Play(_movement.y > 0 ? "WalkUp" : "WalkDown");
                }
            }
            else
            {
                // Play idle animation matching the direction the player was facing when they stopped moving
                if (_lastMovement.x != 0)
                {
                    //_animator.Play(_lastMovement.x > 0 ? "IdleRight" : "IdleLeft");
                }
                else if (_lastMovement.y != 0)
                {
                    //_animator.Play(_lastMovement.y > 0 ? "IdleUp" : "IdleDown");
                }
            }
        }

        void FixedUpdate()
        {
            // Use normalized to prevent diagonal movement from being faster than horizontal or vertical movement.
            _rigidbody2D.velocity = _movement.normalized * _movementSpeed;
        }
    }
}