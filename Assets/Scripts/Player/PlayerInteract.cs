using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Texture2D _pointingHandTexture; // Changed from Texture to Texture2D
    [SerializeField] private GameObject _infoObject; // Display info text when hovering over interactable object
    private Vector2 _mousePosition;
    public bool CanInteract { get; set; } = true;
    private Camera _mainCamera;
    private readonly CursorMode cursorMode = CursorMode.Auto;
    private readonly Vector2 hotSpot = new(16.5f, 4.5f); // Changed from Vector2 to Vector2(16.5f, 16.5f
    private bool _isHovering = false;
    private Collider2D _playerCollider;


    void Start()
    {
        _playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!CanInteract) // Prevent interaction if the player is not allowed to interact
        {
            if (_isHovering)
            {
                Cursor.SetCursor(null, Vector2.zero, cursorMode); // Reset cursor to default when not hovering over interactable
                _infoObject.SetActive(false); // Hide info text when not hovering over interactable object
                _isHovering = false;
            }

            return;
        }

        if (_mainCamera == null)  // Optimize performance by only finding the main camera when a scene is loaded
        {
            _mainCamera = Camera.main;
        }

        _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(_mousePosition, Vector2.zero, Mathf.Infinity, _interactableLayer); // Raycast in the interactable layer.
        if (hit.collider != null) // If hit something
        {
            if (!_isHovering)
            {
                _isHovering = true;
                Cursor.SetCursor(_pointingHandTexture, hotSpot, cursorMode); // Change cursor to custom texture
                _infoObject.GetComponentInChildren<TMP_Text>().text = hit.collider.gameObject.name; // Set info text to the name of the interactable object
                Vector2 screenPoint = _mainCamera.WorldToScreenPoint(hit.collider.transform.position); // Convert world position to screen position
                _infoObject.GetComponent<RectTransform>().position = screenPoint; // Set info text position to the interactable object position
                _infoObject.SetActive(true); // Display info text when hovering over interactable object
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable)) // If collider implemented the IInteractable interface
                {
                    Collider2D[] objectColliders = hit.collider.GetComponents<Collider2D>(); // Get all colliders on the object

                    foreach (Collider2D collider in objectColliders)
                    {
                        if (collider.isTrigger)
                        {
                            if (BoundsIntersect2D(collider.bounds, _playerCollider.bounds)) // Check if the player is within the trigger bounds
                            {
                                interactable.Interact();
                                return;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode); // Reset cursor to default when not hovering over interactable
            _infoObject.SetActive(false); // Hide info text when not hovering over interactable object
            _isHovering = false;
        }
    }

    bool BoundsIntersect2D(Bounds a, Bounds b)
    {
        return a.min.x < b.max.x && a.max.x > b.min.x &&
               a.min.y < b.max.y && a.max.y > b.min.y;
    }
}
