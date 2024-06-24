using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    private const float _symbolDifference = 1f / 9f; // this value is the amount of uvrect between each symbol

    public bool rowStopped;
    public string _stoppedSlot;
    public string StoppedSlot => _stoppedSlot;
    private bool _spin = true;
    public bool Spin { get => _spin; set => _spin = value;}
    private float _elapsedTime; // Global elapsedTime variable
    private readonly Dictionary<float, string> _symbols = new()
    {
        {Mathf.Round(0 * _symbolDifference * 1000) / 1000, "Strawberry"},
        {Mathf.Round(1 * _symbolDifference * 1000) / 1000, "Seven"},
        {Mathf.Round(2 * _symbolDifference * 1000) / 1000, "Grapes"},
        {Mathf.Round(3 * _symbolDifference * 1000) / 1000, "Lemon"},
        {Mathf.Round(4 * _symbolDifference * 1000) / 1000, "Melon"},
        {Mathf.Round(5 * _symbolDifference * 1000) / 1000, "Orange"},
        {Mathf.Round(6 * _symbolDifference * 1000) / 1000, "Cherry"},
        {Mathf.Round(7 * _symbolDifference * 1000) / 1000, "Pineapple"},
        {Mathf.Round(8 * _symbolDifference * 1000) / 1000, "Plum"}
    };

    void Start()
    {
        rowStopped = true;
        var rawImage = GetComponent<RawImage>();
        var uvRect = rawImage.uvRect;
        uvRect.y = _symbolDifference * Random.Range(0, 9);
        rawImage.uvRect = uvRect;
    }

    public void StartRotating()
    {
        _stoppedSlot = "";
        _spin = true;
        StartCoroutine(Rotate());
    }

    public void Stop()
    {
        _spin = false;
    }

    private IEnumerator Rotate()
    {
        rowStopped = false;
        float timeToMove = 0.75f;
        RawImage rawImage = GetComponent<RawImage>();
        float initialY = rawImage.uvRect.y;

        while (_spin)
        {
            _elapsedTime = 0f;

            while (_elapsedTime < timeToMove)
            {
                _elapsedTime += Time.deltaTime;
                float newY = Mathf.Lerp(initialY, initialY + 1, _elapsedTime / timeToMove);
                rawImage.uvRect = new Rect(rawImage.uvRect.x, newY % 1, rawImage.uvRect.width, rawImage.uvRect.height);
                yield return null;
            }

            initialY = rawImage.uvRect.y;
        }

        // Continue rotating at the same speed until it reaches symbolToStop
        float targetY = _symbolDifference * Random.Range(0, 9);
        float currentY = rawImage.uvRect.y;
        float distanceToTravel = targetY > currentY ? targetY - currentY : 1 + targetY - currentY;
        _elapsedTime = 0f;

        while (_elapsedTime < distanceToTravel * timeToMove)
        {
            _elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(currentY, currentY + distanceToTravel, _elapsedTime / (distanceToTravel * timeToMove));
            rawImage.uvRect = new Rect(rawImage.uvRect.x, newY % 1, rawImage.uvRect.width, rawImage.uvRect.height);
            yield return null;
        }

        // Set the final position to symbolToStop
        rawImage.uvRect = new Rect(rawImage.uvRect.x, targetY % 1, rawImage.uvRect.width, rawImage.uvRect.height);
        rowStopped = true;

        float final_symbolDifference = Mathf.Round(targetY % 1 * 1000) / 1000; // Round to 3 decimal places

        // Define a tolerance for the approximation
        float tolerance = 0.001f;

        foreach (var symbol in _symbols)
        {
            if (Mathf.Abs(symbol.Key - final_symbolDifference) < tolerance)
            {
                _stoppedSlot = symbol.Value;
                break;
            }
        }
    }
}
