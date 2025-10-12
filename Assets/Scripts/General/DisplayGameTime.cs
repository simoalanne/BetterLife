using TMPro;
using UnityEngine;

public class DisplayGameTime : MonoBehaviour
{
    [SerializeField] private TMP_Text currentDayText;
    [SerializeField] private TMP_Text currentTimeText;
    
    private GameTimer _gameTimer;

    private void Update()
    {
        _gameTimer ??= Services.GameTimer;
        if (_gameTimer == null) return;
        currentDayText.text = $"Day {_gameTimer.Days+ 1}";
        currentTimeText.text = $"{_gameTimer.Hours:00}:{_gameTimer.Minutes:00}";
    }
}
