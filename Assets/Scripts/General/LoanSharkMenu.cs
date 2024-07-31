using UnityEngine;
using Player;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UI.Extensions;

public class LoanSharkMenu : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _takeLoanMenu;
    [SerializeField] private GameObject _payBackLoanMenu;
    [SerializeField] private GridLayoutGroup _loansGrid;
    [SerializeField] private DialogueTrigger _notEnoughMoneyToPayBackLoanDialogue;
    [SerializeField] private DialogueTrigger _loanSharkFirstTimeDialogue;
    [SerializeField] private float _panelMoveDuration = 0.25f;
    private Vector2 _panelsOffScreenPos = new(-1250, -100);
    private Vector2 _panelsOnScreenPos = new(100, -100);

    [SerializeField] private Loan[] _loansToDisplay;

    void Awake()
    {
        InitializeLoans();
    }

    public void Interact()
    {
        DialogueManager.Instance.OnYesClicked += OpenTakeLoanMenu;

        if (PlayerManager.Instance.HasTalkedToLoanShark == false)
        {
            PlayerManager.Instance.HasTalkedToLoanShark = true;
            _loanSharkFirstTimeDialogue.TriggerDialogue();
            return;
        }

        Debug.Log("Interacting with LoanSharkMenu");
        if (PlayerHUD.Instance.ActiveLoan != null)
        {
            if (PlayerManager.Instance.MoneyInBankAccount >= PlayerHUD.Instance.ActiveLoan.ActualAmount)
            {
                OpenPayBackLoanMenu();
            }
            else
            {
                Debug.Log("Not enough money to pay back loan");
                _notEnoughMoneyToPayBackLoanDialogue.TriggerDialogue();
            }
        }
        else
        {
            Debug.Log("Opening take loan menu");
            OpenTakeLoanMenu();
        }

        DialogueManager.Instance.OnYesClicked -= OpenTakeLoanMenu;
    }

    void InitializeLoans()
    {
        // sort loans by amount
        _loansToDisplay = _loansToDisplay.OrderBy(loan => loan.loanAmount).ToArray();
        // set loan stats for each item in the grid
        var gridItems = _loansGrid.GetComponentsInChildren<Button>();
        for (int i = 0; i < gridItems.Length; i++)
        {
            var loan = _loansToDisplay[i];
            gridItems[i].transform.Find("LoanAmount").GetComponent<TMP_Text>().text = loan.loanAmount + "€";
            gridItems[i].transform.Find("InterestRate").GetComponent<TMP_Text>().text = loan.interestRate + "%";
            string dayString = loan.daysToRepay > 1 ? " days" : " day";
            gridItems[i].transform.Find("DaysToRepay").GetComponent<TMP_Text>().text = loan.daysToRepay + dayString;
            gridItems[i].onClick.AddListener(() => TakeOutLoan(loan));
        }
    }

    void TakeOutLoan(Loan loan)
    {
        PlayerManager.Instance.MoneyInBankAccount += loan.loanAmount;
        PlayerHUD.Instance.EnableActiveLoanPanel(loan);
        CloseTakeLoanMenu();
    }

    public void PayBackLoan()
    {
        PlayerManager.Instance.MoneyInBankAccount -= PlayerHUD.Instance.ActiveLoan.ActualAmount;
        PlayerHUD.Instance.DisableActiveLoanPanel();
        ClosePayBackLoanMenu();
    }

    public void OpenTakeLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(UIAnimations.MoveObject(_takeLoanMenu.GetComponent<RectTransform>(), _panelsOnScreenPos, _panelMoveDuration));
        PlayerManager.Instance.DisableInputs();
        GameTimer.Instance.IsPaused = true;
    }

    public void OpenPayBackLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(UIAnimations.MoveObject(_payBackLoanMenu.GetComponent<RectTransform>(), _panelsOnScreenPos, _panelMoveDuration));
        PlayerManager.Instance.DisableInputs();
        GameTimer.Instance.IsPaused = true;
    }

    public void CloseTakeLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(UIAnimations.MoveObject(_takeLoanMenu.GetComponent<RectTransform>(), _panelsOffScreenPos, _panelMoveDuration));
        PlayerManager.Instance.EnableInputs();
        GameTimer.Instance.IsPaused = false;
    }
    public void ClosePayBackLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(UIAnimations.MoveObject(_payBackLoanMenu.GetComponent<RectTransform>(), _panelsOffScreenPos, _panelMoveDuration));
        PlayerManager.Instance.EnableInputs();
        GameTimer.Instance.IsPaused = false;
    }

    void OnDestroy()
    {
        DialogueManager.Instance.OnYesClicked -= OpenTakeLoanMenu;
    }
}
