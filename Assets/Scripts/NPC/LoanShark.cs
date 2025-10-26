using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using ScriptableObjects;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace NPC
{
    public class LoanShark : MonoBehaviour, IInteractable
    {
        [Header("Take loan")]
        [SerializeField] private HideableElement takeLoanMenu;
        [SerializeField] private GridLayoutGroup loansGrid;

        [Header("Pay back loan")]
        [SerializeField] private HideableElement payBackLoanMenu;

        [Header("Conversations")]
        [SerializeField] private Conversation notEnoughMoneyToPayBackLoan;
        [SerializeField] private Conversation firstTimeTalking;

        private List<Loan> loans = new();


        private void Awake()
        {
            InitializeLoans();
        }

        public void Interact()
        {
            if (!Services.PlayerManager.StoryProperties.HasTalkedToLoanShark)
            {
                firstTimeTalking.Start(state =>
                {
                    if (state is DialogueState.YesClicked)
                    {
                        OpenTakeLoanMenu();
                    }

                    Services.PlayerManager.StoryProperties.HasTalkedToLoanShark = true;
                });
                return;
            }

            if (Services.PlayerHUD.ActiveLoan is null)
            {
                OpenTakeLoanMenu();
                return;
            }

            if (Services.PlayerManager.MoneyInBankAccount >= Services.PlayerHUD.ActiveLoan?.ActualAmount)
            {
                OpenPayBackLoanMenu();
                return;
            }

            notEnoughMoneyToPayBackLoan.Start();
        }

        private void InitializeLoans()
        {
            loans = Resources.LoadAll<Loan>("Loans").OrderBy(loan => loan.loanAmount).ToList();
            var gridItems = loansGrid.GetComponentsInChildren<Button>();
            for (var i = 0; i < gridItems.Length; i++)
            {
                var loan = loans[i];
                gridItems[i].transform.Find("LoanAmount").GetComponent<TMP_Text>().text = loan.loanAmount + "€";
                gridItems[i].transform.Find("InterestRate").GetComponent<TMP_Text>().text = loan.interestRate + "%";
                var dayString = loan.daysToRepay > 1 ? " days" : " day";
                gridItems[i].transform.Find("DaysToRepay").GetComponent<TMP_Text>().text = loan.daysToRepay + dayString;
                gridItems[i].onClick.AddListener(() => TakeOutLoan(loan));
            }
        }

        private void TakeOutLoan(Loan loan)
        {
            Services.PlayerManager.MoneyInBankAccount += loan.loanAmount;
            Services.PlayerHUD.EnableActiveLoanPanel(loan);
            CloseTakeLoanMenu();
        }

        public void PayBackLoan()
        {
            Services.PlayerManager.MoneyInBankAccount -= Services.PlayerHUD.ActiveLoan.ActualAmount;
            Services.PlayerHUD.DisableActiveLoanPanel();
            ClosePayBackLoanMenu();
        }

        private void OpenTakeLoanMenu() => TogglePanel(takeLoanMenu, true);

        private void OpenPayBackLoanMenu() => TogglePanel(payBackLoanMenu, true);

        public void CloseTakeLoanMenu() => TogglePanel(takeLoanMenu, false);

        public void ClosePayBackLoanMenu() => TogglePanel(payBackLoanMenu, false);

        private static void TogglePanel(HideableElement panel, bool isOpen)
        {
            panel.Toggle(isOpen);
            Services.InputManager.EnablePlayerInput(!isOpen);
            Services.GameTimer.IsPaused = isOpen;
        }
    }
}
