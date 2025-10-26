using ScriptableObjects;
using UnityEngine;

namespace Home
{
    public class PhoneHandler : MonoBehaviour, IInteractable
    {
        [SerializeField] private HUDAttachablePanel endGameUI;
        [SerializeField] private Conversation loanActiveConversation;
        [SerializeField] private Conversation noteNotReadConversation;

        public void Interact()
        {
            // Phone menu is used to end the game. If the player has an active loan, or they haven't read the note yet
            // show corresponding dialogue instead
            if (!Services.PlayerManager.StoryProperties.HasReadGoodbyeNote)
            {
                noteNotReadConversation.Start();
                return;
            }

            if (Services.PlayerHUD.ActiveLoan is not null)
            {
                loanActiveConversation.Start();
                return;
            }

            endGameUI.AttachToHUD().Show();
        }
    }
}
