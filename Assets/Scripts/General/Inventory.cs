using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Serializable]
    public class StartingInventory
    {
        [Header("Details")]
        public InventoryItem item;
        public int amount;
    }
    [SerializeField] private int _totalItemSlots = 8; // The total amount of item slots in the inventory.
    private int _usedItemSlots = 0; // The amount of item slots currently in use.
    public List<StartingInventory> startingInventory;
    private readonly Dictionary<InventoryItem, int> _inventory = new();
    public Dictionary<InventoryItem, int> GetInventory => _inventory;

    public event Action OnInventoryChanged; // Can be subscribed to by other classes to update UI for example.
    public event Action OnInventoryLoaded;

    /// <summary>
    /// Adds n number of items to the inventory. If the item is unique and already in the inventory,
    /// or if the items max amount in the inventory has been reached, then the item won't be added.
    /// </summary>
    /// 

    void Start()
    {
        foreach (var entry in startingInventory)
        {
            Debug.Log("Adding item to inventory: " + entry.item.itemName + " with amount: " + entry.amount);
            AddToInventory(entry.item, entry.amount);
        }

        //OnInventoryLoaded?.Invoke(); 
    }

    public void AddToInventory(InventoryItem item, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogError("Amount must be greater than 0...");
            return;
        }

        if (amount > item.maxAmount)
        {
            Debug.LogError("Amount is greater than the max amount of the item.");
            return;
        }

        if (item.isUnique && _inventory.ContainsKey(item))
        {
            Debug.LogWarning("This Unique item is already in the inventory.");
            return;
        }

        if (item.isUnique && amount > 1)
        {
            Debug.LogWarning("This item is unique and can only be added once.");
            return;
        }

        if (_usedItemSlots >= _totalItemSlots)
        {
            Debug.Log("Inventory full.");
            return;
        }

        if (_inventory.ContainsKey(item)) // If the item is already in the inventory
        {
            if (_inventory[item] < item.maxAmount) // If the max amount of the item hasn't been reached.
            {
                _inventory[item] += amount; // Add to the stack
                Debug.Log("Added 1 to stack.");
            }
            else
            {
                Debug.Log("Inventory full.");
            }
        }
        else if (_inventory.ContainsKey(item) == false) // If the item isn't in the inventory
        {
            _inventory.Add(item, amount); // Add new item to inventory dictionary.
            _usedItemSlots++;
            Debug.Log("Added item to inventory, value." + amount);
        }
        OnInventoryChanged?.Invoke();
    }

    public void RemoveFromInventory(InventoryItem item, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogError("Amount must be greater than 0...");
            return;
        }

        if (amount > _inventory[item])
        {
            Debug.LogError("Amount is greater than the stack.");
            return;
        }

        if (_inventory.ContainsKey(item) == false) // If the item isn't in the inventory
        {
            Debug.LogError("Item not in inventory.");
            return;
        }

        if (_inventory[item] > amount)
        {
            _inventory[item] -= amount;
            Debug.Log($"Removed {amount} from stack.");
        }
        else
        {
            _usedItemSlots--;
            _inventory.Remove(item);
            Debug.Log("Removed item from inventory.");
        }
        OnInventoryChanged?.Invoke();
    }
}
