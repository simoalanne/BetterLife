using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCutscene : MonoBehaviour
{
    [SerializeField] private Animator animator; // Kidnappaus animaattori
    [SerializeField] public CanvasGroup canvasGroup; // Fade mustaan
    [SerializeField] public AudioSource audioSource; // Scenen audiosource
    [SerializeField] public AudioClip[] audioClips; // Kaikki soundeffectit
    [SerializeField] private float _transitionInDuration = 0.5f; // Kauanko fade in
    [SerializeField] private float _transitionOutDuration = 0.25f; // Kauanko fade out
    [SerializeField] public Image waterImage;

    private void Start()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        var timer = 0f;
        animator.SetBool("Kidnapped", true); // Soitetaan kidnappaus-animaatio
        audioSource.clip = audioClips[0]; 
        audioSource.Play(); // Ensimm‰inen ‰‰niefekti soitetaan
        yield return new WaitForSeconds(audioSource.clip.length);

        while (timer < _transitionInDuration) // Fade in
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / _transitionInDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;
        animator.SetBool("Kidnapped", false);

        for(int i = 1; i < audioClips.Length; i++) // Soitetaan sound effectit
        {
            audioSource.clip = audioClips[i];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        
        waterImage.enabled = true;
        timer = 0f;
        while (timer < _transitionOutDuration) // Fade out
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Pow(timer / _transitionOutDuration, 3);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
