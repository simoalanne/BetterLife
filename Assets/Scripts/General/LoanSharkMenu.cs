using UnityEngine;
using Player;
using UnityEngine.UI;
using TMPro;
using UI.Extensions;

/// <summary>
/// Should be instantiated to player HUD as a child when player interacts with the loan shark.
/// Contains options take a loan or pay back a loan.
/// Is destroyed when the player closes the menu.
/// </summary>
public class LoanSharkMenu : MonoBehaviour
{
    [SerializeField] private GameObject _takeLoanMenu;
    [SerializeField] private GameObject _payBackLoanMenu;
    [SerializeField] private TMP_Text _notEnoughMoneyText;
    [SerializeField] private Button _payAllLoansButton;

    void Awake()
    {
        gameObject.SetActive(true);
        CheckForLoans();

    }

    void CheckForLoans()
    {
        var Loans = PlayerInventory.Instance.GetLoans();

        int allLoansSum = 0;
        foreach (var loan in PlayerInventory.Instance.GetLoans())
        {
            allLoansSum += loan.loanAmount;
        }

        if (allLoansSum > PlayerManager.Instance.MoneyInBankAccount)
        {
            _payAllLoansButton.interactable = false;
        }
    }

    public void TakeLoanOptionClicked()
    {
        _takeLoanMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void PayBackLoanOptionClicked()
    {
        _payBackLoanMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void TakeLoanMenuClosed()
    {
        gameObject.SetActive(true);
    }

    public void CloseMainMenu()
    {
        Destroy(gameObject);
    }

    public void CloseTakeLoanMenu()
    {
        _takeLoanMenu.SetActive(false);
        gameObject.SetActive(true);
    }

    public void ClosePayBackLoanMenu()
    {
        _payBackLoanMenu.SetActive(false);
        gameObject.SetActive(true);
    }

    public void PayBackLoan(Loan loan)
    {
        StopAllCoroutines();
        if (PlayerManager.Instance.MoneyInBankAccount < loan.loanAmount)
        {
            UIAnimations.ObjectPopup(_notEnoughMoneyText.gameObject, 0.1f);
            return;
        }

        PlayerManager.Instance.MoneyInBankAccount -= loan.loanAmount;
        PlayerInventory.Instance.RemoveFromInventory(loan);
    }

    public void PayAllLoans()
    {
        foreach (var loan in PlayerInventory.Instance.GetLoans())
        {
            PayBackLoan(loan);

        }
    }

    public void TakeOutLoan(Loan loan)
    {
        PlayerManager.Instance.MoneyInBankAccount += loan.loanAmount;
        PlayerInventory.Instance.AddToInventory(loan);
    }
}
