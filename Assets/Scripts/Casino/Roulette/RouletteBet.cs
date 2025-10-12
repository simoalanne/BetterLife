using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Casino.Roulette
{
    [RequireComponent(typeof(BetKeyStorer))]
    public class RouletteBet : MonoBehaviour
    {
        [SerializeField] private GameObject chipPrefab;
        [SerializeField] private float horizontalOffsetRandomness = 5f;
        private IRouletteBetKey _betKey;

        private void Awake()
        {
            _betKey = GetComponent<BetKeyStorer>().BetKey;
        }

        public void OnClick()
        {
            var currentBet = Services.Get<BetSizeManager>().CurrentBetSize;
            Services.Get<RouletteMoneyHandler>().PlaceBet(currentBet, _betKey);
        }

        public void PlaceChips(List<Sprite> chipSprites)
        {
            // Destroy any existing chips first
            transform.Cast<Transform>()
                .Where(child => child.name is "Chip")
                .ToList()
                .ForEach(child => Destroy(child.gameObject));

            // Place new chips
            chipSprites
                .Select((sprite, index) => (sprite, offset: new Vector2(
                    Random.Range(-horizontalOffsetRandomness, horizontalOffsetRandomness),
                    2f * index)))
                .ToList()
                .ForEach(pair =>
                {
                    var chip = Instantiate(chipPrefab, transform.position, Quaternion.identity, transform);
                    chip.GetComponent<RectTransform>().anchoredPosition = pair.offset;
                    chip.name = "Chip";
                    chip.GetComponent<Image>().sprite = pair.sprite;
                });
        }
    }
}
