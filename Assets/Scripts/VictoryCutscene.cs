using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCutscene : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip[] audioClips;
    [SerializeField] public CanvasGroup canvasGroup;
    [SerializeField] private float _transitionInDuration = 0.5f; // Kauanko fade in
    [SerializeField] private float _transitionOutDuration = 0.25f; // Kauanko fade out


    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.ShowHud(false);
        }

        var timer = 0f;
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < audioClips.Length; i++) // Soitetaan sound effectit
        {
            audioSource.clip = audioClips[i];
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }

        while (timer < _transitionOutDuration) // Fade out
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1 - Mathf.Pow(timer / _transitionOutDuration, 3);
            yield return null;
        }

        yield return null;
    }
}
