using Helpers;
using TMPro;
using UnityEngine;

namespace Casino
{
    /// <summary>
    /// This script handles displaying and updating the player's balance and current bet in the UI. It works for both
    /// table games and slot games. In table games the current bet represents the total bet amount for the round, while in
    /// slot games its equivalent to <see cref="BetSizeManager.CurrentBetSize"/>.
    /// </summary>
    public class BalanceAndBetUI : MonoBehaviour
    {
        [SerializeField, Tooltip("In table games current bet is total bet, in slots it's just the bet size.")]
        private bool isTableGame = true;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text currentBetText;
        [SerializeField] private TMP_Text minMaxBetText;
        
        private void Start()
        {
            var moneyHandler = Services.MoneyHandler;
            var betSizeManager = Services.BetSizeManager;
            var minBet = betSizeManager.MinBetSize;
            var maxBet = moneyHandler.MaxBetLimit;
            minMaxBetText.text = $"{FormatMoney(minBet)} | {FormatMoney(maxBet)}";
            moneyHandler.OnMoneyChanged += UpdateUI;
            if (isTableGame) return;
            betSizeManager.OnBetSizeChanged += UpdateCurrentBet;
        }

        private void UpdateUI(MoneyChangedEvent e)
        {
            balanceText.text = FormatMoney(e.newBalance);
            if (!isTableGame) return;
            currentBetText.text = FormatMoney(e.currentTotalBet);
        }

        private void UpdateCurrentBet(float newBetSize)
        {
            currentBetText.text = FormatMoney(newBetSize);
        }

        private static string FormatMoney(float amount) => $"{amount}€";
    }
}