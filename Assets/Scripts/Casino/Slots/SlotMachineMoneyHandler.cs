using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace Casino.Slots
{
    public class SlotMachineMoneyHandler : CasinoMoneyHandler<string, List<SlotSymbol>, float>
    {
        [SerializedDictionary("Slot Symbol", "Multipliers")]
        [SerializeField] public SerializedDictionary<SlotSymbol, SlotMultipliers> symbolMultipliers = new()
        {
            { SlotSymbol.Strawberry, new SlotMultipliers(2, 4) },
            { SlotSymbol.Plum, new SlotMultipliers(3, 6) },
            { SlotSymbol.Pineapple, new SlotMultipliers(4, 8) },
            { SlotSymbol.Cherry, new SlotMultipliers(5, 10) },
            { SlotSymbol.Orange, new SlotMultipliers(6, 12) },
            { SlotSymbol.Melon, new SlotMultipliers(7, 14) },
            { SlotSymbol.Lemon, new SlotMultipliers(8, 16) },
            { SlotSymbol.Grapes, new SlotMultipliers(10, 20) },
            { SlotSymbol.Seven, new SlotMultipliers(25, 50) }
        };

        [SerializeField] private UnityEvent onWinningsCredited;

        protected override float CalculateWinnings(List<SlotSymbol> symbols)
        {
            var winningSymbol = symbols.GroupBy(s => s)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
            var winnings = symbols.Distinct().Count() switch
            {
                1 => symbolMultipliers[winningSymbol].threeOfAKindMultiplier * CurrentTotalBet,
                2 => symbolMultipliers[winningSymbol].twoOfAKindMultiplier * CurrentTotalBet,
                _ => 0
            };
            if (winnings > 0) onWinningsCredited?.Invoke();
            return winnings;
        }

        protected override float ResolveGameResult(List<SlotSymbol> _, float winnings) => winnings;
    }
}
