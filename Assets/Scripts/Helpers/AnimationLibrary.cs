using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Helpers
{
    public interface ISpriteComponent
    {
        void SetSprite(Sprite sprite);
    }

    public record RendererSprite(SpriteRenderer Renderer) : ISpriteComponent
    {
        public void SetSprite(Sprite sprite) => Renderer.sprite = sprite;
    }

    public record ImageSprite(Image Image) : ISpriteComponent
    {
        public void SetSprite(Sprite sprite) => Image.sprite = sprite;
    }

    /// <summary>Exposes commonly used animations as coroutines.</summary>
    public static class AnimationLibrary
    {
        public static IEnumerator SpriteFlip(this Transform t, ISpriteComponent spriteHolder, Sprite sprite,
            float duration = 0.1f)
        {
            yield return FunctionLibrary.DoOverTime(duration / 2,
                progress => t.localRotation = Quaternion.Euler(0f, Mathf.Lerp(0f, 90f, progress), 0f));

            t.localRotation = Quaternion.Euler(0f, 90f, 0f);

            spriteHolder.SetSprite(sprite);
            var originalScale = t.localScale;
            t.localScale = new Vector3(-1f, 1f, 1f); // Ensure the sprite won't be mirrored

            yield return FunctionLibrary.DoOverTime(duration / 2,
                progress => t.localRotation = Quaternion.Euler(0f, Mathf.Lerp(90f, 180f, progress), 0f));


            t.localRotation = Quaternion.Euler(0f, 0f, 0f);
            t.localScale = originalScale;
        }

        public static IEnumerator Fade(this CanvasGroup canvasGroup, float alpha, float duration = 0.5f)
        {
            canvasGroup.gameObject.SetActive(true);
            var startAlpha = canvasGroup.alpha;
            yield return FunctionLibrary.DoOverTime(duration, t =>
            {
                var isFadingOut = alpha < startAlpha;
                var progress = isFadingOut ? Mathf.Pow(t, 3) : t;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, alpha, progress);
            });
            canvasGroup.alpha = alpha;
        }


        public static IEnumerator Move(this Transform t, Vector2 endPos, float moveDuration = 0.1f)
        {
            t.Activate();
            var startPos = t.GetPosition();
            yield return FunctionLibrary.DoOverTime(moveDuration,
                progress => t.SetPosition(Vector2.Lerp(startPos, endPos, progress)));

            t.SetPosition(endPos);
        }

        public static IEnumerator Scale(this Transform t, Vector3 to, float duration)
        {
            t.Activate();
            var startScale = t.localScale;
            yield return FunctionLibrary.DoOverTime(duration,
                progress => t.localScale = Vector3.Lerp(startScale, to, progress));
            t.localScale = to;
        }

        public static IEnumerator TypeMessage(this TMP_Text text, string message, float typeSpeed = 0.02f)
        {
            text.maxVisibleCharacters = 0;
            text.text = message;

            while (text.maxVisibleCharacters < text.text.Length)
            {
                text.maxVisibleCharacters++;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }
        }

        private static Vector3 GetPosition(this Transform t) =>
            t is RectTransform rect ? rect.anchoredPosition : t.position;

        private static void SetPosition(this Transform t, Vector2 newPos = default)
        {
            if (t is RectTransform rect) rect.anchoredPosition = newPos;
            else t.position = newPos;
        }

        private static void Activate(this Transform t) => t.gameObject.SetActive(true);
    }
}
