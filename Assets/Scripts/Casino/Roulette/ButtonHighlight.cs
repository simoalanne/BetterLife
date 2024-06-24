using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Casino.Roulette
{
    public class ButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const float _colorAdjustment = 0.4f; // Makes the color whiteish when highlighted
        private Dictionary<string, GameObject> _betObjects = new();

        private void Awake()
        {
            // Assuming all bet objects are children of a parent object "Bets"
            foreach (Transform child in GameObject.Find("BettingTable").transform)
            {
                _betObjects.Add(child.name, child.gameObject);
            }
        }

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
            else
            {
                ChangeBetColor(gameObject.transform.parent.name, isSelecting);
            }
        }

        void ChangeBetColor(string betName, bool isSelecting)
        {
            if (!_betObjects.TryGetValue(betName, out GameObject bet))
            {
                Debug.LogError($"Bet object with name {betName} not found");
                return;
            }

            var image = bet.GetComponentInChildren<Button>().GetComponent<Image>();
            float adjustment = isSelecting ? _colorAdjustment : -_colorAdjustment;
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