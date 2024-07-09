using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;

    public void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        List<InventoryItem> items = Inventory.Instance.Items;
        if (items.Count == 0)
        {
            Debug.Log("No items in inventory");
            return;
        }
        foreach (InventoryItem item in items)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryPanel.transform);
            itemSlot.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
            itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
        }
    }
}