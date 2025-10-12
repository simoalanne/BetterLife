using DialogueSystem;
using UnityEngine;

namespace City
{
    /// <summary>
    /// Mailbox gives player money once a day when interacted with.
    /// </summary>
    public class MailBox : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueTrigger beforeMoneyCollected;
        [SerializeField] private DialogueTrigger noMoneyAvailable;
        [SerializeField] private float moneyAmount = 50f;

        public bool CanInteract { get; set; } = true;
        private static int _lastMoneyCollectedDay = -1; // TODO: don't use static some more scalable solution instead


        public void Interact()
        {
            var currentDay = Services.GameTimer.Days;

            if (currentDay != _lastMoneyCollectedDay)
            {
                beforeMoneyCollected?.TriggerDialogue(() =>
                {
                    Services.PlayerManager.MoneyInBankAccount += moneyAmount;
                    _lastMoneyCollectedDay = currentDay;
                });
            }
            else
            {
                noMoneyAvailable?.TriggerDialogue();
            }
        }
    }
}
