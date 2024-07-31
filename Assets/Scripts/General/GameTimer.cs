using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [Header("Game Timer Settings")]
    [SerializeField] private float _gameMinuteInRealTimeSeconds = 0.6f;

    private float _totalElapsedSeconds = 0;
    private int _fullMinutes = 0;
    private int _fullHours = 0;
    private int _fullDays = 0;
    private bool _isPaused = false;

    public float GameMinuteInRealTimeSeconds => _gameMinuteInRealTimeSeconds;
    public int TotalElapsedRealSeconds => Mathf.RoundToInt(_totalElapsedSeconds);
    public int TotalElapsedRealMinutes => Mathf.RoundToInt(_totalElapsedSeconds / 60);
    public int TotalElapsedRealHours => Mathf.RoundToInt(_totalElapsedSeconds / 3600);

    public int FullMinutes => _fullMinutes;
    public int FullHours => _fullHours;
    public int FullDays => _fullDays;
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            AddToGameTime(0, 10, 0); // Game starts at 12am
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
        }
        // LoadGameTime();
    }

    // Update is called once per frame
    void Update()
    {

        if (!_isPaused)
        {
            _totalElapsedSeconds += Time.deltaTime;
            float totalElapsedGameTime = _totalElapsedSeconds / _gameMinuteInRealTimeSeconds * 60; // Convert real-time seconds to game seconds 

            _fullMinutes = Mathf.FloorToInt(totalElapsedGameTime / 60) % 60;
            _fullHours = Mathf.FloorToInt(totalElapsedGameTime / 3600) % 24;
            _fullDays = Mathf.FloorToInt(totalElapsedGameTime / 86400);
        }
    }

    /// <summary>
    /// Adds the specified amount of time to the game time.
    /// Can be used in many ways, for example, to skip time when the player sleeps,
    /// or when a new game is started etc.
    public void AddToGameTime(int minutes, int hours, int days)
    {
        // Convert days and hours to minutes, then sum up all minutes
        int totalGameMinutes = days * 24 * 60 + hours * 60 + minutes;

        // Convert game minutes to real-time seconds
        float realTimeSecondsToAdd = totalGameMinutes * _gameMinuteInRealTimeSeconds;

        // Add the calculated real-time seconds to the total elapsed seconds
        _totalElapsedSeconds += realTimeSecondsToAdd;
    }

    public void SkipToNexDay(int wakeUpHour)
    {
        // Calculate the total game minutes to add
        int minutesToAdd = (24 - _fullHours + wakeUpHour) * 60 - _fullMinutes;

        // Convert game minutes to real-time seconds
        float realTimeSecondsToAdd = minutesToAdd * _gameMinuteInRealTimeSeconds;

        // Add the calculated real-time seconds to the total elapsed seconds
        _totalElapsedSeconds += realTimeSecondsToAdd;

        // Update the game time
        _fullDays = Mathf.FloorToInt(_totalElapsedSeconds / (_gameMinuteInRealTimeSeconds * 60 * 24));
        _fullHours = wakeUpHour;
        _fullMinutes = 0;
    }

    void OnApplicationQuit()
    {
        // SaveGameTime();
    }

    void SaveGameTime()
    {
        // Save the game time to a file
        Destroy(gameObject);
    }
}
