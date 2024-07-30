using UnityEngine;

[RequireComponent(typeof(Inventory))]
/// <summary>
/// Makes calling the players inventory easier.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    private Inventory _inventory;
    public static PlayerInventory Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _inventory = GetComponent<Inventory>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddToInventory(InventoryItem item, int amount = 1)
    {
        _inventory.AddToInventory(item);
    }

    public void RemoveFromInventory(InventoryItem item, int amount = 1)
    {
        _inventory.RemoveFromInventory(item);
    }

}
