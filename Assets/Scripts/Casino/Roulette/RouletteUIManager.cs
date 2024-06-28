using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

namespace Casino.Roulette
{
    public class RouletteUIManager : MonoBehaviour
    {
        [Header("Scripts")]
        [SerializeField] private RouletteSpinner _rouletteSpinner;
        [SerializeField] private RouletteBetHandler _rouletteBetHandler;

        [Header("GameObjects")]
        [SerializeField] private GameObject _winningNumberStrip; // Shows the winning number and the left and right numbers of it
        [SerializeField] private GameObject _winningPanel; // Shows the amount of winnings
        [SerializeField] private GameObject _losingPanel; // Shows the player that they didn't win

        [Header("Canvases")]
        [SerializeField] private Canvas _resultsCanvas;

        [Header("Buttons")]
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _undoBetButton;
        //[SerializeField] private Button _resetBetsButton;
        [SerializeField] private Button _exitTableButton;

        [Header("Text")]
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private TMP_Text _totalBetText;
        [SerializeField] private TMP_Text _noBalanceText;

        [Header("Variables")]
        [SerializeField] private float _noBalanceTextScreenTime = 0.75f;

        void Awake()
        {
            _spinButton.interactable = false;
            //_resetBetsButton.interactable = false;
            _undoBetButton.interactable = false;
            _exitTableButton.interactable = true;
        }

        public void Spin()
        {
            _spinButton.interactable = false;
            //_resetBetsButton.interactable = false;
            _undoBetButton.interactable = false;
            _exitTableButton.interactable = false;
            _rouletteSpinner.SpinTheWheel();
        }

        public void BetPlaced()
        {
            _spinButton.interactable = true;
            //_resetBetsButton.interactable = true;
            _undoBetButton.interactable = true;
            _exitTableButton.interactable = false;
        }

        public void NoBetsPlaced()
        {
            _undoBetButton.interactable = false;
            _spinButton.interactable = false;
            //_resetBetsButton.interactable = false;
            _exitTableButton.interactable = true;
        }

        public void UndoBet()
        {
            _rouletteBetHandler.UndoLatestBet();
        }

        /*public void ClearAllBets()
        {
            _resetBetsButton.interactable = false;
            _spinButton.interactable = false;
            _rouletteBetHandler.ResetAllBets();
        } */

        public void ExitTable()
        {
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
            }

            if (_undoBetButton.interactable)
            {
                _undoBetButton.interactable = false;
                _rouletteBetHandler.ResetAllBets();
            }

            PlayerManager.Instance.MoneyInBankAccount = _rouletteBetHandler.PlayerBalance;
            SceneLoader.Instance.LoadScene("Casino", SceneLoader.PlayerVisibility.Visible, SceneLoader.TransitionType.Circle);
        }

        public void SetBalanceAndTotalBetText(float balance, float totalBet)
        {
            _totalBetText.text = $"{totalBet} €";
            _balanceText.text = $"{balance} €";
        }

        public IEnumerator EnableResultsPanel(int winningNumber, float winnings)
        {
            string winningNumberStr = winningNumber.ToString();

            int winningIndex = Array.IndexOf(_rouletteSpinner.RouletteNumbers, winningNumberStr);

            int leftIndex = (winningIndex - 1 + _rouletteSpinner.RouletteNumbers.Length) % _rouletteSpinner.RouletteNumbers.Length;
            int rightIndex = (winningIndex + 1) % _rouletteSpinner.RouletteNumbers.Length;

            string leftNumber = _rouletteSpinner.RouletteNumbers[leftIndex];
            string rightNumber = _rouletteSpinner.RouletteNumbers[rightIndex];

            _winningNumberStrip.transform.Find("LeftNumber").transform.Find("LeftNumberText").GetComponent<TMP_Text>().text = leftNumber;
            _winningNumberStrip.transform.Find("RightNumber").transform.Find("RightNumberText").GetComponent<TMP_Text>().text = rightNumber;
            _winningNumberStrip.transform.Find("WinningNumber").transform.Find("WinningNumberText").GetComponent<TMP_Text>().text = winningNumberStr;

            SetThreeNumbersColor(leftNumber, _winningNumberStrip.transform.Find("LeftNumber").transform.Find("LeftNumberColor").GetComponent<Image>());
            SetThreeNumbersColor(rightNumber, _winningNumberStrip.transform.Find("RightNumber").transform.Find("RightNumberColor").GetComponent<Image>());
            SetThreeNumbersColor(winningNumberStr, _winningNumberStrip.transform.Find("WinningNumber").transform.Find("WinningNumberColor").GetComponent<Image>());

            _winningNumberStrip.SetActive(true);
            yield return new WaitForSeconds(1.5f);

            if (winnings > 0)
            {
                _winningPanel.SetActive(true);
                _winningPanel.transform.Find("ValueText").GetComponent<TMP_Text>().text = $"{winnings} €";
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

        public IEnumerator ShowNoBalanceText()
        {
            _noBalanceText.enabled = true;
            yield return new WaitForSeconds(_noBalanceTextScreenTime);
            _noBalanceText.enabled = false;
        }

        void SetThreeNumbersColor(string number, Image image)
        {
            string[] redNumbers = new string[] { "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36" };
            string[] blackNumbers = new string[] { "2", "4", "6", "8", "10", "11", "13", "15", "17", "20", "22", "24", "26", "28", "29", "31", "33", "35" };

            Color redColor = new(128f / 255f, 0, 0);
            Color blackColor = new(50f / 255f, 50f / 255f, 50f / 255f);
            Color greenColor = new(0, 128f / 255f, 0);

            if (redNumbers.Contains(number))
            {
                image.color = redColor;
            }
            else if (blackNumbers.Contains(number))
            {
                image.color = blackColor;
            }
            else
            {
                image.color = greenColor;
            }
        }
    }
}
