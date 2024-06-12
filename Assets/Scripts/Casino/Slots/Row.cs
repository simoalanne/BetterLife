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
            if (transform.position.y <= -9.25f) // -9.25f on korkeimman symbolin transformi positio
            {
                transform.position = new Vector2(transform.position.x, 5.95f); // 5.95 on matalimaan symbolin transform positio
            }

            transform.position = new Vector2(transform.position.x, transform.position.y - 1.9f); // uusi symboli on 1.9 y-arvon v‰lein
            yield return new WaitForSeconds(timeInterval);
        }

        randomValue = Random.Range(60, 120);

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
            if (transform.position.y <= -9.25f)
            {
                transform.position = new Vector2(transform.position.x, 5.95f);
            }

            transform.position = new Vector2(transform.position.x, transform.position.y - 1.9f);

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

        /*switch (transform.position.y) // T‰ss‰ annetaan rivin stoppedSlotille arvo y-position mukaan.
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
        } */

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
