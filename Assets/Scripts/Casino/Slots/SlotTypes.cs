using System;

namespace Casino.Slots
{
    public enum SlotSymbol
    {
        Strawberry,
        Plum,
        Pineapple,
        Cherry,
        Orange,
        Melon,
        Lemon,
        Grapes,
        Seven
    }

    [Serializable]
    public struct SlotMultipliers
    {
        public int twoOfAKindMultiplier;
        public int threeOfAKindMultiplier;

        public SlotMultipliers(int two, int three)
        {
            twoOfAKindMultiplier = two;
            threeOfAKindMultiplier = three;
        }
    }
}
