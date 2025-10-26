using ScriptableObjects;
using UnityEngine;

namespace Home
{
    public class GoodbyeNote : MonoBehaviour
    {
        [SerializeField] private ConversationWithTokens afterRead = new("moneyCurrently");
        
        /// <summary> this should be called from inspector through UnityEvent after the note is read </summary>
        public void AfterGoodbyeNoteRead()
        {
            var dialogue = afterRead.InjectTokenValues(Services.PlayerManager.MoneyInBankAccount);
            Services.PlayerManager.StoryProperties.HasReadGoodbyeNote = true;
            dialogue.Start();
        }
    }
}
