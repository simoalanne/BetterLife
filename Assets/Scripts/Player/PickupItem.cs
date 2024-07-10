using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item to pick up")]
    [SerializeField] private InventoryItem item;

    public void Interact()
    {
        Inventory.Instance.AddItem(item);
    }
}