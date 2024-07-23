using UnityEngine;

[CreateAssetMenu(fileName = "New General Inventory Item", menuName = "ScriptableObjects/General Inventory Item", order = 1)]
public class InventoryItem : ScriptableObject
{
    [Header("General item settings")]
    public string itemName;
    [TextArea(3, 10)]
    public string itemDescription = "";
    public Sprite icon;
    [Range(1, 99)]
    public int maxAmount = 99;
    [Tooltip("Can the item be stacked in the inventory or\ndoes each item take up a separate slot?")]
    public bool isStackable = true;
    [Tooltip("Can the player have more than one of this item?")]
    public bool isUnique;
}
