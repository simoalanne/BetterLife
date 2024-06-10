using UnityEngine;
using UnityEngine.UI;

namespace Casino
{
    public class BetSizeManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] _chipSprites; // Array to store the chip sprites. Stored in the order of increasing value.
        [SerializeField] private Image _activeChipImage; // Image to display the active chip.
        private int _currentChipIndex;
        private int _maxIndex;
        [SerializeField] private Button _increaseBetSizeButton; // Button to increase the bet size.
        [SerializeField] private Button _decreaseBetSizeButton; // Button to decrease the bet size.
        private float _currentBetSize;
        public float CurrentBetSize => _currentBetSize;
        private readonly float[] _betSizes = { 10, 25, 50, 100 };


        void Awake()
        {
            _currentChipIndex = 0;
            _maxIndex = _chipSprites.Length - 1;
            _currentBetSize = _betSizes[_currentChipIndex];
        }

        public void IncreaseBetSize()
        {
            if (_currentChipIndex < _maxIndex)
            {
                _currentChipIndex++;
                _activeChipImage.sprite = _chipSprites[_currentChipIndex];
                _currentBetSize = _betSizes[_currentChipIndex];
            }
            else
            {
                _increaseBetSizeButton.interactable = false; // Disable the increase bet size button when increasing from max bet size.
            }

            if (_decreaseBetSizeButton.interactable == false)
            {
                _decreaseBetSizeButton.interactable = true; // Enable the decrease bet size button when increasing from min bet size.
            }
       
        }

        public void DecreaseBetSize()
        {
            if (_currentChipIndex > 0)
            {
                _currentChipIndex--;
                _activeChipImage.sprite = _chipSprites[_currentChipIndex];
                _currentBetSize = _betSizes[_currentChipIndex];
            }
            else
            {
                _decreaseBetSizeButton.interactable = false; // Disable the decrease bet size button when decreasing from min bet size.
            }

            if (_increaseBetSizeButton.interactable == false)
            {
                _increaseBetSizeButton.interactable = true; // Enable the increase bet size button when decreasing from max bet size.
            }
        }
    }
}
