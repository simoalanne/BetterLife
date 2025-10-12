using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class RouletteStripElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image image;

        public void Init(string number, Color color)
        {
            text.text = number;
            image.color = color;
        }
    }
}
