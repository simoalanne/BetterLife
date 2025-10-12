using UnityEngine;
public class OpenPanelOnInteract : MonoBehaviour, IInteractable

{
    [SerializeField] private GameObject _panelToOpen; // A prefab of the panel to open
    
    public bool CanInteract { get; set; } = true;
    
    public void Interact()
    {
        Services.PlayerManager.DisableInputs();
        Services.GameTimer.IsPaused = true;
        Instantiate(_panelToOpen, Services.PlayerHUD.transform); // Instantiate the panel as a child of the player HUD. Position is the same as set in the prefab
    }
}
