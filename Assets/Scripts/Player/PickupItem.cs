using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item to pick up")]
    [SerializeField] private InventoryItem item;
    
    public bool CanInteract { get; set; } = true;
    
    public void Interact()
    {
        PlayerInventory.Instance.AddToInventory(item);
    }
}