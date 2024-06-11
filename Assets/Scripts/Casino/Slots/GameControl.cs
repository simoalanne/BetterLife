using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class GameControl : MonoBehaviour
{
    // HandlePulled eventti
    public static event Action HandlePulled = delegate { };

    [SerializeField] 
    private TextMeshProUGUI prizeText; // palkintoteksti

    [SerializeField] 
    private Row[] rows; // Rivit

    [SerializeField] 
    private Transform handle; // Koneen kahvan transform

    private int prizeValue; // Palkintoarvo

    private bool resultsChecked = false; // voiton tarkistus

    void Update()
    {
        if (!rows[0].rowStopped || !rows[1].rowStopped || !rows[2].rowStopped) // Kone pyörii
        {
            prizeValue = 0;
            prizeText.enabled = false;
            resultsChecked = false;
        }

        if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped && !resultsChecked) // Kone lopettanut pyörimisen
        {
            CheckResults();
            prizeText.enabled = true;
            prizeText.text = "Prize: " + prizeValue;
        }
    }

    private void OnMouseDown() // Klikkaa kahvaa
    {
        if (rows[0].rowStopped && rows[1].rowStopped && rows[2].rowStopped)
        {
            StartCoroutine("PullHandle");
        }
    }

    private IEnumerator PullHandle() // Kahvan liikuttaminen ja eventti
    {
        for (int i = 0; i < 15; i += 5)
        {
            handle.Rotate(0f, 0f, i);
            yield return new WaitForSeconds(0.1f);
        }

        HandlePulled();

        for (int i = 0; i < 15; i += 5)
        {
            handle.Rotate(0f, 0f, -i);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CheckResults()
    {
        // Jos kaikki symbolit samoja niin voittoo hullusti
        if (rows[0].stoppedSlot == rows[1].stoppedSlot && rows[1].stoppedSlot == rows[2].stoppedSlot)
        {
            switch(rows[0].stoppedSlot)
            {
                case "Diamond":
                    prizeValue = 200;
                    break;
                case "Crown":
                    prizeValue = 400;
                    break;
                case "Melon":
                    prizeValue = 600;
                    break;
                case "Bar":
                    prizeValue = 800;
                    break;
                case "Seven":
                    prizeValue = 1500;
                    break;
                case "Cherry":
                    prizeValue = 3000;
                    break;
                case "Lemon":
                    prizeValue = 5000;
                    break;
                default:
                    prizeValue = 0;
                    break;
            }
        }
        // Kaksi ekaa symbolia samat
        else if (rows[0].stoppedSlot == rows[1].stoppedSlot && rows[1].stoppedSlot != rows[2].stoppedSlot)
        {
            switch(rows[0].stoppedSlot)
            {
                case "Diamond":
                    prizeValue = 100;
                    break;
                case "Crown":
                    prizeValue = 300;
                    break;
                case "Melon":
                    prizeValue = 500;
                    break;
                case "Bar":
                    prizeValue = 700;
                    break;
                case "Seven":
                    prizeValue = 1000;
                    break;
                case "Cherry":
                    prizeValue = 2000;
                    break;
                case "Lemon":
                    prizeValue = 4000;
                    break;
                default:
                    prizeValue = 0;
                    break;
            }
        }
        resultsChecked = true;
    }
}
