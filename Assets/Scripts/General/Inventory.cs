using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    private readonly List<InventoryItem> items = new();
    public List<InventoryItem> Items => items;

    public static Inventory Instance { get; private set; }

    private void Awake()
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

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
        GetComponent<InventoryUI>().AddToInventory(item);
    }
}
