using UnityEngine;

public class OpenPanelOnInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _panelToOpen; // A prefab of the panel to open
    private Canvas _playerHUD;

    void Awake()
    {
        _playerHUD = GameObject.Find("PlayerHUD").GetComponent<Canvas>();
    }

    public void Interact()
    {
        Instantiate(_panelToOpen, _playerHUD.transform); // Instantiate the panel as a child of the player HUD. Position is the same as set in the prefab
    }
}
