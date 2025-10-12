using System;
using System.Collections;
using UI;
using Helpers;
using TMPro;
using UnityEngine;

namespace Casino.BlackJack
{
    public class BlackjackUIManager : MonoBehaviour
    {
        [Header("Before Round Elements")]
        [SerializeField] private HideableElement beforeRoundPanel;

        [Header("In Round Elements")]
        [SerializeField] private HideableElement inRoundPanel;
        [SerializeField] private HideableElement surrenderButton;
        [SerializeField] private HideableElement doubleDownButton;
        [SerializeField] private HideableElement splitButton;
        [SerializeField] private HideableElement insuranceOfferPanel;

        [Header("Post Round Elements")]
        [SerializeField] private HideableElement postRoundPanel;
        [SerializeField] private HideableElement repeatBetButton;
        [SerializeField] private HideableElement doubleBetButton;

        [Header("Winnings")]
        [SerializeField] private TMP_Text winningsText;
        [SerializeField] private string displayedWinningsText = "You win <color=#F2D271>{amount} €</color>!";

        private void Start()
        {
            beforeRoundPanel.InitialVisibility(true);
            inRoundPanel.InitialVisibility(false);
            postRoundPanel.InitialVisibility(false);
            insuranceOfferPanel.InitialVisibility(false);
            var manager = Services.BlackjackManager;
            manager.OnInRoundActionsUpdated += HandleInRoundUpdates;
            manager.OnPostRoundActionsUpdated += HandlePostRoundUpdates;
            Services.BlackjackMoneyHandler.OnWinningsCredited += HandleWinningsCredited;
        }

        private void HandleInRoundUpdates(AllowedInRoundActions actions)
        {
            if (actions == null)
            {
                inRoundPanel.Toggle(false);
                insuranceOfferPanel.Toggle(false);
                return;
            }

            if (actions.CanPlaceInsurance)
            {
                insuranceOfferPanel.Toggle(true);
                inRoundPanel.Toggle(false);
                return;
            }

            inRoundPanel.Toggle(true);
            surrenderButton.Toggle(actions.CanSurrender);
            doubleDownButton.Toggle(actions.CanDoubleDown);
            splitButton.Toggle(actions.CanSplit);
            insuranceOfferPanel.Toggle(false);
        }

        private void HandlePostRoundUpdates(AllowedPostRoundActions actions)
        {
            if (actions == null)
            {
                beforeRoundPanel.Toggle(true);
                return;
            }

            postRoundPanel.Toggle(true);
            repeatBetButton.Toggle(actions.CanRepeatBet);
            doubleBetButton.Toggle(actions.CanDoubleBet);
        }

        public void DealClicked()
        {
            if (Services.BlackjackMoneyHandler.CurrentTotalBet <= 0) return;
            beforeRoundPanel.Toggle(false);
            Services.BlackjackManager.StartRound();
        }

        public void BetClicked()
        {
            Services.BlackjackMoneyHandler.PlaceBet(Services.BetSizeManager.CurrentBetSize,
                new HandBet(0, IsDoubleDown: false));
        }

        public void UndoBetClicked()
        {
            var moneyHandler = Services.Get<BlackjackMoneyHandler>();
            moneyHandler.UndoLatestBet();
        }

        public void StandClicked() => Services.BlackjackManager.Stand();

        public void HitClicked() => Services.BlackjackManager.Hit();

        public void DoubleDownClicked() => StartCoroutine(Services.BlackjackManager.HitAndStand());

        public void SplitClicked() => StartCoroutine(Services.BlackjackManager.Split());

        public void MakeInsuranceDecision(bool accept) => Services.BlackjackManager.InsuranceAccepted(accept);

        public void SurrenderClicked() => Services.BlackjackManager.Surrender();

        public void RepeatLastRoundBetClicked()
        {
            postRoundPanel.Toggle(false);
            Services.BlackjackMoneyHandler.RepeatPreviousRoundBet();
            DealClicked();
        }

        public void DoubleLastRoundBetClicked()
        {
            postRoundPanel.Toggle(false);
            Services.BlackjackMoneyHandler.DoublePreviousRoundBet();
            DealClicked();
        }

        public void ContinueClicked()
        {
            postRoundPanel.Toggle(false);
            beforeRoundPanel.Toggle(true);
        }

        private void HandleWinningsCredited(float amount) => StartCoroutine(AnimateWinnings(amount));

        private IEnumerator AnimateWinnings(float amount)
        {
            winningsText.gameObject.SetActive(true);
            yield return FunctionLibrary.DoOverTime(2f,
                progress => winningsText.text = displayedWinningsText.Interpolate(Mathf.RoundToInt(amount * progress)));
            winningsText.text = displayedWinningsText.Interpolate(amount);
            yield return new WaitForSeconds(2f);
            winningsText.gameObject.SetActive(false);
        }
    }
}
