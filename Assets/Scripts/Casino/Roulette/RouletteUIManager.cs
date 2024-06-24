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
        [SerializeField] private Button _turboSpeedButton;
        [SerializeField] private Button _resetBetsButton;
        [SerializeField] private Button _exitTableButton;

        [Header("Text")]
        [SerializeField] private TMP_Text _balanceText;
        [SerializeField] private TMP_Text _totalBetText;
        [SerializeField] private TMP_Text _noBalanceText;
        [SerializeField] private TMP_Text _turboSpeedText;
        [SerializeField] private TMP_Text _winningNumberText;
        [SerializeField] private TMP_Text _winningsText;

        [Header("Variables")]
        [SerializeField] private float _turboSpeedTimeScale = 2f;
        [SerializeField] private float _noBalanceTextScreenTime = 0.75f;
        [SerializeField] private float _roundEndTextsOnScreenTime = 4f;

        private bool _isTurboSpeed = false;

        void Awake()
        {
            _spinButton.interactable = false;
            _turboSpeedButton.interactable = true;
            _resetBetsButton.interactable = false;
            _exitTableButton.interactable = true;
            _totalBetText.text = "Total Bet: 0";
            _turboSpeedText.text = "turbo speed";
            _winningNumberText.text = "";
            _winningsText.text = "";
        }

        public void Spin()
        {
            _spinButton.interactable = false;
            _resetBetsButton.interactable = false;
            _exitTableButton.interactable = false;
            _rouletteSpinner.SpinTheWheel();
        }

        public void BetPlaced()
        {
            _spinButton.interactable = true;
            _resetBetsButton.interactable = true;
            _exitTableButton.interactable = false;
        }

        public void NoBetsPlaced()
        {
            _spinButton.interactable = false;
            _resetBetsButton.interactable = false;
            _exitTableButton.interactable = true;
        }

        public void TurboSpeed()
        {
            if (_isTurboSpeed)
            {
                _isTurboSpeed = false;
                Time.timeScale = 1f;
                _turboSpeedText.text = "turbo speed";
            }
            else
            {
                _isTurboSpeed = true;
                Time.timeScale = _turboSpeedTimeScale;
                _turboSpeedText.text = "normal speed";
            }
        }

        public void ResetBets()
        {
            _resetBetsButton.interactable = false;
            _spinButton.interactable = false;
            _rouletteBetHandler.ResetBets();
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
                _rouletteBetHandler.ResetBets();
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
