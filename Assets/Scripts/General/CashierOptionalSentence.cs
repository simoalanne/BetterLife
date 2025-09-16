using UnityEngine;

public class CashierOptionalSentence : MonoBehaviour, IOptionalSentence, IInteractable
{
    private bool _displayOptionalSentence;
    
    public bool CanInteract { get; set; } = true;

    public void Interact()
    {
        _displayOptionalSentence = true;
    }

    public bool DisplayOptionalSentence()
    {
        return _displayOptionalSentence;
    }
}
