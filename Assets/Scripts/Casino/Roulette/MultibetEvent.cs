using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class MultibetEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            ProcessBetNames(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ProcessBetNames(false);
        }

        void ProcessBetNames(bool isSelecting)
        {
            if (gameObject.name.Contains(" and ")) // If the bet is a multibet
            {
                string[] betNames = gameObject.name.Split(" and ");
                foreach (string betName in betNames)
                {
                    ChangeBetColor(betName, isSelecting);
                }
            }
        }

        void ChangeBetColor(string betName, bool isSelecting)
        {
            GameObject bet = GameObject.Find(betName);
            var image = bet.GetComponentInChildren<Button>().GetComponent<Image>();
            float adjustment = isSelecting ? -42.5f / 255f : 42.5f / 255f;
            if (image.color.r == image.color.g && image.color.g == image.color.b)
            {
                adjustment *= 0.625f; // If the color is grayscale, adjust the color less since the impact is more noticeable
            }

            float newRed = Mathf.Clamp(image.color.r + adjustment, 0, 1);
            float newGreen = Mathf.Clamp(image.color.g + adjustment, 0, 1);
            float newBlue = Mathf.Clamp(image.color.b + adjustment, 0, 1);

            image.color = new Color(newRed, newGreen, newBlue);
        }
    }
}
