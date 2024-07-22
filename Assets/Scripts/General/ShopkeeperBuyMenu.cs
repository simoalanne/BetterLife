using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;
using UnityEngine.EventSystems;


/// <summary>
/// This class is used to handle the shopkeeper's buy menu.
/// Should be instantiated to players HUD when the player interacts with the shopkeeper
/// and should be destroyed when the player is done interacting with the shopkeeper.
/// Create a prefab gameobject with this script attached and set the itemButtonPrefab and itemGrid in the inspector.
/// </summary>
public class ShopkeeperBuyMenu : MonoBehaviour
{

    [Header("Shop variables")]
    [SerializeField] private Button _itemButtonPrefab;
    [SerializeField] private Button _exitButton;
    [SerializeField] private GridLayoutGroup _itemGrid;
    [SerializeField] private TMP_Text _noMoneyText;
    [SerializeField] private TMP_Text _playerMoneyText;

    [Header("Hover menu")]
    [SerializeField] private GameObject _hoverMenu;
    [SerializeField] private TMP_Text _hoverItemName;
    [SerializeField] private TMP_Text _hoverItemDescription;
    private RectTransform _hoverMenuRectTransform;
    private Coroutine _noMoneyTextCoroutine;
    private GameObject _shopCopy;

    [System.Serializable]
    public class Item
    {
        public InventoryItem item;
        public int itemPrice;
    }

    [SerializeField] List<Item> itemsForSale;

    private readonly Dictionary<Item, Button> itemButtonMap = new();

    void OnEnable() // This method is called when the object becomes enabled and active
    {
        PlayerManager.Instance.DisableInputs();
        gameObject.SetActive(true);

    }

    void OnDestroy() // This method is called when the object is being destroyed
    {
        PlayerManager.Instance.EnableInputs();
    }

    void Awake() // Runs when the objects is being instantiated
    {
        _exitButton.onClick.AddListener(ExitShop);
        _hoverMenu.SetActive(false);
        _hoverMenuRectTransform = _hoverMenu.GetComponent<RectTransform>();
        _noMoneyText.gameObject.SetActive(false);
        _playerMoneyText.text = $"{PlayerManager.Instance.MoneyInBankAccount}€";

        foreach (var item in itemsForSale)
        {
            Button itemButton = Instantiate(_itemButtonPrefab, _itemGrid.transform);
            itemButton.onClick.AddListener(() => BuyItem(item));
            itemButton.transform.Find("Icon").GetComponent<Image>().sprite = item.item.icon;
            itemButton.transform.Find("Price").GetComponent<TMP_Text>().text = $"{item.itemPrice}€";
            itemButton.transform.Find("Amount").GetComponent<TMP_Text>().text = item.item.isUnique ? "1" : "∞";
            itemButtonMap.Add(item, itemButton);
            AddHoverEvents(itemButton.gameObject, item);
        }
    }

    void ExitShop()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (_hoverMenu.activeSelf)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_hoverMenu.transform.parent, Input.mousePosition, null, out var position);
            _hoverMenu.transform.localPosition = position +
            new Vector2(_hoverMenuRectTransform.rect.width / 2, -_hoverMenuRectTransform.rect.height / 2) + new Vector2(32, -32);
        }
    }

    private void AddHoverEvents(GameObject itemButton, Item item)
    {
        var eventTrigger = itemButton.AddComponent<EventTrigger>();
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnHover(item)); // when the pointer enters the button, show the hover menu
        eventTrigger.triggers.Add(pointerEnter);
        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => _hoverMenu.SetActive(false)); // Hide the hover menu
        eventTrigger.triggers.Add(pointerExit);
    }

    void OnHover(Item item) // Accept Item parameter
    {
        _hoverMenu.SetActive(true);
        _hoverItemName.text = item.item.itemName;
        _hoverItemDescription.text = item.item.itemDescription;
    }

    void BuyItem(Item item)
    {
        if (PlayerManager.Instance.MoneyInBankAccount >= item.itemPrice)
        {
            PlayerManager.Instance.MoneyInBankAccount -= item.itemPrice;
            _playerMoneyText.text = $"${PlayerManager.Instance.MoneyInBankAccount} €";
            PlayerInventory.Instance.AddToInventory(item.item);
            if (item.item.isUnique)
            {
                itemsForSale.Remove(item);
                if (itemButtonMap.TryGetValue(item, out var buttonToDisable))
                {
                    buttonToDisable.interactable = false;
                    buttonToDisable.transform.Find("Price").GetComponent<TMP_Text>().text = "Sold out";
                    buttonToDisable.transform.Find("Amount").GetComponent<TMP_Text>().text = "<color=red>0</color>";
                }
            }
        }
        else
        {
            if (_noMoneyTextCoroutine != null)
            {
                StopCoroutine(_noMoneyTextCoroutine);
                _noMoneyText.gameObject.SetActive(false);
            }

            _noMoneyTextCoroutine = StartCoroutine(ShowNoMoneyText());
        }
    }

    IEnumerator ShowNoMoneyText()
    {
        _noMoneyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _noMoneyText.gameObject.SetActive(false);
    }
}