using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerInventory : MonoBehaviour
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

    public event Action OnInventoryChanged; // Can be subscribed to by other classes to update UI for example.
    public event Action OnInventoryLoaded;

    public static PlayerInventory Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var entry in startingInventory)
        {
            Debug.Log("Adding item to inventory: " + entry.item.itemName + " with amount: " + entry.amount);
            AddToInventory(entry.item, entry.amount);
        }
        OnInventoryChanged?.Invoke();
    }

    public bool AddToInventory(InventoryItem item, int amount = 1) // Returns true if item was added to inventory, false if not.
    {
        if (amount <= 0)
        {
            Debug.LogError("Amount must be greater than 0...");
            return false;
        }

        if (amount > item.maxAmount)
        {
            Debug.LogError("Amount is greater than the max amount of the item.");
            return false;
        }

        if (_usedItemSlots >= _totalItemSlots && _inventory.ContainsKey(item) == false) 
        {
            Debug.Log("Inventory full.");
            return false;
        }

        if (_inventory.ContainsKey(item)) // If the item is already in the inventory
        {
            if (_inventory[item] < item.maxAmount) // If the max amount of the item hasn't been reached.
            {
                _inventory[item] += amount; // Add to the stack
                Debug.Log($"Added {amount} {item.itemName} to inventory stack.\n stack size is now: {_inventory[item]}");
            }
            else
            {
                Debug.Log($"max amount of {item.itemName} reached.\nmax amount: {item.maxAmount}");
                return false;
            }
        }
        else if (_inventory.ContainsKey(item) == false) // If the item isn't in the inventory
        {
            _inventory.Add(item, amount); // Add new item to inventory dictionary.
            _usedItemSlots++;
            Debug.Log($"Added {amount} {item.itemName} to inventory.");
        }
        OnInventoryChanged?.Invoke();
        return true;
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
            Debug.Log($"Removed {amount} {item.itemName} from inventory stack.");
        }
        else
        {
            _usedItemSlots--;
            _inventory.Remove(item);
            Debug.Log($"Removed {item.itemName} from inventory altogether.");
        }
        OnInventoryChanged?.Invoke();
    }

    public List<PowerUp> GetPowerUps()
    {
        var powerUps = new List<PowerUp>();
        foreach (var item in _inventory)
        {
            if (item.Key is PowerUp powerUp) // if type is PowerUp
            {
                powerUps.Add(powerUp);
            }
        }
        return powerUps;
    }

    public Dictionary<InventoryItem, int> GetInventory(bool sortByName = false)
    {
        return sortByName ? _inventory.OrderBy(x => x.Key.itemName).ToDictionary(x => x.Key, x => x.Value) : _inventory;
    }
}
