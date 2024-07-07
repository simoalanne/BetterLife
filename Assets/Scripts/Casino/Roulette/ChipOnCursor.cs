using Casino;
using UnityEngine;
using UnityEngine.UI;

namespace Roulette
{
    public class ChipOnCursor : MonoBehaviour
    {
        [SerializeField] private Image _chipImage;
        [SerializeField] private BetSizeManager _betSizeManager;

        private Canvas canvas; // Canvas variable to hold the reference

        private void Awake()
        {
            // Get the Canvas component from the GameObject or its parents
            canvas = GetComponentInParent<Canvas>();
            _chipImage.raycastTarget = false; // Disable raycast target for the chip image
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _chipImage.sprite = _betSizeManager.ChipSprites[_betSizeManager.CurrentChipIndex];
                _chipImage.enabled = true;

                // Convert screen position to local position within the canvas
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localPoint);
                _chipImage.rectTransform.anchoredPosition = localPoint;
            }
            else
            {
                _chipImage.enabled = false;
            }
        }
    }
}