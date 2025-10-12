using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Helpers
{
    /// <summary>
    /// Contains "general purpose" functions that can be used throughout the project.
    /// </summary>
    public static class FunctionLibrary
    {
#if UNITY_EDITOR
        public static List<string> GetAllSceneNames() => EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path))
            .ToList();
#endif

        /// <summary>
        /// Shuffles a list of items in place using the Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        /// <summary> Calls specified action every frame for specified time duration.</summary>
        public static IEnumerator DoOverTime(float time, Action<float> onUpdate,
            Action onComplete = null)
        {
            var elapsed = 0f;
            while (elapsed < time)
            {
                elapsed += Time.unscaledDeltaTime;
                var progress = Mathf.Clamp01(elapsed / time);
                onUpdate?.Invoke(progress);
                yield return null;
            }

            onComplete?.Invoke();
        }

        /// <summary> Waits until the user clicks the left mouse button. </summary>
        public static IEnumerator WaitForMouseClick()
        {
            // Ensure that possible click that triggered this coroutine is ignored
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        private static readonly Regex Placeholder = new(@"\{([^\}]*)\}");

        /// <summary>
        /// Replaces placeholders in the format {0}, {1}, ... with provided values. While Unity's localization package
        /// has similar functionality, its overkill since there won't be localization only templating.
        /// </summary>
        /// <param name="template">String containing placeholders in format {placeholder} or {}.</param>
        /// <param name="replacements">Values to replace placeholders with.</param>
        /// <returns>Interpolated string.</returns>
        public static string Interpolate(this string template, params object[] replacements)
        {
            var index = 0;
            return Placeholder.Replace(template,
                match => index < replacements.Length ? replacements[index++].ToString() : match.Value
            );
        }
    }
}
