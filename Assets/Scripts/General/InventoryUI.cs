using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Player;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GridLayoutGroup _inventoryGrid;
    [SerializeField] private Button itemSlotPrefab;
    [SerializeField] private TMP_Text _itemDetails;
    [SerializeField] private GridLayoutGroup _loanGrid;
    [SerializeField] private Button _loanSlotPrefab;
    private readonly List<KeyValuePair<InventoryItem, GameObject>> _itemSlots = new();
    private Inventory _inventory;

    void Awake()
    {
        _inventoryPanel.SetActive(false);
        _inventory = GetComponent<Inventory>();
        _inventory.OnInventoryLoaded += PopulateInventory;
        _inventory.OnInventoryChanged += UpdateUI;
    }

    void PopulateInventory()
    {
        Debug.Log("PopulateInventory called");
        var inventory = _inventory.GetInventory;
        foreach (var item in inventory)
        {
            Debug.Log("Processing item: " + item.Key.itemName + " with amount: " + item.Value);
            if (item.Key.isStackable && _itemSlots.Exists(x => x.Key == item.Key))
            {
                IncreaseStack(item);
            }
            else
            {
                CreateItemSlot(new KeyValuePair<InventoryItem, int>(item.Key, item.Value));
            }
        }
    }

    void IncreaseStack(KeyValuePair<InventoryItem, int> item)
    {
        Debug.Log("Increasing stack for item: " + item.Key.itemName + " with amount: " + item.Value);
        var itemSlot = _itemSlots.Find(x => x.Key == item.Key);
        int currentAmount = int.Parse(itemSlot.Value.transform.Find("Amount").GetComponent<TMP_Text>().text);
        Debug.Log("Current amount: " + currentAmount);
        int newAmount = currentAmount + item.Value;
        itemSlot.Value.transform.Find("Amount").GetComponent<TMP_Text>().text = newAmount.ToString();
        Debug.Log("New amount: " + newAmount);
    }

    void CreateItemSlot(KeyValuePair<InventoryItem, int> item)
    {
        Debug.Log("Creating item slot for item: " + item.Key.itemName + " with amount: " + item.Value);
        var itemSlot = Instantiate(itemSlotPrefab, _inventoryGrid.transform);
        if (item.Key.isStackable)
        {
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = item.Value.ToString();
        }
        else
        {
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = "";
        }

        itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = item.Key.icon;
        _itemSlots.Add(new KeyValuePair<InventoryItem, GameObject>(item.Key, itemSlot.gameObject));
        itemSlot.onClick.AddListener(() => _inventory.RemoveFromInventory(item.Key));
        AddHoverEvents(itemSlot, item.Key);
    }

    void AddHoverEvents(Button itemSlot, InventoryItem item)
    {
        EventTrigger trigger = itemSlot.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new()
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((eventData) => { OnItemSlotHover(item); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new()
        {
            eventID = EventTriggerType.PointerExit
        };
        entryExit.callback.AddListener((eventData) => { OnItemSlotExit(); });
        trigger.triggers.Add(entryExit);
    }

    public void OnItemSlotHover(InventoryItem item)
    {
        _itemDetails.text = $"Name: {item.itemName}";
        Debug.Log("Hovering over item slot");
    }

    public void OnItemSlotExit()
    {
        _itemDetails.text = "";
        Debug.Log("Exiting item slot");
    }

    void UpdateUI()
    {
        Debug.Log("UpdateUI called");
        foreach (Transform child in _inventoryGrid.transform)
        {
            Destroy(child.gameObject);
        }
        _itemSlots.Clear();
        var inventory = _inventory.GetInventory;
        foreach (var item in inventory)
        {
            if (item.Value > 0)
            {
                Debug.Log("Item amount: " + item.Value + " " + item.Key.itemName);
            }
        }
        PopulateInventory();
    }

    void CheckItemType(InventoryItem item)
    {
        if (item is Loan)
        {
            Debug.Log("This is a loan");
        }
        else
        {
            Debug.Log("This is not a loan");
        }
    }

    public void OpenInventory()
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

    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
    }
}