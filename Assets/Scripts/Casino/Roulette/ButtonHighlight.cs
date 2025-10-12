using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class ButtonHighlight : MonoBehaviour
    {
        private record HighlightData(Image Image, Color OriginalColor);
        
        [SerializeField] private Color highlightColor = Color.yellow;
        
        private Dictionary<int, HighlightData> _highlightables = new();

        private void Awake()
        {
            Services.Register(this);
            var keyStorers = FindObjectsByType<BetKeyStorer>(FindObjectsSortMode.None);
            _highlightables = keyStorers
                .Where(ks => ks.BetKey is InsideBetKey { Numbers: { Count: 1 } })
                .ToDictionary(
                    ks => ((InsideBetKey)ks.BetKey).Numbers.First(),
                    ks =>
                    {
                        var image = ks.GetComponent<Image>();
                        return new HighlightData(image, image.color);
                    }
                );
        }

        public void HighlightNumbers(HashSet<int> numbers)
        {
            foreach (var (number, data) in _highlightables)
            {
                data.Image.color = numbers.Contains(number)
                    ? highlightColor
                    : data.OriginalColor;
            }
        }
    }
}
