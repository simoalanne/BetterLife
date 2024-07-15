using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Button itemSlotPrefab;

    private Dictionary<string, int> _itemQuantities = new(); // Dictionary to hold the amount of each item in the inventory.
    private PlayerControls _playerControls;

    void Awake()
    {
        _inventoryPanel.SetActive(false);
        _playerControls = new PlayerControls();
        _playerControls.Player.OpenInventory.performed += context => OpenInventory();
    }

    void OpenInventory()
    {
        if (_inventoryPanel.activeSelf)
        {
            _inventoryPanel.SetActive(false);
            PlayerManager.Instance.EnablePlayerMovement();
            PlayerManager.Instance.EnablePlayerInteract();
        }
        else
        {
            _inventoryPanel.SetActive(true);
            PlayerManager.Instance.DisablePlayerMovement();
            PlayerManager.Instance.DisablePlayerInteract();
        }
    }


    public void AddToInventory(InventoryItem item)
    {
        if (item.isUnique && _itemQuantities.ContainsKey(item.itemName))
        {
            Debug.Log("You already have this item.");
            return;

        }

        if (item.isStackable)
        {
            if (_itemQuantities.ContainsKey(item.itemName))
            {
                if (_itemQuantities[item.itemName] < item.maxAmount)
                {
                    _itemQuantities[item.itemName]++;
                    _inventoryPanel.transform.Find(item.itemName).Find("Amount").GetComponent<TMP_Text>().text = _itemQuantities[item.itemName].ToString();
                    Debug.Log("Added 1 to stack.");
                    return;
                }
                else
                {
                    Debug.Log("You have reached the maximum amount of this item.");
                    return;
                }
            }
            else
            {
                Debug.Log("Added to dictionary.");
                _itemQuantities.Add(item.itemName, 1);
            }
        }

        Debug.Log("Added a new item to the inventory.");
        Button itemSlot = Instantiate(itemSlotPrefab, _inventoryPanel.transform);
        itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
        itemSlot.transform.Find("Name").GetComponent<TMP_Text>().text = item.itemName;
        if (item.isStackable)
        {
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = "1";
        }
        else
        {
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = "";
        }

        itemSlot.gameObject.name = item.itemName;
        itemSlot.onClick.AddListener(() => UseItem(item));
    }

    void UseItem(InventoryItem item)
    {
        if (item.isStackable && _itemQuantities[item.itemName] > 1)
        {
            _itemQuantities[item.itemName]--;
            _inventoryPanel.transform.Find(item.itemName).Find("Amount").GetComponent<TMP_Text>().text = _itemQuantities[item.itemName].ToString();
            Debug.Log("Used 1 from stack.");
        }
        else
        {
            _itemQuantities.Remove(item.itemName);
            Destroy(_inventoryPanel.transform.Find(item.itemName).gameObject);
            Debug.Log("Removed item from inventory.");
            return;
        }
    }

    void OnEnable()
    {
        _playerControls.Player.Enable();
    }

    void OnDisable()
    {
        _playerControls.Player.Disable();
    }

    void OnDestroy()
    {
        _playerControls.Player.OpenInventory.performed -= context => OpenInventory();
    }
}