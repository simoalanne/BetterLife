using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class RouletteUIManager : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] private GameObject _winningNumberStrip; // Shows the winning number and the left and right numbers of it
        [SerializeField] private GameObject _winningPanel; // Shows the amount of winnings
        [SerializeField] private GameObject _losingPanel; // Shows the player that they didn't win

        [Header("Canvases")]
        [SerializeField] private Canvas _resultsCanvas;

        [Header("Buttons")]
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _undoBetButton;
        [SerializeField] private Button resetBetsButton;
        
        [Header("Variables")]
        [SerializeField] private float _noBalanceTextScreenTime = 0.75f;

        [Header("Colors")]
        [SerializeField] private Color redStripColor = new Color(128f/255f, 0, 0);
        [SerializeField] private Color blackStripColor = new Color(50f/255f, 50f/255f, 50f/255f);
        [SerializeField] private Color greenStripColor = new Color(0, 128f/255f, 0);

        private void Start()
        {
            var moneyHandler = Services.RouletteMoneyHandler;
            moneyHandler.OnWinningsCredited += ShowResultsPanel;
            moneyHandler.OnFirstBetPlaced += BetPlaced;
            moneyHandler.OnAllBetsCleared += NoBetsPlaced;
            resetBetsButton.interactable = false;
            _undoBetButton.interactable = true;
        }

        public void Spin()
        {
            _spinButton.interactable = false;
            resetBetsButton.interactable = false;
            _undoBetButton.interactable = false;
            Services.RouletteSpinner.StartRound();
        }

        private void BetPlaced()
        {
            _spinButton.interactable = true;
            //_resetBetsButton.interactable = true;
            _undoBetButton.interactable = true;
        }

        private void NoBetsPlaced()
        {
            _undoBetButton.interactable = false;
            _spinButton.interactable = false;
            //_resetBetsButton.interactable = false;
        }

        public void UndoBet() => Services.RouletteMoneyHandler.UndoLatestBet();
        
        private void ShowResultsPanel(ResolvedRouletteResult result) => StartCoroutine(EnableResultsPanel(result));

        private IEnumerator EnableResultsPanel(ResolvedRouletteResult result)
        {
            var stripElements = _winningNumberStrip.GetComponentsInChildren<RouletteStripElement>();
            // Since the order will be left, right, winning the latter two need to be swapped should be fixed
            (stripElements[0], stripElements[1], stripElements[2]) = (
                stripElements[0], stripElements[2], stripElements[1]);
            stripElements.Zip(result.WinningNumsStrip, (element, number) => (element, number)).ToList()
                .ForEach(tuple =>
                {
                    var isRed = RouletteConstants.OutsideBetsDict[OutsideBet.Red].Numbers.Contains(tuple.number);
                    var isBlack = RouletteConstants.OutsideBetsDict[OutsideBet.Black].Numbers.Contains(tuple.number);
                    var color = isRed ? redStripColor : isBlack ? blackStripColor : greenStripColor;
                    tuple.element.Init(tuple.number.ToString(), color);
                });

            _winningNumberStrip.SetActive(true);
            yield return new WaitForSeconds(1.5f);

            if (result.Winnings > 0)
            {
                _winningPanel.SetActive(true);
                _winningPanel.transform.Find("ValueText").GetComponent<TMP_Text>().text = $"{result.Winnings} €";
            }
            else
            {
                _losingPanel.SetActive(true);
            }

            yield return new WaitForSeconds(3f);

            var canvasGroup = _resultsCanvas.GetComponent<CanvasGroup>();
            float timer = 1.5f;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                canvasGroup.alpha = timer;
                yield return null;
            }

            _winningNumberStrip.SetActive(false);
            _winningPanel.SetActive(false);
            _losingPanel.SetActive(false);
            canvasGroup.alpha = 1;
        }
    }
}
