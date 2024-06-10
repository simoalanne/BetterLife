using UnityEngine;

namespace Casino.Roulette
{
    public class RouletteBet : MonoBehaviour
    {
        private RouletteBetHandler _rouletteBetHandler;
        private BetSizeManager _betSizeManager;

        void Awake()
        {
            _rouletteBetHandler = FindObjectOfType<RouletteBetHandler>();
            _betSizeManager = FindObjectOfType<BetSizeManager>();
        }

        public void OnClick()
        {
            /* TODO: Add chips to the bet type. Also an ability to be able split the chips in between the bet types when clicking between them 
            and also a option to remove the chips from the bet type. */
            _rouletteBetHandler.PlaceBet(transform.parent.name, _betSizeManager.CurrentBetSize);
        }
    }
}
