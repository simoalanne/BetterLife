using System;
using Helpers;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [RequireComponent(typeof(HUDAttachablePanel))]
    public class EndGame : MonoBehaviour
    {
        [Header("End Game Settings")]
        [SerializeField] private float moneyToPay = 10000;
        [Header("Texts")]
        [SerializeField] private TMP_Text _endGameText;
        [Header("Buttons")]
        [SerializeField] private Button _endGameButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField, Scene] private string victoryScene = "EndGameCutscene";

        private void OnEnable()
        {
            _endGameButton.onClick.AddListener(PayMoneyAndEndGame);
            _cancelButton.onClick.AddListener(CancelEndGame);
            _endGameText.text = $"Pay {moneyToPay} to end the game?";
            _endGameButton.interactable = Services.PlayerManager.MoneyInBankAccount >= moneyToPay &&
                                          Services.PlayerHUD.ActiveLoan == null;
        }

        private void PayMoneyAndEndGame()
        {
            Services.PlayerManager.ResetMoney(); 
            victoryScene.LoadScene();
        }

        private void CancelEndGame() => GetComponent<HUDAttachablePanel>().Close();
        
    }
}
