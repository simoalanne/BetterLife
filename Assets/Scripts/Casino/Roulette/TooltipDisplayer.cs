using AYellowpaper.SerializedCollections;
using Helpers;
using TMPro;
using UnityEngine;

namespace Casino.Roulette
{
    public class TooltipDisplayer : MonoBehaviour
    {
        [Header("Bet Info Settings")]
        [SerializeField] private Vector2 tooltipCursorOffset = new(0, -50);
        [SerializeField] private Vector2 tooltipBackgroundPadding = new(20, 10);

        [Header("Bet Descriptions")]
        [SerializedDictionary("Bet Key", "Description")]
        public SerializedDictionary<InsideBet, string> insideBetDescriptions = new()
        {
            { InsideBet.StraightUp, "Straight Up" },
            { InsideBet.Split, "Split" },
            { InsideBet.Street, "Street" },
            { InsideBet.Corner, "Corner" },
            { InsideBet.SixLine, "Six Line" },
        };
        [SerializedDictionary("Bet Key", "Description")]
        public SerializedDictionary<OutsideBet, string> outsideBetDescriptions = new()
        {
            { OutsideBet.Red, "Red" },
            { OutsideBet.Black, "Black" },
            { OutsideBet.Odd, "Odd" },
            { OutsideBet.Even, "Even" },
            { OutsideBet.OneToEighteen, "1 to 18" },
            { OutsideBet.NineteenToThirtySix, "19 to 36" },
            { OutsideBet.FirstDozen, "1st Dozen" },
            { OutsideBet.SecondDozen, "2nd Dozen" },
            { OutsideBet.ThirdDozen, "3rd Dozen" },
            { OutsideBet.TopColumn, "2 to 1" },
            { OutsideBet.MiddleColumn, "2 to 1" },
            { OutsideBet.BottomColumn, "2 to 1" },
        };

        private CanvasGroup _canvasGroup;
        private RectTransform _backgroundRect;
        private Canvas _parentCanvas;
        private TMP_Text _betInfo;

        private void Awake()
        {
            Services.Register(this);
            _backgroundRect = GetComponent<RectTransform>();
            _parentCanvas = _backgroundRect.GetComponentInParent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _betInfo = GetComponentInChildren<TMP_Text>();
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Update()
        {
            if (_canvasGroup.alpha == 0) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentCanvas.transform as RectTransform,
                Input.mousePosition,
                _parentCanvas.worldCamera, out var cursorPosition);
            _backgroundRect.anchoredPosition = cursorPosition + tooltipCursorOffset;
        }

        public void SetBetInfo(IRouletteBetKey betKey)
        {
            var (showBetType, showBetOdds) = GetDisplaySettings();
            if (!showBetType && !showBetOdds) return;
            _canvasGroup.alpha = 1;
            var description = showBetType ? GetBetDescription(betKey) : "";
            var odds = showBetOdds ? GetFormattedOdds(betKey) : "";
            _betInfo.text = $"{description}\n{odds}".TrimStart();
            CalculateBackgroundSize();
        }

        public void HideBetInfo() => _canvasGroup.alpha = 0;


        private void CalculateBackgroundSize()
        {
            var textSize = _betInfo.GetPreferredValues();
            _backgroundRect.sizeDelta = textSize + tooltipBackgroundPadding;
        }

        private static (bool showBetType, bool showBetOdds) GetDisplaySettings() =>
            Services.Get<RouletteGameSettings>().GetDisplaySettings();

        private string GetBetDescription(IRouletteBetKey betKey)
        {
            return betKey switch
            {
                InsideBetKey insideBet => insideBetDescriptions[
                    RouletteConstants.InsideCountToBet[insideBet.Numbers.Count]],
                OutsideBetKey outsideBet => outsideBetDescriptions[outsideBet.BetCategory],
                _ => "Unknown Bet"
            };
        }

        private static string GetFormattedOdds(IRouletteBetKey betKey)
        {
            return betKey switch
            {
                InsideBetKey insideBet =>
                    $"{RouletteConstants.InsideBetsDict[RouletteConstants.InsideCountToBet[insideBet.Numbers.Count]]}:1",
                OutsideBetKey outsideBet => $"{RouletteConstants.OutsideBetsDict[outsideBet.BetCategory].Payout}:1",
                _ => "Unknown Odds"
            };
        }
    }
}
