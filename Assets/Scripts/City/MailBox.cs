using System.Collections.Generic;
using DialogueSystem;
using ScriptableObjects;
using UnityEngine;

namespace City
{
    /// <summary>
    /// Mailbox gives player money or an invoice randomly upon interaction.
    /// </summary>
    public class Mailbox : MonoBehaviour, IInteractable
    {
        [SerializeField] private Conversation somethingInsideMailbox;
        [SerializeField] private ConversationWithTokens itWasMoney = new(nameof(moneyAmount));
        [SerializeField] private ConversationWithTokens itWasAnInvoice = new(nameof(moneyAmount));
        [SerializeField] private float moneyAmount = 50f;

        public void Interact()
        {
            var sign = Random.Range(0, 2) == 0 ? 1 : -1;
            var decidedToGamble = false;
            somethingInsideMailbox.Start(
                conversationChain: new List<ConversationChainItem>
                {
                    new(() => decidedToGamble,
                        sign == 1
                            ? itWasMoney.InjectTokenValues(moneyAmount)
                            : itWasAnInvoice.InjectTokenValues(moneyAmount))
                },
                onStateChange: state =>
                {
                    if (state is DialogueState.YesClicked)
                    {
                        decidedToGamble = true;
                        return;
                    }
                    
                    Debug.Log("sate changed + " + state);
                    if (state is DialogueState.DialogueFinished && decidedToGamble)
                        Services.PlayerManager.MoneyInBankAccount += moneyAmount * sign;
                }
            );
        }
    }
}
