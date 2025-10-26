using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Casino.Roulette
{
    public class BetInfoTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ButtonHighlight _buttonHighlight;
        private TooltipDisplayer _tooltipDisplayer;
        private IRouletteBetKey _betKey;

        private void Start()
        {
            _betKey = GetComponent<BetKeyStorer>().BetKey;
            _buttonHighlight = Services.ButtonHighlight;
            _tooltipDisplayer = Services.TooltipDisplayer;
        }

        public void OnPointerEnter(PointerEventData _)
        {
            var numbersToHighlight = _betKey switch
            {
                InsideBetKey insideBetKey => insideBetKey.Numbers,
                OutsideBetKey outsideBetKey => RouletteConstants.OutsideBetsDict[outsideBetKey.BetCategory].Numbers,
                _ => new HashSet<int>()
            };
            _buttonHighlight.HighlightNumbers(numbersToHighlight);
            _tooltipDisplayer.SetBetInfo(_betKey);
        }

        public void OnPointerExit(PointerEventData _)
        {
            _buttonHighlight.HighlightNumbers(new HashSet<int>());
            _tooltipDisplayer.HideBetInfo();
        }
    }
}
