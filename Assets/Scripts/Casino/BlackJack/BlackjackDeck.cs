using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;

namespace Casino.BlackJack
{
    public class BlackjackDeck : MonoBehaviour
    {
        private record DeckCard(CardData Data, bool IsDrawn);

        [SerializeField] private bool includeJokers = true;

        private List<DeckCard> _deck;

        private void Awake()
        {
            Services.Register(this);
            PopulateDeck();
            ShuffleAndResetDeck();
        }

        private void PopulateDeck()
        {
            var sprites = CardGameHelper.LoadCardSprites(includeJokers);
            var uniqueRankIndex = 0;
            const int acesEndIndex = 4;
            const int jokersStartIndex = 52;

            _deck = sprites.Select((sprite, i) =>
            {
                if (i % 4 == 0) uniqueRankIndex++;
                return i switch
                {
                    < acesEndIndex => new DeckCard(new CardData(sprite, new AceValue(), uniqueRankIndex), false),
                    < jokersStartIndex => new DeckCard(new CardData(
                            sprite,
                            new RegularValue(Mathf.Clamp(uniqueRankIndex, 2, 10)),
                            uniqueRankIndex),
                        false),
                    _ => new DeckCard(new CardData(sprite, new JokerValue(), uniqueRankIndex), false)
                };
            }).ToList();
        }

        public CardData DrawCard(bool jokersAllowed = true)
        {
            var availableCards = _deck
                .Where(deck => !deck.IsDrawn && (jokersAllowed || deck.Data.Value is not JokerValue))
                .ToList();

            var drawnCard = availableCards[Random.Range(0, availableCards.Count)].Data;

            _deck = _deck
                .Select(c => c.Data.Sprite == drawnCard.Sprite ? c with { IsDrawn = true } : c)
                .ToList();

            return drawnCard;
        }

        public void ShuffleAndResetDeck()
        {
            FunctionLibrary.Shuffle(_deck);
            _deck = _deck.Select(c => c with { IsDrawn = false }).ToList();
        }
    }
}
