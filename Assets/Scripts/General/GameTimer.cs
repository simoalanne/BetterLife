using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    [Header("Debugging")]
    [SerializeField] private float _timeScale = 1f;

    [Header("Game Timer Settings")]
    [SerializeField] private float _gameMinuteInRealTimeSeconds = 0.6f;

    [Header("Global lighting settings")]
    [SerializeField] private float _maxGlobalLightIntensity = 0.8f;
    [SerializeField] private float _minGlobalLightIntensity = 0.05f;
    [SerializeField] private float _startDecreasingIntensityHour = 15;
    [SerializeField] private float _minIntensityReachedHour = 22;
    private float _lightingIntensity;

    private float _totalElapsedSeconds = 0;
    private int _fullMinutes = 0;
    private int _fullHours = 0;
    private int _fullDays = 0;
    private bool _isPaused = false;
    private readonly int wakeUpHour = 10;

    public float GameMinuteInRealTimeSeconds => _gameMinuteInRealTimeSeconds;
    public int TotalElapsedRealSeconds => Mathf.RoundToInt(_totalElapsedSeconds);
    public int TotalElapsedRealMinutes => Mathf.RoundToInt(_totalElapsedSeconds / 60);
    public int TotalElapsedRealHours => Mathf.RoundToInt(_totalElapsedSeconds / 3600);
    public float LightIntensity => _lightingIntensity;

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
            AddToGameTime(0, 10, 0); // Game starts at 10am
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        Time.timeScale = _timeScale;
#endif
        if (!_isPaused)
        {
            _totalElapsedSeconds += Time.deltaTime;
            float totalElapsedGameTime = _totalElapsedSeconds / _gameMinuteInRealTimeSeconds * 60; // Convert real-time seconds to game seconds 

            _fullMinutes = Mathf.FloorToInt(totalElapsedGameTime / 60) % 60;
            _fullHours = Mathf.FloorToInt(totalElapsedGameTime / 3600) % 24;
            _fullDays = Mathf.FloorToInt(totalElapsedGameTime / 86400);

            UpdateLightingIntensity();
        }
    }

    private void UpdateLightingIntensity()
    {
        if (_fullHours >= _minIntensityReachedHour || _fullHours < wakeUpHour - 1)
        {
            _lightingIntensity = _minGlobalLightIntensity;
        }
        else if (_fullHours >= wakeUpHour && _fullHours < _startDecreasingIntensityHour)
        {
            _lightingIntensity = _maxGlobalLightIntensity;
        }
        else if (_fullHours >= _startDecreasingIntensityHour && _fullHours < _minIntensityReachedHour)
        {
            float totalDecreaseDuration = _minIntensityReachedHour - _startDecreasingIntensityHour;
            float elapsedDecreaseDuration = _fullHours - _startDecreasingIntensityHour + _fullMinutes / 60f;
            float decreasePercentage = elapsedDecreaseDuration / totalDecreaseDuration;
            _lightingIntensity = _maxGlobalLightIntensity - decreasePercentage * (_maxGlobalLightIntensity - _minGlobalLightIntensity);
        }
    }

    public void AddToGameTime(int minutes, int hours, int days)
    {
        // Convert days and hours to minutes, then sum up all minutes
        int totalGameMinutes = days * 24 * 60 + hours * 60 + minutes;

        // Convert game minutes to real-time seconds
        float realTimeSecondsToAdd = totalGameMinutes * _gameMinuteInRealTimeSeconds;

        // Add the calculated real-time seconds to the total elapsed seconds
        _totalElapsedSeconds += realTimeSecondsToAdd;
    }

    public void SkipToNextDay(int wakeUpHour)
    {
        // Calculate the total game minutes to add
        bool isAlreadyNextDay = _fullHours < wakeUpHour && _fullHours > 0;
        int minutesToAdd;
        if (isAlreadyNextDay)
        {
            minutesToAdd = (wakeUpHour - _fullHours) * 60 - _fullMinutes;
        }
        else
        {
            minutesToAdd = (24 - _fullHours + wakeUpHour) * 60 - _fullMinutes;
        }

        // Convert game minutes to real-time seconds
        float realTimeSecondsToAdd = minutesToAdd * _gameMinuteInRealTimeSeconds;

        // Add the calculated real-time seconds to the total elapsed seconds
        _totalElapsedSeconds += realTimeSecondsToAdd;

        // Update the game time
        _fullDays = Mathf.FloorToInt(_totalElapsedSeconds / (_gameMinuteInRealTimeSeconds * 60 * 24));
        _fullHours = wakeUpHour;
        _fullMinutes = 0;
    }
}
