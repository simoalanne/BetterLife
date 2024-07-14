using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;


public class MoneyLender : MonoBehaviour, IInteractable
{
    [Header("Scriptable Object")]
    [SerializeField] private Loan[] _loans; // ScriptableObject that holds loan data.

    [Header("UI")]
    [SerializeField] private Canvas _loanUICanvas; // The canvas for the loan UI.
    [SerializeField] private Image _loanUI; // The UI for the loan.

    [Header("Loan UI Controllers")]
    [SerializeField] private Button _loanButton; // The button to take out a loan.

    [Header("Loan UI Buttons")]
    [SerializeField] private Button _exitButton; // The button to exit the loan UI.

    void Awake()
    {
        _loanUICanvas.gameObject.SetActive(false);
        _exitButton.onClick.AddListener(ExitLoanUI);
        Array.Sort(_loans, (loan1, loan2) => loan1.loanAmount.CompareTo(loan2.loanAmount)); // Sort loans by loan amount.
        SetPresetLoanAmounts();
    }

    public void Interact()
    {
        PlayerManager.Instance.DisablePlayerMovement();
        PlayerManager.Instance.DisablePlayerInteract();
        Time.timeScale = 0;
        _loanUICanvas.gameObject.SetActive(true);
    }

    public void TakeOutLoan(int loanAmount)
    {
        PlayerManager.Instance.MoneyInBankAccount += loanAmount;
        ExitLoanUI();
    }

    public void ExitLoanUI()
    {
        _loanUICanvas.gameObject.SetActive(false);
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        Time.timeScale = 1;
    }

    private void SetPresetLoanAmounts()
    {
        foreach (var loan in _loans)
        {
            Button presetLoanButton = Instantiate(_loanButton, _loanUI.transform);
            presetLoanButton.transform.Find("LoanAmount").GetComponent<TMP_Text>().text =
            $"Loan: {loan.loanAmount} €";

            presetLoanButton.transform.Find("InterestRate").GetComponent<TMP_Text>().text =
            $"Interest rate: {loan.interestRate}%";

            presetLoanButton.transform.Find("DaysToRepay").GetComponent<TMP_Text>().text =
            $"Days to repay: {loan.daysToRepay}";

            presetLoanButton.transform.Find("AmountToRepay").GetComponent<TMP_Text>().text =
            $"Amount to repay: {loan.loanAmount + (loan.loanAmount * loan.interestRate / 100)} €";

            presetLoanButton.onClick.AddListener(() => TakeOutLoan(loan.loanAmount));
        }
    }
}
