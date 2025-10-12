using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Casino.Slots
{
    internal enum ReelState
    {
        Spinning,
        Stopping,
        Stopped
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Reel : MonoBehaviour
    {
        internal ReelState ReelState { get; private set; } = ReelState.Stopped;

        internal SlotSymbol StoppedSlot { get; private set; }

        private float _elapsedTime;

        private Material _mat;

        private void Awake()
        {
            _mat = GetComponent<SpriteRenderer>().material;
        }

        public void StartSpinning(float reelSpinSpeed)
        {
            ReelState = ReelState.Spinning;
            StartCoroutine(Spin(reelSpinSpeed));
        }

        public void StopSpinning()
        {
            ReelState = ReelState.Stopping;
        }

        private IEnumerator Spin(float reelSpinSpeed)
        {
            var symbolsCount = Enum.GetNames(typeof(SlotSymbol)).Length;
            var randomIndex = Random.Range(0, symbolsCount);
            var targetOffset = -1f / symbolsCount * randomIndex;
            const float tolerance = 0.01f;

            while (ReelState == ReelState.Spinning || (ReelState == ReelState.Stopping &&
                                                      Math.Abs(_mat.mainTextureOffset.y - targetOffset) > tolerance))
            {
                var newY = (_mat.mainTextureOffset.y - Time.deltaTime * reelSpinSpeed) % 1;
                _mat.mainTextureOffset = new Vector2(0, newY);
                yield return null;
            }

            _mat.mainTextureOffset = new Vector2(0, targetOffset);
            StoppedSlot = (SlotSymbol)randomIndex;
            ReelState = ReelState.Stopped;
        }
    }
}