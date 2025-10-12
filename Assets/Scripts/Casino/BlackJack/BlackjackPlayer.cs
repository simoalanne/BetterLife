using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Casino.BlackJack
{
    public class BlackjackPlayer : BlackjackParticipant
    {
        public bool CurrentHandStood { get; private set; }

        public bool HasBustedAllHands => hands.Where(hand => hand.Cards.Count > 0).All(hand => hand.GetHandValue > 21);

        public void StandCurrentHand() => CurrentHandStood = true;

        public IEnumerator SplitCurrentHand()
        {
            var secondCard = hands[CurrentHandIndex].TakeSecondCard();
            yield return hands[CurrentHandIndex + 1].AddCardToHand(secondCard);
        }

        public bool HasMoreHands => CurrentHandIndex + 1 < hands.Where(hand => hand.Cards.Count > 0).ToList().Count;

        public bool CanSplit => CurrentHandCards.Count == 2 &&
                                CurrentHandCards[0].Data.UniqueRank == CurrentHandCards[1].Data.UniqueRank;

        public bool AllHandsBlackjack => hands.Where(hand => hand.Cards.Count > 0)
            .All(hand => hand.GetHandValue == 21 && hand.Cards.Count == 2);


        public void MoveToNextHand()
        {
            if (!HasMoreHands) return;
            CurrentHandStood = false;
            CurrentHandIndex++;
        }

        public void MoveToPreviousHand()
        {
            if (CurrentHandIndex - 1 < 0) return;
            CurrentHandStood = false;
            CurrentHandIndex--;
        }

        public override void ResetHands()
        {
            base.ResetHands();
            CurrentHandStood = false;
        }

        public List<HandResult> GetPlayerHandResults() =>
            hands.Select(hand => new HandResult(CardCount: hand.Cards.Count, hand.GetHandValue)).ToList();
    }
}
