using UnityEngine;

public class InventoryItem : ScriptableObject
{
    [Header("General item settings")]
    public string itemName;
    public Sprite icon;
    [Range(1, 99)]
    public int maxAmount;
    [Tooltip("Can the item be stacked in the inventory or\ndoes each item take up a separate slot?")]
    public bool isStackable = true;
    [Tooltip("Can the player have more than one of this item?")]
    public bool isUnique; 
}
