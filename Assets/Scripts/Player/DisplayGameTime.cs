using TMPro;
using UnityEngine;

namespace Player
{
    public class DisplayGameTime : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentDayText;
        [SerializeField] private TMP_Text currentTimeText;

        private GameTimer _gameTimer;
        
        private void Start() => _gameTimer = Services.GameTimer;
        

        private void Update()
        {
            currentDayText.text = $"Day {_gameTimer.Days + 1}";
            currentTimeText.text = $"{_gameTimer.Hours:00}:{_gameTimer.Minutes:00}";
        }
    }
}
