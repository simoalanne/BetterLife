using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using UnityEngine;

namespace Casino.BlackJack
{
    public record AllowedInRoundActions(bool CanSurrender, bool CanDoubleDown, bool CanSplit, bool CanPlaceInsurance);

    public record AllowedPostRoundActions(bool CanRepeatBet, bool CanDoubleBet);

    public class BlackjackManager : MonoBehaviour
    {
        [SerializeField] private float cardTravelTime = 2f;

        [SerializeField] private BlackjackDealer dealer;
        [SerializeField] private BlackjackPlayer player;
        [SerializeField] private BlackjackCard card;
        [SerializeField] private float delayBetweenDealtCards = 0.5f;

        public event Action<AllowedInRoundActions> OnInRoundActionsUpdated = delegate { };
        public event Action<AllowedPostRoundActions> OnPostRoundActionsUpdated = delegate { };

        [Serializable]
        public struct TestCardData
        {
            public Sprite sprite;
            public bool isAce;
            public bool isJoker;
            public int value; // only used if not ace or joker
            public int uniqueRank; // to differentiate between cards of same value
        }


        [SerializeField, Tooltip("For testing purposes. Cards here override random draws from deck")]
        private List<TestCardData> testStartingCards = new();

        private CardData GetTestCard()
        {
            var element = testStartingCards.FirstOrDefault();
            if (element.sprite is null) return null;
            testStartingCards.RemoveAt(0);
            return new CardData(element.sprite,
                element.isAce ? new AceValue() :
                element.isJoker ? new JokerValue() :
                new RegularValue(Mathf.Clamp(element.value, 2, 10)),
                element.uniqueRank);
        }

        private void Awake()
        {
            Services.Register(this);
        }

        public void StartRound()
        {
            player.ResetHands();
            dealer.ResetHands();
            Services.BlackjackDeck.ShuffleAndResetDeck();
            OnInRoundActionsUpdated.Invoke(null); // No actions allowed during dealing
            StartCoroutine(
                DealStartingCards());
        }

        public void Hit()
        {
            StartCoroutine(DealCard(participant: player));
        }

        public void Stand()
        {
            player.StandCurrentHand();
            DetermineNextTurn();
        }

        public IEnumerator HitAndStand()
        {
            yield return DealCard(participant: player, viaDoubleDown: true);
            Stand();
        }

        private IEnumerator DealStartingCards() // for testing purposes
        {
            for (var i = 0; i < 4; i++)
            {
                yield return DealCard(participant: i % 2 == 0 ? player : dealer, isHoleCard: i == 3);
            }

            var (canDoubleDown, canSplit) = CanOrDoubleDownOrSplit();
            var canPlaceInsurance = CanPlaceInsurance();
            OnInRoundActionsUpdated.Invoke(new AllowedInRoundActions(
                CanSurrender: true,
                CanDoubleDown: canDoubleDown,
                CanSplit: canSplit,
                CanPlaceInsurance: canPlaceInsurance
            ));

            if (!canPlaceInsurance)
            {
                DetermineNextTurn();
            }
        }

        public void InsuranceAccepted(bool accepted)
        {
            if (accepted)
            {
                Services.BlackjackMoneyHandler.PlaceInsuranceBet();
            }

            StartCoroutine(CheckForDealerBlackjack());
        }

        private IEnumerator CheckForDealerBlackjack()
        {
            OnInRoundActionsUpdated.Invoke(null);
            yield return new WaitForSeconds(1f);
            if (dealer.CurrentHand.GetHandValue == 21)
            {
                yield return dealer.CurrentHand.RevealHoleCard();
                RoundOver();
                yield break;
            }

            DetermineNextTurn();
        }

        private IEnumerator DealCard(BlackjackParticipant participant, bool isHoleCard = false,
            bool viaDoubleDown = false)
        {
            OnInRoundActionsUpdated.Invoke(null);
            var testCard = GetTestCard();
            testCard ??= Services.BlackjackDeck.DrawCard(jokersAllowed: participant is BlackjackPlayer);
            var newCard = Instantiate(card, transform.position, Quaternion.identity);
            newCard.Init(testCard, faceUp: !isHoleCard);
            yield return participant.CurrentHand.AddCardToHand(newCard, viaDoubleDown);
            yield return new WaitForSeconds(delayBetweenDealtCards);
            if (participant is BlackjackPlayer && dealer.CurrentHandCards.Count == 2)
            {
                DetermineNextTurn();
            }
        }

        // Call when player stands or hits
        private void DetermineNextTurn()
        {
            var currentHandEnded = player.CurrentHand.GetHandValue >= 21 || player.CurrentHandStood;

            if (!currentHandEnded)
            {
                var hasNeverHit = player.CurrentHand.Cards.Count < 3 && player.CurrentHandIndex == 0;
                var (canDoubleDown, canSplit) = CanOrDoubleDownOrSplit();
                OnInRoundActionsUpdated.Invoke(new AllowedInRoundActions(
                    CanSurrender: hasNeverHit,
                    CanDoubleDown: canDoubleDown,
                    CanSplit: canSplit,
                    CanPlaceInsurance: false
                ));
                return;
            }

            if (player.HasMoreHands)
            {
                player.MoveToNextHand();
                var (canDoubleDown, canSplit) = CanOrDoubleDownOrSplit();
                OnInRoundActionsUpdated.Invoke(new AllowedInRoundActions(
                    CanSurrender: false,
                    CanDoubleDown: canDoubleDown,
                    CanSplit: canSplit,
                    CanPlaceInsurance: false
                ));
                return;
            }

            // Dealer doesn't have to play if player can't win with any hands
            if (player.HasBustedAllHands)
            {
                RoundOver();
                return;
            }

            // If player has blackjack and dealer doesn't, dealer also won't play but will reveal hole card
            if (player.AllHandsBlackjack && dealer.CurrentHand.GetHandValue != 21)
            {
                StartCoroutine(dealer.CurrentHand.RevealHoleCard()); // Dealer still reveals hole card
                RoundOver();
                return;
            }

            OnInRoundActionsUpdated.Invoke(null);
            StartCoroutine(HitDealer());
        }

        public IEnumerator Split()
        {
            yield return player.SplitCurrentHand();
            var moneyHandler = Services.BlackjackMoneyHandler;
            moneyHandler.PlaceBet(moneyHandler.GetInitialBetAmountForCurrentRound(),
                new HandBet(player.CurrentHandIndex + 1, IsDoubleDown: false));
            // deal two more cards, one to each hand
            yield return DealCard(participant: player);
            player.MoveToNextHand();
            yield return DealCard(participant: player);
            player.MoveToPreviousHand();
        }


        private IEnumerator HitDealer()
        {
            yield return dealer.CurrentHand.RevealHoleCard();
            OnInRoundActionsUpdated.Invoke(null);
            yield return new WaitForSeconds(1f);

            while (dealer.CurrentHand.GetHandValue < 17)
            {
                yield return DealCard(participant: dealer);
            }

            RoundOver();
        }

        public void Surrender() => RoundOver(viaSurrender: true);

        private void RoundOver(bool viaSurrender = false)
        {
            OnInRoundActionsUpdated.Invoke(null);
            var moneyHandler = Services.BlackjackMoneyHandler;
            if (moneyHandler.CurrentTotalBet == 0)
            {
                return;
            }

            moneyHandler.CreditWinnings(new RoundEndResult(dealer.GetDealerHandResult(),
                viaSurrender ? null : player.GetPlayerHandResults()));
            var (canRepeat, canDouble) = moneyHandler.CanAffordRepeatOrDoubleBetFromPreviousRound();
            OnPostRoundActionsUpdated.Invoke(new AllowedPostRoundActions(canRepeat, canDouble));
        }

        private (bool canDoubleDown, bool canSplit) CanOrDoubleDownOrSplit()
        {
            if (player.CurrentHandCards.Count > 2)
            {
                return (false, false);
            }

            var canAffordExtraBets = Services.BlackjackMoneyHandler.CanAffordExtraBetsForCurrentRound();
            return (canAffordExtraBets, player.CanSplit && canAffordExtraBets);
        }

        private bool CanPlaceInsurance()
        {
            return dealer.CanOfferInsurance && Services.BlackjackMoneyHandler.CanAffordInsuranceBetForCurrentRound();
        }
    }
}
