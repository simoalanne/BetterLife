using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Types
{
    [Serializable]
    public struct RandomInRange
    {
        [SerializeField] private float min;
        [SerializeField] private float max;
        private float? _cachedValue;

        public RandomInRange(float min, float max)
        {
            this.min = min;
            this.max = max;
            _cachedValue = null;
        }

        private float GetValue(bool useCached = false)
        {
            var randomValue = Random.Range(min, max);
            if (useCached)
            {
                _cachedValue ??= randomValue;
                return _cachedValue.Value;
            }

            _cachedValue = null;
            return randomValue;
        }

        public float CachedValue => GetValue(true);

        public void Reset() => _cachedValue = GetValue();

        public static implicit operator float(RandomInRange range) => range.GetValue();
    }
}
