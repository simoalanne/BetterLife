using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializedDictionary("Scene Name", "Music")]
        [SerializeField] private SerializedDictionary<string, AudioClip> musicClips = new();

        private AudioSource _audioSource;

        private void Awake()
        {
            if (!Services.Register(this, persistent: true)) return;
            _audioSource = GetComponent<AudioSource>();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene _, Scene newScene)
        {
            {
                var clip = musicClips.GetValueOrDefault(newScene.name);
                if (clip is null)
                {
                    _audioSource.Stop();
                    return;
                }

                if (_audioSource.clip == clip) return;
                _audioSource.clip = clip;
                _audioSource.Play();
                _audioSource.loop = true;
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void Reset()
        {
            var sceneNames = FunctionLibrary.GetAllSceneNames();
            sceneNames.ForEach(scene => musicClips.Add(scene, null));
        }
    }
}
