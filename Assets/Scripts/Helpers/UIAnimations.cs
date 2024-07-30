using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.Extensions
{
    /// <summary>
    /// A collection of common UI animations.
    /// Does not handle checking if the UI animations are already running, 
    /// so it's up to the caller to handle that.
    /// </summary>
    public static class UIAnimations
    {
        /// <summary>
        /// Flips a UI element horizontally by rotating it 90 degrees and changing its sprite.
        /// </summary>   
        public static IEnumerator FlipObjectHorizontally(RectTransform rectTransform, Sprite newSprite, float flipDuration = 0.1f)
        {
            float elapsed = 0f;
            float halfDuration = flipDuration / 2f;
            Image imageComponent = rectTransform.GetComponent<Image>();

            // Rotate to 90 degrees (halfway)
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float angle = Mathf.Lerp(0f, 90f, elapsed / halfDuration);
                rectTransform.localRotation = Quaternion.Euler(0f, angle, 0f);
                yield return null;
            }

            // Set the rotation to exactly 90 degrees
            rectTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);

            // Change the sprite to the new sprite
            if (imageComponent != null)
            {
                imageComponent.sprite = newSprite;
            }

            // Continue the rotation from 90 degrees to 180 degrees
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float angle = Mathf.Lerp(90f, 180f, elapsed / halfDuration);
                rectTransform.localRotation = Quaternion.Euler(0f, angle, 0f);

                // Flip the scale on X-axis to prevent flipped text appearance
                if (angle >= 90f)
                {
                    rectTransform.localScale = new Vector3(-1f, 1f, 1f);
                }

                yield return null;
            }

            // Reset the rotation to 0 degrees and scale to normal for the next flip
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        /// <summary>
        /// Fades in a UI element by changing its canvas group alpha value.
        /// </summary>
        public static IEnumerator FadeInObject(CanvasGroup canvasGroup, float targetAlpha, float fadeDuration)
        {
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = elapsed / fadeDuration;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        /// <summary>
        /// Fades out a UI element by changing its canvas group alpha value.
        /// </summary>
        public static IEnumerator FadeOutObject(CanvasGroup canvasGroup, float fadeDuration, float exponentialMultiplier = 1f)
        {
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = 1f - Mathf.Pow(elapsed / fadeDuration, exponentialMultiplier);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// Moves a UI element from its current position to the specified end position.
        /// </summary>
        public static IEnumerator MoveObject(RectTransform rectTransform, Vector2 endPos, float moveDuration = 0.1f)
        {
            Debug.Log("Moving object");
            Vector2 startPos = rectTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / moveDuration;
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
                yield return null;
            }
            rectTransform.anchoredPosition = endPos;
        }

        public static IEnumerator ScaleFromZeroToOriginal(RectTransform rectTransform, float scaleDuration)
        {
            rectTransform.gameObject.SetActive(false); // Disable the object so it doesnt pop up at its original scale
            float elapsed = 0f;
            Vector3 originalScale = rectTransform.localScale;
            rectTransform.localScale = Vector3.zero;
            rectTransform.gameObject.SetActive(true); // Now it's safe to set the object active

            while (elapsed < scaleDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / scaleDuration;
                rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, progress);
                yield return null;
            }
            rectTransform.localScale = originalScale;
        }

        public static IEnumerator ScaleFromOriginalToZero(RectTransform rectTransform, float scaleDuration)
        {
            float elapsed = 0f;
            Vector3 originalScale = rectTransform.localScale;

            while (elapsed < scaleDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / scaleDuration;
                rectTransform.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);
                yield return null;
            }
            rectTransform.localScale = Vector3.zero;
        }

        public static IEnumerator TypeMessage(TMP_Text text, string message, float typeSpeed = 0.02f)
        {
            text.maxVisibleCharacters = 0;
            text.text = message;

            while (text.maxVisibleCharacters < text.text.Length)
            {
                text.maxVisibleCharacters++;
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        public static IEnumerator ObjectPopup(GameObject obj, float timeVisible)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(timeVisible);
            obj.SetActive(false);
        }

        public static IEnumerator ScaleToZeroAndDestroy(RectTransform rectTransform, float scaleDuration)
        {
            yield return ScaleFromOriginalToZero(rectTransform, scaleDuration);
            Object.Destroy(rectTransform.gameObject);
        }

        public static IEnumerator IncreaseValue(TMP_Text valueText, string followedText, float startValue, float endValue, float duration = 1f)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                valueText.text = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, progress)).ToString() + followedText;
                yield return null;
            }
            valueText.text = endValue + followedText;
        }
    }
}
