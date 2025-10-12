namespace Casino.BlackJack
{
    public class BlackjackDealer : BlackjackParticipant
    {
        public bool CanOfferInsurance => CurrentHandCards.Count == 2 && CurrentHandCards[0].Data.Value is AceValue;

        public HandResult GetDealerHandResult() => new(CardCount: CurrentHandCards.Count, CurrentHand.GetHandValue);
    }
}
