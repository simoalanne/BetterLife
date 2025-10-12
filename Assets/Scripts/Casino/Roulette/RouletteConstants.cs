#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Casino.Roulette
{
    public enum InsideBet
    {
        StraightUp,
        Split,
        Street,
        Corner,
        SixLine,
    }

    [Serializable]
    public enum OutsideBet
    {
        None,
        Red,
        Black,
        Odd,
        Even,
        OneToEighteen,
        NineteenToThirtySix,
        FirstDozen,
        SecondDozen,
        ThirdDozen,
        TopColumn,
        MiddleColumn,
        BottomColumn,
    }

    public static class RouletteConstants
    {
        public static readonly HashSet<int> NumbersInWheel = new()
        {
            0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30,
            8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29,
            7, 28, 12, 35, 3, 26
        };
        
        /// <summary> Returns the winning number and its two adjacent numbers on the wheel. </summary>
        public static List<int> GetWinningNumberStrip(int winningNumber)
        {
            var numbersList = NumbersInWheel.ToList();
            var winningIndex = numbersList.IndexOf(winningNumber);
            var leftIndex = (winningIndex - 1 + numbersList.Count) % numbersList.Count;
            var rightIndex = (winningIndex + 1) % numbersList.Count;
            return new List<int>
            {
                numbersList[leftIndex],
                winningNumber,
                numbersList[rightIndex]
            };
        }

        public static readonly Dictionary<InsideBet, int> InsideBetsDict = new()
        {
            { InsideBet.StraightUp, 35 },
            { InsideBet.Split, 17 },
            { InsideBet.Street, 11 },
            { InsideBet.Corner, 8 },
            { InsideBet.SixLine, 5 },
        };

        public static int RandomRouletteNumber =>
            NumbersInWheel.ElementAt(UnityEngine.Random.Range(0, NumbersInWheel.Count));

        public static readonly Dictionary<OutsideBet, OutsideBetInfo> OutsideBetsDict = new()
        {
            {
                OutsideBet.Red, new OutsideBetInfo(1, new HashSet<int>
                    { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 })
            },
            {
                OutsideBet.Black, new OutsideBetInfo(1, new HashSet<int>
                    { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 })
            },
            { OutsideBet.Odd, new OutsideBetInfo(1, Enumerable.Range(1, 36).Where(n => n % 2 != 0).ToHashSet()) },
            { OutsideBet.Even, new OutsideBetInfo(1, Enumerable.Range(1, 36).Where(n => n % 2 == 0).ToHashSet()) },
            { OutsideBet.OneToEighteen, new OutsideBetInfo(1, Enumerable.Range(1, 18).ToHashSet()) },
            { OutsideBet.NineteenToThirtySix, new OutsideBetInfo(1, Enumerable.Range(19, 18).ToHashSet()) },
            { OutsideBet.FirstDozen, new OutsideBetInfo(2, Enumerable.Range(1, 12).ToHashSet()) },
            { OutsideBet.SecondDozen, new OutsideBetInfo(2, Enumerable.Range(13, 12).ToHashSet()) },
            { OutsideBet.ThirdDozen, new OutsideBetInfo(2, Enumerable.Range(25, 12).ToHashSet()) },
            {
                OutsideBet.TopColumn,
                new OutsideBetInfo(2, Enumerable.Range(1, 36).Where(n => n % 3 == 0).ToHashSet())
            },
            {
                OutsideBet.MiddleColumn,
                new OutsideBetInfo(2, Enumerable.Range(1, 36).Where(n => n % 3 == 2).ToHashSet())
            },
            {
                OutsideBet.BottomColumn,
                new OutsideBetInfo(2, Enumerable.Range(1, 36).Where(n => n % 3 == 1).ToHashSet())
            },
        };

        public static readonly Dictionary<int, InsideBet> InsideCountToBet = new()
        {
            { 1, InsideBet.StraightUp },
            { 2, InsideBet.Split },
            { 3, InsideBet.Street },
            { 4, InsideBet.Corner },
            { 6, InsideBet.SixLine }
        };
        
        public record OutsideBetInfo(int Payout, HashSet<int> Numbers);
    }
}
