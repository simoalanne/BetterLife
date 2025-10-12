using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Casino
{
    public class BetSizeManager : MonoBehaviour
    {
        [Serializable]
        private struct BetSizeSpritePair
        {
            public float betSize;
            public Sprite chipSprite;

            public BetSizeSpritePair(float betSize, Sprite chipSprite = null)
            {
                this.betSize = betSize;
                this.chipSprite = chipSprite;
            }
        }

        [SerializeField] private List<BetSizeSpritePair> betSizeSprites = new()
        {
            new BetSizeSpritePair(1),
            new BetSizeSpritePair(2),
            new BetSizeSpritePair(5),
            new BetSizeSpritePair(10),
            new BetSizeSpritePair(25),
            new BetSizeSpritePair(50),
            new BetSizeSpritePair(100),
            new BetSizeSpritePair(250),
            new BetSizeSpritePair(500),
            new BetSizeSpritePair(1000),
            new BetSizeSpritePair(2500),
            new BetSizeSpritePair(5000)
        };

        [SerializeField] private float flipDuration = 0.1f;

        [Header("References")]
        [SerializeField] private Image chipImage;
        [SerializeField] private Button increaseBetSizeButton;
        [SerializeField] private Button decreaseBetSizeButton;

        private int _currentChipIndex;
        public float CurrentBetSize { get; private set; }
        private Sprite CurrentChipSprite => betSizeSprites[_currentChipIndex].chipSprite;
        public float MinBetSize => betSizeSprites.First().betSize;

        public event Action<float> OnBetSizeChanged;

        private void Awake()
        {
            Services.Register(this);
            betSizeSprites.Sort((x, y) => x.betSize.CompareTo(y.betSize));
            _currentChipIndex = (betSizeSprites.Count - 1) / 2;
            var initialPair = betSizeSprites[_currentChipIndex];
            CurrentBetSize = initialPair.betSize;
            chipImage.sprite = initialPair.chipSprite;

            increaseBetSizeButton.onClick.AddListener(IncreaseBetSize);
            decreaseBetSizeButton.onClick.AddListener(DecreaseBetSize);
        }

        private void IncreaseBetSize()
        {
            decreaseBetSizeButton.interactable = true;
            _currentChipIndex++;
            CurrentBetSize = betSizeSprites[_currentChipIndex].betSize;
            FlipChip();

            if (_currentChipIndex == betSizeSprites.Count - 1) increaseBetSizeButton.interactable = false;
            OnBetSizeChanged?.Invoke(CurrentBetSize);
        }

        private void DecreaseBetSize()
        {
            increaseBetSizeButton.interactable = true;
            _currentChipIndex--;
            CurrentBetSize = betSizeSprites[_currentChipIndex].betSize;
            FlipChip();

            if (_currentChipIndex == 0) decreaseBetSizeButton.interactable = false;
            OnBetSizeChanged?.Invoke(CurrentBetSize);
        }

        private void FlipChip()
        {
            StopAllCoroutines();
            StartCoroutine(chipImage.rectTransform.SpriteFlip(new ImageSprite(chipImage), CurrentChipSprite,
                flipDuration));
        }

        /// <summary> Returns a list of chip sprites representing the total bet amount with the least number of chips. </summary>
        public List<Sprite> GetTotalBetAsChipSprites(float totalBet)
        {
            var remainingAmount = totalBet;
            return betSizeSprites
                .OrderByDescending(p => p.betSize)
                .SelectMany(pair =>
                {
                    var count = (int)(remainingAmount / pair.betSize);
                    remainingAmount -= count * pair.betSize;
                    return Enumerable.Repeat(pair.chipSprite, count);
                }).ToList();
        }
    }
}
