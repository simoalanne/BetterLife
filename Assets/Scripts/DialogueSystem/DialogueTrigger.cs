using System;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private Dialogue[] dialogue;
        [Tooltip("Leave empty if don't want to change the default sprite")]
        [SerializeField] private Sprite _playerSprite;
        [SerializeField] private Sprite _NPCSprite;

        public event Action onDialogueTrigger;

        [field: SerializeField] public bool CanInteract { get; set; } = true;

        void Awake()
        {
            RemoveTrailingAndLeadingSpaces();
        }

        /// <summary>
        /// Checks for accidental leading and trailing spaces in the dialogue sentences and removes them.
        /// </summary>
        private void RemoveTrailingAndLeadingSpaces()
        {
            for (int i = 0; i < dialogue.Length; i++)
            {
                dialogue[i].sentence = dialogue[i].sentence.Trim();
                if (dialogue[i].isQuestion)
                {
                    foreach (Branch branch in dialogue[i].yesBranch)
                    {
                        branch.sentence = branch.sentence.Trim();
                    }

                    foreach (Branch branch in dialogue[i].noBranch)
                    {
                        branch.sentence = branch.sentence.Trim();
                    }
                }
            }
        }

        public void Interact()
        {
            if (!CanInteract) return;
            TriggerDialogue();
        }

        public void TriggerDialogue(Action onDialogueFinished = null)
        {
            onDialogueTrigger?.Invoke();
            Services.DialogueHandler.StartDialogue(dialogue, _playerSprite, _NPCSprite, onDialogueFinished);
        }
    }
}
