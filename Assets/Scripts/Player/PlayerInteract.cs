using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles player interaction with interactable objects using raycasting.
/// Also handles switching cursor to appropriate icon when hovering over interactable objects.
/// Uses object's only collider or the trigger collider if there are multiple colliders on the object.
/// This makes it easier to have a specific area for interaction like a door instead of the whole building.
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    [System.Serializable]
    public struct CursorType
    {
        public Texture2D cursorTexture;
        public Texture2D cursorTextureHalfTransparent;
    }

    [Header("Interaction settings")]
    [SerializeField, Tooltip("How far the player can interact with objects."), Range(0, 1)] private float _interactionRange = 0.25f;

    [Header("Cursor types")]
    [SerializeField] private CursorType _generalInteractCursor;
    [SerializeField] private CursorType _NPCTalkCursor;
    [SerializeField] private CursorType _magnifyingGlassCursor;
    private int _interactableLayer;
    private Vector2 _mousePosition;
    public bool CanInteract { get; set; } = true;
    private Camera _mainCamera;
    private readonly CursorMode cursorMode = CursorMode.Auto;
    private Collider2D _playerCollider;

    void Start()
    {
        _interactableLayer = LayerMask.GetMask("Interactable");
        _playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!CanInteract)
        {
            SetDefaultCursor();
            return;
        }

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(_mousePosition, Vector2.zero, Mathf.Infinity, _interactableLayer);

        Collider2D targetCollider = null;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                var colliders = hit.collider.GetComponents<Collider2D>();
                if (colliders.Length == 1 || hit.collider.isTrigger)
                {
                    targetCollider = hit.collider;
                    break;
                }
            }
        }

        if (targetCollider != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Cursor.SetCursor(null, Vector2.zero, cursorMode);
                return;
            }

            if (BoundsIntersect2D(targetCollider.bounds, _mousePosition)) // If the mouse is within the bounds of the target collider
            {
                var cursorType = targetCollider.tag switch
                {
                    "NPC" => _NPCTalkCursor,
                    "Inspectable" => _magnifyingGlassCursor,
                    _ => _generalInteractCursor
                };

                SetCustomCursor(cursorType, IsPlayerWithinInteractionRange(targetCollider));
            }

            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && targetCollider.TryGetComponent(out IInteractable interactable))
            {
                if (IsPlayerWithinInteractionRange(targetCollider))
                {
                    interactable.Interact();
                    return;
                }
            }
        }
        else
        {
            SetDefaultCursor();
        }
    }

    void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    void SetCustomCursor(CursorType cursorType, bool isWithinInteractionRange)
    {
        Cursor.SetCursor(isWithinInteractionRange ? cursorType.cursorTexture : cursorType.cursorTextureHalfTransparent, Vector2.zero, cursorMode);
    }

    /// <summary>
    /// Checks if mouse position is within the bounds of the collider.
    /// </summary>
    /// <returns>True if collider's bounds contain the point, false otherwise.</returns>
    bool BoundsIntersect2D(Bounds bounds, Vector2 mousePosition)
    {
        return bounds.min.x <= mousePosition.x && bounds.max.x >= mousePosition.x &&
               bounds.min.y <= mousePosition.y && bounds.max.y >= mousePosition.y;
    }

    /// <summary>
    /// Checks if the player is within the interaction range of the target collider.
    /// </summary>
    /// <param name="targetCollider">The collider of the object the cursor is hovering over.</param>
    /// <returns>True if the player is in collider's bounds or within the interaction range, false otherwise.</returns>
    bool IsPlayerWithinInteractionRange(Collider2D targetCollider)
    {
        if (BoundsIntersect2D(targetCollider.bounds, _playerCollider.transform.position))
        {
            return true;
        }
        float distanceToEdge = Vector2.Distance(_playerCollider.ClosestPoint(targetCollider.bounds.ClosestPoint(_playerCollider.transform.position)),
        targetCollider.bounds.ClosestPoint(_playerCollider.transform.position));
        return distanceToEdge <= _interactionRange;
    }
}
