using System;
using UnityEngine;

namespace Home
{
    public class PhoneHandler : MonoBehaviour
    {
        private OpenPanelOnInteract _phonePanelOpener;
        [SerializeField]
        private DialogueTrigger loanActiveDialogueTrigger;
        [SerializeField]
        private DialogueTrigger noteNotReadDialogueTrigger;

        private bool _noteRead;
        
        [SerializeField]
        private DialogueTrigger noteReadDialogueTrigger;

        private void Start()
        {
            _phonePanelOpener = GetComponent<OpenPanelOnInteract>();
            
            if (!_phonePanelOpener && !loanActiveDialogueTrigger)
            {
                Debug.LogError("PhoneHandler requires both OpenPanelOnInteract and DialogueTrigger components.");
            }
            // why is playerHUD having this info is a good question
            bool isPhoneUsable = PlayerHUD.Instance.ActiveLoan;
            if (isPhoneUsable)
            {
                _phonePanelOpener.CanInteract = false;
                noteNotReadDialogueTrigger.CanInteract = false;
                loanActiveDialogueTrigger.CanInteract = true;
            }
            else
            {
                noteNotReadDialogueTrigger.CanInteract = false;
                _phonePanelOpener.CanInteract = true;
                loanActiveDialogueTrigger.CanInteract = false;
            }
            _noteRead = Player.PlayerManager.Instance.HasReadGoodbyeNote;
            if (_noteRead) return;
            loanActiveDialogueTrigger.CanInteract = false;
            _phonePanelOpener.CanInteract = false;
            noteNotReadDialogueTrigger.CanInteract = true;
            
            Action onGoodbyeNoteReadAction = null;
            onGoodbyeNoteReadAction = () =>
                {
                    _noteRead = true;
                    noteReadDialogueTrigger.CanInteract = true;
                    noteNotReadDialogueTrigger.CanInteract = false;
                    _phonePanelOpener.CanInteract = true;
                    loanActiveDialogueTrigger.CanInteract = false;
                    noteNotReadDialogueTrigger.onDialogueTrigger -= onGoodbyeNoteReadAction;
                };
        }
    }
}
