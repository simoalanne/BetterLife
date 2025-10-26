using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Casino.Roulette
{
    public class BetKeyStorer : MonoBehaviour
    {
        [Serializable]
        private struct SerializedBetKey
        {
            public OutsideBet betCategory;
            public List<int> numbers;

            public SerializedBetKey(OutsideBet betCategory)
            {
                this.betCategory = betCategory;
                numbers = new List<int>();
            }

            public SerializedBetKey(List<int> numbers)
            {
                this.numbers = numbers;
                betCategory = OutsideBet.None;
            }
        }

        [SerializeField] private SerializedBetKey serializedKey;
        public IRouletteBetKey BetKey { get; private set; }
        
        private static readonly Dictionary<IRouletteBetKey, HashSet<MonoBehaviour>> ComponentLookup = new();

        public static T TryGetComponent<T>(IRouletteBetKey key) where T : MonoBehaviour
        {
            return ComponentLookup.TryGetValue(key, out var components)
                ? components.OfType<T>().FirstOrDefault()
                : null;
        }

        private void Awake()
        {
            if (BetKey == null)
            {
                Reset();
            }

            var siblings = GetComponentsInChildren<MonoBehaviour>().ToHashSet();
            ComponentLookup.TryAdd(BetKey, siblings);
        }
        
        // Because the lookup is static it must be cleaned when the scene is unloaded and these objects destroyed
        private void OnDestroy() => ComponentLookup.Remove(BetKey);
        

        private void Reset()
        {
            serializedKey = TryResolveSerializedKey(gameObject.name);
            Debug.Log($"Resolved {gameObject.name} to {serializedKey}");
            BetKey = ResolveBetKey(serializedKey);
        } 

        private void OnValidate()
        {
            // enforce that if any numbers are set then betCategory is None and vice versa
            if (serializedKey.betCategory != OutsideBet.None)
            {
                serializedKey.numbers = new List<int>();
            }

            BetKey = ResolveBetKey(serializedKey);
        }

        private static IRouletteBetKey ResolveBetKey(SerializedBetKey betKey) =>
            betKey.betCategory != OutsideBet.None
                ? new OutsideBetKey(betKey.betCategory)
                : new InsideBetKey(betKey.numbers != null ? betKey.numbers.ToHashSet() : new HashSet<int>());

        private static SerializedBetKey TryResolveSerializedKey(string gameObjectName) =>
            gameObjectName switch
            {
                _ when gameObjectName.Length <= 2 && int.TryParse(gameObjectName, out var num) =>
                    new SerializedBetKey(new List<int> { num }),
                _ when gameObjectName.Contains(" and ") =>
                    new SerializedBetKey(gameObjectName.Split(" and ")
                        .Select(s => int.TryParse(s, out var num) ? num : -1)
                        .Where(n => n != -1).ToList()),
                "Red" => new SerializedBetKey(OutsideBet.Red),
                "Black" => new SerializedBetKey(OutsideBet.Black),
                "Odd" => new SerializedBetKey(OutsideBet.Odd),
                "Even" => new SerializedBetKey(OutsideBet.Even),
                "1 to 18" => new SerializedBetKey(OutsideBet.OneToEighteen),
                "19 to 36" => new SerializedBetKey(OutsideBet.NineteenToThirtySix),
                "1st 12" => new SerializedBetKey(OutsideBet.FirstDozen),
                "2nd 12" => new SerializedBetKey(OutsideBet.SecondDozen),
                "3rd 12" => new SerializedBetKey(OutsideBet.ThirdDozen),
                "Top Column" => new SerializedBetKey(OutsideBet.TopColumn),
                "Middle Column" => new SerializedBetKey(OutsideBet.MiddleColumn),
                "Bottom Column" => new SerializedBetKey(OutsideBet.BottomColumn),
                _ => new SerializedBetKey(OutsideBet.None)
            };
    }
}
