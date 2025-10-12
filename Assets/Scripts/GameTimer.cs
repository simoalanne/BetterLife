using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("Game Timer Settings")]
    [SerializeField] private float gameMinutesPerRealSecond = 1f; // How many in-game minutes pass per real-time second
    [SerializeField] private int startHour = 10;

    [Header("Global lighting settings")]
    [SerializeField, Range(0.5f, 1f)] private float maxGlobalLightIntensity = 0.8f;
    [SerializeField, Range(0f, 0.5f)] private float minGlobalLightIntensity = 0.05f;
    [SerializeField, Range(0, 23)] private float intensityStartsDecreasing = 16;
    [SerializeField, Range(0, 23)] private float minIntensityReached = 23f;
    [SerializeField, Range(0, 23)] private float intensityStartsIncreasing = 6f;
    [SerializeField, Range(0, 23)] private float maxIntensityReached = 10f;

    public float LightIntensity { get; private set; }

    public bool IsPaused { private get; set; }

    private int _totalElapsedMinutes;
    private float _minuteAccumulator;

    public int Days => _totalElapsedMinutes / 1440;
    public int Hours => _totalElapsedMinutes % 1440 / 60;
    public int Minutes => _totalElapsedMinutes % 60;

    private void Awake()
    {
        Services.Register(this, dontDestroyOnLoad: true);
        LightIntensity = maxGlobalLightIntensity;
        Reset();
    }

    public void Reset()
    {
        _totalElapsedMinutes = startHour * 60;
        IsPaused = false;
    }

    private void Update()
    {
        if (IsPaused) return;
        _minuteAccumulator += gameMinutesPerRealSecond * Time.deltaTime;
        if (_minuteAccumulator < 1f) return;
        _totalElapsedMinutes++;
        _minuteAccumulator = 0;
        UpdateLightIntensity();
    }

    public void AddToGameTime(int days, int hours, int minutes) =>
        _totalElapsedMinutes += days * 1440 + hours * 60 + minutes;

    public void SkipToNextDayAtHour(int hour) => _totalElapsedMinutes = 1440 * (Days + 1) + hour * 60;


    private void UpdateLightIntensity()
    {
        var currentHour = Hours + Minutes / 60f;

        // 1. Intensity at max
        if (currentHour >= maxIntensityReached && currentHour < intensityStartsDecreasing)
        {
            LightIntensity = maxGlobalLightIntensity;
            return;
        }

        // 2. Intensity at min
        if (currentHour >= minIntensityReached && currentHour < intensityStartsIncreasing)
        {
            LightIntensity = minGlobalLightIntensity;
            return;
        }

        // 3. Intensity decreasing
        if (currentHour >= intensityStartsDecreasing && currentHour < minIntensityReached)
        {
            var t = (currentHour - intensityStartsDecreasing) / (minIntensityReached - intensityStartsDecreasing);
            LightIntensity = Mathf.Lerp(maxGlobalLightIntensity, minGlobalLightIntensity, t);
            return;
        }

        // 4. Intensity increasing
        if (currentHour >= intensityStartsIncreasing && currentHour < maxIntensityReached)
        {
            var t = (currentHour - intensityStartsIncreasing) / (maxIntensityReached - intensityStartsIncreasing);
            LightIntensity = Mathf.Lerp(minGlobalLightIntensity, maxGlobalLightIntensity, t);
        }
    }
}
