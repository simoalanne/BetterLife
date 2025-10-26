using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Cutscenes
{
    [RequireComponent(typeof(AudioSource))]
    public class GameOverCutscene : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private float initialDelay = 0.5f;
        [SerializeField] private float delayAfterFade = 0.5f;
        [SerializeField] private float fadeDuration = 0.1f;
        [SerializeField] private List<AudioClip> beforeFadeClips = new();
        [SerializeField] private List<AudioClip> afterFadeClips = new();
        [SerializeField] private AudioClip underWaterSound;
        [SerializeField] private Image waterImage;
        [SerializeField] private Image characterImage;
        [SerializeField] private float waterAnimationLength = 7.5f;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private HideableElement mainMenuButton;
        [SerializeField] private HideableElement quitGameButton;

        private void Awake() => StartCoroutine(GameOver());

        private IEnumerator GameOver()
        {
            _audioSource = GetComponent<AudioSource>();
            yield return new WaitForSeconds(initialDelay);
            foreach (var clip in beforeFadeClips)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                yield return new WaitForSeconds(_audioSource.clip.length);
            }

            yield return Services.SceneLoader.TemporaryFade(true, overrideDuration: fadeDuration);
            yield return new WaitForSeconds(delayAfterFade);

            foreach (var clip in afterFadeClips)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                yield return new WaitForSeconds(_audioSource.clip.length);
            }

            _audioSource.clip = underWaterSound;
            _audioSource.loop = true;
            _audioSource.Play();

            var spritesToDisable = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            spritesToDisable.ToList().ForEach(s => s.enabled = false);

            waterImage.gameObject.SetActive(true);
            mainMenuButton.Toggle(false, instant: true);
            quitGameButton.Toggle(false, instant: true);
            particles.Play();

            yield return Services.SceneLoader.TemporaryFade(false);
            StartCoroutine(waterImage.rectTransform.Move(new Vector2(0, 0), waterAnimationLength));
            yield return characterImage.rectTransform.Move(
                new Vector2(0, -waterImage.rectTransform.sizeDelta.y + characterImage.rectTransform.sizeDelta.y),
                waterAnimationLength);
            particles.Stop();
            yield return new WaitForSeconds(particles.main.startLifetime.constantMax);
            mainMenuButton.Toggle(true);
            quitGameButton.Toggle(true);
        }
    }
}
