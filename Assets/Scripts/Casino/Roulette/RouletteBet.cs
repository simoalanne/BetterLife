using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class RouletteBet : MonoBehaviour
    {
        private RouletteBetHandler _rouletteBetHandler;
        private BetSizeManager _betSizeManager;
        [SerializeField] private GameObject _chipPrefab;
        private bool _betPlaced = false;
        public bool BetPlaced => _betPlaced; // Informs the RouletteBetHandler if the bet is placed.

        void Awake()
        {
            _rouletteBetHandler = FindObjectOfType<RouletteBetHandler>();
            _betSizeManager = FindObjectOfType<BetSizeManager>();
        }

        public void OnClick()
        {
            /* This is checked because the multibets directly contain the bet name, while the single bets are children of the bet name.
            This is a thing because of hierarchy organization. */
            string betName = transform.parent.name == "BettingTable" ? transform.name : transform.parent.name;

            if (_rouletteBetHandler.PlaceBet(betName, _betSizeManager.CurrentBetSize)) // Place the bet and instantiate the chip if bet is accepted
            {
                TryPlaceChip();
            }
        }

        private void TryPlaceChip()
        {
            var chip = Instantiate(_chipPrefab, transform.position, Quaternion.identity, transform);
            chip.GetComponent<Image>().sprite = _betSizeManager.ChipSprites[_betSizeManager.CurrentChipIndex]; // Instantiate the chip and set the sprite to the active chip sprite
            chip.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0); // Set the anchored position of the chip to the center of the parent
            _betPlaced = true; // Set the bet placed to true when the chip is placed
        }

        public void DestroyChips()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Image>() != null) // if the child is a chip (has a Image component). Assumes that only chips have that
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
