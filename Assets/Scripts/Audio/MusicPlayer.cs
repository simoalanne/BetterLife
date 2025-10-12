using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Helpers;
using UnityEditor;
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
            if (!Services.Register(this, dontDestroyOnLoad: true)) return;
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

#if UNITY_EDITOR
        private void Reset()
        {
            var sceneNames = FunctionLibrary.GetAllSceneNames();

            var originalKeysCount = musicClips.Keys.Count;

            sceneNames.ForEach(scene =>
            {
                if (!musicClips.ContainsKey(scene))
                {
                    musicClips.Add(scene, null);
                }
            });

            musicClips.Keys.ToList().ForEach(key =>
            {
                if (!sceneNames.Contains(key))
                {
                    musicClips.Remove(key);
                }
            });

            var changesHappened = originalKeysCount != sceneNames.Count;
            if (changesHappened)
                EditorUtility.SetDirty(this);
        }
#endif
    }
}
