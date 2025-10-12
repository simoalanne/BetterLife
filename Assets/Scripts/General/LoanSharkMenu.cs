using System.Linq;
using DialogueSystem;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
    public bool CanInteract { get; set; } = true;

    void Awake()
    {
        InitializeLoans();
    }

    public void Interact()
    {
        Services.DialogueHandler.OnYesClicked += OpenTakeLoanMenu;

        if (Services.PlayerManager.HasTalkedToLoanShark == false)
        {
            Services.PlayerManager.HasTalkedToLoanShark = true;
            _loanSharkFirstTimeDialogue.TriggerDialogue();
            return;
        }
        
        if (Services.PlayerHUD.ActiveLoan != null)
        {
            if (Services.PlayerManager.MoneyInBankAccount >= Services.PlayerHUD.ActiveLoan.ActualAmount)
            {
                OpenPayBackLoanMenu();
            }
            else
            {
                _notEnoughMoneyToPayBackLoanDialogue.TriggerDialogue();
            }
        }
        else
        {
            OpenTakeLoanMenu();
        }

        Services.DialogueHandler.OnYesClicked -= OpenTakeLoanMenu;
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

    public void OpenTakeLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationLibrary.Move(_takeLoanMenu.GetComponent<RectTransform>(), _panelsOnScreenPos, _panelMoveDuration));
        Services.PlayerManager.DisableInputs();
        Services.GameTimer.IsPaused = true;
    }

    public void OpenPayBackLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationLibrary.Move(_payBackLoanMenu.GetComponent<RectTransform>(), _panelsOnScreenPos, _panelMoveDuration));
        Services.PlayerManager.DisableInputs();
        Services.GameTimer.IsPaused = true;
    }

    public void CloseTakeLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationLibrary.Move(_takeLoanMenu.GetComponent<RectTransform>(), _panelsOffScreenPos, _panelMoveDuration));
        Services.PlayerManager.EnableInputs();
        Services.GameTimer.IsPaused = false;
    }
    public void ClosePayBackLoanMenu()
    {
        StopAllCoroutines();
        StartCoroutine(AnimationLibrary.Move(_payBackLoanMenu.GetComponent<RectTransform>(), _panelsOffScreenPos, _panelMoveDuration));
        Services.PlayerManager.EnableInputs();
        Services.GameTimer.IsPaused = false;
    }

    void OnDestroy()
    {
        Services.DialogueHandler.OnYesClicked -= OpenTakeLoanMenu;
    }
}
