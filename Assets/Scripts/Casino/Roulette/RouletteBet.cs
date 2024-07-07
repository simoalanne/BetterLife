using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class RouletteBet : MonoBehaviour
    {
        private RouletteBetHandler _rouletteBetHandler;
        private BetSizeManager _betSizeManager;
        [SerializeField] private GameObject _chipPrefab;
        [SerializeField] private Vector2 _chipTransformOffsetToPrevious = new(0, 2f); // This is used to simulate the effect of stacking chips on top of each other
        [SerializeField] private float _horizontalOffsetRandomness = 2f; // This is used to simulate the effect of stacking chips on top of each other

        private bool _betPlaced = false;
        public bool BetPlaced => _betPlaced; // Informs the RouletteBetHandler if the bet is placed.

        private static List<GameObject> _allPlacedChips = new(); // This is used to keep track of the latest chip placed on the table
        public static List<GameObject> AllPlacedChips => _allPlacedChips; // Informs the RouletteBetHandler of all the placed chips on the table

        void Awake()
        {
            _rouletteBetHandler = FindObjectOfType<RouletteBetHandler>();
            _betSizeManager = FindObjectOfType<BetSizeManager>();
        }

        public void OnClick()
        {
            if (_rouletteBetHandler.PlaceBet(transform.name, _betSizeManager.CurrentBetSize)) // Place the bet and instantiate the chip if bet is accepted
            {
                TryPlaceChip();
            }
        }

        private void TryPlaceChip()
        {
            int chipCount = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Chip")
                {
                    chipCount++;
                }
            }

            _chipTransformOffsetToPrevious =
            new Vector2(Random.Range(-_horizontalOffsetRandomness, _horizontalOffsetRandomness), 2f * chipCount);

            var chip = Instantiate(_chipPrefab, transform.position, Quaternion.identity, transform);
            chip.GetComponent<Image>().sprite = _betSizeManager.ChipSprites[_betSizeManager.CurrentChipIndex]; // Instantiate the chip and set the sprite to the active chip sprite
            chip.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0) + _chipTransformOffsetToPrevious; // Set the position of the chip to the position of the bet + the offset
            chip.name = "Chip"; // Set the name of the chip to "Chip" for easier identification
            _betPlaced = true; // Set the bet placed to true when the chip is placed
            _allPlacedChips.Add(chip); // Add the chip to the list of all placed chips
        }

        public void DestroyChips()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Image>() != null && child.name == "Chip")
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
