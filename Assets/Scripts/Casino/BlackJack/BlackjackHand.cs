using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.BlackJack
{
    public class BlackjackHand : MonoBehaviour
    {
        private List<Transform> _handPositions;
        public List<BlackjackCard> Cards { get; } = new();
        [SerializeField] private Image handValueContainer;
        [SerializeField] private TMP_Text handValueText;
        [SerializeField] private bool isDealerHand;

        private void Awake()
        {
            _handPositions = GetComponentsInChildren<Transform>().Skip(1).ToList();
        }

        public IEnumerator RevealHoleCard()
        {
            if (Cards.Count < 2) yield break;
            yield return Cards[1].Reveal();
            UpdateHandDisplay(showFullValue: true);
        }

        public IEnumerator AddCardToHand(BlackjackCard card, bool viaDoubleDown = false)
        {
            Cards.Add(card);
            yield return CardGameHelper.AnimateCardToPosition(card.gameObject, card.transform.position,
                _handPositions[Cards.Count - 1].position, 0.25f);
            card.transform.SetParent(_handPositions[Cards.Count - 1]);
            // Double down card is rotated sideways to indicate that the action has been taken
            if (viaDoubleDown) card.transform.rotation = Quaternion.Euler(0, 0, 90);

            handValueContainer.gameObject.SetActive(true);
            var shouldShowFullValue = !isDealerHand || Cards.Count > 2;
            UpdateHandDisplay(shouldShowFullValue);
        }

        public void ClearHand()
        {
            Cards.ForEach(c => c.DestroyCard());
            Cards.Clear();
            handValueContainer.gameObject.SetActive(false);
        }

        public BlackjackCard TakeSecondCard()
        {
            if (Cards.Count < 2) return null;
            var card = Cards[1];
            Cards.RemoveAt(1);
            UpdateHandDisplay(showFullValue: true);
            return card;
        }

        public int GetHandValue => Cards.OrderBy(c => c.Data.Value.OrderingPriority)
            .Aggregate(0, (total, card) => total + card.Data.Value.GetDynamicValue(total));

        private int GetFirstCardValue() => Cards.First().Data.Value.StaticValue;

        private void UpdateHandDisplay(bool showFullValue)
        {
            handValueText.text = showFullValue ? GetHandValue.ToString() : GetFirstCardValue().ToString();
        }
    }
}
