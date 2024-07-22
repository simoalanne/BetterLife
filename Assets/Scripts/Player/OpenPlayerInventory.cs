using UnityEngine;
using Player;

public class OpenPlayerInventory : MonoBehaviour
{
    private PlayerControls _playerControls;
    private bool _isInventoryOpen;
    private InventoryUI _inventoryUI;
    private bool _canOpenInventory = true;
    public bool CanOpenInventory { get => _canOpenInventory; set => _canOpenInventory = value; }

    void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Player.OpenInventory.performed += ctx => ToggleInventory();
        _inventoryUI = GetComponent<InventoryUI>();
    }

    void ToggleInventory()
    {
        if (!_canOpenInventory) return;

        if (!_isInventoryOpen)
        {
            _inventoryUI.OpenInventory();
        }
        else
        {
            _inventoryUI.CloseInventory();
        }
        _isInventoryOpen = !_isInventoryOpen;
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
        _playerControls.Player.OpenInventory.performed -= ctx => ToggleInventory();
    }
}
