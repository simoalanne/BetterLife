using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cutscenes
{
    [RequireComponent(typeof(AudioSource))]
    public class VictoryCutscene : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> audioClips;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            StartCoroutine(PlayCutscene());
        }

        private IEnumerator PlayCutscene()
        {
            yield return Services.SceneLoader.TemporaryFade(true, instant: true);
            foreach (var clip in audioClips)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                yield return new WaitForSeconds(clip.length);
            }

            yield return Services.SceneLoader.TemporaryFade(false);
        }
    }
}
