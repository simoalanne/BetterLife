using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item to pick up")]
    [SerializeField] private InventoryItem item;

    [Header("Game world interact options")]
    [SerializeField] private Vector2 _interactMinDistance = new(0.5f, 0.5f);
    [SerializeField] private bool _isInteractable = true;

    public Vector2 InteractMinDistance { get; set; }
    public bool IsInteractable { get; set; }

    private void Awake()
    {
        InteractMinDistance = _interactMinDistance;
        IsInteractable = _isInteractable;
    }

    public void Interact()
    {
        Inventory.Instance.AddItem(item);
    }
}