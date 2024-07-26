using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverCutscene : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] audioClips;
    [SerializeField] private float _transitionInDuration = 0.5f;
    [SerializeField] private float _transitionOutDuration = 0.25f;

    private void Start()
    {
        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        var timer = 0f;
        animator.SetBool("Kidnapped", true);
        audioSource.clip = audioClips[0];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);

        while (timer < _transitionInDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / _transitionInDuration;
            yield return null;
        }
        canvasGroup.alpha = 1;
        animator.SetBool("Kidnapped", false);

        for(int i = 1; i < audioClips.Length; i++)
        {
            audioSource.clip = audioClips[i];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
            
        timer = 0f;
        while (timer < _transitionOutDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Pow(timer / _transitionOutDuration, 3);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
