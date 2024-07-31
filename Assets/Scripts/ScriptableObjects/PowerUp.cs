using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp", menuName = "ScriptableObjects/PowerUp")]
public class PowerUp : InventoryItem
{
    public PowerUpsInInventory.PowerUpType powerUpType;
}
