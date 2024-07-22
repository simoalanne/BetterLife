using UnityEngine;
using Player;
public class OpenPanelOnInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _panelToOpen; // A prefab of the panel to open
    private Canvas _playerHUD;

    void Start()
    {
        _playerHUD = PlayerHUD.Instance.GetComponent<Canvas>();
    }

    public void Interact()
    {
        PlayerManager.Instance.DisablePlayerMovement();
        PlayerManager.Instance.DisablePlayerInteract();
        GameTimer.Instance.IsPaused = true;
        Instantiate(_panelToOpen, _playerHUD.transform); // Instantiate the panel as a child of the player HUD. Position is the same as set in the prefab
    }
}
