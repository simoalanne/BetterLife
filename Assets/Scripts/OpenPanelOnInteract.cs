using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Opens a HUD panel when interacted with. Invokes an event when the panel is hidden.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OpenPanelOnInteractOrTriggerEnter : MonoBehaviour, IInteractable
{
    [Serializable]
    public enum ActivationType
    {
        Interact,
        TriggerEnter
    }

    private void OnValidate() =>
        gameObject.layer = LayerMask.NameToLayer(activationType == ActivationType.Interact
            ? "Interactable"
            : "Default"
        );


    [SerializeField] private ActivationType activationType = ActivationType.Interact;
    [SerializeField] private UnityEvent onPanelHidden;

    [SerializeField] private HUDAttachablePanel panelPrefab;
    public void Interact() => panelPrefab.AttachToHUD(onPanelHidden.Invoke).Show();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activationType != ActivationType.TriggerEnter) return;
        panelPrefab.AttachToHUD(onPanelHidden.Invoke).Show();
    }
}
