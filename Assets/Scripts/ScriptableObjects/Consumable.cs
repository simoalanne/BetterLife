using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "ScriptableObjects/Consumable", order = 1)]

public class Consumable : InventoryItem
{
    [Header("Consumable settings")]
    [Range(-100, 100)]
    public float hungerRestored;
    [Range(-100, 100)]
    public float energyRestored;
}
