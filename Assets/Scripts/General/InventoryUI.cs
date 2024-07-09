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
        // Clear existing item slots
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
            bool itemExists = false;

            // Check if item already exists in the inventory panel
            foreach (Transform child in inventoryPanel.transform)
            {
                TextMeshProUGUI itemNameText = child.Find("Name").GetComponent<TextMeshProUGUI>();
                if (itemNameText.text == item.itemName)
                {
                    // Item exists, update the amount
                    TMP_Text amountText = child.Find("Amount").GetComponent<TMP_Text>();
                    amountText.text = item.amount.ToString();
                    itemExists = true;
                    break;
                }
            }

            if (!itemExists)
            {
                // Item does not exist, create a new item slot
                GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryPanel.transform);
                itemSlot.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = item.itemName;
                itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
            }
        }
    }
}