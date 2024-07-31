using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;
using UnityEngine.EventSystems;
using UI.Extensions;

public class ShopkeeperBuyMenu : MonoBehaviour, IInteractable
{

    [Header("Shop variables")]
    [SerializeField] private GameObject _shopMenu;
    [SerializeField] private Button _itemButtonPrefab;
    [SerializeField] private GridLayoutGroup _itemGrid;
    [SerializeField] private TMP_Text _noMoneyText;
    [SerializeField] private TMP_Text _playerMoneyText;

    [Header("Dialogue")]
    [SerializeField] private DialogueTrigger _firstTimeDialogue;

    [Header("Hover menu")]
    [SerializeField] private GameObject _hoverMenu;
    [SerializeField] private TMP_Text _hoverItemName;
    [SerializeField] private TMP_Text _hoverItemDescription;
    private RectTransform _hoverMenuRectTransform;
    private Coroutine _noMoneyTextCoroutine;
    private GameObject _shopCopy;
    private Vector2 _menuOriginalPosition;


    [System.Serializable]
    public class Item
    {
        public InventoryItem item;
        public int itemPrice;
    }

    [SerializeField] List<Item> itemsForSale;

    private readonly Dictionary<Item, Button> itemButtonMap = new();

    void Awake() // Runs when the objects is being instantiated
    {
        _menuOriginalPosition = _shopMenu.GetComponent<RectTransform>().anchoredPosition;
        _hoverMenu.SetActive(false);
        _hoverMenuRectTransform = _hoverMenu.GetComponent<RectTransform>();
        _noMoneyText.gameObject.SetActive(false);
        //sort items by price
        itemsForSale.Sort((a, b) => a.itemPrice.CompareTo(b.itemPrice));
        foreach (var item in itemsForSale)
        {
            Button itemButton = Instantiate(_itemButtonPrefab, _itemGrid.transform);
            itemButton.onClick.AddListener(() => BuyItem(item));
            itemButton.transform.Find("Icon").GetComponent<Image>().sprite = item.item.icon;
            itemButton.transform.Find("Price").GetComponent<TMP_Text>().text = $"{item.itemPrice} €";
            itemButtonMap.Add(item, itemButton);
            AddHoverEvents(itemButton.gameObject, item);
        }
    }

    public void Interact()
    {
        _playerMoneyText.text = $"{PlayerManager.Instance.MoneyInBankAccount}€";

        if (PlayerManager.Instance.HasTalkedToShopkeeper == true)
        {
            OpenShopMenu();
        }
        else
        {
            DialogueManager.Instance.OnYesClicked += OpenShopMenu;
            _firstTimeDialogue.TriggerDialogue();
            PlayerManager.Instance.HasTalkedToShopkeeper = true;
        }
    }

    void OpenShopMenu()
    {
        PlayerManager.Instance.DisableInputs();
        GameTimer.Instance.IsPaused = true;
        StartCoroutine(UIAnimations.MoveObject(_shopMenu.GetComponent<RectTransform>(), new Vector2(100, _menuOriginalPosition.y)));
        FindObjectOfType<PlayerInventoryUI>().OpenInventory(); // Open the inventory to the side so the player can see what they have
    }

    public void CloseShopMenu()
    {
        PlayerManager.Instance.EnableInputs();
        GameTimer.Instance.IsPaused = false;
        StartCoroutine(UIAnimations.MoveObject(_shopMenu.GetComponent<RectTransform>(), _menuOriginalPosition));
        FindObjectOfType<PlayerInventoryUI>().CloseInventory();
    }

    void Update()
    {
        if (_hoverMenu.activeSelf)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_hoverMenu.transform.parent, Input.mousePosition, null, out var position);
            _hoverMenu.transform.localPosition = position +
            new Vector2(_hoverMenuRectTransform.rect.width / 2, -_hoverMenuRectTransform.rect.height / 2) + new Vector2(16, -16);
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
            Debug.Log("Buying + " + item.item.itemName);
            bool successfullyAdded = PlayerInventory.Instance.AddToInventory(item.item);

            if (!successfullyAdded) return; // if couldn't add to inventory, return and don't subtract money
            Debug.Log("Successfully bought " + item.item.itemName);
            PlayerManager.Instance.MoneyInBankAccount -= item.itemPrice;
            _playerMoneyText.text = $"{PlayerManager.Instance.MoneyInBankAccount}€";
            if (item.item is PowerUp)
            {
                itemsForSale.Remove(item);
                if (itemButtonMap.TryGetValue(item, out var buttonToDisable))
                {
                    buttonToDisable.interactable = false;
                    buttonToDisable.transform.Find("Price").GetComponent<TMP_Text>().text = "Sold out";
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