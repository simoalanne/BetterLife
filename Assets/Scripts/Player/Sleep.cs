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
    [SerializeField] private int _wakeUpHour = 10; // The hour the player wakes up after sleeping.
    [SerializeField] private float _fadeInDuration = 1f; // How long the fade to black takes.
    [SerializeField] private float _fadeOutDuration = 1f; // How long the fade back from black takes.
    [SerializeField] private float _screenBlackTime = 3f; // How long the screen is black when sleeping.
    [SerializeField] private Collider2D _homeExitNormalCollider; // Totally belongs to this script.
    [SerializeField] private Collider2D _homeExitGameOverCollider; // Totally belongs to this script. Changes the scene that player loads to when they exit the house.
    [SerializeField] private DialogueTrigger _dialogueTrigger; // when game about to be over trigger dialogue.
    [SerializeField] private DialogueTrigger _afterPassOutDialogueTrigger; // when player passes out trigger dialogue.
    [SerializeField] private SpawnPoint _childsBedSpawnPoint;
    private bool _isGameOver = false;
    private bool _sleepCancelled = false;

    void Awake()
    {
        _sleepUI.gameObject.SetActive(false);
        _fadeImage.gameObject.SetActive(false);
        _fadeImage.color = Color.clear;
        GameTimer.Instance.IsPaused = false;
    }

    void Start()
    {
        if (PlayerManager.Instance.HasPlayerPassedOut == true)
        {
            SleepInBed();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player entered bed trigger.");
        if (other.CompareTag("Player") && !_sleepCancelled)
        {
            _sleepUI.gameObject.SetActive(true);
            PlayerManager.Instance.DisablePlayerMovement();
            PlayerManager.Instance.DisablePlayerInteract();
            GameTimer.Instance.IsPaused = true;
        }
    }

    public void CancelSleep()
    {
        _sleepUI.gameObject.SetActive(false);
        _sleepCancelled = true;
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        GameTimer.Instance.IsPaused = false;
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

    private IEnumerator SleepInBedCoroutine()
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
        _isGameOver = PlayerHUD.Instance.ReduceLoanDaysLeft(); // Reduce the days left on the loan.
        Needs.Instance.MaxOutEnergy();
        GameTimer.Instance.SkipToNextDay(_wakeUpHour);
        PlayerManager.Instance.transform.position = _childsBedSpawnPoint.spawnPoint;
        yield return new WaitForSeconds(_screenBlackTime);

        float timer2 = 0;
        while (timer2 < _fadeOutDuration)
        {
            timer2 += Time.unscaledDeltaTime;
            _fadeImage.color = new Color(0, 0, 0, 1 - Mathf.Pow(timer2 / _fadeOutDuration, 3));
            yield return null;
        }
        _fadeImage.color = Color.clear;
        EndSleep();
        if (_isGameOver)
        {
            _homeExitNormalCollider.enabled = false;
            _homeExitGameOverCollider.enabled = true;
            _dialogueTrigger.TriggerDialogue();
        }
        else
        {
            _homeExitNormalCollider.enabled = true;
            _homeExitGameOverCollider.enabled = false;
        }
    }

    void InitializeSleep()
    {
        _sleepUI.gameObject.SetActive(false);
        _fadeImage.gameObject.SetActive(true);
        GameTimer.Instance.IsPaused = true;
    }

    void EndSleep()
    {
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        PlayerManager.Instance.HasPlayerPassedOut = false;
        GameTimer.Instance.IsPaused = false;
        _fadeImage.gameObject.SetActive(false);
    }
}
