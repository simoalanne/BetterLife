using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Player;

public class Sleep : MonoBehaviour
{
    [Header("Sleep UI")]
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Image _sleepUI;

    [Header("Sleep Settings")]
    [SerializeField] private int _hoursToSleep = 8; // Maybe controlled by a player instead of a fixed value.
    [SerializeField] private float _fadeInDuration = 1f; // How long the fade to black takes.
    [SerializeField] private float _fadeOutDuration = 1f; // How long the fade back from black takes.
    [SerializeField] private float _screenBlackTime = 3f; // How long the screen is black when sleeping.

    private bool _sleepCancelled = false;

    void Awake()
    {
        _sleepUI.gameObject.SetActive(false);
        _fadeImage.color = Color.clear;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered bed trigger.");
        if (other.CompareTag("Player") && !_sleepCancelled)
        {
            _sleepUI.gameObject.SetActive(true);
            PlayerManager.Instance.DisablePlayerMovement();
            PlayerManager.Instance.DisablePlayerInteract();
            Time.timeScale = 0;
        }
    }

    public void CancelSleep()
    {
        _sleepUI.gameObject.SetActive(false);
        _sleepCancelled = true;
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        Time.timeScale = 1;
    }

    /// <summary>
    /// If player cancels sleep they have to exit the trigger and re-enter it to try again. 
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _sleepCancelled = false;
        }
    }

    public void SleepInBed()
    {
        StartCoroutine(SleepInBedCoroutine());
    }

    public IEnumerator SleepInBedCoroutine()
    {
        InitializeSleep();
        float timer = 0;
        while (timer < _fadeInDuration)
        {
            timer += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ignore the time scale.
            _fadeImage.color = new Color(0, 0, 0, timer / _fadeInDuration);
            yield return null;
        }
        _fadeImage.color = Color.black;
        GameTimer.Instance.AddToGameTime(0, _hoursToSleep, 0); // Add time here so player can't see it being added.
        yield return new WaitForSecondsRealtime(_screenBlackTime);

        float timer2 = 0;
        while (timer2 < _fadeOutDuration)
        {
            timer2 += Time.unscaledDeltaTime;
            _fadeImage.color = new Color(0, 0, 0, 1 - Mathf.Pow(timer2 / _fadeOutDuration, 3));
            yield return null;
        }
        _fadeImage.color = Color.clear;
        EndSleep();
    }

    void InitializeSleep()
    {
        _sleepUI.gameObject.SetActive(false);
        Time.timeScale = 0;
    }

    void EndSleep()
    {
        Time.timeScale = 1;
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
    }
}
