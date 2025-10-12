#nullable enable

using System.Linq;
using Helpers;

namespace Casino.BlackJack
{
    public class BlackjackMoneyHandler : CasinoMoneyHandler<IBlackjackBet, RoundEndResult, float>
    {
        protected override float CalculateWinnings(RoundEndResult roundResult)
        {
            var totalWinnings = 0f;
            // Check insurance bets first
            var insuranceBets = CurrentBets.Where(bet => bet.Key is SideBet { BetType: BlackjackBetType.Insurance });
            insuranceBets.ToList().ForEach(bet =>
            {
                if (roundResult.DealerResult.HandValue is 21 && roundResult.DealerResult.CardCount is 2)
                {
                    totalWinnings += bet.Amount * 3;
                }
            });

        var regularBets = CurrentBets.Where(bet => bet.Key is HandBet);

            // Player surrendered, return half the bet amount
            if (roundResult.PlayerResults == null)
            {
                regularBets.ToList().ForEach(bet => totalWinnings += bet.Amount / 2f);
                return totalWinnings;
            }

            regularBets.ToList().ForEach(bet =>
            {
                var index = (bet.Key as HandBet)!.HandIndex;
                var playerHandResult = roundResult.PlayerResults.ElementAtOrDefault(index);
                if (playerHandResult == null) throw new System.Exception($"Bet placed on index {index} has no result!");

                var playerHandValue = playerHandResult.HandValue;
                var dealerHandValue = roundResult.DealerResult.HandValue;
                var playerCardCount = playerHandResult.CardCount;
                var dealerCardCount = roundResult.DealerResult.CardCount;

                var playerBusts = playerHandValue > 21;
                var dealerBusts = dealerHandValue > 21;
                var dealerBlackjack = dealerHandValue is 21 && dealerCardCount is 2;
                var playerBlackjack = playerHandValue is 21 && playerCardCount is 2;


                var dealerBeatsPlayer = playerBusts || (dealerBlackjack && !playerBlackjack) ||
                                        (dealerHandValue > playerHandValue && !dealerBusts);

                if (dealerBeatsPlayer) return;

                var playerBeatsDealer = playerBlackjack && !dealerBlackjack || dealerBusts ||
                                        playerHandValue > dealerHandValue;

                if (playerBeatsDealer)
                {
                    // A split hand is not considered a blackjack even if it has 21 with 2 cards
                    // If the player has split at least once you can derive that no hand is a natural blackjack
                    var hasSplitAtLeastOnce = roundResult.PlayerResults.Count > 1;
                    totalWinnings += bet.Amount * (playerBlackjack && !hasSplitAtLeastOnce ? 2.5f : 2f);
                    return;
                }

                totalWinnings += bet.Amount;
            });
            return totalWinnings;
        }
        
        protected override float ResolveGameResult(RoundEndResult gameResult, float winnings) => winnings;

        public bool CanAffordExtraBetsForCurrentRound()
        {
            return CanAffordBet(GetInitialBetAmountForCurrentRound());
        }

        public bool CanAffordInsuranceBetForCurrentRound()
        {
            return CanAffordBet(GetInitialBetAmountForCurrentRound() / 2);
        }

        public (bool canRepeat, bool canDouble) CanAffordRepeatOrDoubleBetFromPreviousRound()
        {
            var initialBet = GetInitialBetAmountForPreviousRound();
            return (CanAffordBet(initialBet), CanAffordBet(initialBet * 2));
        }

        public void PlaceInsuranceBet() => PlaceBet(GetInitialBetAmountForCurrentRound() / 2,
            new SideBet(BlackjackBetType.Insurance));


        public float GetInitialBetAmountForCurrentRound() =>
            GetTotalBetByKey(new HandBet(0, IsDoubleDown: false), forCurrentBets: true);


        private float GetInitialBetAmountForPreviousRound() =>
            GetTotalBetByKey(new HandBet(0, IsDoubleDown: false), forCurrentBets: false);


        public void RepeatPreviousRoundBet() =>
            PlaceBet(GetInitialBetAmountForPreviousRound(), new HandBet(0, IsDoubleDown: false));

        public void DoublePreviousRoundBet() =>
            PlaceBet(GetInitialBetAmountForPreviousRound() * 2, new HandBet(0, IsDoubleDown: false));

        protected override void PlaceBetSideEffect() => PlaceChips(CurrentTotalBet);
        protected override void UndoBetSideEffect(Bet<IBlackjackBet> _) => PlaceChips(CurrentTotalBet);

        protected override void CreditWinningsSideEffect(float amount) => PlaceChips(amount);

        private static void PlaceChips(float amount)
        {
            Services.ChipPlacer.PlaceChips(Services.BetSizeManager.GetTotalBetAsChipSprites(amount));
        }
    }
}
