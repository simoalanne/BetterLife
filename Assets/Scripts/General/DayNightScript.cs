using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayNightScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayCountText;
    [SerializeField] private TextMeshProUGUI currentTimeText;

    [SerializeField] private float tick;
    private float seconds;
    private float minutes;
    private float hours;
    private float days;

    private void FixedUpdate()
    {
        CalcTime();
        DisplayTime();
    }

    private void CalcTime()
    {
        seconds += Time.deltaTime * tick;

        if (seconds >= 60)
        {
            seconds = 0;
            minutes++;
        }

        if (minutes >= 60)
        {
            minutes = 0;
            hours++;
        }

        if (hours >= 24)
        {
            hours = 0;
            days++;
        }
    }

    private void DisplayTime()
    {
        currentTimeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
        dayCountText.text = "Day: " + days;
    }
}
