using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    private int randomValue;
    private float timeInterval;
    private const float stepValue = 1f / 9f; // this value is the amount of uvrect between each symbol

    public bool rowStopped;
    public string stoppedSlot;
    [SerializeField] private float _firstRowStopDelay = 3f; // how long the the rows will spin before stopping.

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.name == "Row1")
        {
        }

        rowStopped = true;
        GameControl.HandlePulled += StartRotating; // Vet�� kahvasta niin startrotating kutsutaan
        var rawImage = GetComponent<RawImage>();
        var uvRect = rawImage.uvRect;
        uvRect.y = stepValue * Random.Range(0, 9);
        rawImage.uvRect = uvRect;
    }

    private void StartRotating()
    {
        stoppedSlot = "";
        StartCoroutine(Rotate());

    }

    private IEnumerator Rotate()
    {
        rowStopped = false;
        float timeToMove = 0.5f; // Time to move from 0 to 1

        // Get the initial uvRect.y value
        RawImage rawImageInitial = GetComponent<RawImage>();
        float initialY = rawImageInitial.uvRect.y;

        while (true)
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


        /*switch (randomValue % 3) // Modulo operaattori, googleta lol
        {
            case 1:
                randomValue += 2;
                break;
            case 2:
                randomValue += 1;
                break;
            default:
                randomValue = 0;
                break;
        }

        for (int i = 0; i < randomValue; i++)
        {
            if (transform.position.y <= -9.25f)
            {
                transform.position = new Vector2(transform.position.x, 5.95f);
            }
            else
            {
                transform.position = new Vector2(transform.position.x, transform.position.y - 1.9f);
            }

            if (i > Mathf.RoundToInt(randomValue * 0.25f))
            {
                timeInterval = 0.05f;
            }
            if (i > Mathf.RoundToInt(randomValue * 0.5f))
            {
                timeInterval = 0.1f;
            }
            if (i > Mathf.RoundToInt(randomValue * 0.75f))
            {
                timeInterval = 0.15f;
            }
            if (i > Mathf.RoundToInt(randomValue * 0.95f))
            {
                timeInterval = 0.2f;
            }

            yield return new WaitForSeconds(timeInterval);
        }

        /*switch (transform.position.y) // T�ss� annetaan rivin stoppedSlotille arvo y-position mukaan.
        {
            case -9.25f:
                stoppedSlot = "Strawberry";
                break;
            case -7.35f:
                stoppedSlot = "Plum";
                break;
            case -5.45f:
                stoppedSlot = "Pineapple";
                break;
            case -3.55f:
                stoppedSlot = "Cherry";
                break;
            case -1.65f:
                stoppedSlot = "Orange";
                break;
            case 0.25f:
                stoppedSlot = "Melon";
                break;
            case 2.15f:
                stoppedSlot = "Lemon";
                break;
            case 4.05f:
                stoppedSlot = "Grapes";
                break;
            case 5.95f:
                stoppedSlot = "Seven";
                break;
        } 

        if (Mathf.Approximately(transform.position.y, -9.25f))
        {
            stoppedSlot = "Strawberry";
        }
        else if (Mathf.Approximately(transform.position.y, -7.35f))
    {
    stoppedSlot = "Plum";
    }
    else if (Mathf.Approximately(transform.position.y, -5.45f))
    {
    stoppedSlot = "Pineapple";
    }
    else if (Mathf.Approximately(transform.position.y, -3.55f))
    {
    stoppedSlot = "Cherry";
    }
    else if (Mathf.Approximately(transform.position.y, -1.65f))
    {
    stoppedSlot = "Orange";
    }
    else if (Mathf.Approximately(transform.position.y, 0.2499996f))
    {
    stoppedSlot = "Melon";
    }
    else if (Mathf.Approximately(transform.position.y, 2.15f))
    {
    stoppedSlot = "Lemon";
    }
    else if (Mathf.Approximately(transform.position.y, 4.05f))
    {
    stoppedSlot = "Grapes";
    }
    else if (Mathf.Approximately(transform.position.y, 5.95f))
    {
    stoppedSlot = "Seven";
    }

    rowStopped = true;
    }

    private void OnDestroy()
    {
    GameControl.HandlePulled -= StartRotating;
    }
    }
    */
    }
}

