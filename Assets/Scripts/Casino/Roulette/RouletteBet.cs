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
            _betPlaced = true; // Set the bet placed to true when the chip is placed
            /* TODO: Add chips to the bet type. Also an ability to be able split the chips in between the bet types when clicking between them 
            and also a option to remove the chips from the bet type. */
            if (_rouletteBetHandler.PlaceBet(transform.parent.name, _betSizeManager.CurrentBetSize)) // Place the bet and instantiate the chip if bet is accepted
            {
                Instantiate(_chipPrefab, transform.position, Quaternion.identity, transform).
                GetComponent<Image>().sprite = _betSizeManager.ChipSprites[_betSizeManager.CurrentChipIndex]; // Instantiate the chip and set the sprite to the active chip sprite
            }
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
