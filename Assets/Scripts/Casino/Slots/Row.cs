using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    private int randomValue;
    private float timeInterval;

    public bool rowStopped;
    public string stoppedSlot;

    // Start is called before the first frame update
    void Start()
    {
        rowStopped = true;
        GameControl.HandlePulled += StartRotating; // Vet‰‰ kahvasta niin startrotating kutsutaan
    }

    private void StartRotating()
    {
        stoppedSlot = "";
        StartCoroutine("Rotate");

    }

    private IEnumerator Rotate()
    {
        rowStopped = false;
        timeInterval = 0.025f;

        for (int i = 0; i < 30; i++)
        {
            if (transform.position.y <= -3.25f) // -3.25f on korkeimman symbolin transformi positio
            {
                transform.position = new Vector2(transform.position.x, 1.65f); // 1.65 on matalimaan symbolin transform positio
            }

            transform.position = new Vector2(transform.position.x, transform.position.y - 0.7f); // uusi symboli on 0.7 y-arvon v‰lein
            yield return new WaitForSeconds(timeInterval);
        }

        randomValue = Random.Range(60, 100);

        switch (randomValue % 3) // Modulo operaattori, googleta lol
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

        for(int i = 0; i < randomValue; i++)
        {
            if (transform.position.y <= -3.25f)
            {
                transform.position = new Vector2(transform.position.x, 1.65f);
            }

            transform.position = new Vector2(transform.position.x, transform.position.y - 0.7f);

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

        switch (transform.position.y) // T‰ss‰ annetaan rivin stoppedSlotille arvo y-position mukaan.
        {
            case -3.25f:
                stoppedSlot = "Diamond";
                break;
            case -2.55f:
                stoppedSlot = "Crown";
                break;
            case -1.85f:
                stoppedSlot = "Melon";
                break;
            case -1.15f:
                stoppedSlot = "Bar";
                break;
            case -0.45f:
                stoppedSlot = "Seven";
                break;
            case 0.25f:
                stoppedSlot = "Cherry";
                break;
            case 0.95f:
                stoppedSlot = "Lemon";
                break;
            case 1.65f:
                stoppedSlot = "Diamond";
                break;
        }

        rowStopped = true;
    }

    private void OnDestroy()
    {
        GameControl.HandlePulled -= StartRotating;
    }
}
