using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "ScriptableObjects/Consumable", order = 1)]

public class Consumable : InventoryItem
{
    [Header("Consumable settings")]
    public float hungerRestored;
    public float energyRestored;
}
