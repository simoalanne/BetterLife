using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Player;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GridLayoutGroup _standardItemsGrid;
    [SerializeField] private TMP_Text _itemDetails;
    private Button[] _standardItemSlots;
    private PlayerInventory _inventory;

    void Start()
    {
        _inventoryPanel.SetActive(false);
        _inventory = GetComponent<PlayerInventory>();
        _inventory.OnInventoryChanged += PopulateInventory;
        _standardItemSlots = _standardItemsGrid.GetComponentsInChildren<Button>();
    }

    void PopulateInventory()
    {
        var inventory = _inventory.GetInventory();
        int index = 0;
        _itemDetails.text = "";
        foreach (var itemSlot in _standardItemSlots)
        {
            itemSlot.onClick.RemoveAllListeners(); // Remove all listeners so that button doesnt try to use the same item multiple times
            if (itemSlot.TryGetComponent<EventTrigger>(out var eventTrigger))
            {
                Debug.Log("Removing event trigger");
                Destroy(eventTrigger);
            }
        }

        foreach (var item in inventory)
        {
            if (index >= _standardItemSlots.Length)
            {
                Debug.LogWarning("there should be 8 item slots in the inventory as set in PlayerInventory.cs");
                break;
            }

            var itemSlot = _standardItemSlots[index];
            itemSlot.onClick.AddListener(() => UseItem(item.Key));
            var icon = itemSlot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = item.Key.icon;
            icon.color = new Color(1, 1, 1, 1); // Set color to white
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = item.Value.ToString();
            AddHoverEvents(itemSlot, item.Key);
            index++;
        }
        // Clear remaining slots if any
        for (int i = index; i < _standardItemSlots.Length; i++)
        {
            var itemSlot = _standardItemSlots[i];
            var icon = itemSlot.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = null;
            icon.color = new Color(0, 0, 0, 0); // Set color to transparent
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = "";
        }
    }

    void UseItem(InventoryItem item)
    {
        // this is food item so also increase hunger bar
        if (item is Consumable)
        {
            Debug.Log("Using food item");
            var consumable = item as Consumable;
            Needs.Instance.IncreaseHungerBar(consumable.hungerRestored); // Increase hunger bar

        }
        _inventory.RemoveFromInventory(item);
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
    }

    public void OnItemSlotExit()
    {
        _itemDetails.text = "";
    }

    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        PlayerManager.Instance.DisablePlayerMovement();
        PlayerManager.Instance.DisablePlayerInteract();
        GameTimer.Instance.IsPaused = true;
    }

    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        GameTimer.Instance.IsPaused = false;
    }
}