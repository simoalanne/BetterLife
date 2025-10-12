using System;
using DialogueSystem;
using UnityEngine;

namespace Home
{
    public class PhoneHandler : MonoBehaviour, IInteractable
    {
        [SerializeField] private OpenPanelOnInteract phonePanelOpener;
        [SerializeField] private DialogueTrigger loanActiveDialogueTrigger;
        [SerializeField] private DialogueTrigger noteNotReadDialogueTrigger;

        public bool CanInteract { get; set; } = true;

        private void Awake()
        {
            // Make sure the components can't be directly interacted with
            phonePanelOpener.CanInteract = false;
            loanActiveDialogueTrigger.CanInteract = false;
            noteNotReadDialogueTrigger.CanInteract = false;
        }

        public void Interact()
        {
            // Phone menu is used to end the game. If the player has an active loan, or they haven't read the note yet
            // show corresponding dialogue instead
            if (!Services.PlayerManager.HasReadGoodbyeNote)
            {
                noteNotReadDialogueTrigger.TriggerDialogue();
                return;
            }

            if (Services.PlayerHUD.ActiveLoan is not null)
            {
                loanActiveDialogueTrigger.TriggerDialogue();
                return;
            }

            phonePanelOpener.Interact();
        }
    }
}
