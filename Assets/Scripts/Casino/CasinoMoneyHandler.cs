using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Casino
{
    public struct MoneyChangedEvent
    {
        public float newBalance;
        public float currentTotalBet;
    }

    public record Bet<T>(T Key, float Amount);

    public interface ICasinoMoneyHandler
    {
        event Action<MoneyChangedEvent> OnMoneyChanged;
        event Action OnFirstBetPlaced;
        event Action OnAllBetsCleared;
        float MaxBetLimit { get; }
    }

    public abstract class CasinoMoneyHandler<TBet, TResult, TResolvedResult> : MonoBehaviour, ICasinoMoneyHandler
    {
        [SerializeField] private float freePlayStartingBalance = 10000f;
        [SerializeField] private bool infiniteMoneyInFreePlay = true;
        [field: SerializeField] public float MaxBetLimit { get; private set; } = 10000f;
        private bool _isInFreePlayMode;
        private float _currentBalance;
        public float CurrentTotalBet => CurrentBets.Sum(b => b.Amount);
        public event Action<MoneyChangedEvent> OnMoneyChanged;
        public event Action<TResolvedResult> OnWinningsCredited;
        public event Action OnBetRejected;
        public event Action OnFirstBetPlaced;
        public event Action OnAllBetsCleared;

        protected List<Bet<TBet>> CurrentBets { get; } = new();
        // Keep track of previous round bets so features like repeat or double down can be implemented easily
        protected List<Bet<TBet>> PreviousRoundBets { get; private set; } = new();


        private void Awake()
        {
            Services.Register(this);
        }

        private void Start()
        {
            var playerManager = Services.PlayerManager;
            _isInFreePlayMode = playerManager.PreviousSceneName is "MainMenu" or null;
            Debug.Log(playerManager.PreviousSceneName);
            _currentBalance = _isInFreePlayMode ? freePlayStartingBalance : playerManager!.MoneyInBankAccount;
            OnMoneyChanged?.Invoke(new MoneyChangedEvent
            {
                newBalance = _currentBalance,
                currentTotalBet = CurrentTotalBet
            });
        }

        public bool PlaceBet(float amount, TBet key = default)
        {
            if (_isInFreePlayMode && infiniteMoneyInFreePlay && amount >= _currentBalance)
            {
                AdjustBalance(freePlayStartingBalance);
            }

            if (amount > _currentBalance || amount <= 0 || CurrentTotalBet + amount > MaxBetLimit)
            {
                OnBetRejected?.Invoke();
                return false;
            }

            if (CurrentBets.Count == 0) OnFirstBetPlaced?.Invoke();

            CurrentBets.Add(new Bet<TBet>(key, amount));
            AdjustBalance(-amount);
            PlaceBetSideEffect();
            return true;
        }

        public void UndoLatestBet(TBet key = default!)
        {
            var targetBet = key == null
                ? CurrentBets.LastOrDefault()
                : CurrentBets.LastOrDefault(b => EqualityComparer<TBet>.Default.Equals(b.Key!, key));
            if (targetBet == null) return;
            CurrentBets.Remove(targetBet);
            AdjustBalance(targetBet.Amount);
            UndoBetSideEffect(targetBet);

            if (CurrentBets.Count == 0) OnAllBetsCleared?.Invoke();
        }

        public TResolvedResult CreditWinnings(TResult gameResult)
        {
            var amount = CalculateWinnings(gameResult);
            PreviousRoundBets = new List<Bet<TBet>>(CurrentBets);
            CurrentBets.Clear();
            OnAllBetsCleared?.Invoke();
            AdjustBalance(amount);
            var resolvedResult = ResolveGameResult(gameResult, amount);
            CreditWinningsSideEffect(amount);
            OnWinningsCredited?.Invoke(resolvedResult);
            return resolvedResult;
        }

        protected abstract TResolvedResult ResolveGameResult(TResult gameResult, float winnings);

        private void AdjustBalance(float balanceChange)
        {
            _currentBalance += balanceChange;
            if (!_isInFreePlayMode) Services.PlayerManager.MoneyInBankAccount = _currentBalance;
            OnMoneyChanged?.Invoke(new MoneyChangedEvent
            {
                newBalance = _currentBalance,
                currentTotalBet = CurrentTotalBet
            });
        }

        protected bool CanAffordBet(float amount)
        {
            var exceedsMaxBetLimit = amount > MaxBetLimit;
            if (exceedsMaxBetLimit) return false;
            if (_isInFreePlayMode && infiniteMoneyInFreePlay) return true;
            return amount <= _currentBalance;
        }

        protected abstract float CalculateWinnings(TResult gameResult);

        protected float GetTotalBetByKey(TBet key, bool forCurrentBets = true)
        {
            var targetBets = forCurrentBets ? CurrentBets : PreviousRoundBets;
            return targetBets.Where(b => EqualityComparer<TBet>.Default.Equals(b.Key!, key)).Sum(b => b.Amount);
        }

        ///<summary>Called after bet is successfully placed and CurrentBets has been updated.</summary>
        protected virtual void PlaceBetSideEffect()
        {
        }

        ///<summary>Called after latest bet is successfully undone and CurrentBets has been updated.</summary>
        protected virtual void UndoBetSideEffect(Bet<TBet> undoneBet)
        {
        }

        ///<summary>
        /// Called after winnings (if any) have been credited to player's balance as well as CurrentBets cleared and
        /// PreviousRoundBets updated.
        /// </summary>
        /// <param name="amount">The amount that was credited as winnings or zero if no winnings.</param>
        protected virtual void CreditWinningsSideEffect(float amount)
        {
        }
    }
}
