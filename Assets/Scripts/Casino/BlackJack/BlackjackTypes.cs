#nullable enable

using System.Collections.Generic;
using UnityEngine;

namespace Casino.BlackJack
{
    public interface ICardValue
    {
        int OrderingPriority { get; }
        int StaticValue { get; }
        int GetDynamicValue(int currentHandValue);
    }

    public record RegularValue(int Value) : ICardValue
    {
        public int OrderingPriority => 0;
        public int StaticValue => Value;
        public int GetDynamicValue(int _) => Value;
    }

    public record AceValue : ICardValue
    {
        public int OrderingPriority => 1;
        public int StaticValue => 11;
        public int GetDynamicValue(int currentHandValue) => currentHandValue + 11 <= 21 ? 11 : 1;
    }

    public record JokerValue : ICardValue
    {
        public int OrderingPriority => 2;
        public int StaticValue => 11;
        public int GetDynamicValue(int currentHandValue) => Mathf.Clamp(21 - currentHandValue, 1, 11);
    }
    
    public record CardData(Sprite Sprite, ICardValue Value, int UniqueRank);

    public record HandResult(int CardCount, int HandValue);

    public record RoundEndResult(HandResult DealerResult, List<HandResult>? PlayerResults);

    // This can be extended later to support other common side bets that are not part of the main game flow
    // e.g. Perfect Pairs, 21+3, etc.
    public enum BlackjackBetType
    {
        Insurance
    }

    public interface IBlackjackBet
    {
    }

    public record HandBet(int HandIndex, bool IsDoubleDown) : IBlackjackBet;

    public record SideBet(BlackjackBetType BetType) : IBlackjackBet;
}
