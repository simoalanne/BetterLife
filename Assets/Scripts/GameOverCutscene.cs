using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UI.Extensions;

public class GameOverCutscene : MonoBehaviour
{
    [SerializeField] private Animator animator; // Kidnappaus animaattori
    [SerializeField] public CanvasGroup canvasGroup; // Fade mustaan
    [SerializeField] public AudioSource audioSource; // Scenen audiosource
    [SerializeField] public AudioClip[] audioClips; // Kaikki soundeffectit
    [SerializeField] private float _transitionInDuration = 0.5f; // Kauanko fade in
    [SerializeField] private float _transitionOutDuration = 0.25f; // Kauanko fade out
    [SerializeField] private Image waterImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private float _waterAnimationLength = 7.5f;

    private void Start()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        var timer = 0f;
        animator.SetBool("Kidnapped", true); // Soitetaan kidnappaus-animaatio
        audioSource.clip = audioClips[0];
        audioSource.Play(); // Ensimm�inen ��niefekti soitetaan
        yield return new WaitForSeconds(audioSource.clip.length);

        while (timer < _transitionInDuration) // Fade in
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / _transitionInDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;
        animator.SetBool("Kidnapped", false);

        for (int i = 1; i < audioClips.Length - 1; i++) // Soitetaan sound effectit
        {
            audioSource.clip = audioClips[i];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        waterImage.enabled = true;
        characterImage.enabled = true;
        audioSource.clip = audioClips[audioClips.Length - 1];
        audioSource.Play();
        audioSource.loop = true;
        timer = 0f;
        while (timer < _transitionOutDuration) // Fade out
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Pow(timer / _transitionOutDuration, 3);
            yield return null;
        }
        canvasGroup.alpha = 0;
        StartCoroutine(UIAnimations.MoveObject(waterImage.rectTransform, new Vector2(0, 0), _waterAnimationLength));
        StartCoroutine(UIAnimations.MoveObject(characterImage.rectTransform,
        new Vector2(0, -waterImage.rectTransform.sizeDelta.y + characterImage.rectTransform.sizeDelta.y), _waterAnimationLength));
    }
}
