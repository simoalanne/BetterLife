using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableLayer;
    private Vector2 _mousePosition;
    public bool CanInteract { get; set; } = true;
    private Camera _mainCamera;

    void Update()
    {
        if (!CanInteract) // Prevent interaction if the player is not allowed to interact
        {
            return;
        }

        if (_mainCamera == null)  // Optimize performance by only finding the main camera when a scene is loaded
        {
            _mainCamera = Camera.main;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(_mousePosition, Vector2.zero, Mathf.Infinity, _interactableLayer); // Raycast in the interactable layer.
            if (hit.collider != null) // If hit something
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable)) // If collider implemented the IInteractable interface
                {
                    float distance = Vector2.Distance(transform.position, hit.collider.transform.position); // Distance between player and interactable object center
                    float colliderRadius = hit.collider.bounds.extents.magnitude; // Radius of the interactable object collider
                    float actualDistance = distance - colliderRadius; // Distance between player and interactable object surface

                    if (actualDistance < interactable.InteractMinDistance.magnitude && interactable.IsInteractable) // If player is close enough and object is in interactable state
                    {
                        interactable.Interact(); // Call the interact method on the object that implements from the IInteractable interface
                    }
                    else
                    {
                        Debug.Log("Too far away or object is not in interactable state");
                    }
                }
                else
                {
                    Debug.LogWarning("Object is in interactable layer but doesn't implement IInteractable interface!");
                    Debug.LogWarning("Implement the IInteractable interface or remove the object from the interactable layer.");
                }
            }
        }
    }
}
