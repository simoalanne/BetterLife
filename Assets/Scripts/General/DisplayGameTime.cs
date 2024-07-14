using UnityEngine;
using TMPro;

public class DisplayGameTime : MonoBehaviour
{
    [Header("Time Display Settings")]
    [SerializeField] private DisplayFormat _displayFormat = DisplayFormat.TwelveHour;
    [Header("UI Texts")]
    [SerializeField] private TMP_Text _currentDayText;
    [SerializeField] private TMP_Text _currentTimeText;

    [SerializeField, Range(1, 60)] private int _minutesUpdateRate = 10; // How often the time is updated in game.

    private enum DisplayFormat
    {
        TwelveHour,
        TwentyFourHour
    }

    void Update()
    {
        _currentDayText.text = $"Day {GameTimer.Instance.FullDays + 1}"; // Don't start from day 0.
        _currentTimeText.text = _displayFormat == DisplayFormat.TwelveHour ? DisplayTwelveHourFormat() : DisplayTwentyFourHourFormat();
    }

    string DisplayTwelveHourFormat()
    {
        int displayHours = GameTimer.Instance.FullHours % 12 == 0 ? 12 : GameTimer.Instance.FullHours % 12;
        int displayMinutes = GameTimer.Instance.FullMinutes - GameTimer.Instance.FullMinutes % _minutesUpdateRate;
        return $"{displayHours}:{displayMinutes:D2} {(GameTimer.Instance.FullHours >= 12 ? "pm" : "am")}";
    }

    string DisplayTwentyFourHourFormat()
    {
        int displayHours = GameTimer.Instance.FullHours;
        int displayMinutes = GameTimer.Instance.FullMinutes - GameTimer.Instance.FullMinutes % _minutesUpdateRate;
        return $"{displayHours:D2}:{displayMinutes:D2}";
    }
}
