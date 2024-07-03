using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Game world interact options")]
    [SerializeField] private Vector2 _interactMinDistance = new Vector2(0.5f, 0.5f);
    [SerializeField] private bool _isInteractable = true;

    public Vector2 InteractMinDistance { get; set; }
    public bool IsInteractable { get; set; }
    public Dialogue dialogue;

    void Awake()
    {
        InteractMinDistance = _interactMinDistance;
        IsInteractable = _isInteractable;
    }

    public void Interact()
    {
        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
