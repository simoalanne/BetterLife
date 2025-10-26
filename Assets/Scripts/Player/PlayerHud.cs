using System.Collections;
using DialogueSystem;
using Helpers;
using NaughtyAttributes;
using ScriptableObjects;
using TMPro;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private GameObject _activeLoanHUD;
        [SerializeField] private TMP_Text _loanPaybackAmount;
        [SerializeField] private TMP_Text _loanDaysLeft;
        [SerializeField] private Conversation loanDeadlineMissed;
        [SerializeField] private HideableElement screenBeforeGameOver;
        [SerializeField] private float delayBeforeLoadingGameOverScene = 3f;
        [SerializeField, Scene] private string sceneToLoadAfterLoanDlMissed = "GameOverCutscene";
        private int
            _firstLoanDay = 1; // The first day of the loan. This is reduced first before reducing the days to repay.
        public Loan ActiveLoan { get; private set; }
        private CanvasGroup _hudCanvasGroup;

        private void Awake()
        {
            Services.Register(this, persistent: true);
            _hudCanvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            Services.GameTimer.OnDayPassed += ReduceLoanDaysLeft;
        }

        public void ShowHud(bool show)
        {
            if (show)
            {
                _hudCanvasGroup.alpha = 1;
                _hudCanvasGroup.blocksRaycasts = true;
            }
            else
            {
                _hudCanvasGroup.alpha = 0;
                _hudCanvasGroup.blocksRaycasts = false;
            }
        }
        
        public void ResetHUD()
        {
            screenBeforeGameOver.Toggle(false, instant: true);
            ActiveLoan = null;
            _loanPaybackAmount.text = "";
            _loanDaysLeft.text = "";
            var rect = _activeLoanHUD.GetComponent<RectTransform>();
            var pos = rect.anchoredPosition;
            var endPos = new Vector2(rect.sizeDelta.x, pos.y);
            rect.anchoredPosition = endPos;
        }

        public void EnableActiveLoanPanel(Loan loan) => StartCoroutine(EnableOrDisableLoanPanel(loan, true));

        public void DisableActiveLoanPanel() => StartCoroutine(EnableOrDisableLoanPanel(null, false));

        private IEnumerator EnableOrDisableLoanPanel(Loan loan, bool enable)
        {
            var rect = _activeLoanHUD.GetComponent<RectTransform>();
            var pos = rect.anchoredPosition;
            _loanPaybackAmount.text = "";
            _loanDaysLeft.text = "";

            if (enable)
            {
                _firstLoanDay = 1;
                var endPos = new Vector2(-25, pos.y);
                _loanPaybackAmount.text = $"-{loan.ActualAmount} €";
                Debug.Log(loan.interestRate / 100);
                _loanDaysLeft.text = loan.daysToRepay == 1 ? $"{loan.daysToRepay} day" : $"{loan.daysToRepay} days";
                yield return StartCoroutine(AnimationLibrary.Move(rect, endPos));
                ActiveLoan = loan;
            }
            else
            {
                ActiveLoan = null;
                var endPos = new Vector2(rect.sizeDelta.x, pos.y);
                yield return StartCoroutine(AnimationLibrary.Move(rect, endPos));
            }
        }

        private void ReduceLoanDaysLeft()
        {
            if (ActiveLoan == null) return;

            if (_firstLoanDay == 1)
            {
                Debug.Log("First loan day, dont reduce days to repay");
                _firstLoanDay--;
            }
            else
            {
                ActiveLoan.daysToRepay--;
                _loanDaysLeft.text = ActiveLoan.daysToRepay == 1
                    ? $"{ActiveLoan.daysToRepay} day"
                    : $"{ActiveLoan.daysToRepay} days";
                if (ActiveLoan.daysToRepay > 0) return;

                _loanDaysLeft.text = "DL missed";
                loanDeadlineMissed.Start(onStateChange: state =>
                {
                    if (state is not DialogueState.DialogueFinished) return;
                    StartCoroutine(PrepareGameOver());
                });
            }
        }
        
        private IEnumerator PrepareGameOver()
        {
            Services.InputManager.EnablePlayerInput(false);
            yield return screenBeforeGameOver.ToggleElement(true, screenBeforeGameOver.Duration);
            yield return new WaitForSeconds(delayBeforeLoadingGameOverScene);
            sceneToLoadAfterLoanDlMissed.LoadScene();
        }
    }
}
