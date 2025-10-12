using Player;
using UnityEngine;

public class OpenPlayerInventory : MonoBehaviour
{
    private PlayerControls _playerControls;
    private bool _isInventoryOpen;
    private PlayerInventoryUI _inventoryUI;
    private bool _canOpenInventory = true;
    public bool CanOpenInventory { get => _canOpenInventory; set => _canOpenInventory = value; }

    void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Player.OpenInventory.performed += ctx => ToggleInventory();
        _inventoryUI = GetComponent<PlayerInventoryUI>();
    }

    void ToggleInventory()
    {
        if (_canOpenInventory == false) return;

        if (_isInventoryOpen == false)
        {
            _inventoryUI.OpenInventory();
        }
        else if (_isInventoryOpen == true)
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
