using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Audio
{
    [System.Serializable]
    public class CustomAudioClip
    {
        public List<AudioClip> variations = new();
        [Range(0f, 1f)] public float volume;
        [Range(-3f, 3f)] public float pitch;
        [Range(0f, 1f)] public float volumeOffset;
        [Range(-3f, 3f)] public float pitchOffset;

        private AudioClip GetRandomClip() =>
            variations.Count == 0 ? null : variations[Random.Range(0, variations.Count)];

        private float GetRandomVolume() =>
            Mathf.Clamp01(volume + Random.Range(-volumeOffset, volumeOffset));

        private float GetRandomPitch() =>
            Mathf.Clamp(pitch + Random.Range(-pitchOffset, pitchOffset), -3f, 3f);

        public (float volume, float pitch, AudioClip clip) GetClip() =>
            (GetRandomVolume(), GetRandomPitch(), GetRandomClip());
    }


    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private List<CustomAudioClip> audioClips = new() { new CustomAudioClip { volume = 1f, pitch = 1f } };

        private AudioSource _audioSource;
        private int _currentListIndex;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public void Play(int listIndex)
        {
            if (listIndex < 0 || listIndex >= audioClips.Count) return;
            var (volume, pitch, clip) = audioClips[listIndex].GetClip();
            if (clip == null) return;
            _currentListIndex = listIndex;
            _audioSource.volume = volume;
            _audioSource.pitch = pitch;
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public void SetToLoop()
        {
            _audioSource.loop = true;
            StopAllCoroutines();
            StartCoroutine(AdjustLoop());
        }

        // Adjust volume and pitch for each loop iteration so it doesn't get too repetitive
        private IEnumerator AdjustLoop()
        {
            while (_audioSource.loop)
            {
                yield return new WaitForSeconds(_audioSource.clip.length);
                var (volume, pitch, _) = audioClips[_currentListIndex].GetClip();
                _audioSource.volume = volume;
                _audioSource.pitch = pitch;
            }

            _audioSource.loop = false;
        }

        public void StopClip(float fadeOutTime = 0f)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOutClip(fadeOutTime));
        }

        private IEnumerator FadeOutClip(float fadeOutTime)
        {
            var startVolume = _audioSource.volume;
            yield return FunctionLibrary.DoOverTime(fadeOutTime,
                prog => _audioSource.volume = Mathf.Lerp(startVolume, 0, prog));
            _audioSource.Stop();
            _audioSource.volume = startVolume;
        }
    }
}
