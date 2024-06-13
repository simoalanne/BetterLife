using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    private const float stepValue = 1f / 9f; // this value is the amount of uvrect between each symbol

    public bool rowStopped;
    public string _stoppedSlot;
    public string StoppedSlot => _stoppedSlot;
    private bool _stop = true;
    private readonly Dictionary<float, string> _symbols = new()
    {
        {Mathf.Round(0 * stepValue * 1000) / 1000, "Strawberry"},
        {Mathf.Round(1 * stepValue * 1000) / 1000, "Seven"},
        {Mathf.Round(2 * stepValue * 1000) / 1000, "Grapes"},
        {Mathf.Round(3 * stepValue * 1000) / 1000, "Lemon"},
        {Mathf.Round(4 * stepValue * 1000) / 1000, "Melon"},
        {Mathf.Round(5 * stepValue * 1000) / 1000, "Orange"},
        {Mathf.Round(6 * stepValue * 1000) / 1000, "Cherry"},
        {Mathf.Round(7 * stepValue * 1000) / 1000, "Pineapple"},
        {Mathf.Round(8 * stepValue * 1000) / 1000, "Plum"}
    };

    void Start()
    {
        rowStopped = true;
        var rawImage = GetComponent<RawImage>();
        var uvRect = rawImage.uvRect;
        uvRect.y = stepValue * Random.Range(0, 9);
        rawImage.uvRect = uvRect;
    }

    public void StartRotating()
    {
        _stoppedSlot = "";
        StartCoroutine(Rotate());

    }

    private IEnumerator Rotate()
    {
        rowStopped = false;
        float timeToMove = 0.75f; // Time to move from 0 to 1

        // Get the initial uvRect.y value
        RawImage rawImageInitial = GetComponent<RawImage>();
        float initialY = rawImageInitial.uvRect.y;

        while (_stop)
        {
            float elapsedTime = 0f; // Time elapsed since the start of the rotation

            while (elapsedTime < timeToMove)
            {
                elapsedTime += Time.deltaTime; // Update the elapsed time
                float newY = Mathf.Lerp(initialY, 1 + initialY, elapsedTime / timeToMove); // Calculate the new y value

                // Update the uvRect
                RawImage rawImage = GetComponent<RawImage>();
                rawImage.uvRect = new Rect(rawImage.uvRect.x, newY % 1, rawImage.uvRect.width, rawImage.uvRect.height);

                yield return null;
            }

            // Reset the uvRect.y to the initial value
            RawImage rawImageReset = GetComponent<RawImage>();
            rawImageReset.uvRect = new Rect(rawImageReset.uvRect.x, initialY, rawImageReset.uvRect.width, rawImageReset.uvRect.height);
        }
    }


    public void Stop()
    {
        _stop = false;
        StartCoroutine(StopRotating());
    }

    private IEnumerator StopRotating()
    {
        float timeToStop = 2.25f; // Time to stop from current speed to 0
        float elapsedTime = 0f; // Time elapsed since the start of the stopping

        // Get the initial uvRect.y value
        RawImage rawImageInitial = GetComponent<RawImage>();
        float initialY = rawImageInitial.uvRect.y;

        // Calculate the final y value after one half rotation plus a random step
        float finalY = initialY + stepValue * 4 + stepValue * Random.Range(0, 9);

        // Calculate the offset to the closest step
        float offset = finalY % stepValue;
        if (offset > stepValue / 2)
        {
            finalY += stepValue - offset; // Add the remaining distance to the next step
        }
        else
        {
            finalY -= offset; // Subtract the distance to the previous step
        }

        while (elapsedTime < timeToStop)
        {
            elapsedTime += Time.deltaTime; // Update the elapsed time
            float newY = Mathf.Lerp(initialY, finalY, elapsedTime / timeToStop); // Calculate the new y value

            // Update the uvRect
            RawImage rawImage = GetComponent<RawImage>();
            rawImage.uvRect = new Rect(rawImage.uvRect.x, newY % 1, rawImage.uvRect.width, rawImage.uvRect.height);

            yield return null;
        }

        // Set the uvRect.y to the final value
        RawImage rawImageFinal = GetComponent<RawImage>();
        rawImageFinal.uvRect = new Rect(rawImageFinal.uvRect.x, finalY % 1, rawImageFinal.uvRect.width, rawImageFinal.uvRect.height);

        rowStopped = true;

        float finalStepValue = Mathf.Round(finalY % 1 * 1000) / 1000; // Round to 3 decimal places

        // Define a tolerance for the approximation
        float tolerance = 0.001f;

        foreach (var symbol in _symbols)
        {
            if (Mathf.Abs(symbol.Key - finalStepValue) < tolerance)
            {
                _stoppedSlot = symbol.Value;
                break;
            }
        }
        _stop = true;
    }
}
