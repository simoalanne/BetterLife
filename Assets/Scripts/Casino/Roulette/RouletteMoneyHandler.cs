using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Casino.Roulette
{
    public interface IRouletteBetKey
    {
    }

    [Serializable]
    public record InsideBetKey(HashSet<int> Numbers) : IRouletteBetKey;

    [Serializable]
    public record OutsideBetKey(OutsideBet BetCategory) : IRouletteBetKey;

    public record ResolvedRouletteResult(List<int> WinningNumsStrip, float Winnings);

    public class RouletteMoneyHandler : CasinoMoneyHandler<IRouletteBetKey, int, ResolvedRouletteResult>
    {
        [SerializeField] private UnityEvent onChipsChanged;

        private void OnEnable() => Services.RouletteSpinner.OnRoundComplete += num => CreditWinnings(num);

        protected override float CalculateWinnings(int winningNumber) =>
            CurrentBets.Sum(bet => GetBetWinnings(winningNumber, bet.Amount, bet.Key));

        protected override ResolvedRouletteResult ResolveGameResult(int winningNumber, float winnings)
        {
            var strip = RouletteConstants.GetWinningNumberStrip(winningNumber);
            return new ResolvedRouletteResult(strip, winnings);
        }

        private static float GetBetWinnings(int winningNumber, float betAmount, IRouletteBetKey key)
            => key switch
            {
                InsideBetKey insideBetKey => GetInsideBetWinnings(winningNumber, betAmount, insideBetKey),
                OutsideBetKey outsideBetKey => GetOutsideBetWinnings(winningNumber, betAmount, outsideBetKey),
                _ => throw new Exception("Invalid bet key type."),
            };

        private static float GetInsideBetWinnings(int winningNumber, float betAmount, InsideBetKey key)
        {
            var payout = RouletteConstants.InsideBetsDict[RouletteConstants.InsideCountToBet[key.Numbers.Count]];
            return key.Numbers.Contains(winningNumber) ? betAmount * (payout + 1) : 0;
        }

        private static float GetOutsideBetWinnings(int winningNumber, float betAmount, OutsideBetKey key)
        {
            var (payout, affectedNumbers) = RouletteConstants.OutsideBetsDict[key.BetCategory];
            return affectedNumbers.Contains(winningNumber) ? betAmount * (payout + 1) : 0;
        }

        protected override void UndoBetSideEffect(Bet<IRouletteBetKey> undoneBet)
        {
            var betSizeManager = Services.Get<BetSizeManager>();
            var totalBetByKey = GetTotalBetByKey(undoneBet.Key);
            var sprites = betSizeManager.GetTotalBetAsChipSprites(totalBetByKey);
            BetKeyStorer.TryGetComponent<RouletteBet>(undoneBet.Key)?.PlaceChips(sprites);
            onChipsChanged.Invoke();
        }

        protected override void PlaceBetSideEffect()
        {
            var lastBet = CurrentBets.Last();
            UndoBetSideEffect(lastBet);
        }

        protected override void CreditWinningsSideEffect(float amount)
        {
            // clear all chips from table
            PreviousRoundBets.Select(b => b.Key).Distinct().ToList().ForEach(b =>
                BetKeyStorer.TryGetComponent<RouletteBet>(b)?.PlaceChips(new List<Sprite>()));
        }
    }
}
