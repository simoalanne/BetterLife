using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Casino.BlackJack
{
    public abstract class BlackjackParticipant : MonoBehaviour
    {
        protected List<BlackjackHand> hands = new();
        
        private void Awake()
        {
            hands = GetComponentsInChildren<BlackjackHand>().ToList();
        }

        public int CurrentHandIndex { get; protected set; }

        public BlackjackHand CurrentHand => hands[CurrentHandIndex];
        public List<BlackjackCard> CurrentHandCards => CurrentHand.Cards;

        public virtual void ResetHands()
        {
            hands.ForEach(hand => hand.ClearHand());
            CurrentHandIndex = 0;
        }
    }
}
