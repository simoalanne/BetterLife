using System.Collections;
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

        [Header("Buttons")]
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _undoBetButton;
        [SerializeField] private Button _resetBetsButton;
        [SerializeField] private Button _exitTableButton;

        [Header("Text")]
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private TMP_Text _totalBetText;
        [SerializeField] private TMP_Text _noBalanceText;
        [SerializeField] private TMP_Text _winningNumberText;
        [SerializeField] private TMP_Text _winningsText;

        [Header("Variables")]
        [SerializeField] private float _noBalanceTextScreenTime = 0.75f;
        [SerializeField] private float _roundEndTextsOnScreenTime = 4f;

        void Awake()
        {
            _spinButton.interactable = false;
            _resetBetsButton.interactable = false;
            _undoBetButton.interactable = false;
            _exitTableButton.interactable = true;
            _totalBetText.text = "Total Bet: 0";
            _winningNumberText.text = "";
            _winningsText.text = "";
        }

        public void Spin()
        {
            _spinButton.interactable = false;
            _resetBetsButton.interactable = false;
            _undoBetButton.interactable = false;
            _exitTableButton.interactable = false;
            _rouletteSpinner.SpinTheWheel();
        }

        public void BetPlaced()
        {
            _spinButton.interactable = true;
            _resetBetsButton.interactable = true;
            _undoBetButton.interactable = true;
            _exitTableButton.interactable = false;
        }

        public void NoBetsPlaced()
        {
            _undoBetButton.interactable = false;
            _spinButton.interactable = false;
            _resetBetsButton.interactable = false;
            _exitTableButton.interactable = true;
        }

        public void UndoBet()
        {
            _rouletteBetHandler.UndoLatestBet();
        }

        public void ClearAllBets()
        {
            _resetBetsButton.interactable = false;
            _spinButton.interactable = false;
            _rouletteBetHandler.ResetAllBets();
        }

        public void ExitTable()
        {
            if (Time.timeScale != 1f)
            {
                Time.timeScale = 1f;
            }

            if (_resetBetsButton.interactable)
            {
                _resetBetsButton.interactable = false;
                _rouletteBetHandler.ResetAllBets();
            }

            PlayerManager.Instance.MoneyInBankAccount = _rouletteBetHandler.PlayerBalance;
            SceneLoader.Instance.LoadScene("Casino", true);
        }

        public void SetBalanceAndTotalBetText(float balance, float totalBet)
        {
            _totalBetText.text = $"Total Bet: {totalBet}";
            _balanceText.text = $"Balance: {balance}";
        }

        public IEnumerator DisplayRoundEndTexts(int winningNumber, float winnings)
        {
            _winningNumberText.text = $"Winning Number: {winningNumber}";
            _winningsText.text = $"Winnings: {winnings}";
            yield return new WaitForSeconds(_roundEndTextsOnScreenTime);
            _winningNumberText.text = "";
            _winningsText.text = "";
        }

        public IEnumerator ShowNoBalanceText()
        {
            _noBalanceText.enabled = true;
            yield return new WaitForSeconds(_noBalanceTextScreenTime);
            _noBalanceText.enabled = false;
        }
    }
}
