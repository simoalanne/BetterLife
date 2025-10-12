using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private enum MovementState
        {
            Stopped,
            Performed
        }

        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private string movementXParam = "MovementX";
        [SerializeField] private string movementYParam = "MovementY";
        [SerializeField] private string isMovingParam = "IsMoving";

        private Animator _animator;
        private Rigidbody2D _rb2D;
        private Vector2 _direction;

        [SerializeField] private UnityEvent onMovementStarted;
        [SerializeField] private UnityEvent onMovementPerformed;
        [SerializeField] private UnityEvent onMovementStopped;

        private MovementState _state = MovementState.Performed;
        private InputAction _moveAction;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rb2D = GetComponent<Rigidbody2D>();
            SetAnimParams(Vector2.zero, false);
        }


        private void FixedUpdate()
        {
            _moveAction ??= Services.InputManager.Controls.Player.Move;
            var input = _moveAction.ReadValue<Vector2>();
            if (input == Vector2.zero && _state is not MovementState.Stopped)
            {
                HandleStopped();
                return;
            }

            if (input == Vector2.zero) return;

            if (_state is MovementState.Stopped)
            {
                onMovementStarted?.Invoke();
                _state = MovementState.Performed;
            }

            onMovementPerformed?.Invoke();
            MoveCharacter(input);
        }

        public void SetFacingDirection(Vector2 facingDirection)
        {
            MoveCharacter(facingDirection, forceStooped: true);
        }

        private void MoveCharacter(Vector2 input, bool forceStooped = false)
        {
            var x = Mathf.RoundToInt(input.x);
            var y = Mathf.RoundToInt(input.y);

            // Because non-diagonal animations are missing purely horizontal or vertical movement is converted to diagonal
            _direction = (x, y) switch
            {
                // So character keeps same horizontal direction when going up or down
                (0, 1) => new Vector2(_direction.x != 0 ? _direction.x : 1, 1),
                (0, -1) => new Vector2(_direction.x != 0 ? _direction.x : 1, -1),
                // Use down diagonals for purely horizontal movement
                (1, 0) => new Vector2(1, -1),
                (-1, 0) => new Vector2(-1, -1),
                (1, 1) => new Vector2(1, 1),
                (-1, 1) => new Vector2(-1, 1),
                (1, -1) => new Vector2(1, -1),
                (-1, -1) => new Vector2(-1, -1),
                _ => _direction
            };

            SetAnimParams(_direction, input != Vector2.zero && !forceStooped);

            if (forceStooped) return;
            _rb2D.MovePosition(
                _rb2D.position + input.normalized * (movementSpeed * Time.fixedDeltaTime)
            );
        }


        private void SetAnimParams(Vector2 direction, bool isMoving = true)
        {
            _animator.SetFloat(movementXParam, direction.x);
            _animator.SetFloat(movementYParam, direction.y);
            _animator.SetBool(isMovingParam, isMoving);
        }

        private void HandleStopped()
        {
            _state = MovementState.Stopped;
            onMovementStopped?.Invoke();
            MoveCharacter(Vector2.zero);
        }
    }
}
