using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "ScriptableObjects/InventoryItem", order = 1)]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [Range(1, 99)]
    public int maxAmount;
    public int amount;
}
