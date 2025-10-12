using System.Collections;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private GameObject _activeLoanHUD;
        [SerializeField] private TMP_Text _loanPaybackAmount;
        [SerializeField] private TMP_Text _loanDaysLeft;
        private int
            _firstLoanDay = 1; // The first day of the loan. This is reduced first before reducing the days to repay.
        public Loan ActiveLoan { get; private set; }
        [SerializeField]
        private CanvasGroup
            _hudCanvasGroup; // Canvas group that contains elements that should be hidden when the player is not in the game scene.
        [SerializeField]
        private GameObject
            _casinoClosingText; // this text is shown when player is playing a casino game so they can see when the casino is closing.
        public Canvas Canvas { get; private set; }

        private void Awake()
        {
            Services.Register(this, dontDestroyOnLoad: true);
            Canvas = GetComponent<Canvas>();


            _hudCanvasGroup = GetComponent<CanvasGroup>();

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (next.name == "MainMenu" || next.name == "Roulette" || next.name == "BlackJack" || next.name == "Slots")
            {
                ShowHud(false);
            }
            else if (current.name == "MainMenu" || current.name == "Roulette" || current.name == "BlackJack" ||
                     current.name == "Slots")
            {
                ShowHud(true);
            }
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

        public void AddToCanvas(GameObject obj)
        {
            Instantiate(obj, transform);
        }

        public void DestroyFromCanvas(GameObject obj)
        {
            Debug.Log("Destroying " + obj.name);
            Destroy(obj);
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public void EnableActiveLoanPanel(Loan loan) => StartCoroutine(EnableOrDisableLoanPanel(loan, true));

        public void DisableActiveLoanPanel() => StartCoroutine(EnableOrDisableLoanPanel(null, false));

        IEnumerator EnableOrDisableLoanPanel(Loan loan, bool enable)
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

        public bool ReduceLoanDaysLeft()
        {
            if (ActiveLoan == null) return false;

            bool isGameOver = false;

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
                if (ActiveLoan.daysToRepay == 0)
                {
                    isGameOver = true;
                    _loanDaysLeft.text = "DL missed";
                }
            }

            return isGameOver;
        }
    }
}
