using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Button itemSlotPrefab;
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
        var inventory = _inventory.GetInventory;
        foreach (var item in inventory)
        {
            CheckItemType(item.Key);
            var itemSlot = Instantiate(itemSlotPrefab, _inventoryPanel.transform);
            itemSlot.transform.Find("Name").GetComponent<TMP_Text>().text = item.Key.itemName;
            itemSlot.transform.Find("Amount").GetComponent<TMP_Text>().text = item.Value.ToString();
            itemSlot.transform.Find("Icon").GetComponent<Image>().sprite = item.Key.icon;
            itemSlot.onClick.AddListener(() => _inventory.RemoveFromInventory(item.Key));
        }
    }


    void UpdateUI()
    {
        foreach (Transform child in _inventoryPanel.transform)
        {
            Destroy(child.gameObject);
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
