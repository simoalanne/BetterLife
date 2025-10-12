using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Casino.BlackJack
{
    public class ChipPlacer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer chipPrefab;
        [SerializeField] private Vector2 gapBetweenChips = new(0.1f, 0.1f);
        [SerializeField] private int maxChipsPerColumn = 15;
        [SerializeField] private UnityEvent onChipPlaced;
        
        private readonly List<GameObject> _placedChips = new();

        private void Awake()
        {
            Services.Register(this);
        }

        public void PlaceChips(List<Sprite> chipSprites)
        {
            ClearChips();
            var startingPosition = transform.position;
            var i = 0;
            chipSprites.ForEach(chipSprite =>
            {
                var chip = Instantiate(chipPrefab, transform);
                _placedChips.Add(chip.gameObject);
                chip.sprite = chipSprite;
                var column = i / maxChipsPerColumn;
                var row = i % maxChipsPerColumn;
                chip.transform.position = new Vector2(
                    startingPosition.x + column * gapBetweenChips.x,
                    startingPosition.y + row * gapBetweenChips.y);
                i++;
            });
            onChipPlaced?.Invoke();
        }

        private void ClearChips()
        {
            _placedChips.ForEach(Destroy);
            _placedChips.Clear();
        }
    }
}
