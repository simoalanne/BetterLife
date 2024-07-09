using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;

    private AudioSource _audioSource;
    public static MusicPlayer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _musicClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
