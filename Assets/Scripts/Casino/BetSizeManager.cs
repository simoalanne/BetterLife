using UnityEngine;
using UnityEngine.UI;

namespace Casino
{
    public class BetSizeManager : MonoBehaviour
    {
        [SerializeField] private float[] _betSizes = { 1, 5, 10, 20, 25, 50, 100, 250, 500, 1000, 2500, 5000 };
        [SerializeField] private Sprite[] _chipSprites; // Array to store the chip sprites. Stored in the order of increasing value.
        [SerializeField] private Image _activeChipImage; // Image to display the active chip.
        private int _currentChipIndex;
        private int _maxIndex;
        [SerializeField] private Button _increaseBetSizeButton; // Button to increase the bet size.
        [SerializeField] private Button _decreaseBetSizeButton; // Button to decrease the bet size.
        private float _currentBetSize;
        public float CurrentBetSize => _currentBetSize;
        public Sprite[] ChipSprites => _chipSprites;
        public int CurrentChipIndex => _currentChipIndex;
        public float[] BetSizes => _betSizes;


        void Awake()
        {
            _currentChipIndex = 0;
            _decreaseBetSizeButton.interactable = false; // Disable the decrease bet size button when the bet size is at the minimum.
            _maxIndex = _chipSprites.Length - 1;
            _currentBetSize = _betSizes[_currentChipIndex];
            _activeChipImage.sprite = _chipSprites[_currentChipIndex];
        }
        public void IncreaseBetSize()
        {
            if (_currentChipIndex < _maxIndex) // disable the increase bet size button when the bet size will be at the maximum
            {
                _currentChipIndex++;
                _activeChipImage.sprite = _chipSprites[_currentChipIndex];
                _currentBetSize = _betSizes[_currentChipIndex];

                if (_currentChipIndex == _maxIndex)
                {
                    _increaseBetSizeButton.interactable = false;
                }
            }

            _decreaseBetSizeButton.interactable = true;
        }

        public void DecreaseBetSize()
        {
            if (_currentChipIndex > 0) // disable the decrease bet size button when the bet size will be at the minimum.
            {
                _currentChipIndex--;
                _activeChipImage.sprite = _chipSprites[_currentChipIndex];
                _currentBetSize = _betSizes[_currentChipIndex];

                if (_currentChipIndex == 0)
                {
                    _decreaseBetSizeButton.interactable = false;
                }
            }

            _increaseBetSizeButton.interactable = true;
        }
    }
}
