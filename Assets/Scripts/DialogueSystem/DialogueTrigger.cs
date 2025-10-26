using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    /// <summary>
    /// Triggers a dialogue when interacted with. If the dialogue needs to trigger based on some condition,
    /// Then the coordinator script should hold reference to <see cref="Conversation"/>(s) and trigger them manually.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DialogueTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private Conversation conversation;
        [SerializeField] private UnityEvent<DialogueState> onStateChanged;

        private void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
            gameObject.tag = "NPC";
        }

        private void Awake() => RemoveTrailingAndLeadingSpaces();


        /// <summary>
        /// Checks for accidental leading and trailing spaces in the dialogue sentences and removes them.
        /// </summary>
        private void RemoveTrailingAndLeadingSpaces()
        {
            if (conversation == null) return;
            conversation.dialogueParts.ForEach(part => part.sentence = part.sentence.Trim());
        }
        
        /// <summary> Only <see cref="PlayerInteract"/> Should ever call this method. </summary>
        public void Interact() => conversation.Start(state => onStateChanged?.Invoke(state));
    }
}
